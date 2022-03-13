using FluentValidation;
using LoanProject.Infrastructure.Models;

namespace LoanProject.Infrastructure.Validators
{
    public class LoanValidator : AbstractValidator<LoanModel>
    {
        public LoanValidator()
        {
            RuleFor(x => x.Type)
               .NotEmpty().WithMessage("Type is required")
               .Must(x => x.ToLower() == "express" || x.ToLower() == "auto" || x.ToLower() == "installment")
               .WithMessage("Type must be Express, Auto, or Installment");

            RuleFor(x => x.Period)
                    .NotEmpty().WithMessage("Period is required")
                    .LessThanOrEqualTo(120).WithMessage("Maximum loan period is 120 months");

            RuleFor(x => x.Amount)
                    .NotEmpty().WithMessage("Amount is required")
                    .GreaterThan(-1).WithMessage("Amount cannot be nagative number")
                    .LessThanOrEqualTo(100000).WithMessage("Maximum amount is 100000");

            RuleFor(x => x.Currency)
                .NotEmpty().WithMessage("Currency is required")
                .Must(x => x.ToUpper() == "GEL" || x.ToUpper() == "USD" || x.ToUpper() == "EUR" || x.ToUpper() == "GBP")
                .WithMessage("Currency must be GEL, USD, EUR or GBP");

        }
    }
}
