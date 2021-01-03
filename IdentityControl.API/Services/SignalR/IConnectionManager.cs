using System.Collections.Generic;

namespace IdentityControl.API.Services.SignalR
{
    public interface IConnectionManager
    {
        IEnumerable<string> OnlineUsers { get; }
        void AddConnection(string userId, string connectionId);
        void RemoveConnection(string connectionId);
        HashSet<string> GetConnections(string userId);
    }
}