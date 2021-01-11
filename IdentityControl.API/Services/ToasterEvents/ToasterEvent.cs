using IdentityControl.API.Extensions;
using IdentityControl.API.Services.SignalR;

namespace IdentityControl.API.Services.ToasterEvents
{
    public class ToasterEvent : IToasterEvent
    {
        public ToasterEvent(string entityType, ToasterType toasterType, ToasterVerbs verb, string identifier = null,
            int count = 1)
        {
            EntityType = entityType;
            ToasterType = toasterType;
            Verb = verb;
            Identifier = identifier;
            Count = count;
        }

        public string EntityType { get; set; }
        public ToasterType ToasterType { get; set; }
        public ToasterVerbs Verb { get; set; }
        public string Identifier { get; set; }
        public int Count { get; set; }

        /// <summary>
        ///     Returns an AppEvent instance as a JSON string
        /// </summary>
        public string GetEvent()
        {
            return EventBuilder.BuildToasterEvent(this).ToJson();
        }

        /// <summary>
        ///     Changes the Toaster instance to reflect the corresponding failure
        /// </summary>
        /// <returns>AppEvent instance as a JSON string</returns>
        public string TransformInFailure()
        {
            return EventBuilder.BuildToasterEvent(new ToasterEvent(EntityType, ToasterType.Error, Verb, Identifier, Count))
                .ToJson();
        }
    }
}