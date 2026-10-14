# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## What this is

`Pipelines.Common` is a tiny .NET Standard 2.0 library implementing the "pipeline of steps" pattern described in
Jeremy Davis's [An Alternative Approach to Pipelines](https://blog.jermdavis.dev/posts/2016/an-alternative-approach-to-pipelines/).
A pipeline is just a `Func<TInput, TOutput>` built by chaining typed steps together via fluent `AddStep` extension
methods. There is no runtime magic, no reflection, no DI requirement in the library itself — composition happens at
compile time through generic type matching (each step's output type must match the next step's input type).

## Build / test commands

```
dotnet build Pipelines.sln
dotnet test tests/Pipelines.UnitTests/Pipelines.UnitTests.csproj
dotnet test tests/Pipelines.UnitTests/Pipelines.UnitTests.csproj --filter FullyQualifiedName~Pipeline3steps_Execute_AllStepsAreInvokedOnce
dotnet run --project tests/Pipelines.FunctionalTests/Pipelines.FunctionalTests.csproj
```

There is no separate lint step; standard `dotnet build` warnings are the only signal.

## Solution layout

- `src/Pipelines.Common` — the library (netstandard2.0). Everything consumers reference.
- `tests/Pipelines.UnitTests` — xUnit + Moq tests against mocked `IPipelineStep<,>` instances.
- `tests/Pipelines.FunctionalTests` — a console app (`Program.cs`) wiring up a real multi-step pipeline
  (`LavadoCompletoPipeline/`) end-to-end, logging via Serilog.
- `tests/api` — an ASP.NET Core sample (`Pipelines.ApiTests`) showing how a pipeline is composed in
  `ServiceCollectionExtensions.AddBookingPipelines` and invoked from a controller via DI
  (`IPipelineStep<HubRequest, HubResponse>` registered as a singleton).

## Core architecture

### Synchronous side

- `IPipelineStep<TInput, TOutput>` — the base contract: `TOutput Process(TInput input)`.
- `IPipelineStep<TInput, TParam1, TOutput>` (and `TParam2`, `TParam3` overloads) — steps that also need extra
  context beyond the piped input (e.g. the original request from an outer/parent pipeline). These are standalone
  interfaces — they do **not** extend the base `IPipelineStep<TInput, TOutput>`, so implementers only need the one
  `Process` overload that actually makes sense for them (see `GetRequestStep` in `tests/api`).
- `BasePipelineStep<TInput, TOutput>` — abstract step base exposing `OnInput`/`OnOutput` events around an
  abstract `ProcessStep`, for subclassing when you want built-in event hooks without wiring them externally.
- `PipelineStep<TInput, TOutput>` — a decorator wrapping any `IPipelineStep<,>` to add `OnInput`/`OnOutput` events
  externally, without requiring the inner step to subclass anything.
- `BasePipeline<TInput, TOutput>` — implements `IPipelineStep<TInput, TOutput>` itself by holding a
  `Func<TInput, TOutput> Steps`, **required via a protected constructor (null-checked, immutable thereafter)**, and
  delegating `Process` to it. **This is why a `Pipeline<,>` can be used as a step inside another pipeline**
  (sub-pipelines, see `SubPipeline.cs` and the nested `Pipeline<ConnectorRequest,...>` in
  `ServiceCollectionExtensions`).
- `Pipeline<TInput, TOutput>` — concrete `BasePipeline<,>` whose constructor takes the `Steps` function directly and
  passes it to `base(...)`.

### Async side

Mirrors the sync side one-to-one: `IAsyncPipelineStep<TInput, TOutput>` (+ standalone `TParam1`/`TParam2`/`TParam3`
overloads, same as the sync side, returning `Task<TOutput>`), `BaseAsyncPipeline<TInput, TOutput>`,
`AsyncPipeline<TInput, TOutput>`. `PipelineStepAsyncExtensions.AddStep` has two overload sets per arity — one taking
`this TInput` (for the first step in a chain) and one taking `this Task<TInput>` (awaiting it before handing the
result to the next step) — which is what lets multiple async steps chain with the same fluent
`.AddStep(a).AddStep(b)` syntax as the sync side. There is still no async equivalent of `BasePipelineStep`/
`PipelineStep` (no event-decorator support for async steps).

Every `IAsyncPipelineStep<,>.ProcessAsync` requires a trailing `CancellationToken` (no default — avoids the C#
"interface default parameter resolved by the static reference type" foot-gun). `Steps` on the async side is
`Func<TInput, CancellationToken, Task<TOutput>>`. `AddStep` and the concrete `AsyncPipeline`/
`BaseAsyncPipeline.ProcessAsync` take an **optional** `CancellationToken cancellationToken = default` instead (safe
on non-interface members), so `.AddStep(step)` and `await pipeline.ProcessAsync(request)` without a token still
compile — cancellation only actually propagates if the pipeline's lambda threads the token through each
`.AddStep(..., ct)` call.

### Composition via extension methods (`Pipelines.Extensions` namespace)

- `PipelineStepEventExtensions.AddStep` — fluent sync composition: `input.AddStep(step)`, with overloads for 1–3
  extra args, plus an overload taking `inputEvent`/`outputEvent` delegates that wraps the step in a `PipelineStep<,>`
  decorator on the fly (used for ad hoc logging around a single step, see `LavadoCompleto.cs`).
- `PipelineStepAsyncExtensions.AddStep` — same shape for `IAsyncPipelineStep<,>`, returning `Task<TOutput>` (no
  event-decorator overload).

A pipeline is defined by passing `Steps` — a chain of `.AddStep(...)` calls starting from the input — to the base
constructor, e.g.:

```csharp
public MyPipeline() : base(input => input
    .AddStep(new Step1())
    .AddStep(new Step2(), someExtraArg)
    .AddStep(new Step3(), i => Log(i), o => Log(o)))
{
}
```

Because each `AddStep` is just an extension method resolved by the generic types involved, **type mismatches between
consecutive steps are compile errors**, not runtime errors — this is the central design guarantee of the library.

## Design flaws (fixed) and remaining ones

The following were found during a design review and have since been fixed:

- ~~Extra-argument steps violated LSP~~ — **fixed**: `IPipelineStep<TInput, TParam1, TOutput>` (and the async
  equivalent) no longer extend the base single-arg interface, so implementers no longer need a throwing
  single-arg `Process` override just to satisfy the type.
- ~~Async steps didn't chain~~ / ~~async extra-argument interfaces inherited the wrong base~~ — **fixed**:
  `PipelineStepAsyncExtensions` has `Task<TInput>`-receiver overloads so `.AddStep(a).AddStep(b)` chains (see
  `tests/Pipelines.UnitTests/AsyncPipelineTests.cs` for the regression test).
- ~~`tests/api`'s DI sample double-container anti-pattern~~ — **fixed**: `AddBookingPipelines`/`AddBookingPipelines2`
  in `ServiceCollectionExtensions` now assemble the pipeline inside the `AddSingleton<...>(sp => ...)` factory
  against the real container, instead of calling `services.BuildServiceProvider()` to build a second one.
- ~~Sample bugs in `tests/api`~~ — **fixed**: `GetResponseStep`/`AggregateStep` now use a `DeterministicHash` helper
  instead of `string.GetHashCode()` (which is randomized per process on .NET Core), `AddBookingPipelines2` now
  actually registers the pipeline it builds, and `SearchController.Get` is declared `ActionResult<HubResponse>`
  instead of `ActionResult<float>`.
- ~~Mutable `Steps`~~ — **fixed**: `BasePipeline<TInput, TOutput>.Steps`/`BaseAsyncPipeline<TInput, TOutput>.Steps`
  are now `{ get; }`-only, required via a `protected` constructor with a null-check. Every pipeline subclass in this
  repo (`Pipeline<,>`, `AsyncPipeline<,>`, `SubPipeline`, `LavadoCompleto`) now passes `Steps` through `base(...)`
  instead of assigning it in the constructor body.
- ~~No cancellation support~~ — **fixed**: see the "Async side" section above for the final shape (required on the
  interface, optional on `AddStep`/`ProcessAsync`). See `tests/Pipelines.UnitTests/AsyncPipelineTests.cs` for the
  cancellation-propagation regression test.

Remaining, lower-severity issues (not fixed, by design — see commit history / plan for rationale):

- **No error-handling primitive.** Any step throwing aborts the whole pipeline; there's no built-in way to
  short-circuit gracefully (e.g. a `Result<T>`-style step or a catch-and-continue mode), so this pattern is best
  suited to pipelines where a thrown exception is an acceptable failure mode for the whole chain.
- **Arity explosion.** Supporting 0–3 extra parameters requires 4 sync interfaces, 4 async interfaces, and 4
  extension-method overloads per `AddStep` family (8 total) — adding a 4th parameter means repeating all of it again.
  Accepted trade-off of the current design, not something to "fix" without a larger rethink.

### Extra-argument steps and sub-pipelines together

The pattern seen in `ServiceCollectionExtensions.AddBookingPipelines` is the idiomatic way to pass outer context into
a nested pipeline: steps inside the sub-pipeline accept the outer input (`hubRq`) as an extra arg via
`IPipelineStep<TInput, TParam1, TOutput>`, and the nested `Pipeline<,>` itself is just another step added with
`.AddStep(...)` on the outer chain.
