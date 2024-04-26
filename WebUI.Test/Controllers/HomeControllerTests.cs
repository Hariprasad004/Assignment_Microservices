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

            var request = _fixture.Create<RequestDto>();
            authServiceMock.Setup(x => x.SendAsync(request, true)).
                ReturnsAsync(new ResponseDto() { 
                    IsSuccess = true,
                    Result = null,
                    Message = "Success"
                });
            var controller = new HomeController(authServiceMock.Object, tokenProviderMock.Object);

            // Act
            var mockFormFile = new Mock<IFormFile>();
            var RegDto = _fixture.Build<RegRequestDto>()
            .With(x=> x.Image, mockFormFile.Object)
            .Create();

            var result = await controller.Register(RegDto);

            // Assert
            var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Login", redirectToActionResult.ActionName);
        }

    }
    //[Fact]
    //public async Task Register_ReturnsViewWithError_WhenRegistrationFails()
    //{
    //    // Arrange
    //    var authServiceMock = new Mock<IAuthService>();
    //    authServiceMock.Setup(auth => auth.SendAsync(It.IsAny<ResponseDto>())).ReturnsAsync(new ResponseDto { IsSuccess = false, Message = "Registration failed" });
    //    var tokenProviderMock = new Mock<IAuthService>();
    //    var controller = new HomeController(authServiceMock.Object, tokenProviderMock.Object);

    //    // Act
    //    var result = await controller.Register(new RegRequestDto());

    //    // Assert
    //    var viewResult = Assert.IsType<ViewResult>(result);
    //    Assert.Equal("Register", viewResult.ViewName);
    //    Assert.Equal("Registration failed", controller.TempData["error"]);
    //}
}
