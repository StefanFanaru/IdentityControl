using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;

namespace IdentityControl.API.Services.SignalR
{
    public class EventHub : Hub
    {
        private readonly IConnectionManager _connectionManager;

        public EventHub(IConnectionManager connectionManager)
        {
            _connectionManager = connectionManager;
        }

        public string GetConnectionId()
        {
            var httpContext = Context.GetHttpContext();
            var userId = httpContext.Request.Query["userId"];
            _connectionManager.AddConnection(userId, Context.ConnectionId);
            return Context.ConnectionId;
        }

        //Called when a connection with the hub is terminated.
        public override async Task OnDisconnectedAsync(Exception exception)
        {
            //get the connectionId
            var connectionId = Context.ConnectionId;
            _connectionManager.RemoveConnection(connectionId);
            await Task.FromResult(0);
        }
    }
}