using System.Threading.Tasks;
using IdentityControl.API.Services.ToasterEvents;

namespace IdentityControl.API.Data
{
    public interface IConfigurationRepository<T> : IEfRepository<T> where T : class
    {
        /// <summary>
        ///     Saves the modified entities to the database. And if the operation succeeded sends an event
        /// </summary>
        Task<int> SaveAsync(IToasterEvent toasterEvent, int expectedResult = 1);
    }
}