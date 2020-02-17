using Microsoft.Extensions.DependencyInjection;

namespace AWSXRay.SqlClient.Extension.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddSqlClientXRayTracing(this IServiceCollection collection)
        {
            return
                collection
                    .AddSingleton<XRaySqlClientDiagnosticLogger>();
        }
    }
}
