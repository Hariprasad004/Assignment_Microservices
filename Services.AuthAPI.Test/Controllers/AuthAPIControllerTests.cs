using Azure;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Services.AuthAPI.Controllers;
using Services.AuthAPI.Models;
using Services.AuthAPI.Models.Dtos;
using Services.AuthAPI.Service;
using Services.AuthAPI.Service.IService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using WebUI.Service.IService;

namespace Services.AuthAPI.Test.Controllers
{
    public class AuthAPIControllerTests
    {
        private readonly Mock<IAuthService> _authServiceMock;
        private readonly Mock<IJwtTokenGenerator> _jwtTokenGenerator;
        private readonly Mock<IPassWordHash> _passWordHash;
        public AuthAPIControllerTests()
        {
            _authServiceMock = new Mock<IAuthService>();
            _jwtTokenGenerator = new Mock<IJwtTokenGenerator>();
            _passWordHash = new Mock<IPassWordHash>();
        }
        [Fact]
        public async Task Register_ReturnsStatusCode500_WhenModelStateIsInvalid()
        {
            // Arrange
            var controller = new AuthAPIController(_authServiceMock.Object, _jwtTokenGenerator.Object, _passWordHash.Object);
            controller.ModelState.AddModelError("Error", "ModelState is invalid");

            // Act
            var result = await controller.Register(new RegRequestDto());

            // Assert
            result.Result.Should().BeAssignableTo<NotFoundResult>();


            Assert(result.Equals)


            Assert.That(result, Is.EqualTo(42));
            var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Error", redirectToActionResult.ActionName);
        }
    }
}
