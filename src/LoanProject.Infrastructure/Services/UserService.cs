using LoanProject.Core.Entities;
using LoanProject.Core.Interfaces;
using LoanProject.Infrastructure.Context;
using LoanProject.Infrastructure.Helpers;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LoanProject.Infrastructure.Services
{
    public class UserService : IUserService
    {
        private readonly AppDbContext _dbContext;
        public UserService(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var dbUser = await _dbContext.Users.FindAsync(id);
            if (dbUser == null)
            {
                return false;
            }
            _dbContext.Users.Remove(dbUser);
            await _dbContext.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<User>> GetUsersAsync()
        {
            return await _dbContext.Users.Include(x => x.Loans).ToListAsync();
        }

        public async Task<User> GetByIdAsync(int id)
        {
            var dbUser = await _dbContext.Users.Include(x => x.Loans).FirstOrDefaultAsync(x => x.Id == id);
            if (dbUser == null)
            {
                return null;
            }
            return dbUser;

        }

        public async Task<bool> UpdateAsync(int id, User user)
        {
            var dbUser = await _dbContext.Users.FindAsync(id);

            if (dbUser == null || dbUser.Id != id)
            {
                return false;
            }

            dbUser.FirstName = user.FirstName;
            dbUser.LastName = user.LastName;
            dbUser.UserName = user.UserName;
            dbUser.Age = user.Age;
            dbUser.Salary = user.Salary;
            dbUser.Email = user.Email;
            dbUser.Password = PasswordHasher.HashPass(user.Password);

            _dbContext.Users.Update(dbUser);
            await _dbContext.SaveChangesAsync();

            return true;

        }

        public async Task<bool> ChangeStatusAsync(int id, bool isblocked)
        {
            var dbUser = await _dbContext.Users.FindAsync(id);

            if (dbUser == null || dbUser.Id != id)
            {
                return false;
            }

            dbUser.IsBlocked = isblocked;

            _dbContext.Users.Update(dbUser);
            await _dbContext.SaveChangesAsync();
            return true;

        }

    }
}
