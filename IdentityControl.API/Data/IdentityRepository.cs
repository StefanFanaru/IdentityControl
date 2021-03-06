﻿using System.Threading.Tasks;
using IdentityControl.API.Extensions;
using IdentityControl.API.Services.SignalR;
using IdentityControl.API.Services.ToasterEvents;

namespace IdentityControl.API.Data
{
    public class IdentityRepository<T> : EfRepository<IdentityContext, T>, IIdentityRepository<T> where T : class
    {
        private readonly IdentityContext _context;
        private readonly IEventSender _eventSender;

        public IdentityRepository(IdentityContext context, IEventSender eventSender) : base(context)
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