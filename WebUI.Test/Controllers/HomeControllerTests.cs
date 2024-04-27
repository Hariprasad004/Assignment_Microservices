using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using Moq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using WebUI.Service.IService;
using WebUI.Controllers;
using WebUI.Models;
using System.ComponentModel.DataAnnotations;
using System.IO;
using AutoFixture;
using System.IO.Abstractions.TestingHelpers;
using AutoFixture.AutoMoq;
using Microsoft.VisualStudio.TestPlatform.CommunicationUtilities;
using static WebUI.Utility.SD;
using WebUI.Utility;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using FluentAssertions;
using System.Runtime.CompilerServices;

namespace WebUI.Test.Controllers
{
    public class HomeControllerTests
    {
        private readonly IFixture _fixture;
        public HomeControllerTests()
        {
            _fixture = new Fixture();
            _fixture.Customize(new AutoMoqCustomization());
        }

        //Register
        [Fact]
        public async Task Register_ReturnsView_WhenModelStateIsInvalid()
        {
            // Arrange
            var authServiceMock = new Mock<IAuthService>();
            var tokenProviderMock = new Mock<ITokenProvider>();
            var controller = new HomeController(authServiceMock.Object, tokenProviderMock.Object);
            controller.ModelState.AddModelError("Error", "ModelState is invalid");

            // Act
            var result = await controller.Register(new RegRequestDto());

            // Assert
            var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Error", redirectToActionResult.ActionName);
        }

        [Fact]
        public async Task Register_RedirectsToLogin_WhenRegistrationSuccessful()
        {
            // Arrange
            var authServiceMock = new Mock<IAuthService>();
            var tokenProviderMock = new Mock<ITokenProvider>();

            var httpContext = new DefaultHttpContext();
            var tempData = new TempDataDictionary(httpContext, Mock.Of<ITempDataProvider>());
            authServiceMock.Setup(x => x.SendAsync(It.IsAny<RequestDto>(), It.IsAny<bool>())).
                ReturnsAsync(new ResponseDto()
                {
                    IsSuccess = true,
                    Result = null,
                    Message = "Success"
                });

            var controller = new HomeController(authServiceMock.Object, tokenProviderMock.Object)
            {
                TempData = tempData //mocking tempdata
            };
            var mockFormFile = new Mock<IFormFile>();
            var RegDto = _fixture.Build<RegRequestDto>()
            .With(x=> x.Image, mockFormFile.Object)
            .Create();

            // Act
            var result = await controller.Register(RegDto);

            // Assert
            var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Login", redirectToActionResult.ActionName);
        }

        [Fact]
        public async Task Register_ReturnsViewWithError_WhenRegistrationFails()
        {
            // Arrange
            var authServiceMock = new Mock<IAuthService>();
            var tokenProviderMock = new Mock<ITokenProvider>();

            var httpContext = new DefaultHttpContext();
            var tempData = new TempDataDictionary(httpContext, Mock.Of<ITempDataProvider>());
            authServiceMock.Setup(x => x.SendAsync(It.IsAny<RequestDto>(), It.IsAny<bool>())).
                ReturnsAsync(new ResponseDto()
                {
                    IsSuccess = false,
                    Message = "Registration failed"
                });
            var controller = new HomeController(authServiceMock.Object, tokenProviderMock.Object)
            {
                TempData=tempData
            };
            var mockFormFile = new Mock<IFormFile>();
            var RegDto = _fixture.Build<RegRequestDto>()
            .With(x => x.Image, mockFormFile.Object)
            .Create();

            // Act
            var result = await controller.Register(RegDto) as ViewResult;

            // Assert
            //hre cr viewname matching reqd'
            Assert.Equal("Registration failed", controller.TempData["error"]);
        }

        //Login
        [Fact]
        public async Task Login_ReturnsView_WhenModelStateIsInvalid()
        {
            // Arrange
            var authServiceMock = new Mock<IAuthService>();
            var tokenProviderMock = new Mock<ITokenProvider>();
            var controller = new HomeController(authServiceMock.Object, tokenProviderMock.Object);
            controller.ModelState.AddModelError("Error", "ModelState is invalid");

            // Act
            var result = await controller.Login(new LoginRequestDto());

            // Assert
            var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Error", redirectToActionResult.ActionName);
        }
        [Fact]
        public async Task Login_RedirectsToLoginSucc_WhenLoginSuccessful()
        {
            // Arrange
            var authServiceMock = new Mock<IAuthService>();
            var tokenProviderMock = new Mock<ITokenProvider>();

            authServiceMock.Setup(x => x.SendAsync(It.IsAny<RequestDto>(), It.IsAny<bool>())).
                ReturnsAsync(new ResponseDto()
                {
                    IsSuccess = true,
                    Message = "Success"
                });

            authServiceMock.Setup(x => x.SignInUser(It.IsAny<AuthResponseDto>()));
            tokenProviderMock.Setup(x => x.SetToken(It.IsAny<string>()));

            var controller = new HomeController(authServiceMock.Object, tokenProviderMock.Object);

            // Act
            var result = await controller.Login(new LoginRequestDto());

            // Assert
            var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("LoginSuccessful", redirectToActionResult.ActionName);
            Assert.Equal("Home", redirectToActionResult.ControllerName);

        }

        [Fact]
        public async Task Login_ReturnsViewWithError_WhenloginFails()
        {
            // Arrange
            var authServiceMock = new Mock<IAuthService>();
            var tokenProviderMock = new Mock<ITokenProvider>();

            var httpContext = new DefaultHttpContext();
            var tempData = new TempDataDictionary(httpContext, Mock.Of<ITempDataProvider>());
            authServiceMock.Setup(x => x.SendAsync(It.IsAny<RequestDto>(), It.IsAny<bool>())).
                ReturnsAsync(new ResponseDto()
                {
                    IsSuccess = false,
                    Message = "Login failed"
                });
            var controller = new HomeController(authServiceMock.Object, tokenProviderMock.Object)
            {
                TempData = tempData
            };
            

            // Act
            var result = await controller.Login(new LoginRequestDto()) as ViewResult;

            // Assert
            //hre cr viewname matching reqd'
            Assert.Equal("Login failed", controller.TempData["error"]);
        }


    }

}
