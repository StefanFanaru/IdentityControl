using Microsoft.Extensions.Configuration;

namespace IdentityControl.API.Common.Extensions
{
    public static class SecretsExtensions
    {
        private static readonly IConfiguration Configuration = Startup.StaticConfiguration;

        // TODO: Find a better way to deal with this
        /// <summary>
        ///     Adds the secret stamp to any secret value to prepare it for storage
        /// </summary>
        public static string Stamp(this string secretValue)
        {
            return secretValue + '.' + Configuration["SecretStamp:Stamp"];
        }

        /// <summary>
        ///     Removes the secret stamp to make it safe for displaying in a client
        /// </summary>
        public static string Unstamp(this string secretValue)
        {
            return secretValue.Split('.')[0];
        }
    }
}