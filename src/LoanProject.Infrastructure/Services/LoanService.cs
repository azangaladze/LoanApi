using LoanProject.Core.Entities;
using LoanProject.Core.FieldStrings;
using LoanProject.Core.Interfaces;
using LoanProject.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LoanProject.Infrastructure.Services
{
    public class LoanService : ILoanService
    {
        private readonly AppDbContext _dbContext;
        public LoanService(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task<Loan> CreateAsync(Loan loan)
        {
            await _dbContext.Loans.AddAsync(loan);
            await _dbContext.SaveChangesAsync();
            return loan;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var dbLoan = await _dbContext.Loans.FindAsync(id);
            if (dbLoan == null)
            {
                return false;
            }
            _dbContext.Loans.Remove(dbLoan);
            await _dbContext.SaveChangesAsync();
            return true;
        }

        public async Task<Loan> GetByIdAsync(int id)
        {
            var dbLoan = await _dbContext.Loans.FindAsync(id);
            if (dbLoan == null)
            {
                return null;
            }
            return dbLoan;
        }

        public async Task<IEnumerable<Loan>> GetLoansAsync()
        {
            return await _dbContext.Loans.ToListAsync();
        }

        public async Task<bool> UpdateAsync(int id, Loan loan)
        {
            var dbLoan = await _dbContext.Loans.FindAsync(id);

            if (dbLoan == null || dbLoan.Id != id)
            {
                return false;
            }

            dbLoan.Type = loan.Type;
            dbLoan.Amount = loan.Amount;
            dbLoan.Currency = loan.Currency;
            dbLoan.Period = loan.Period;
            dbLoan.Loanstatus = loan.Loanstatus;

            _dbContext.Loans.Update(dbLoan);
            await _dbContext.SaveChangesAsync();
            return true;

        }

        public async Task<bool> ChangeLoanStatusAsync(int id, string status)
        {
            var dbLoan = await _dbContext.Loans.FindAsync(id);

            if (status.ToLower() == LoanStatus.Positive.ToLower() ||
                status.ToLower() == LoanStatus.Negative.ToLower())
            {
                dbLoan.Loanstatus = status;

                _dbContext.Loans.Update(dbLoan);
                await _dbContext.SaveChangesAsync();
                return true;
            }

            return false;
        }
    }
}
