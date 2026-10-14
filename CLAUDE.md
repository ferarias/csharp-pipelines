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
  context beyond the piped input (e.g. the original request from an outer/parent pipeline). These still implement
  the base single-arg `Process` too (commonly throwing if called without the extra args — see `GetRequestStep` in
  `tests/api`). This is a Liskov Substitution violation: anything holding only an `IPipelineStep<TInput, TOutput>`
  reference can call `Process(input)` and hit a runtime throw instead of a compile error — see "Design flaws" below.
- `BasePipelineStep<TInput, TOutput>` — abstract step base exposing `OnInput`/`OnOutput` events around an
  abstract `ProcessStep`, for subclassing when you want built-in event hooks without wiring them externally.
- `PipelineStep<TInput, TOutput>` — a decorator wrapping any `IPipelineStep<,>` to add `OnInput`/`OnOutput` events
  externally, without requiring the inner step to subclass anything.
- `BasePipeline<TInput, TOutput>` — implements `IPipelineStep<TInput, TOutput>` itself by holding a
  `Func<TInput, TOutput> Steps` and delegating `Process` to it. **This is why a `Pipeline<,>` can be used as a step
  inside another pipeline** (sub-pipelines, see `SubPipeline.cs` and the nested `Pipeline<ConnectorRequest,...>` in
  `ServiceCollectionExtensions`).
- `Pipeline<TInput, TOutput>` — concrete `BasePipeline<,>` whose constructor takes the `Steps` function directly.

### Async side

Mirrors the sync side one-to-one: `IAsyncPipelineStep<TInput, TOutput>` (+ `TParam1`/`TParam2`/`TParam3` overloads,
all extending `IAsyncPipelineStep<TInput, TOutput>`, returning `Task<TOutput>`), `BaseAsyncPipeline<TInput, TOutput>`,
`AsyncPipeline<TInput, TOutput>`. `PipelineStepAsyncExtensions.AddStep` has two overload sets per arity — one taking
`this TInput` (for the first step in a chain) and one taking `this Task<TInput>` (awaiting it before handing the
result to the next step) — which is what lets multiple async steps chain with the same fluent
`.AddStep(a).AddStep(b)` syntax as the sync side. There is still no async equivalent of `BasePipelineStep`/
`PipelineStep` (no event-decorator support for async steps), and no `CancellationToken` support.

### Composition via extension methods (`Pipelines.Extensions` namespace)

- `PipelineStepEventExtensions.AddStep` — fluent sync composition: `input.AddStep(step)`, with overloads for 1–3
  extra args, plus an overload taking `inputEvent`/`outputEvent` delegates that wraps the step in a `PipelineStep<,>`
  decorator on the fly (used for ad hoc logging around a single step, see `LavadoCompleto.cs`).
- `PipelineStepAsyncExtensions.AddStep` — same shape for `IAsyncPipelineStep<,>`, returning `Task<TOutput>` (no
  event-decorator overload).

A pipeline is defined by assigning `Steps` to a chain of `.AddStep(...)` calls starting from the input, e.g.:

```csharp
Steps = input => input
    .AddStep(new Step1())
    .AddStep(new Step2(), someExtraArg)
    .AddStep(new Step3(), i => Log(i), o => Log(o));
```

Because each `AddStep` is just an extension method resolved by the generic types involved, **type mismatches between
consecutive steps are compile errors**, not runtime errors — this is the central design guarantee of the library.
It only holds for the base `IPipelineStep<TInput, TOutput>`/`IAsyncPipelineStep<TInput, TOutput>` shape, though:
chaining an extra-argument step via its base single-arg `Process` still compiles and throws at runtime (see below).

## Design flaws

- **Extra-argument steps violate LSP.** `IPipelineStep<TInput, TParam1, TOutput>` extends
  `IPipelineStep<TInput, TOutput>`, forcing implementers to also provide a single-arg `Process` that commonly just
  throws (`GetRequestStep.cs`, `GetResponseStep.cs` in `tests/api` both throw `ArgumentNullException` from it — the
  wrong exception type for "this overload isn't supported" as well). Nothing stops `.AddStep(stepWithArgs)` (omitting
  the extra arg) from compiling and failing at runtime. Since `Steps` is just a lambda that can close over outer
  variables, extra-argument steps are usually unnecessary — closing over the value directly avoids the whole problem.
  *Proposed fix (not yet applied):* drop the `: IPipelineStep<TInput, TOutput>` base from the `TParam1`/`TParam2`/
  `TParam3` interfaces (sync and async) entirely — `AddStep`'s param-arity overloads only require the param-arity
  interface directly, so nothing in this repo depends on that inheritance (confirmed via grep).
- ~~Async steps don't chain~~ / ~~async extra-argument interfaces inherit the wrong base~~ — **fixed**:
  `PipelineStepAsyncExtensions` now has `Task<TInput>`-receiver overloads so `.AddStep(a).AddStep(b)` chains, and the
  `IAsyncPipelineStep<TInput, TParamN, TOutput>` interfaces now extend `IAsyncPipelineStep<TInput, TOutput>` instead
  of the sync interface. See the "Async side" section above and `tests/Pipelines.UnitTests/AsyncPipelineTests.cs`
  for the regression test.
- **No cancellation support** anywhere in the async API (`ProcessAsync` takes no `CancellationToken`).
- **Mutable `Steps`.** `BasePipeline<TInput, TOutput>.Steps` has a `protected set`, so a pipeline instance shared as
  a DI singleton (as in `tests/api`) could have its behavior swapped out from under callers by any subclass code
  path that reassigns it after construction. There's also no null-check — a subclass that forgets to set `Steps`
  fails with a `NullReferenceException` inside `Process`, not at construction.
- **No error-handling primitive.** Any step throwing aborts the whole pipeline; there's no built-in way to
  short-circuit gracefully (e.g. a `Result<T>`-style step or a catch-and-continue mode), so this pattern is best
  suited to pipelines where a thrown exception is an acceptable failure mode for the whole chain.
- **Arity explosion.** Supporting 0–3 extra parameters requires 4 sync interfaces, 4 async interfaces, and 4
  extension-method overloads per `AddStep` family (8 total) — adding a 4th parameter means repeating all of it again.
- **`tests/api`'s DI sample is an anti-pattern.** `ServiceCollectionExtensions.AddBookingPipelines` calls
  `services.BuildServiceProvider()` inside `ConfigureServices` to resolve loggers before building the pipeline by
  hand with `new`. Building a second temporary container like this is a known ASP.NET Core anti-pattern (resolved
  services can end up distinct from the ones the real container later creates) — don't copy this pattern.
- **Sample bugs worth knowing about if you touch `tests/api`:** `GetResponseStep` and `AggregateStep` use
  `string.GetHashCode()` for dictionary keys/values, which is randomized per process on .NET Core — non-deterministic
  and a potential `ArgumentException` on key collisions in `ToDictionary`. `AddBookingPipelines2` builds a pipeline
  and never registers or returns it. `SearchController.Get` is declared `ActionResult<float>` but returns a
  `HubResponse`.

### Extra-argument steps and sub-pipelines together

The pattern seen in `ServiceCollectionExtensions.AddBookingPipelines` is the idiomatic way to pass outer context into
a nested pipeline: steps inside the sub-pipeline accept the outer input (`hubRq`) as an extra arg via
`IPipelineStep<TInput, TParam1, TOutput>`, and the nested `Pipeline<,>` itself is just another step added with
`.AddStep(...)` on the outer chain.
