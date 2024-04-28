using AutoFixture;
using AutoFixture.AutoMoq;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Services.DataProcessAPI.Controllers;
using Services.DataProcessAPI.Data;
using Services.DataProcessAPI.Models;
using WebUI.Models;

namespace Services.DataProcessAPI.Test.Controllers
{
    public class DataProcessAPIControllerTests
    {
        private readonly AppDbContext _DbContext;
        private readonly IFixture _fixture;
        public DataProcessAPIControllerTests()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

            _DbContext = new AppDbContext(options);

            _DbContext.Database.EnsureCreated();

            if (_DbContext.Users.Count() <= 0)
            {
                _DbContext.Users.Add(new User()
                {
                    ID = 1,
                    Email = "Test@gmail.com",
                    Name = "TestName",
                    Password = "TestPwd",
                    PhoneNumber = "9999999999",
                    DateOfBirth = DateTime.Now,
                    ImageUrl = "Test/ImgUrl",
                    ImageLocalPath = "Test/ImageLocalPath"
                });
                _DbContext.SaveChanges();
            }

            _fixture = new Fixture();
            _fixture.Customize(new AutoMoqCustomization());
        }

        //--------------Get----------------
        [Fact]
        public void Get_ReturnsNullResult_WhenUserDoesNotExists()
        {
            //Arrange
            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<User, UserDto>();
                cfg.CreateMap<UserDto, User>();
            });

            var mapper = config.CreateMapper();
            var controller = new DataProcessAPIController(_DbContext, mapper);

            //Act
            var result = controller.Get("Test1@gmail.com");

            //Assert
            Assert.Equal("User does not exists", result.Message);
            Assert.False(result.IsSuccess);

        }

        [Fact]
        public void Get_ReturnsUser_WhenUserExists()
        {
            //Arrange
            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<User, UserDto>();
                cfg.CreateMap<UserDto, User>();
            });

            var mapper = config.CreateMapper();
            var controller = new DataProcessAPIController(_DbContext, mapper);

            //Act
            var result = controller.Get("Test@gmail.com");

            //Assert
            Assert.NotNull(result);
            Assert.IsType<ResponseDto>(result);
            Assert.True(result.IsSuccess);
        }

        //------------------Register------------------- 
        [Fact]
        public void Register_ReturnsSuccessFail_WhenModelStateIsInvalid()
        {
            // Arrange
            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<User, UserDto>();
                cfg.CreateMap<UserDto, User>();
            });

            var mapper = config.CreateMapper();
            var controller = new DataProcessAPIController(_DbContext, mapper);
            controller.ModelState.AddModelError("Error", "ModelState is invalid");

            // Act
            var result = controller.Register(new UserDto());

            // Assert
            Assert.Equal("An error occurred while processing your request", result.Message);
            Assert.False(result.IsSuccess);
        }
        [Fact]
        public void Register_ReturnsSuccess_WhenRegistrationSuccessful()
        {
            // Arrange
            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<User, UserDto>();
                cfg.CreateMap<UserDto, User>();
            });

            var mapper = config.CreateMapper();
            var controller = new DataProcessAPIController(_DbContext, mapper);

            // Act
            var result = controller.Register(new UserDto
            {
                Email = "Test1@gmail.com",
                Name = "TestName",
                Password = "TestPwd",
                PhoneNumber = "7777777777",
                DateOfBirth = DateTime.Now
            });

            //Assert
            Assert.NotNull(result);
            Assert.IsType<ResponseDto>(result);
            Assert.True(result.IsSuccess);
        }

        [Fact]
        public void Register_ReturnsFail_WhenEmailAlreadyExists()
        {
            // Arrange
            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<User, UserDto>();
                cfg.CreateMap<UserDto, User>();
            });

            var mapper = config.CreateMapper();
            var controller = new DataProcessAPIController(_DbContext, mapper);

            // Act
            var result = controller.Register(new UserDto
            {
                Email = "Test@gmail.com",
                Name = "TestName",
                Password = "TestPwd",
                PhoneNumber = "7777777777",
                DateOfBirth = DateTime.Now
            });

            //Assert
            Assert.NotNull(result);
            Assert.IsType<ResponseDto>(result);
            Assert.False(result.IsSuccess);
            Assert.Equal("Email id already exists", result.Message);
        }
    }
}
