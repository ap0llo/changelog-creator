﻿using System.Linq;
using System.Threading.Tasks;
using Grynwald.ChangeLog.Model;
using Grynwald.ChangeLog.Pipeline;
using Grynwald.ChangeLog.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Xunit;

namespace Grynwald.ChangeLog.Test.Pipeline
{
    /// <summary>
    /// Tests for <see cref="ChangeLogPipeline"/>
    /// </summary>
    public class ChangeLogPipelineTest
    {
        private readonly ILogger<ChangeLogPipeline> m_Logger = NullLogger<ChangeLogPipeline>.Instance;


        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(10)]
        public async Task Run_executes_all_tasks_in_the_insertion_order(int numberOfTasks)
        {
            // ARRANGE
            var tasks = Enumerable.Range(0, numberOfTasks)
                .Select(_ =>
                {
                    var mock = new Mock<IChangeLogTask>(MockBehavior.Strict);
                    mock.Setup(x => x.RunAsync(It.IsAny<ApplicationChangeLog>())).Returns(Task.FromResult(ChangeLogTaskResult.Success));
                    return mock;
                })
                .ToArray();

            var sut = new ChangeLogPipeline(m_Logger, tasks.Select(x => x.Object));

            // ACT 
            var result = await sut.RunAsync();

            // ASSERT
            Assert.NotNull(result);
            Assert.True(result.Success);
            Assert.NotNull(result.Value);
            Assert.All(tasks, task => task.Verify(x => x.RunAsync(It.IsAny<ApplicationChangeLog>()), Times.Once));
        }

        [Fact]
        public async Task Run_aborts_execution_if_a_task_fails()
        {
            // ARRANGE
            var tasks = Enumerable.Range(0, 3)
                .Select(_ =>
                {
                    var mock = new Mock<IChangeLogTask>(MockBehavior.Strict);
                    mock.Setup(x => x.RunAsync(It.IsAny<ApplicationChangeLog>())).Returns(Task.FromResult(ChangeLogTaskResult.Success));
                    return mock;
                })
                .ToArray();

            tasks[1].Setup(x => x.RunAsync(It.IsAny<ApplicationChangeLog>())).Returns(Task.FromResult(ChangeLogTaskResult.Error));

            var sut = new ChangeLogPipeline(m_Logger, tasks.Select(x => x.Object));

            // ACT 
            var result = await sut.RunAsync();

            // ASSERT
            Assert.NotNull(result);
            Assert.False(result.Success);
            tasks[0].Verify(x => x.RunAsync(It.IsAny<ApplicationChangeLog>()), Times.Once);
            tasks[1].Verify(x => x.RunAsync(It.IsAny<ApplicationChangeLog>()), Times.Once);
            tasks[2].Verify(x => x.RunAsync(It.IsAny<ApplicationChangeLog>()), Times.Never);
        }

        [Fact]
        public async Task Run_continues_execution_if_a_task_is_skipped()
        {
            // ARRANGE
            var tasks = Enumerable.Range(0, 3)
                .Select(_ =>
                {
                    var mock = new Mock<IChangeLogTask>(MockBehavior.Strict);
                    mock.Setup(x => x.RunAsync(It.IsAny<ApplicationChangeLog>())).Returns(Task.FromResult(ChangeLogTaskResult.Success));
                    return mock;
                })
                .ToArray();

            tasks[1].Setup(x => x.RunAsync(It.IsAny<ApplicationChangeLog>())).Returns(Task.FromResult(ChangeLogTaskResult.Skipped));

            var sut = new ChangeLogPipeline(m_Logger, tasks.Select(x => x.Object));

            // ACT 
            var result = await sut.RunAsync();

            // ASSERT
            Assert.NotNull(result);
            Assert.True(result.Success);
            Assert.NotNull(result.Value);
            Assert.All(tasks, task => task.Verify(x => x.RunAsync(It.IsAny<ApplicationChangeLog>()), Times.Once));
        }
    }
}
