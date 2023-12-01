namespace WT.Logging
{
    public static class DefaultHttpLoggerKeys
    {
        public const string Tenant = "Tenant";
        public const string TenantId = "TenantId";
        public const string PrincipalId = "PrincipalId";
        public const string ExternalPrincipalId = "ExternalPrincipalId";
        public const string Name = "Name";
        public const string Email = "Email";
        public const string OnBehalfOfPrincipalId = "OnBehalfOfPrincipalId";
        public const string OnBehalfOfExternalPrincipalId = "OnBehalfOfExternalPrincipalId";
        public const string InvalidJWT = "InvalidJWT";
    }

    public static class EnricherHttpHeaders
    {
        public const string XTenant = "X-Tenant-Name";
        public const string XOnBehalfOfUserId = "X-OnBehalfOf-User-Identifier";
        public const string XOnBehalfOfExternalUserId = "X-OnBehalfOf-External-User-Identifier";
    }
}
