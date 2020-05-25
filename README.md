# csharp-pipelines
Very basic implementation of pipelines (a series of consecutive steps) in .NET

This project is strongly based upon the works by Jeremy Davis [An Alternative Approach to Pipelines](https://jermdavis.wordpress.com/2016/10/03/an-alternative-approach-to-pipelines/)

# Creating a basic pipeline

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
        public string Process(string input)
        {
            return int.Parse(input);
        }
    }

public class Step3 : IPipelineStep<int, double>
    {
        public string Process(int input)
        {
            return (double)input;
        }
    }

```

Then we build the pipeline by combinining the steps in order.

```csharp
public class MyPipeline : Pipeline<string, int>
    {
        public MyPipeline()
        {
            Steps = input => input
            .AddStep(new Step1())
            .AddStep(new Step2())
            .AddStep(new Step3()));
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

