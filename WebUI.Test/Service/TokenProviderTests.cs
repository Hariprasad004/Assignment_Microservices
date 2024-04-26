using Microsoft.AspNetCore.Http;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebUI.Controllers;
using WebUI.Service;
using WebUI.Service.IService;

namespace WebUI.Test.Service
{
    public class TokenProviderTests
    {
        private readonly Mock<IHttpContextAccessor> _contextAccessorMock;

        public TokenProviderTests()
        {
            _contextAccessorMock = new Mock<IHttpContextAccessor>();
        }

        //    [Fact]
        //    public void ClearToken_DeletesTokenCookie()
        //    {
        //        // Arrange
        //        var _httpContextAccessor = new Mock<IHttpContextAccessor>();
        //        var tokenprovider = new TokenProvider(_httpContextAccessor.Object);

        //        //Act
        //        tokenprovider.ClearToken();

        //        //Assert
        //        verify(publishing, times(1))
        //    }
    }
    }
