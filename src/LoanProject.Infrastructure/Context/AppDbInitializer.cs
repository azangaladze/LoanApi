using LoanProject.Core.Entities;
using LoanProject.Core.EntityFields;
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
                context.Users.AddRange(

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
                },


                new User()
                {
                    FirstName = "Test",
                    LastName = "User",
                    UserName = "user",
                    Age = 25,
                    Email = "test@test.com",
                    Salary = 1000.0,
                    Password = PasswordHasher.HashPass("user1234"),
                    IsBlocked = false,
                    Role = Roles.User,
                });
                context.SaveChanges();
            }    
        }
    }
}
