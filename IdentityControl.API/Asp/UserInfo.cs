using System.Security.Claims;
using IdentityControl.Data.Constants;

namespace IdentityControl.API.Asp
{
    public class UserInfo : IUserInfo
    {
        public UserInfo(ClaimsPrincipal user)
        {
            Id = user.FindFirst(Claims.UserId).Value;
            FirstName = user.FindFirst(Claims.FirstName).Value;
            LastName = user.FindFirst(Claims.LastName).Value;
            Email = user.FindFirst(Claims.Email).Value;
            Role = user.FindFirst(Claims.Role).Value;
        }

        public string Id { get; }
        public string FirstName { get; }
        public string LastName { get; }
        public string Email { get; }
        public string Role { get; }
        public string Name => $"{(FirstName ?? "?")[0]}.{LastName}";
    }
}
