using System;
using System.Security.Claims;

namespace LoanProject.Api.Helper
{
    public static class UserExtensions
    {
        public static int GetUserId(this ClaimsPrincipal user)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            var userId = user.FindFirstValue(ClaimTypes.NameIdentifier);

            if (userId == null)
            {
                return -1;
            }

            if (int.TryParse(userId, out int userIdInt))
            {
                return userIdInt;
            }

            return -1;
        }
    }
}
