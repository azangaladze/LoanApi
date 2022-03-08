using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using LoanProject.Infrastructure.Models;
using AutoMapper;
using LoanProject.Core.Entities;
using Microsoft.AspNetCore.Authorization;
using LoanProject.Core.FieldStrings;
using LoanProject.Api.Validators;
using System.Security.Claims;
using LoanProject.Api.Helper;
using LoanProject.Core.Interfaces;

namespace LoanProject.Api.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IMapper _mapper;
        public UserController(IUserService userService, IMapper mapper)
        {
            _userService = userService;
            _mapper = mapper;
        }
        [Authorize(Roles = Roles.Accountant)]
        [HttpGet("getall")]
        public async Task<ActionResult<IEnumerable<UserModel>>> GetAllUsersAsync()
        {
            var users = await _userService.GetUsersAsync();
            if (users == null)
            {
                return BadRequest();
            }

            return Ok(users);

        }
        [HttpGet("{id}")]
        public async Task<ActionResult<UserModel>> GetUserByIdAsync(int id)
        {
            var userId = User.GetUserId();
            if (User.IsInRole(Roles.User) && userId != id)
            {
                return BadRequest($"You cannot view other user's information");
            }
            var user = await _userService.GetByIdAsync(id);

            if (user == null)
            {
                return NotFound($"There is no user with id {id}");
            }

            return Ok(user);
        }

        [Authorize(Roles = Roles.Accountant)]
        [HttpDelete("delete/{id}")]
        public async Task<ActionResult> DeleteUserAsync(int id)
        {
            var deleteUser = await _userService.DeleteAsync(id);

            if (id == 1) {
                return BadRequest($"You cannot delete your own user");
            }

            if (deleteUser == false)
            {
                NotFound($"There is no user with id {id}");
            }

            return Ok($"User with id {id} deleted");
        }

        [HttpPut("update/{id}")]
        public async Task<ActionResult> UpdateUserAsync(int id, UserModel user)
        {
            var userId = User.GetUserId();
            if (User.IsInRole(Roles.User) && userId != id)
            {
                return BadRequest($"You cannot update other user's information");
            }

            var mapped = _mapper.Map<User>(user);

            var validator = new UserValidator();
            var result = validator.Validate(mapped);
            List<string> errorsList = new();
            if (!result.IsValid)
            {
                foreach (var item in result.Errors)
                {
                    errorsList.Add(item.ErrorMessage);
                }
                return BadRequest(errorsList);
            }

            var updateUser = await _userService.UpdateAsync(id, mapped);

            if (updateUser == false)
            {
                NotFound($"There is no user with id {id}");
            }

            return Ok($"User with id {id} updated");
        }

        [Authorize(Roles = Roles.Accountant)]
        [HttpPatch("changeblockstatus/{id}")]
        public async Task<ActionResult> ChangeUserStatusAsync(int id, bool IsBlocked)
        {
            var changed = await _userService.ChangeStatusAsync(id, IsBlocked);
            if (changed == false)
            {
                return NotFound($"There is no user with id {id}");
            }

            return Ok($"Blocked status for user with id {id} changed to {IsBlocked}");
        }

    }
}
