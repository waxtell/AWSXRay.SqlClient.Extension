using Microsoft.Extensions.Hosting;

// ReSharper disable once CheckNamespace
namespace AWSXRay.SqlClient.Extension
{
    public static class HostExtensions
    {
        public static IHost ActivateXRaySqlClientDiagnosticsLogging(this IHost host)
        {
            return
                host
                    .ActivateService<XRaySqlClientDiagnosticLogger>();
        }
    }
}
