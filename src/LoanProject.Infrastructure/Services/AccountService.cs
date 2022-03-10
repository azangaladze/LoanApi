using LoanProject.Core.Entities;
using LoanProject.Core.Exceptions;
using LoanProject.Core.Interfaces;
using LoanProject.Infrastructure.Context;
using LoanProject.Infrastructure.Helpers;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace LoanProject.Infrastructure.Services
{
    public class AccountService : IAccountService
    {
        private readonly AppDbContext _dbContext;
        public AccountService(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<User> CreateAsync(User user)
        {
            var userExists = _dbContext.Users.Any(i => i.UserName.ToLower() == user.UserName.ToLower());
            if (userExists)
            {
                throw new UserExistsException();
            }
            await _dbContext.Users.AddAsync(user);
            await _dbContext.SaveChangesAsync();
            return user;
        }

        public User Login(string userName, string password)
        {

            var user = _dbContext.Users.SingleOrDefault(x => x.UserName == userName);

            if (user == null)
            {
                throw new EntityNotFoundException<User>();
            }

            if (PasswordHasher.HashPass(password) != user.Password)
            {
                throw new IncorrectPasswordException();
            }

            return user;
        }
    }
}
