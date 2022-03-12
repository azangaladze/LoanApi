using LoanProject.Core.FieldStrings;
using System.Collections.Generic;

namespace LoanProject.Core.Entities
{
    public class User
    {
        public int Id { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public int Age { get; set; } = 0;
        public string Email { get; set; } = string.Empty;
        public double Salary { get; set; }
        public bool IsBlocked { get; set; } = false;
        public string Password { get; set; } = string.Empty;
        public string Role { get; set; } = Roles.User;
        public virtual ICollection<Loan> Loans { get; set; } = new HashSet<Loan>();

    }
}
