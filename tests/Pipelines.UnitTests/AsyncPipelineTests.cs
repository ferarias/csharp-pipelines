using System;
using System.Threading;
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
            step1mock.Setup(x => x.ProcessAsync(It.IsAny<string>(), It.IsAny<CancellationToken>())).ReturnsAsync(1);

            var step2mock = new Mock<IAsyncPipelineStep<int, double>>();
            step2mock.Setup(x => x.ProcessAsync(It.IsAny<int>(), It.IsAny<CancellationToken>())).ReturnsAsync(2d);

            var step3mock = new Mock<IAsyncPipelineStep<double, float>>();
            step3mock.Setup(x => x.ProcessAsync(It.IsAny<double>(), It.IsAny<CancellationToken>())).ReturnsAsync(1.3f);

            var pipeline = new AsyncPipeline<string, float>((string input, CancellationToken ct) => input
            .AddStep(step1mock.Object, ct).AddStep(step2mock.Object, ct).AddStep(step3mock.Object, ct));

            // Act
            var output = await pipeline.ProcessAsync(input);

            // Assert
            Assert.Equal(1.3f, output);
            step1mock.Verify(x => x.ProcessAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Once);
            step2mock.Verify(x => x.ProcessAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()), Times.Once);
            step3mock.Verify(x => x.ProcessAsync(It.IsAny<double>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        private class CancellableStep : IAsyncPipelineStep<string, string>
        {
            public async Task<string> ProcessAsync(string input, CancellationToken cancellationToken)
            {
                await Task.Delay(Timeout.Infinite, cancellationToken).ConfigureAwait(false);
                return input;
            }
        }

        [Fact]
        public async Task AsyncPipeline_CancellationTokenIsPropagated_StepObservesCancellation()
        {
            // Arrange
            var pipeline = new AsyncPipeline<string, string>((string input, CancellationToken ct) => input
                .AddStep(new CancellableStep(), ct));

            using var cts = new CancellationTokenSource();
            cts.CancelAfter(50);

            // Act & Assert
            await Assert.ThrowsAnyAsync<OperationCanceledException>(
                () => pipeline.ProcessAsync("xxxxx", cts.Token));
        }
    }
}
