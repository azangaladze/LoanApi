using LoanProject.Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace LoanProject.Infrastructure.Context
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {

        }

        public virtual DbSet<User> Users { get; set; }
        public virtual DbSet<Loan> Loans { get; set; }

    }
}
