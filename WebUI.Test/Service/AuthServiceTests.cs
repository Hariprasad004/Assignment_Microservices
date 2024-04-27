using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Moq.Protected;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using WebUI.Models;
using WebUI.Service;
using WebUI.Service.IService;
using WebUI.Utility;

namespace WebUI.Test.Service
{
    public class AuthServiceTests
    {
        private readonly Mock<IHttpClientFactory> _httpClientFactoryMock;
        private readonly Mock<ITokenProvider> _tokenProviderMock;
        public AuthServiceTests()
        {
            _httpClientFactoryMock = new Mock<IHttpClientFactory>();
            _tokenProviderMock = new Mock<ITokenProvider>();
        }

        [Fact]
        public async Task SendAsync_ValidRequest_ReturnsApiResponse()
        {
            // Arrange
            var msgHandler = new Mock<IHttpMessageHandlerFactory>();
            var mocked = msgHandler.Protected();

            var setupApiRequest = mocked.Setup<Task<HttpResponseMessage>>(
        "SendAsync",
        ItExpr.IsAny<HttpRequestMessage>(),
        ItExpr.IsAny<CancellationToken>()
        );
            var apiMockedResponse =
        setupApiRequest.ReturnsAsync(new HttpResponseMessage()
        {
            StatusCode = HttpStatusCode.OK,
            Content = new StringContent("mocked API response")
        });


            var httpClient = _httpClientFactoryMock.Setup(x => x.CreateClient("WebAPI"));


            //var authService = new AuthService(_httpClientFactoryMock.Object, _tokenProviderMock.Object);
            ////Act
            //var result = await authService.SendAsync(new RequestDto()
            //{
            //    Url= SD.AuthAPI + "/api/auth/Register",
            //    AccessToken="test",
            //    Data=null
            //});


            //result.Result.As<OkObjectResult>().Value
            //    .Should()
            //    .NotBeNull();
        }
    }
}
