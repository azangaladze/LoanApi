using LoanProject.Core.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LoanProject.Core.Interfaces
{
    public interface IUserService
    {
        Task<IEnumerable<User>> GetUsersAsync();
        Task<User> GetByIdAsync(int id);
        Task<bool> UpdateAsync(int id, User user);
        Task<bool> DeleteAsync(int id);
        Task<bool> ChangeStatusAsync(int id, bool isblocked);
    }
}
