using System.Threading.Tasks;
using Moq;
using Pipelines.Extensions;
using Xunit;

namespace Pipelines.UnitTests
{
    public class AsyncPipelineTests
    {
        [Fact]
        public async Task AsyncPipeline3steps_Execute_AllStepsAreInvokedOnce()
        {
            // Arrange
            const string input = "xxxxx";

            var step1mock = new Mock<IAsyncPipelineStep<string, int>>();
            step1mock.Setup(x => x.ProcessAsync(It.IsAny<string>())).ReturnsAsync(1);

            var step2mock = new Mock<IAsyncPipelineStep<int, double>>();
            step2mock.Setup(x => x.ProcessAsync(It.IsAny<int>())).ReturnsAsync(2d);

            var step3mock = new Mock<IAsyncPipelineStep<double, float>>();
            step3mock.Setup(x => x.ProcessAsync(It.IsAny<double>())).ReturnsAsync(1.3f);

            var pipeline = new AsyncPipeline<string, float>((string input) => input
            .AddStep(step1mock.Object).AddStep(step2mock.Object).AddStep(step3mock.Object));

            // Act
            var output = await pipeline.ProcessAsync(input);

            // Assert
            Assert.Equal(1.3f, output);
            step1mock.Verify(x => x.ProcessAsync(It.IsAny<string>()), Times.Once);
            step2mock.Verify(x => x.ProcessAsync(It.IsAny<int>()), Times.Once);
            step3mock.Verify(x => x.ProcessAsync(It.IsAny<double>()), Times.Once);
        }
    }
}
