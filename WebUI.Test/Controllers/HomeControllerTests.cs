using Moq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using WebUI.Service.IService;
using WebUI.Controllers;
using WebUI.Models;
using AutoFixture;
using AutoFixture.AutoMoq;
using Microsoft.AspNetCore.Mvc.ViewFeatures;

namespace WebUI.Test.Controllers
{
    public class HomeControllerTests
    {
        private readonly IFixture _fixture;
        private readonly Mock<IAuthService> _authServiceMock;
        private readonly Mock<ITokenProvider> _tokenProviderMock;
        public HomeControllerTests()
        {
            _fixture = new Fixture();
            _fixture.Customize(new AutoMoqCustomization());
            _authServiceMock = new Mock<IAuthService>();
            _tokenProviderMock= new Mock<ITokenProvider>();
        }

        //--------------------Register---------------------
        [Fact]
        public async Task Register_ReturnsView_WhenModelStateIsInvalid()
        {
            // Arrange
            var controller = new HomeController(_authServiceMock.Object, _tokenProviderMock.Object);
            controller.ModelState.AddModelError("Error", "ModelState is invalid");

            // Act
            var result = await controller.Register(new RegisterRequestDto());

            // Assert
            var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Error", redirectToActionResult.ActionName);
        }

        [Fact]
        public async Task Register_RedirectsToLogin_WhenRegistrationSuccessful()
        {
            // Arrange
            var httpContext = new DefaultHttpContext();
            var tempData = new TempDataDictionary(httpContext, Mock.Of<ITempDataProvider>());
            _authServiceMock.Setup(x => x.SendAsync(It.IsAny<RequestDto>(), It.IsAny<bool>())).
                ReturnsAsync(new ResponseDto()
                {
                    IsSuccess = true,
                    Result = null,
                    Message = "Success"
                });

            var controller = new HomeController(_authServiceMock.Object, _tokenProviderMock.Object)
            {
                TempData = tempData //mocking tempdata
            };
            var mockFormFile = new Mock<IFormFile>();
            var RegDto = _fixture.Build<RegisterRequestDto>()
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
            var httpContext = new DefaultHttpContext();
            var tempData = new TempDataDictionary(httpContext, Mock.Of<ITempDataProvider>());
            _authServiceMock.Setup(x => x.SendAsync(It.IsAny<RequestDto>(), It.IsAny<bool>())).
                ReturnsAsync(new ResponseDto()
                {
                    IsSuccess = false,
                    Message = "Registration failed"
                });
            var controller = new HomeController(_authServiceMock.Object, _tokenProviderMock.Object)
            {
                TempData=tempData
            };
            var mockFormFile = new Mock<IFormFile>();
            var RegDto = _fixture.Build<RegisterRequestDto>()
            .With(x => x.Image, mockFormFile.Object)
            .Create();

            // Act
            var result = await controller.Register(RegDto) as ViewResult;

            // Assert
            Assert.Equal("Registration failed", controller.TempData["error"]);
        }

        //----------------------Login------------------------
        [Fact]
        public async Task Login_ReturnsView_WhenModelStateIsInvalid()
        {
            // Arrange
            var controller = new HomeController(_authServiceMock.Object, _tokenProviderMock.Object);
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
            _authServiceMock.Setup(x => x.SendAsync(It.IsAny<RequestDto>(), It.IsAny<bool>())).
                ReturnsAsync(new ResponseDto()
                {
                    IsSuccess = true,
                    Message = "Success"
                });

            _authServiceMock.Setup(x => x.SignInUser(It.IsAny<AuthResponseDto>()));
            _tokenProviderMock.Setup(x => x.SetToken(It.IsAny<string>()));

            var controller = new HomeController(_authServiceMock.Object, _tokenProviderMock.Object);

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
            var httpContext = new DefaultHttpContext();
            var tempData = new TempDataDictionary(httpContext, Mock.Of<ITempDataProvider>());
            _authServiceMock.Setup(x => x.SendAsync(It.IsAny<RequestDto>(), It.IsAny<bool>())).
                ReturnsAsync(new ResponseDto()
                {
                    IsSuccess = false,
                    Message = "Login failed"
                });
            var controller = new HomeController(_authServiceMock.Object, _tokenProviderMock.Object)
            {
                TempData = tempData
            };
            

            // Act
            var result = await controller.Login(new LoginRequestDto()) as ViewResult;

            // Assert
            Assert.Equal("Login failed", controller.TempData["error"]);
        }
    }

}
