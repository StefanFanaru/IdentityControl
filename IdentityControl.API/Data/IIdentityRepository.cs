using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IdentityControl.API.Services.ToasterEvents;

namespace IdentityControl.API.Data
{
    public interface IIdentityRepository<T> where T : class
    {
        /// <summary>
        ///     Gets the query on the database entities.
        /// </summary>
        IQueryable<T> Query();

        /// <summary>
        ///     Gets the entity by identifier.
        /// </summary>
        /// <returns></returns>
        Task<T> GetByIdAsync(object id);

        /// <summary>
        ///     Inserts the specified entity into the database.
        /// </summary>
        void Insert(T entity);

        /// <summary>
        ///     Inserts the specified entity into the database.
        /// </summary>
        Task InsertAsync(T entity);

        /// <summary>
        ///     Inserts multiple entities in a single trip.
        /// </summary>
        Task InsertRange(IEnumerable<T> entities);

        /// <summary>
        ///     Deletes the entity coresponding to the specified id.
        /// </summary>
        void Delete(object id);

        /// <summary>
        ///     Deletes the specified entity.
        /// </summary>
        void Delete(T entity);

        /// <summary>
        ///     Updates the specified entity.
        /// </summary>
        void Update(T entity);

        /// <summary>
        ///     Saves the modified entities to the database.
        /// </summary>
        void Save();

        /// <summary>
        ///     Saves the modified entities to the database.
        /// </summary>
        Task<int> SaveAsync();

        /// <summary>
        ///     Saves the modified entities to the database. And if the operation succeeded sends an event
        /// </summary>
        Task<int> SaveAsync(IToasterEvent toasterEvent, int expectedResult = 1);
    }
}