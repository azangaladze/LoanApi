using LoanProject.Core.Entities;
using LoanProject.Core.FieldStrings;
using LoanProject.Infrastructure.Helpers;
using System.Linq;


namespace LoanProject.Infrastructure.Context
{
    public static class AppDbInitializer
    {
        public static void Seed(this AppDbContext context)
        {
            if (!context.Users.Any())
            {
                context.Users.Add(

                new User()
                {
                    FirstName = "Main",
                    LastName = "Accountant",
                    UserName = "acc",
                    Age = 25,
                    Email = "acc@test.com",
                    Salary = 0.0,
                    Password = PasswordHasher.HashPass("pass1234"),
                    IsBlocked = false,
                    Role = Roles.Accountant,
                });
                context.SaveChanges();
            }    
        }
    }
}
