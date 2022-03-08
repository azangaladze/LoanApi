using LoanProject.Core.Entities;
using System.Threading.Tasks;

namespace LoanProject.Core.Interfaces
{
    public interface IAccountService
    {
        User Login(string userName, string password);
        Task<User> CreateAsync(User user);
    }
}
