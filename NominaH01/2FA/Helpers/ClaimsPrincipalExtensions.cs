using System;
using System.Security.Claims;

namespace _2FA.Helpers
{
    public static class ClaimsPrincipalExtensions
    {
        public static int RequireEmployeeId(this ClaimsPrincipal user)
        {
            var value = user.FindFirst("EmployeeId")?.Value;
            if (string.IsNullOrEmpty(value)) throw new UnauthorizedAccessException("EmployeeId claim missing");
            return int.Parse(value);
        }
    }
}
