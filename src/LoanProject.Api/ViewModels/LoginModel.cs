using System.ComponentModel.DataAnnotations;

namespace LoanProject.Api.ViewModels
{
    public class LoginModel
    {
        [Required]
        public string Username { get; set;}
        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set;}
    }
}