using System.Threading.Tasks;
using IdentityControl.API.Extensions;
using IdentityControl.API.Services.SignalR;
using IdentityControl.API.Services.ToasterEvents;
using IdentityServer4.EntityFramework.DbContexts;

namespace IdentityControl.API.Data
{
    public class IdentityRepository<T> : EfRepository<ConfigurationDbContext, T>, IIdentityRepository<T> where T : class
    {
        private readonly ConfigurationDbContext _context;
        private readonly IEventSender _eventSender;

        public IdentityRepository(ConfigurationDbContext context, IEventSender eventSender) : base(context)
        {
            _context = context;
            _eventSender = eventSender;
        }

        public async Task<int> SaveAsync(IToasterEvent toasterEvent, int expectedResult = 1)
        {
            var result = await _context.SaveChangesAsync();

            if (result == expectedResult)
            {
                var success = EventBuilder.BuildToasterEvent(toasterEvent);
                await _eventSender.SendAsync(success.ToJson());
            }
            else
            {
                var failure = toasterEvent.TransformInFailure();
                await _eventSender.SendAsync(failure);
            }

            return result;
        }
    }
}