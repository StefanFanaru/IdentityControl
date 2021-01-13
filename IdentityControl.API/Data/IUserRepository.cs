using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IdentityControl.API.Data.Entities;

namespace IdentityControl.API.Data
{
    public interface IUserRepository
    {
        IQueryable<ApplicationUser> Query();
        Task<ApplicationUser> GetByIdAsync(object id);
        void Insert(ApplicationUser entity);
        Task InsertAsync(ApplicationUser entity);
        Task InsertRange(IEnumerable<ApplicationUser> entities);
        void Delete(object id);
        void Delete(ApplicationUser entity);
        void Update(ApplicationUser entity);
        void Save();
        Task<int> SaveAsync();
    }
}