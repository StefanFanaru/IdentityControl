using IdentityControl.API.Data.Entities;

namespace IdentityControl.API.Data
{
    public class UserRepository : EfRepository<IdentityContext, ApplicationUser>, IUserRepository
    {
        public UserRepository(IdentityContext context) : base(context)
        {
        }
    }
}