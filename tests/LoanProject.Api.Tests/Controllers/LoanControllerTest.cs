using AutoFixture;
using AutoMapper;
using FluentAssertions;
using LoanProject.Api.Controllers;
using LoanProject.Core.Entities;
using LoanProject.Core.EntityFields;
using LoanProject.Core.Exceptions;
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
    public class LoanControllerTest
    {
        private readonly IFixture _fixture;
        private readonly Mock<ILoanService> _loanServiceMock;
        private readonly LoanController _loanController;
        private readonly Mock<IUserService> _userService = new();
        private readonly Mock<IMapper> _mapper;
        private readonly Mock<ILogger<LoanController>> _logger = new();

        public LoanControllerTest()
        {
            _mapper = new Mock<IMapper>();
            _fixture = new Fixture();
            _loanServiceMock = _fixture.Freeze<Mock<ILoanService>>();
            _loanController = new LoanController(_loanServiceMock.Object, 
                _userService.Object, 
                _mapper.Object, 
                _logger.Object);
            _fixture.Behaviors.OfType<ThrowingRecursionBehavior>().ToList()
            .ForEach(b => _fixture.Behaviors.Remove(b));
            _fixture.Behaviors.Add(new OmitOnRecursionBehavior());

        }
        [Fact]
        public async Task GetAllLoansAsync_ShouldReturnOkResponse_WhenLoansFound()
        {
            //Arrange

            var loansMock = _fixture.Create<IEnumerable<Loan>>();
            _loanServiceMock.Setup(x => x.GetLoansAsync()).ReturnsAsync(loansMock);
            //Act

            var result = await _loanController.GetAllLoansAsync();

            //Assert

            result.Should().NotBeNull();
            result.Should().BeAssignableTo<IActionResult>();
            result.Should().BeAssignableTo<OkObjectResult>();
            _loanServiceMock.Verify(x => x.GetLoansAsync(), Times.Once());
        }

        [Fact]
        public async Task GetAllLoansAsync_ShouldReturnNotFound_WhenLoansNotFound()
        {
            //Arrange

            List<Loan> loans = new();
            _loanServiceMock.Setup(x => x.GetLoansAsync()).ReturnsAsync(loans);
            //Act

            var result = await _loanController.GetAllLoansAsync();
            //Assert
            result.Should().NotBeNull();
            result.Should().BeAssignableTo<NotFoundObjectResult>();
            _loanServiceMock.Verify(x => x.GetLoansAsync(), Times.Once());
        }

        [Fact]
        public async Task ChangeLoanStatusAsync_ShouldReturnOkResult_WhenStatusChanges()
        {
            //Arrange
            int id = _fixture.Create<int>();
            var status = LoanStatuses.Positive;
            _loanServiceMock.Setup(x => x.ChangeLoanStatusAsync(id, status)).ReturnsAsync(true);
            //Act

            var result = await _loanController.ChangeLoanStatusAsync(id, status);
            //Assert

            result.Should().NotBeNull();
            result.Should().BeAssignableTo<IActionResult>();
            result.Should().BeAssignableTo<OkObjectResult>();
            _loanServiceMock.Verify(x => x.ChangeLoanStatusAsync(id, status), Times.Once());
        }

        [Fact]
        public async Task ChangeLoanStatusAsync_ShouldReturnBadRequest_WhenStatusIsInvalid()
        {
            //Arrange
            int id = _fixture.Create<int>();
            LoanStatuses status = 0;
            _loanServiceMock.Setup(x => x.ChangeLoanStatusAsync(id, status)).ReturnsAsync(false);
            //Act

            var result = await _loanController.ChangeLoanStatusAsync(id, status);
            //Assert

            result.Should().NotBeNull();
            result.Should().BeAssignableTo<IActionResult>();
            result.Should().BeAssignableTo<BadRequestObjectResult>();
            _loanServiceMock.Verify(x => x.ChangeLoanStatusAsync(id, status), Times.Never);
        }


        [Fact]
        public void ChangeLoanStatusAsync_ShouldReturnNotFound_WhenIdIsInvalid()
        {
            //Arrange
            int id = _fixture.Create<int>();
            LoanStatuses status = _fixture.Create<LoanStatuses>();
            _loanServiceMock.Setup(x => x.ChangeLoanStatusAsync(id, status)).Throws<EntityNotFoundException<Loan>>();
            //Act

            var result = _loanController.ChangeLoanStatusAsync(id, status);
           

            //Assert
            result.Result.Should().NotBeNull();
            result.Result.Should().BeAssignableTo<IActionResult>();
            result.Result.Should().BeAssignableTo<NotFoundObjectResult>();
            _loanServiceMock.Verify(x => x.ChangeLoanStatusAsync(id, status), Times.Once());

        }

    }
}
