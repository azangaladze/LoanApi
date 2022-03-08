using AutoMapper;
using LoanProject.Api.Validators;
using LoanProject.Api.ViewModels;
using LoanProject.Core.Entities;
using LoanProject.Core.Interfaces;
using LoanProject.Infrastructure.Helpers;
using LoanProject.Infrastructure.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
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
        public async Task<ActionResult<UserModel>> CreateUserAsync(UserModel user)
        {

            user.Password = PasswordHasher.HashPass(user.Password);

            var mapped = _mapper.Map<User>(user);
            var validator = new UserValidator();
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
                var create = await _accountService.CreateAsync(mapped);

                if (create == null)
                {
                    return BadRequest($"User with username {user.UserName} is already registered");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                throw;
            }


            return Created("", mapped);

        }

        [AllowAnonymous]
        [HttpPost("login")]
        public IActionResult Login(LoginModel userModel)
        {
            var user = _accountService.Login(userModel.Username, userModel.Password);
            if (user == null)
            {
                return BadRequest("Username or Password is incorrect");
            }
            var mapped = _mapper.Map<User>(user);
            string tokenString = GenerateToken(mapped);
            return Ok(new
            {
                user.FirstName,
                user.LastName,
                user.UserName,
                Token = tokenString
            });
        }

        private string GenerateToken(User user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_appSettings.Secret);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.Name, user.UserName.ToString()),
                    new Claim(ClaimTypes.Name, user.FirstName.ToString()),
                    new Claim(ClaimTypes.Surname, user.LastName.ToString()),
                    new Claim(ClaimTypes.Role, user.Role.ToString()),
                    new Claim(ClaimTypes.NameIdentifier, user.Id.ToString())
                }),
                Expires = DateTime.UtcNow.AddMinutes(30),
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(key),
                    SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            var tokenString = tokenHandler.WriteToken(token);
            return tokenString;
        }
    }
}
