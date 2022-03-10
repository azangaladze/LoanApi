using AutoFixture;
using AutoMapper;
using FluentAssertions;
using LoanProject.Api.Controllers;
using LoanProject.Core.Entities;
using LoanProject.Core.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace LoanProject.Api.Tests.Controllers
{
    public class UserControllerTest
    {
        private readonly IFixture _fixture;
        private readonly Mock<IUserService> _userServiceMock;
        private readonly UserController _userController;
        private readonly Mock<IMapper> _mapper;
        private readonly Mock<ILogger<UserController>> _logger = new();

        public UserControllerTest()
        {
            _mapper = new Mock<IMapper>();
            _fixture = new Fixture();
            _userServiceMock = _fixture.Freeze<Mock<IUserService>>();
            _userController = new UserController(
                _userServiceMock.Object, 
                _mapper.Object, 
                _logger.Object);
            _fixture.Behaviors.OfType<ThrowingRecursionBehavior>().ToList()
            .ForEach(b => _fixture.Behaviors.Remove(b));
            _fixture.Behaviors.Add(new OmitOnRecursionBehavior());

        }
        [Fact]
        public async Task GetAllUsersAsync_ShouldReturnOkResponse_WhenUsersFound()
        {
            //Arrange

            var usersMock = _fixture.Create<IEnumerable<User>>();
            _userServiceMock.Setup(x => x.GetUsersAsync()).ReturnsAsync(usersMock);
            //Act

            var result = await _userController.GetAllUsersAsync();

            //Assert

            result.Should().NotBeNull();
            result.Should().BeAssignableTo<IActionResult>();
            result.Should().BeAssignableTo<OkObjectResult>();
            _userServiceMock.Verify(x => x.GetUsersAsync(), Times.Once());
        }

        [Fact]
        public async Task GetAllUsersAsync_ShouldReturnNotFound_WhenUsersNotFound()
        {
            //Arrange

            List<User> users = new();
            _userServiceMock.Setup(x => x.GetUsersAsync()).ReturnsAsync(users);
            //Act

            var result = await _userController.GetAllUsersAsync();
            //Assert

            result.Should().NotBeNull();
            result.Should().BeAssignableTo<NotFoundObjectResult>();
            _userServiceMock.Verify(x => x.GetUsersAsync(), Times.Once());
        }

        [Fact]
        public async Task ChangeUserStatusAsync_ShouldReturnOkResult_WhenStatusChanges()
        {
            //Arrange
            int id = _fixture.Create<int>();
            bool status = _fixture.Create<bool>();
            _userServiceMock.Setup(x => x.ChangeStatusAsync(id, status)).ReturnsAsync(true);
            //Act

            var result = await _userController.ChangeUserStatusAsync(id, status);
            //Assert

            result.Should().NotBeNull();
            result.Should().BeAssignableTo<IActionResult>();
            result.Should().BeAssignableTo<OkObjectResult>();
            _userServiceMock.Verify(x => x.ChangeStatusAsync(id, status), Times.Once());
        }

        [Fact]
        public async Task ChangeUserStatusAsync_ShouldReturnNotFound_WhenStatusDoesNotChange()
        {
            //Arrange
            int id = _fixture.Create<int>();
            bool status = _fixture.Create<bool>();
            _userServiceMock.Setup(x => x.ChangeStatusAsync(id, status)).ReturnsAsync(false);
            //Act

            var result = await _userController.ChangeUserStatusAsync(id, status);
            //Assert

            result.Should().NotBeNull();
            result.Should().BeAssignableTo<NotFoundObjectResult>();
            _userServiceMock.Verify(x => x.ChangeStatusAsync(id, status), Times.Once());
        }
    }
}
