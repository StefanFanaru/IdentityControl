using System;
using IdentityControl.API.Extensions;
using IdentityControl.API.Services.ToasterEvents;

namespace IdentityControl.API.Services.SignalR
{
    public static class EventBuilder
    {
        public static AppEvent BuildToasterEvent(IToasterEvent model)
        {
            var identifier = model.Identifier == null ? string.Empty : $" \"{model.Identifier}\" ";
            var eventVerb = model.Verb.ToString().ToLower();
            var eventTypeString = model.EntityType.FormatType();
            var title = model.Verb.ToString();
            var message = $"{eventTypeString}{identifier} has been {eventVerb} successfully";

            if (model.Count > 1)
            {
                message = $"{model.Count} {eventTypeString}s were {eventVerb} successfully";
            }

            if (model.ToasterType == ToasterType.Error)
            {
                title = "Application Error";
                message = $"Failed to {eventVerb.Substring(0, eventVerb.Length - 1)} {eventTypeString}";
                message = model.Count > 1 ? message + "s" : message + $" {identifier}";
            }

            return new AppEvent
            {
                Title = title,
                Message = message,
                Type = model.ToasterType.ToString(),
                CreatedAt = DateTime.UtcNow
            };
        }
    }
}