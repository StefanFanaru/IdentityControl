namespace IdentityControl.API.Common.Constants
{
    public static class AppConstants
    {
        /// <summary>
        ///     Used to declare entities that should not be modified. Can't save it as a entity prop because these are part of an
        ///     external inextensible DbContext
        /// </summary>
        public static class ReadOnlyEntities
        {
            public const string IdentityControlApiScope = "identity_control_full";
            public const string IdentityControlApiResource = "identity_control_full";
            public const string AngularClient = "identity_control_ng";
            public static readonly string[] AllApiScopes = {IdentityControlApiScope};
            public static readonly string[] AllApiResources = {IdentityControlApiResource};
            public static readonly string[] AllClients = {AngularClient};
        }

        public static class SecretTypes
        {
            public const string SharedSecret = "SharedSecret";
            public const string VisibleOneTime = "VisibleOneTime";
        }
    }
}