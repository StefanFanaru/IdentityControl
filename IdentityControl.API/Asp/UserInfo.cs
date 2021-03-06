﻿using System.Security.Claims;
using IdentityControl.API.Common.Constants;
using IdentityControl.API.Extensions;

namespace IdentityControl.API.Asp
{
    public class UserInfo : IUserInfo
    {
        public UserInfo(ClaimsPrincipal user)
        {
            if (user != null)
            {
                Id = user.FindFirstValue(Claims.UserId);
                FirstName = user.FindFirstValue(Claims.FirstName);
                LastName = user.FindFirstValue(Claims.LastName);
                Email = user.FindFirstValue(Claims.Email);
                Role = user.FindFirstValue(Claims.Role);
                OwnedBlogs = user.FindFirstValue(Claims.OwnedBlogs).FromJson<string[]>();
            }
        }

        public string Id { get; }
        public string FirstName { get; }
        public string LastName { get; }
        public string Email { get; }
        public string Role { get; }
        public string Name => $"{FirstName} {LastName}";
        public string[] OwnedBlogs { get; }
    }
}