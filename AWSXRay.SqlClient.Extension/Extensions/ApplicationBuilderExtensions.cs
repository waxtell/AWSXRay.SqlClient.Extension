using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

// ReSharper disable once CheckNamespace
namespace AWSXRay.SqlClient.Extension
{
    public static class ApplicationBuilderExtensions
    {
        public static IApplicationBuilder ActivateXRaySqlClientDiagnosticsLogging(this IApplicationBuilder app)
        {
            app.ApplicationServices.GetService<XRaySqlClientDiagnosticLogger>();

            return app;
        }
    }
}
