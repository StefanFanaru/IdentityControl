using System;

namespace IdentityControl.API.Services.SignalR
{
    public class AppEvent
    {
        public string Title { get; set; }
        public string Message { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public string Type { get; set; }
    }
}