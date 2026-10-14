# csharp-pipelines
Very basic implementation of pipelines (a series of consecutive steps) in .NET

This project is strongly based upon the works by Jeremy Davis [An Alternative Approach to Pipelines](https://jermdavis.wordpress.com/2016/10/03/an-alternative-approach-to-pipelines/)

## Creating a basic pipeline

First we need to create some steps. You can use whatever type you need. Usually you will use some complex types instead of these CLR types.

The only thing you must keep in mind is that the output type of one step must match the input type of the following.

Of course, the **input type** of the **first** step must match the input type of the whole pipeline, and the **output type** of the **last** step must match the output type of the whole pipeline.

```csharp
public class Step1 : IPipelineStep<string, string>
{
    public string Process(string input)
    {
        return input.ToUpper();
    }
}

public class Step2 : IPipelineStep<string, int>
{
    public int Process(string input)
    {
        return int.Parse(input);
    }
}

public class Step3 : IPipelineStep<int, double>
{
    public double Process(int input)
    {
        return (double)input;
    }
}
```

Then we build the pipeline by combining the steps in order.

```csharp
public class MyPipeline : Pipeline<string, double>
{
    public MyPipeline() : base(input => input
        .AddStep(new Step1())
        .AddStep(new Step2())
        .AddStep(new Step3()))
    {
    }
}
```

Now we call the pipeline

```csharp
var pipeline = new MyPipeline();

Log.Information("Input Value: {value} [{type}]", input, input.GetType().Name);

var output = pipeline.Process(input);

Log.Information("Output Value: {value} [{type}]", output, output.GetType().Name);
```

That's it, actually. Browse the code and suggest whatever you'd like to implement here.

## Steps with built-in event hooks

Instead of implementing `IPipelineStep<TInput, TOutput>` directly, a step can derive from `BasePipelineStep<TInput, TOutput>`
and implement `ProcessStep` instead of `Process`. This gives the step `OnInput`/`OnOutput` events that fire automatically
around the call, without the step's own logic having to know about them:

```csharp
public class Lavado : BasePipelineStep<string, string>
{
    protected override string ProcessStep(string input)
    {
        return input.Replace(" ", "");
    }
}
```

If you want to hook into `OnInput`/`OnOutput` for a step you don't control (one that only implements `IPipelineStep<,>`),
`AddStep` has an overload that wraps it in a `PipelineStep<TInput, TOutput>` decorator on the fly:

```csharp
Steps = input => input
    .AddStep(new Prelavado())
    .AddStep(new Aclarado(),
        i => Log.Information("Pre-aclarado '{input}'...", i),
        o => Log.Information("Post-aclarado '{output}'...", o));
```

## Steps that need extra arguments

Sometimes a step needs more than just the piped input — for example, context from the original request of an outer
pipeline. For this, implement `IPipelineStep<TInput, TParam1, TOutput>` (overloads exist up to three extra
parameters) instead of the base `IPipelineStep<TInput, TOutput>`:

```csharp
public class GetRequestStep : IPipelineStep<ConnectorRequest, HubRequest, ProviderRequest>
{
    public ProviderRequest Process(ConnectorRequest connectorRequest, HubRequest hubRequest)
    {
        return new ProviderRequest
        {
            Id = connectorRequest.Id,
            Properties = connectorRequest.Properties.Select(x => $"{hubRequest.HubExtraInfo}-{x}")
        };
    }
}
```

`IPipelineStep<TInput, TParam1, TOutput>` does **not** extend the base `IPipelineStep<TInput, TOutput>`, so a step
like this only needs the one `Process` overload that actually makes sense for it — there's no forced single-argument
`Process` to implement (and throw from) just to satisfy the interface.

Pass the extra argument(s) when wiring the step into the pipeline:

```csharp
Steps = input => input
    .AddStep(getRequestStep, hubRequest)
    .AddStep(getResponseStep, hubRequest)
    .AddStep(aggregateStep);
```

Note: since `Steps` is a plain lambda, `hubRequest` here is just a captured closure variable — in most cases it's
simpler to close over it directly inside a step than to thread it through as an extra pipeline argument.

## Sub-pipelines

Since `BasePipeline<TInput, TOutput>` (and therefore `Pipeline<TInput, TOutput>`) implements `IPipelineStep<TInput, TOutput>`,
a whole pipeline can be used as a single step inside another pipeline:

```csharp
public class SubPipeline : BasePipeline<string, string>
{
    public SubPipeline() : base(input => input
        .AddStep(new Prelavadito())
        .AddStep(new Lavadito()))
    {
    }
}

public class LavadoCompleto : BasePipeline<string, int>
{
    public LavadoCompleto() : base(input => input
        .AddStep(new Prelavado())
        .AddStep(new Lavado())
        .AddStep(new SubPipeline())
        .AddStep(new Aclarado())
        .AddStep(new Centrifugado()))
    {
    }
}
```

This composes naturally with steps that take extra arguments, letting a sub-pipeline's steps access the outer
pipeline's original input:

```csharp
var searchPipeline = new Pipeline<HubRequest, HubResponse>(hubRequest => hubRequest
    .AddStep(mapping)
    // Sub-pipeline whose steps also need the outer HubRequest
    .AddStep(new Pipeline<ConnectorRequest, ConnectorResponse>(connectorRequest => connectorRequest
        .AddStep(getRequest, hubRequest)
        .AddStep(getResponse, hubRequest)
        .AddStep(aggregate)))
    .AddStep(dedupe));
```

## Async pipelines

For asynchronous steps, use `IAsyncPipelineStep<TInput, TOutput>` (with the same 1-to-3 extra-parameter overloads as
the sync side) and `AsyncPipeline<TInput, TOutput>` instead of their synchronous counterparts. Each step's
`ProcessAsync` returns a `Task<TOutput>`, and `AddStep` composes them the same way, awaiting each step's result
before handing it to the next:

```csharp
public class MappingStep : IAsyncPipelineStep<HubRequest, ConnectorRequest>
{
    public async Task<ConnectorRequest> ProcessAsync(HubRequest hubRequest)
    {
        await Task.Delay(1000).ConfigureAwait(false);
        return new ConnectorRequest { Id = hubRequest.Id };
    }
}

public class DedupeStep : IAsyncPipelineStep<ConnectorRequest, HubResponse>
{
    public async Task<HubResponse> ProcessAsync(ConnectorRequest connectorRequest)
    {
        await Task.Delay(1000).ConfigureAwait(false);
        return new HubResponse { Id = connectorRequest.Id };
    }
}

var pipeline = new AsyncPipeline<HubRequest, HubResponse>(hubRequest => hubRequest
    .AddStep(new MappingStep())
    .AddStep(new DedupeStep()));

var output = await pipeline.ProcessAsync(request);
```

Async steps still have no equivalent to `BasePipelineStep`/`PipelineStep` event hooks, and `ProcessAsync` takes no
`CancellationToken`.

## Dependency injection

Because a pipeline is just an object implementing `IPipelineStep<TInput, TOutput>`, it can be built once and
registered in a DI container like any other service, then injected wherever it's needed. The sample below (from
`tests/api`) assembles the pipeline inside the `AddSingleton<...>(sp => ...)` factory delegate, resolving each
step's dependencies from the real container at the point the pipeline is first requested — avoiding the classic
ASP.NET Core "second, temporary `BuildServiceProvider()` container" anti-pattern.

```csharp
public static IServiceCollection AddBookingPipelines(this IServiceCollection services)
{
    return services.AddSingleton<IPipelineStep<HubRequest, HubResponse>>(sp =>
    {
        var mapping = new MappingStep(sp.GetRequiredService<ILogger<MappingStep>>());
        var getRequest = new GetRequestStep(sp.GetRequiredService<ILogger<GetRequestStep>>());
        var getResponse = new GetResponseStep(sp.GetRequiredService<ILogger<GetResponseStep>>());
        var aggregate = new AggregateStep(sp.GetRequiredService<ILogger<AggregateStep>>());
        var dedupe = new DedupeStep(sp.GetRequiredService<ILogger<DedupeStep>>());

        return new Pipeline<HubRequest, HubResponse>(hubRq => hubRq
            .AddStep(mapping)
            .AddStep(new Pipeline<ConnectorRequest, ConnectorResponse>(connRq => connRq
                .AddStep(getRequest, hubRq)
                .AddStep(getResponse, hubRq)
                .AddStep(aggregate)))
            .AddStep(dedupe));
    });
}
```

```csharp
public class SearchController : ControllerBase
{
    private readonly IPipelineStep<HubRequest, HubResponse> _pipeline;

    public SearchController(IPipelineStep<HubRequest, HubResponse> pipeline)
    {
        _pipeline = pipeline;
    }

    [HttpGet("{id}")]
    public ActionResult<HubResponse> Get(int id)
    {
        var request = new HubRequest { Id = id, Properties = new[] { "1", "2", "3" } };
        return Ok(_pipeline.Process(request));
    }
}
```

## Project layout

- `src/Pipelines.Common` — the library itself (targets `netstandard2.0`).
- `tests/Pipelines.UnitTests` — xUnit/Moq tests exercising pipeline composition with mocked steps.
- `tests/Pipelines.FunctionalTests` — a runnable console sample (`LavadoCompletoPipeline`) wired end-to-end,
  including a sub-pipeline and event-hooked steps, logging via Serilog.
- `tests/api` — an ASP.NET Core sample showing a pipeline (with extra-argument steps and a sub-pipeline)
  registered in DI and invoked from a controller.

## Building and testing

```
dotnet build Pipelines.sln
dotnet test tests/Pipelines.UnitTests/Pipelines.UnitTests.csproj
dotnet run --project tests/Pipelines.FunctionalTests/Pipelines.FunctionalTests.csproj
```
