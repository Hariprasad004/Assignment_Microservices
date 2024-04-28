using AutoFixture;
using AutoFixture.AutoMoq;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Services.AuthAPI.Controllers;
using Services.AuthAPI.Models;
using Services.AuthAPI.Models.Dtos;
using Services.AuthAPI.Service.IService;
using WebUI.Service.IService;

namespace Services.AuthAPI.Test.Controllers
{
    public class AuthAPIControllerTests
    {
        private readonly Mock<IBaseService> _authServiceMock;
        private readonly Mock<IJwtTokenGenerator> _jwtTokenGenerator;
        private readonly Mock<IPassWordHash> _passWordHash;
        private readonly IFixture _fixture;

        public AuthAPIControllerTests()
        {
            _authServiceMock = new Mock<IBaseService>();
            _jwtTokenGenerator = new Mock<IJwtTokenGenerator>();
            _passWordHash = new Mock<IPassWordHash>();
            _fixture = new Fixture();
            _fixture.Customize(new AutoMoqCustomization());
        }

        //------------------------Register------------------------
        [Fact]
        public async Task Register_ReturnsStatusCode500_WhenModelStateIsInvalid()
        {
            // Arrange
            var controller = new AuthAPIController(_authServiceMock.Object, _jwtTokenGenerator.Object, _passWordHash.Object);
            controller.ModelState.AddModelError("Error", "ModelState is invalid");

            // Act
            var result = await controller.Register(new RegisterRequestDto()) as ObjectResult;
                
            // Assert
            Assert.Equal(StatusCodes.Status500InternalServerError, result.StatusCode);
        }

        [Fact]
        public async Task Register_ReturnsOk_WhenRegistrationSuccessful()
        {
            // Arrange
            _authServiceMock.Setup(x => x.SendAsync(It.IsAny<RequestDto>())).
                ReturnsAsync(new ResponseDto()
                {
                    IsSuccess = true,
                    Result = null,
                    Message = "Success"
                });
            var controller = new AuthAPIController(_authServiceMock.Object, _jwtTokenGenerator.Object, _passWordHash.Object);

            var mockFormFile = new Mock<IFormFile>();
            var RegDto = _fixture.Build<RegisterRequestDto>()
            .With(x => x.Image, mockFormFile.Object)
            .Create();

            // Act
            var result = await controller.Register(RegDto) as ObjectResult;

            // Assert
            Assert.Equal(StatusCodes.Status200OK, result.StatusCode);
        }
        [Fact]
        public async Task Register_ReturnsBadReq_WhenRegistrationFails()
        {
            // Arrange
            _authServiceMock.Setup(x => x.SendAsync(It.IsAny<RequestDto>())).
                ReturnsAsync(new ResponseDto()
                {
                    IsSuccess = false,
                    Result = null,
                    Message = "Success"
                });
            var controller = new AuthAPIController(_authServiceMock.Object, _jwtTokenGenerator.Object, _passWordHash.Object);

            var mockFormFile = new Mock<IFormFile>();
            var RegDto = _fixture.Build<RegisterRequestDto>()
            .With(x => x.Image, mockFormFile.Object)
            .Create();

            // Act
            var result = await controller.Register(RegDto) as ObjectResult;

            // Assert
            Assert.Equal(StatusCodes.Status400BadRequest, result.StatusCode);
        }

        //-------------------Login------------------- 
        [Fact]
        public async Task Login_ReturnsStatusCode500_WhenModelStateIsInvalid()
        {
            // Arrange
            var controller = new AuthAPIController(_authServiceMock.Object, _jwtTokenGenerator.Object, _passWordHash.Object);
            controller.ModelState.AddModelError("Error", "ModelState is invalid");

            // Act
            var result = await controller.Login(new LoginRequestDto()) as ObjectResult;

            // Assert
            Assert.Equal(StatusCodes.Status500InternalServerError, result.StatusCode);
        }
        [Fact]
        public async Task Login_ReturnsBadReq_WhenLoginFails()
        {
            // Arrange
            _authServiceMock.Setup(x => x.SendAsync(It.IsAny<RequestDto>())).
                ReturnsAsync(new ResponseDto()
                {
                    IsSuccess = false,
                    Result = null,
                    Message = "Success"
                });
            var controller = new AuthAPIController(_authServiceMock.Object, _jwtTokenGenerator.Object, _passWordHash.Object);

            // Act
            var result = await controller.Login(_fixture.Create<LoginRequestDto>()) as ObjectResult;

            // Assert
            Assert.Equal(StatusCodes.Status400BadRequest, result.StatusCode);
        }
        [Fact]
        public async Task Login_ReturnsOk_WhenLoginSuccessful()
        {
            // Arrange
            _authServiceMock.Setup(x => x.SendAsync(It.IsAny<RequestDto>())).
                ReturnsAsync(new ResponseDto()
                {
                    IsSuccess = true,
                    Result = null,
                    Message = "Success"
                });
            _jwtTokenGenerator.Setup(x => x.GenerateToken(It.IsAny<RegisterRequestDto>())).Returns(It.IsAny<string>());
            _passWordHash.Setup(x => x.VerifyPassword(It.IsAny<string>(), It.IsAny<string>())).Returns(true);
            var controller = new AuthAPIController(_authServiceMock.Object, _jwtTokenGenerator.Object, _passWordHash.Object);

            // Act
            var result = await controller.Login(_fixture.Create<LoginRequestDto>()) as ObjectResult;

            // Assert
            Assert.Equal(StatusCodes.Status200OK, result.StatusCode);
        }

        [Fact]
        public async Task Login_ReturnsBadRequest_WhenPwdIncorrect()
        {
            // Arrange
            _authServiceMock.Setup(x => x.SendAsync(It.IsAny<RequestDto>())).
                ReturnsAsync(new ResponseDto()
                {
                    IsSuccess = false,
                    Result = null,
                    Message = "Password is incorrect"
                });
            _jwtTokenGenerator.Setup(x => x.GenerateToken(It.IsAny<RegisterRequestDto>())).Returns(It.IsAny<string>());
            _passWordHash.Setup(x => x.VerifyPassword(It.IsAny<string>(), It.IsAny<string>())).Returns(true);
            var controller = new AuthAPIController(_authServiceMock.Object, _jwtTokenGenerator.Object, _passWordHash.Object);

            // Act
            var result = await controller.Login(_fixture.Create<LoginRequestDto>()) as ObjectResult;

            // Assert
            Assert.Equal(StatusCodes.Status400BadRequest, result.StatusCode);
        }
    }
}
