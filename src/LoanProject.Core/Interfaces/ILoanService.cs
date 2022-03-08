using LoanProject.Core.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LoanProject.Core.Interfaces
{
    public interface ILoanService
    {
        Task<IEnumerable<Loan>> GetLoansAsync();
        Task<Loan> GetByIdAsync(int id);
        Task<Loan> CreateAsync(Loan loan);
        Task<bool> UpdateAsync(int id, Loan loan);
        Task<bool> DeleteAsync(int id);
        Task<bool> ChangeLoanStatusAsync(int id, string status);
    }
}
