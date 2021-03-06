﻿using ConfigurationSection.Inheritance.Extension;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

// ReSharper disable once CheckNamespace
namespace AWSXRay.SqlClient.Extension
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddSqlClientXRayTracing(this IServiceCollection collection)
        {
            return
                AddSqlClientXRayTracing(collection, new XRaySqlClientLoggerOptions());
        }

        public static IServiceCollection AddSqlClientXRayTracing(this IServiceCollection collection, IConfiguration config, string configKey = nameof(XRaySqlClientLoggerOptions))
        {
            return
                AddSqlClientXRayTracing
                (
                    collection, 
                    config
                        .GetSection(configKey)
                        .GetEx<XRaySqlClientLoggerOptions>()
                );
        }

        public static IServiceCollection AddSqlClientXRayTracing(this IServiceCollection collection, XRaySqlClientLoggerOptions options)
        {
            return
                collection
                    .AddSingleton(options)
                    .AddSingleton<XRaySqlClientDiagnosticLogger>();
        }
    }
}
