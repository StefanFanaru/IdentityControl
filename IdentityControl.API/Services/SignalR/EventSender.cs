using System;
using System.Threading.Tasks;
using IdentityControl.API.Asp;
using IdentityControl.API.Extensions;
using Microsoft.AspNetCore.SignalR;
using Serilog;

namespace IdentityControl.API.Services.SignalR
{
    public class EventSender : IEventSender
    {
        private readonly IConnectionManager _connection;
        private readonly IHubContext<EventHub> _hubContext;
        private readonly IUserInfo _userInfo;

        public EventSender(IHubContext<EventHub> hubContext, IConnectionManager connection, IUserInfo userInfo)
        {
            _hubContext = hubContext;
            _connection = connection;
            _userInfo = userInfo;
        }

        public async Task SendAsync(string eventJson, string userId = null)
        {
            userId ??= _userInfo.Id;
            var connections = _connection.GetConnections(userId);

            try
            {
                if (connections != null && connections.Count > 0)
                {
                    foreach (var connection in connections)
                        try
                        {
                            await _hubContext.Clients.Clients(connection).SendAsync("socket", eventJson);
                        }
                        catch (Exception e)
                        {
                            Log.Error(e.Message);
                        }

                    Log.Debug($"Send event to user {userId.GetHumanReadableId()}");
                }
                else
                {
                    Log.Error($"No connections found for user {userId.GetHumanReadableId()}");
                }
            }
            catch (Exception e)
            {
                Log.Error(e.Message);
            }
        }

        public async Task SendToAllAsync(AppEvent appEvent)
        {
            await _hubContext.Clients.All.SendAsync("socket", appEvent.ToJson());
        }
    }
}