using Moq;
using Pipelines.Extensions;
using Xunit;

namespace Pipelines.UnitTests
{
    public class UnitTest1
    {
        [Fact]
        public void Pipeline3steps_Execute_AllStepsAreInvokedOnce()
        {
            // Arrange
            const string input = "xxxxx";

            var step1mock = new Mock<IPipelineStep<string, int>>();
            step1mock.Setup(x => x.Process(It.IsAny<string>())).Returns(1);

            var step2mock = new Mock<IPipelineStep<int, double>>();
            step2mock.Setup(x => x.Process(It.IsAny<int>())).Returns(2d);

            var step3mock = new Mock<IPipelineStep<double, float>>();
            step3mock.Setup(x => x.Process(It.IsAny<double>())).Returns(1.3f);

            var pipeline = new Pipeline<string, float>((string input) => input
            .AddStep(step1mock.Object).AddStep(step2mock.Object).AddStep(step3mock.Object));

            // Act
            var output = pipeline.Process(input);

            // Assert
            Assert.Equal(1.3f, output);
            step1mock.Verify(x => x.Process(It.IsAny<string>()), Times.Once);
            step2mock.Verify(x => x.Process(It.IsAny<int>()), Times.Once);
            step3mock.Verify(x => x.Process(It.IsAny<double>()), Times.Once);
        }
    }

}
