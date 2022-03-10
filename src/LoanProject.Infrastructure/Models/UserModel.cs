using System.Text.Json.Serialization;

namespace LoanProject.Infrastructure.Models
{
    public class UserModel
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string UserName { get; set; }
        public int Age { get; set; }
        public string Email { get; set; }
        public double Salary { get; set; }
        public string Password { get; set; }
        
    }
}
