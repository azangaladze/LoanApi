using AutoMapper;
using LoanProject.Api.Helpers;
using LoanProject.Api.ViewModels;
using LoanProject.Core.Entities;
using LoanProject.Core.Exceptions;
using LoanProject.Core.Interfaces;
using LoanProject.Infrastructure.Helpers;
using LoanProject.Infrastructure.Models;
using LoanProject.Infrastructure.Validators;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace LoanProject.Api.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly IAccountService _accountService;
        private readonly AppSettings _appSettings;
        private readonly IMapper _mapper;
        private readonly ILogger<AccountController> _logger;
        public AccountController(IAccountService accountService,
            IOptions<AppSettings> appSettings,
            IMapper mapper,
            ILogger<AccountController> logger)
        {
            _accountService = accountService;
            _appSettings = appSettings.Value;
            _mapper = mapper;
            _logger = logger;
        }

        [AllowAnonymous]
        [HttpPost("register")]
        public async Task<IActionResult> CreateUserAsync(UserModel user)
        {

            var validator = new UserValidator();
            var result = validator.Validate(user);

            if (!result.IsValid)
            {
                _logger.LogError("Error - One or more Validation errors for user occured");
                return BadRequest(result.Errors.Select(s => s.ErrorMessage));
            }

            user.Password = PasswordHasher.HashPass(user.Password);
            var mapped = _mapper.Map<User>(user);
            try
            {
                await _accountService.CreateAsync(mapped);
            }
            catch (UserExistsException)
            {
                _logger.LogError("Error - Username already registered");
                return BadRequest($"User with username {user.UserName} is already registered");
            }

            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return StatusCode(500, ex.Message);
            }

            return Created("", mapped);
        }

        [AllowAnonymous]
        [HttpPost("login")]
        public IActionResult Login(LoginModel userModel)
        {
            if (string.IsNullOrEmpty(userModel.Username) || string.IsNullOrEmpty(userModel.Password))
            {
                _logger.LogError("Error - Username or Password IsNullOrEmpty");
                return BadRequest("Username and Password is required");
            }
            try
            {
                var user = _accountService.Login(userModel.Username, userModel.Password);
                var mapped = _mapper.Map<User>(user);
                string tokenString = new TokenGenerator(_appSettings.Secret).GenerateToken(mapped);
                return Ok(new
                {
                    user.FirstName,
                    user.LastName,
                    user.UserName,
                    Token = tokenString
                });

            }
            catch (EntityNotFoundException<User>)
            {
                _logger.LogError("Error - Username not found");
                return NotFound("Username not found");
            }
            catch (IncorrectPasswordException)
            {
                _logger.LogError("Error - Incorrect password");
                return BadRequest("Password is incorrect");
            }

        }

    }
}
