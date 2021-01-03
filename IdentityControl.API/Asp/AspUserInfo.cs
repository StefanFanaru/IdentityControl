using Microsoft.AspNetCore.Http;

namespace IdentityControl.API.Asp
{
    public class AspUserInfo : UserInfo
    {
        public AspUserInfo(IHttpContextAccessor httpContextAccessor)
            : base(httpContextAccessor?.HttpContext?.User)
        {
        }
    }
}