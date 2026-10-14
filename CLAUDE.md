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
  the base single-arg `Process` too (commonly throwing `NotImplementedException`/`ArgumentNullException` if called
  without the extra args — see `GetRequestStep` in `tests/api`).
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

Mirrors the sync side one-to-one: `IAsyncPipelineStep<TInput, TOutput>` (+ param overloads returning `Task<TOutput>`),
`BaseAsyncPipeline<TInput, TOutput>`, `AsyncPipeline<TInput, TOutput>`. There is no async equivalent of
`BasePipelineStep`/`PipelineStep` (no event-decorator support yet for async steps).

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

### Extra-argument steps and sub-pipelines together

The pattern seen in `ServiceCollectionExtensions.AddBookingPipelines` is the idiomatic way to pass outer context into
a nested pipeline: steps inside the sub-pipeline accept the outer input (`hubRq`) as an extra arg via
`IPipelineStep<TInput, TParam1, TOutput>`, and the nested `Pipeline<,>` itself is just another step added with
`.AddStep(...)` on the outer chain.
