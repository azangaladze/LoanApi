using FluentValidation;
using LoanProject.Infrastructure.Models;

namespace LoanProject.Infrastructure.Validators
{
    public class UserValidator : AbstractValidator<UserModel>
    {
        public UserValidator()
        {
            RuleFor(x => x.FirstName)
                .NotEmpty().WithMessage("First Name is required")
                .MaximumLength(50).WithMessage("First Name is too long, enter less than 50 letters");

            RuleFor(x => x.LastName)
                .NotEmpty().WithMessage("Last Name is required")
                .MaximumLength(50).WithMessage("Last Name is too long, enter less than 50 letters");

            RuleFor(x => x.Salary)
                .NotNull().WithMessage("Salary cannot be empty")
                .GreaterThan(-1).WithMessage("Salary cannot be nagative number");

            RuleFor(x => x.Age)
                .NotEmpty().WithMessage("Age is required")
                .LessThan(100).WithMessage("Incorrect Age Value")
                .GreaterThanOrEqualTo(18).WithMessage("Users with age under 18 are not allowed");

            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email is required")
                .EmailAddress().WithMessage("Email format is incorrect");

            RuleFor(x => x.UserName)
                .NotEmpty().WithMessage("Username is required")
                .MaximumLength(50).WithMessage("Username is too long, enter less than 50 letters");

            RuleFor(x => x.Password)
                .NotEmpty().WithMessage("Password is required")
                .MinimumLength(8).WithMessage("Password should be at least 8 symbols");
        }
    }
}
