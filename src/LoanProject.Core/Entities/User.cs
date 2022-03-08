using LoanProject.Core.FieldStrings;
using System.Collections.Generic;

namespace LoanProject.Core.Entities
{
    public class User
    {

        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string UserName { get; set; }
        public int Age { get; set; }
        public string Email { get; set; }
        public double Salary { get; set; }
        public bool IsBlocked { get; set; } = false;
        public string Password { get; set; }
        public string Role { get; set; } = Roles.User;
        public virtual ICollection<Loan> Loans { get; set; } = new HashSet<Loan>();

    }
}
