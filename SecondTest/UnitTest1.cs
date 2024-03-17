using Microsoft.AspNetCore.Mvc;
using System.Net;
using second_2.Controllers;
using second_2.Service;
using System.Collections.Generic;
using NUnit.Framework;
using System.Net.Http;
using System.Threading.Tasks;
using Moq;
using Moq.Protected;
using System.Threading;

namespace SecondTest
{
    public class Tests
    {
        public class RemoteTaskControllerTests
        {
            [Test]
            public async Task GetTasksFromServer_ReturnsTasks()
            {
                // Arrange
                var expectedTasks = new List<TaskModel>
            {
                new TaskModel { Question = "Question1", Answer = "Answer1" },
                new TaskModel { Question = "Question2", Answer = "Answer2" }
            };

                var mockHttpMessageHandler = new Mock<HttpMessageHandler>(MockBehavior.Strict);
                mockHttpMessageHandler
                    .Protected()
                    .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
                    .ReturnsAsync(new HttpResponseMessage
                    {
                        StatusCode = HttpStatusCode.OK,
                        Content = new StringContent("[{\"Question\":\"Question1\",\"Answer\":\"Answer1\"},{\"Question\":\"Question2\",\"Answer\":\"Answer2\"}]"),
                    });

                var httpClient = new HttpClient(mockHttpMessageHandler.Object);
                var controller = new RemoteTaskController(httpClient);

                // Act
                var result = await controller.GetTasksFromServer() as OkObjectResult;

                // Assert
                Assert.NotNull(result);
                Assert.AreEqual((int)HttpStatusCode.OK, result.StatusCode);

                var tasks = result.Value as List<TaskModel>;
                Assert.NotNull(tasks);
                Assert.AreEqual(expectedTasks.Count, tasks.Count);
                for (int i = 0; i < expectedTasks.Count; i++)
                {
                    Assert.AreEqual(expectedTasks[i].Question, tasks[i].Question);
                    Assert.AreEqual(expectedTasks[i].Answer, tasks[i].Answer);
                }
            }
        }

    }
}
    