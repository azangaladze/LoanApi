using AutoMapper;
using FluentAssertions;
using LoanProject.Api.Controllers;
using LoanProject.Api.ViewModels;
using LoanProject.Core.Entities;
using LoanProject.Core.Interfaces;
using LoanProject.Infrastructure.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace LoanProject.Api.Tests.Controllers
{
    public class AccountControllerTest
    {
        private readonly Mock<IAccountService> _accountServiceMock;
        private readonly AccountController _accountController;
        private readonly Mock<IMapper> _mapper;
        private readonly Mock<ILogger<AccountController>> _logger = new();

        public AccountControllerTest()
        {
            _mapper = new Mock<IMapper>();
            _accountServiceMock = new Mock<IAccountService>();
            var appsettings = Options.Create(new AppSettings());
            _accountController = new AccountController(_accountServiceMock.Object,
                appsettings,
                _mapper.Object,
               _logger.Object
            );


        }
        [Fact]
        public async Task CreateUserAsync_ShouldReturnCreated_WhenDataIsValid()
        {

            //Arrange

            var userModel = GetUserModel();
            var user = GetUser();
            
            _accountServiceMock.Setup(x => x.CreateAsync(user)).ReturnsAsync(user);
            //Act

            var result = await _accountController.CreateUserAsync(userModel);
            //Assert
            result.Should().NotBeNull();
            result.Should().BeAssignableTo<CreatedAtRouteResult>();
            _accountServiceMock.Verify(x => x.CreateAsync(user), Times.Never());

        }

        [Fact]
        public void Login_ShouldReturnOkResult_WhenUserNameAndPasswordAreCorrect()
        {

            //Arrange

            var loginmodel = new LoginModel();
            loginmodel.Username = "user";
            loginmodel.Password = "string123";
            
            var user = GetUser();
            user.UserName = loginmodel.Username;
            user.Password = loginmodel.Password;


            _accountServiceMock.Setup(x => x.Login(loginmodel.Username, loginmodel.Password)).Returns(user);
            //Act

            var result = _accountController.Login(loginmodel);
            //Assert
            result.Should().NotBeNull();
            result.Should().BeAssignableTo<OkObjectResult>();
            _accountServiceMock.Verify(x => x.CreateAsync(user), Times.Never());

        }

        private static UserModel GetUserModel()
        {
            return new UserModel()
            {

                FirstName = "Name",
                LastName = "Last Name",
                UserName = "name",
                Age = 25,
                Email = "email@test.com",
                Salary = 100.0,
                Password = "password"

            };
        }
        private static User GetUser()
        {
            return new User()
            {
                Id = 0,
                FirstName = "Name",
                LastName = "Last name",
                UserName = "name",
                Email = "email@test.com",
                Age = 25,
                Salary = 100.0,
                Password = "password",
                IsBlocked = false,
                Role = "User",
                Loans = new HashSet<Loan>()


            };
        }
    }
}
