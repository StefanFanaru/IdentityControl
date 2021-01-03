namespace IdentityControl.API.Services.ToasterEvents
{
    public interface IToasterEvent
    {
        string EntityType { get; set; }
        ToasterType ToasterType { get; set; }
        ToasterVerbs Verb { get; set; }
        string Identifier { get; set; }
        int Count { get; set; }

        /// <summary>
        ///     Returns an AppEvent instance as a JSON string
        /// </summary>
        string GetEvent();

        /// <summary>
        ///     Changes the Toaster instance to reflect the corresponding failure
        /// </summary>
        /// <returns>AppEvent instance as a JSON string</returns>
        string TransformInFailure();
    }
}