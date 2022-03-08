using AutoMapper;
using LoanProject.Api.Helper;
using LoanProject.Api.Validators;
using LoanProject.Core.Entities;
using LoanProject.Core.FieldStrings;
using LoanProject.Core.Interfaces;
using LoanProject.Infrastructure.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
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
        private readonly ILogger<AccountController> _logger;
        public LoanController(ILoanService loanService,
            IUserService userService,
            IMapper mapper,
            ILogger<AccountController> logger)
        {
            _loanService = loanService;
            _userService = userService;
            _mapper = mapper;
            _logger = logger;
        }

        [Authorize(Roles = Roles.Accountant)]
        [HttpGet("getall")]
        public async Task<ActionResult<IEnumerable<LoanModel>>> GetAllLoansAsync()
        {
            var response = await _loanService.GetLoansAsync();
            if (!response.Any())
            {
                return NotFound($"There are no loans");
            }

            return Ok(response);

        }
        [HttpGet("{id}")]
        public async Task<ActionResult<LoanModel>> GetLoanByIdAsync(int id)
        {
            var loan = await _loanService.GetByIdAsync(id);

            var userId = User.GetUserId();

            if (loan != null && userId != loan.UserId)
            {
                return BadRequest($"You cannot view loan with id {id}, it does not belong to you");
            }

            if (loan == null)
            {
                return NotFound($"There is no loan with id {id}");
            }

            return Ok(loan);
        }

        [HttpPost("take")]
        public async Task<ActionResult<LoanModel>> TakeLoanAsync(LoanModel loan)
        {
            if (User.IsInRole(Roles.Accountant))
            {
                return BadRequest("Accountant cannot take loan, log in as User and try again");
            }
            loan.UserId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var mapped = _mapper.Map<Loan>(loan);
            var validator = new LoanValidator();
            var result = validator.Validate(mapped);
            List<string> errorsList = new();
            if (!result.IsValid)
            {
                foreach (var error in result.Errors)
                {
                    errorsList.Add(error.ErrorMessage);
                }
                return BadRequest(errorsList);
            }

            try
            {
                await _loanService.CreateAsync(mapped);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                throw;
            }

            return CreatedAtAction(nameof(GetLoanByIdAsync), new { id = loan.Id }, mapped);

        }

        [HttpDelete("delete/{id}")]
        public async Task<ActionResult> DeleteLoan(int id)
        {

            var loanToDelete = await _loanService.GetByIdAsync(id);
            var mappedLoan = _mapper.Map<Loan>(loanToDelete);

            if (!User.IsInRole(Roles.User) && mappedLoan.Loanstatus.ToLower() != LoanStatus.Processing.ToLower())
            {
                return BadRequest($"Process of loan with id {id} is finished, you cannot delete it");
            }

            var userId = User.GetUserId();
            if (userId != mappedLoan.UserId)
            {
                return BadRequest($"You cannot delete loan with id {id}, it does not belong to you");
            }

            var user = await _userService.GetByIdAsync(loanToDelete.UserId);
            var mappedUser = _mapper.Map<User>(user);
            if (mappedUser.IsBlocked)
            {
                return BadRequest("You are in a black list, you cannot delete your loan");
            }

            try
            {
                var deleteLoan = await _loanService.DeleteAsync(id);

                if (deleteLoan == false)
                {
                    NotFound($"There is no loan with id {id}");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                throw;
            }


            return Ok($"Loan with id {id} deleted");
        }

        [HttpPut("update/{id}")]
        public async Task<ActionResult> UpdateUser(int id, LoanModel loan)
        {
            var loanToUpdate = await _loanService.GetByIdAsync(id);

            if (loanToUpdate == null)
            {
                return NotFound($"There is no loan with id {id}");
            }

            var userId = User.GetUserId();

            if (userId != loanToUpdate.UserId)
            {
                return BadRequest($"You cannot update loan with id {id}, it does not belong to you");
            }

            var user = await _userService.GetByIdAsync(loanToUpdate.UserId);
            var mappedUser = _mapper.Map<User>(user);
            if (mappedUser.IsBlocked)
            {
                return BadRequest("You are in a black list, you cannot update your loan");
            }

            if (User.IsInRole(Roles.User) && loanToUpdate.Loanstatus.ToLower() != LoanStatus.Processing.ToLower())
            {
                return BadRequest($"Status of loan with id {id} is finished, you cannot update it");
            }
            var mappedloan = _mapper.Map<Loan>(loan);

            var validator = new LoanValidator();
            var result = validator.Validate(mappedloan);
            List<string> errorsList = new();
            if (!result.IsValid)
            {
                foreach (var error in result.Errors)
                {
                    errorsList.Add(error.ErrorMessage);
                }
                return BadRequest(errorsList);
            }

            await _loanService.UpdateAsync(id, mappedloan);

            return Ok($"Loan with id {id} updated");
        }

        [Authorize(Roles = Roles.Accountant)]
        [HttpPatch("changeloanstatus/{id}")]
        public async Task<ActionResult> ChangeLoanStatusAsync(int id, string Status)
        {
            var loan = await _loanService.GetByIdAsync(id);
            if (loan == null)
            {
                return NotFound($"There is no loan with id {id}");
            }

            try
            {
                var changed = await _loanService.ChangeLoanStatusAsync(id, Status);
                if (changed == false)
                {
                    return BadRequest($"Status can only be changed to Positive or Negative");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                throw;
            }
            return Ok($"Status for loan with id {id} changed to {Status}");
        }
    }
}
