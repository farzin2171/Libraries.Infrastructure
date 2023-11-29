using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.Net.Http.Headers;
using Serilog.Core;
using Serilog.Events;

namespace WT.Logging
{
    public class DefaultHttpLoggerEnricher : ILogEventEnricher
    {
        private readonly HttpContext _context;

        public DefaultHttpLoggerEnricher(HttpContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Enrich the log event.
        /// </summary>
        /// <param name="logEvent">The log event to enrich.</param>
        /// <param name="propertyFactory">Factory for creating new properties to add to the event.</param>
        public void Enrich(LogEvent logEvent, ILogEventPropertyFactory propertyFactory)
        {
            if (_context?.Request != null)
            {
                var values = _context?.ExtractPropertiesFromRequest(propertyFactory);
                foreach (var value in values)
                {
                    logEvent.AddPropertyIfAbsent(value);
                }
            }
        }
    }

    public static class HttpContextExtensions
    {
        public static IEnumerable<LogEventProperty> ExtractPropertiesFromRequest(this HttpContext context, ILogEventPropertyFactory propertyFactory)
        {
            var properties = new List<LogEventProperty>();
            if (context.Request.Headers.TryGetValue(HeaderNames.Authorization, out var maybeBearerToken))
            {
                try
                {
                    var token = new JwtSecurityToken(jwtEncodedString: maybeBearerToken.ToString().Replace("Bearer", string.Empty, StringComparison.InvariantCultureIgnoreCase).Trim());
                    var jwtProperties = new Dictionary<string, string>()
                    {
                        { DefaultHttpLoggerKeys.Email, token.Payload["email"] as string ?? string.Empty },
                        { DefaultHttpLoggerKeys.PrincipalId, token.Payload.Sub ?? string.Empty },
                        {
                            DefaultHttpLoggerKeys.ExternalPrincipalId,
                            token.Payload["externalobjectidentifier"] as string ?? string.Empty
                        },
                        { DefaultHttpLoggerKeys.Tenant, token.Payload["tenant"] as string ?? string.Empty },
                        { DefaultHttpLoggerKeys.TenantId, token.Payload["tenantId"] as string ?? string.Empty },
                        { DefaultHttpLoggerKeys.Name, token.Payload["name"] as string ?? string.Empty },
                    };

                    foreach (var tokenValue in jwtProperties.Where(_ => !string.IsNullOrEmpty(_.Value)))
                    {
                        properties.Add(propertyFactory.CreateProperty(tokenValue.Key, tokenValue.Value));
                    }
                }
                catch (Exception e)
                {
                    // could not enrich
                    properties.Add(propertyFactory.CreateProperty(DefaultHttpLoggerKeys.InvalidJWT, e, destructureObjects: true));
                }
            }

            if (context.Request.Headers.TryGetValue(EnricherHttpHeaders.XTenant, out var tenant))
            {
                properties.Add(propertyFactory.CreateProperty(DefaultHttpLoggerKeys.Tenant, tenant.ToString()));
            }

            if (context.Request.Headers.TryGetValue(EnricherHttpHeaders.XOnBehalfOfUserId, out var onBehalfOf))
            {
                properties.Add(propertyFactory.CreateProperty(DefaultHttpLoggerKeys.OnBehalfOfPrincipalId, onBehalfOf.ToString()));
            }

            if (context.Request.Headers.TryGetValue(EnricherHttpHeaders.XOnBehalfOfExternalUserId, out var onBehalfOfExternalUserId))
            {
                properties.Add(propertyFactory.CreateProperty(DefaultHttpLoggerKeys.OnBehalfOfExternalPrincipalId, onBehalfOfExternalUserId.ToString()));
            }

            return properties;
        }
    }
}
