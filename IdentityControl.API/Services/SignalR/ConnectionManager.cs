using System;
using System.Collections.Generic;
using IdentityControl.API.Extensions;
using Serilog;

namespace IdentityControl.API.Services.SignalR
{
    public class ConnectionManager : IConnectionManager
    {
        private static readonly Dictionary<string, HashSet<string>> userMap = new Dictionary<string, HashSet<string>>();
        public IEnumerable<string> OnlineUsers => userMap.Keys;

        public void AddConnection(string userId, string connectionId)
        {
            lock (userMap)
            {
                if (!userMap.ContainsKey(userId))
                {
                    userMap[userId] = new HashSet<string>();
                }

                userMap[userId].Add(connectionId);
                Log.Debug($"User {userId.GetHumanReadableId()} has established a connection with the client");
            }
        }

        public void RemoveConnection(string connectionId)
        {
            lock (userMap)
            {
                foreach (var userId in userMap.Keys)
                {
                    if (userMap.ContainsKey(userId) && userMap[userId].Contains(connectionId))
                    {
                        userMap[userId].Remove(connectionId);
                        Log.Debug($"User {userId.GetHumanReadableId()} has disconnected from the client");
                        break;
                    }
                }
            }
        }

        public HashSet<string> GetConnections(string userId)
        {
            HashSet<string> connection;

            try
            {
                lock (userMap)
                {
                    connection = userMap[userId];
                }
            }
            catch (Exception e)
            {
                Log.Error(e.Message);
                connection = null;
            }

            return connection;
        }
    }
}