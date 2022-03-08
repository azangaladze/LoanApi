using LoanProject.Core.Entities;
using LoanProject.Core.Interfaces;
using LoanProject.Infrastructure.Context;
using LoanProject.Infrastructure.Helpers;
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
            foreach (var usr in _dbContext.Users.ToList())
            {
                if (usr.UserName == user.UserName)
                {
                    return null;
                }
            }
            await _dbContext.Users.AddAsync(user);
            await _dbContext.SaveChangesAsync();
            return user;
        }

        public User Login(string userName, string password)
        {
            if (string.IsNullOrEmpty(userName) || string.IsNullOrEmpty(password))
            {
                return null;
            }

            var user = _dbContext.Users.SingleOrDefault(x => x.UserName == userName);

            if (user == null)
            {
                return null;
            }

            if (PasswordHasher.HashPass(password) != user.Password)
            {
                return null;
            }

            return user;
        }
    }
}
