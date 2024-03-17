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
                var mockHttpMessageHandler = new Mock<HttpMessageHandler>();
                mockHttpMessageHandler
                    .Protected()
                    .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
                    .ReturnsAsync(new HttpResponseMessage
                    {
                        StatusCode = HttpStatusCode.OK,
                        Content = new StringContent("[{\"Question\":\"Question1\",\"Answer\":\"Answer1\"},{\"Question\":\"Question2\",\"Answer\":\"Answer2\"}]")
                    });

                var httpClient = new HttpClient(mockHttpMessageHandler.Object);
                var controller = new RemoteTaskController(httpClient);

                // Act
                var result = await controller.GetTasksFromServer();

                // Assert
                Assert.NotNull(result);
                Assert.IsInstanceOf<Microsoft.AspNetCore.Mvc.OkObjectResult>(result);

                var okResult = (Microsoft.AspNetCore.Mvc.OkObjectResult)result;
                Assert.AreEqual(200, okResult.StatusCode);

                var content = okResult.Value.ToString();
                Assert.AreEqual("[{\"Question\":\"Question1\",\"Answer\":\"Answer1\"},{\"Question\":\"Question2\",\"Answer\":\"Answer2\"}]", content);
            }
            [Test]
            public async Task GetTasksFromServer_ReturnsStatusCodeResult_WhenHttpClientReturnsError()
            {
                // Arrange
                var mockHttpMessageHandler = new Mock<HttpMessageHandler>();
                mockHttpMessageHandler
                    .Protected()
                    .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
                    .ReturnsAsync(new HttpResponseMessage
                    {
                        StatusCode = HttpStatusCode.InternalServerError
                    });

                var httpClient = new HttpClient(mockHttpMessageHandler.Object);
                var controller = new RemoteTaskController(httpClient);

                // Act
                var result = await controller.GetTasksFromServer();

                // Assert
                Assert.IsInstanceOf<ObjectResult>(result);
                var statusCodeResult = result as ObjectResult;
                Assert.AreEqual((int)HttpStatusCode.InternalServerError, statusCodeResult.StatusCode);
            }
        }

    }
}
    