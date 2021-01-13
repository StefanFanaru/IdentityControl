using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IdentityControl.API.Data
{
    public interface IEfRepository<TEntity> where TEntity : class
    {
        public IQueryable<TEntity> Query();
        public Task<TEntity> GetByIdAsync(object id);
        public void Insert(TEntity entity);
        public Task InsertAsync(TEntity entity);
        public Task InsertRange(IEnumerable<TEntity> entities);
        public void Delete(object id);
        public void Delete(TEntity entity);
        public void Update(TEntity entity);
        public void Save();
        public Task<int> SaveAsync();
    }
}