using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;

namespace IdentityControl.API.Asp.Authorization
{
    public class SecretKeyRequirement : IAuthorizationRequirement
    {
        private HashSet<string> _secretKeys;

        public SecretKeyRequirement(string secretKey)
        {
            _secretKeys = new HashSet<string>(new List<string> {secretKey});
        }

        public SecretKeyRequirement(IEnumerable<string> secretKeys)
        {
            _secretKeys = new HashSet<string>(secretKeys);
        }

        public bool ContainsSecret(string secretKey)
        {
            return _secretKeys.Contains(secretKey);
        }
    }
}