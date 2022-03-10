using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using LoanProject.Infrastructure.Models;
using AutoMapper;
using LoanProject.Core.Entities;
using Microsoft.AspNetCore.Authorization;
using LoanProject.Core.FieldStrings;
using LoanProject.Api.Validators;
using LoanProject.Core.Interfaces;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using LoanProject.Core.Exceptions;
using LoanProject.Api.Helpers;

namespace LoanProject.Api.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IMapper _mapper;
        private readonly ILogger<UserController> _logger;
        public UserController(IUserService userService,
            IMapper mapper,
            ILogger<UserController> logger)
        {
            _userService = userService;
            _mapper = mapper;
            _logger = logger;

        }

        [Authorize(Roles = Roles.Accountant)]
        [HttpGet("all")]
        public async Task<IActionResult> GetAllUsersAsync()
        {
            var users = await _userService.GetUsersAsync();
            if (!users.Any())
            {
                _logger.LogError("Error - No users found");
                return NotFound($"There are no users");
            }

            return Ok(users);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetUserByIdAsync(int id)
        {
            var userId = User.GetUserId();
            if (User.IsInRole(Roles.User) && userId != id)
            {
                _logger.LogError("Error - Trying to get other user");
                return BadRequest($"You cannot view other user's information");
            }

            try
            {
                return Ok(await _userService.GetByIdAsync(id));
            }
            catch (EntityNotFoundException<User>)
            {
                _logger.LogError("Error - Invalid user id, user not found");
                return NotFound($"There is no user with id {id}");
            }
        }

        [Authorize(Roles = Roles.Accountant)]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUserAsync(int id)
        {
            var userId = User.GetUserId();
            if (id == userId)
            {
                _logger.LogError("Error - Trying to delete own user");
                return BadRequest($"You cannot delete your own user");
            }
            try
            {
                await _userService.DeleteAsync(id);
            }
            catch (EntityNotFoundException<User>)
            {
                _logger.LogError("Error - Invalid user id, user not found");
                return NotFound($"There is no user with id {id}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return StatusCode(500, ex.Message);
            }

            return Ok($"User with id {id} deleted");
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUserAsync(int id, UserModel user)
        {
            var userId = User.GetUserId();
            if (User.IsInRole(Roles.User) && userId != id)
            {
                _logger.LogError("Error - Trying to update someone else's user");
                return BadRequest($"You cannot update other user's information");
            }

            var mapped = _mapper.Map<User>(user);

            var validator = new UserValidator();
            var result = validator.Validate(mapped);

            if (!result.IsValid)
            {
                _logger.LogError("Error - One or more Validation errors occured");
                return BadRequest(result.Errors.Select(s => s.ErrorMessage));
            }

            try
            {
                var updateUser = await _userService.UpdateAsync(id, mapped);

                if (!updateUser)
                {
                    _logger.LogError("Error - Invalid user id, user not found");
                    return NotFound($"There is no user with id {id}");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return StatusCode(500, ex.Message);
            }

            return Ok($"User with id {id} updated");
        }

        [Authorize(Roles = Roles.Accountant)]
        [HttpPatch("status/{id}/{isblocked}")]
        public async Task<IActionResult> ChangeUserStatusAsync(int id, bool isBlocked)
        {
            try
            {
                var changed = await _userService.ChangeStatusAsync(id, isBlocked);
                if (!changed)
                {
                    _logger.LogError("Error - Invalid user id, user not found");
                    return NotFound($"There is no user with id {id}");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return StatusCode(500, ex.Message);
            }

            return Ok($"Blocked status for user with id {id} changed to {isBlocked}");
        }

    }
}
