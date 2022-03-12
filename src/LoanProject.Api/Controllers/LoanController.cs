using AutoMapper;
using LoanProject.Api.Helpers;
using LoanProject.Core.Entities;
using LoanProject.Core.EntityFields;
using LoanProject.Core.Exceptions;
using LoanProject.Core.Interfaces;
using LoanProject.Infrastructure.Models;
using LoanProject.Infrastructure.Validators;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace LoanProject.Api.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class LoanController : ControllerBase
    {
        private readonly ILoanService _loanService;
        private readonly IUserService _userService;
        private readonly IMapper _mapper;
        private readonly ILogger<LoanController> _logger;
        public LoanController(ILoanService loanService,
            IUserService userService,
            IMapper mapper,
            ILogger<LoanController> logger)
        {
            _loanService = loanService;
            _userService = userService;
            _mapper = mapper;
            _logger = logger;
        }

        [Authorize(Roles = Roles.Accountant)]
        [HttpGet("all")]
        public async Task<IActionResult> GetAllLoansAsync()
        {
            var response = await _loanService.GetLoansAsync();
            if (!response.Any())
            {
                _logger.LogError("Error - Loans not found");
                return NotFound($"There are no loans");
            }

            return Ok(response);

        }
        [HttpGet("{id}")]
        public async Task<IActionResult> GetLoanByIdAsync(int id)
        {

            try
            {
                var loan = await _loanService.GetByIdAsync(id);

                var userId = User.GetUserId();

                if (User.IsInRole(Roles.User) && userId != loan.UserId)
                {
                    _logger.LogError("Error - Trying to get someone else's loan");
                    return BadRequest($"You cannot view loan with id {id}, it does not belong to you");
                }
                return Ok(loan);
            }
            catch (EntityNotFoundException<Loan>)
            {
                _logger.LogError("Error - Invalid loan Id, loan not found");
                return NotFound($"There is no loan with id {id}");
            }

        }

        [HttpPost("take")]
        public async Task<IActionResult> TakeLoanAsync(LoanModel loan)
        {
            if (User.IsInRole(Roles.Accountant))
            {
                _logger.LogError("Error - Accountant trying to add loan");
                return BadRequest("Accountant cannot take loan, log in as User and try again");
            }

            loan.UserId = User.GetUserId();
            var user = await _userService.GetByIdAsync(loan.UserId);
            var mappedUser = _mapper.Map<User>(user);
            if (mappedUser.IsBlocked)
            {
                _logger.LogError("Error - Trying to take loan, when user IsBlocked is true");
                return BadRequest("You are in a black list, you cannot take loan");
            }


            var validator = new LoanValidator();
            var result = validator.Validate(loan);

            if (!result.IsValid)
            {
                _logger.LogError("Error - One or more Validation errors occured");
                return BadRequest(result.Errors.Select(s => s.ErrorMessage));
            }

            var mapped = _mapper.Map<Loan>(loan);
            try
            {
                await _loanService.CreateAsync(mapped);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return StatusCode(500, ex.Message);
            }

            return CreatedAtAction(nameof(GetLoanByIdAsync), new { id = loan.Id }, mapped);

        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteLoanAsync(int id)
        {
            try
            {
                var loanToDelete = await _loanService.GetByIdAsync(id);
                var mappedLoan = _mapper.Map<Loan>(loanToDelete);

                if (User.IsInRole(Roles.User) && mappedLoan.Loanstatus != LoanStatuses.Processing)
                {
                    _logger.LogError("Error - Trying to delete finished loan");
                    return BadRequest($"Process of loan with id {id} is finished, you cannot delete it");
                }

                var userId = User.GetUserId();
                if (User.IsInRole(Roles.User) && userId != mappedLoan.UserId)
                {
                    _logger.LogError("Error - Trying to delete someone else's loan");
                    return BadRequest($"You cannot delete loan with id {id}, it does not belong to you");
                }

                var user = await _userService.GetByIdAsync(loanToDelete.UserId);
                var mappedUser = _mapper.Map<User>(user);
                if (mappedUser.IsBlocked)
                {
                    _logger.LogError("Error - Trying to delete loan, when user IsBlocked is true");
                    return BadRequest("You are in a black list, you cannot delete your loan");
                }

                await _loanService.DeleteAsync(id);

            }
            catch (EntityNotFoundException<Loan>)
            {
                _logger.LogError("Error - Invalid loan id, loan not found");
                return NotFound($"There is no loan with id {id}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return StatusCode(500, ex.Message);
            }

            return Ok($"Loan with id {id} deleted");
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateLoanAsync(int id, LoanModel loan)
        {
            try
            {
                var loanToUpdate = await _loanService.GetByIdAsync(id);

                if (loanToUpdate == null)
                {
                    _logger.LogError("Error - Invalid loan id, loan not found");
                    return NotFound($"There is no loan with id {id}");
                }

                var userId = User.GetUserId();

                if (User.IsInRole(Roles.User) && userId != loanToUpdate.UserId)
                {
                    _logger.LogError("Error - Trying to update someone else's loan");
                    return BadRequest($"You cannot update loan with id {id}, it does not belong to you");
                }

                var user = await _userService.GetByIdAsync(loanToUpdate.UserId);
                var mappedUser = _mapper.Map<User>(user);
                if (mappedUser.IsBlocked)
                {
                    _logger.LogError("Error - Trying to update loan, when user IsBlocked is true");
                    return BadRequest("You are in a black list, you cannot update your loan");
                }

                if (User.IsInRole(Roles.User) && loanToUpdate.Loanstatus != LoanStatuses.Processing)
                {
                    _logger.LogError("Error - Trying to update finished loan");
                    return BadRequest($"Status of loan with id {id} is finished, you cannot update it");
                }

                var validator = new LoanValidator();
                var result = validator.Validate(loan);
                if (!result.IsValid)
                {
                    _logger.LogError("Error - One or more Validation errors occured");
                    return BadRequest(result.Errors.Select(s => s.ErrorMessage));
                }

                var mappedloan = _mapper.Map<Loan>(loan);
                await _loanService.UpdateAsync(id, mappedloan);
            }
            catch (EntityNotFoundException<Loan>)
            {
                _logger.LogError("Error - Invalid loan id, loan not found");
                return NotFound($"There is no loan with id {id}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return StatusCode(500, ex.Message);
            }

            return Ok($"Loan with id {id} updated");
        }

        [Authorize(Roles = Roles.Accountant)]
        [HttpPatch("status/{id}")]
        public async Task<IActionResult> ChangeLoanStatusAsync(int id, LoanStatuses status)
        {
            if (status == 0)
            {
                _logger.LogError("Error - Status was not selected");
                return BadRequest("Status is required");
            }

            try
            {
                await _loanService.ChangeLoanStatusAsync(id, status);
            }
            catch (EntityNotFoundException<Loan>)
            {
                _logger.LogError("Error - Invalid loan id, loan not found");
                return NotFound($"There is no loan with id {id}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return StatusCode(500, ex.Message);
            }
            return Ok($"Status for loan with id {id} changed to {status}");
        }
    }
}
