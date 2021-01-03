using System.Threading.Tasks;

namespace IdentityControl.API.Services.SignalR
{
    public interface IEventSender
    {
        Task SendAsync(string eventJson, string userId = null);
    }
}