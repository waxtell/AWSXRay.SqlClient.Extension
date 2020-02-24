# AWSXRay.SqlClient.Extension
Non-invasive AWS XRay tracing for SQL Client

This code is based on the POC provided by [travisgosselin](https://github.com/travisgosselin), which may be found [here](https://github.com/aws/aws-xray-sdk-dotnet/issues/6#issuecomment-439515991).  This project also includes some features provided by the AWS TraceableSqlCommand.

This code/package provides a non-invasive mechanism for tracing sql calls.  Non-invasive, in this context, means not having to introduce TraceableSqlCommand in to your data access layer (Dapper FTW!).

Commands are included/excluded in accordance with rules defined in appsettings.json
```
{
  "XRay": {
    "CollectSqlQueries": "true"
  },
  "XRaySqlClientLoggerOptions": {
    "CaptureQueryParameters": [
      { "type": "include", "Expression": "[Test].[Whatever]", "IsRegEx": false },
      { "type": "exclude", "Expression": "[Test].[Sensitive]", "IsRegEx": false }
    ]
  }
}
```

The sample app needs more work, but it sufficiently demonstrates the configuration requirements.

Please note that you must register the diagnostic logger as well as the XRay middleware.  Additionally, the diagnostic logger must be activated **after** the middleware has been added.  This may be achieved by activating on start (if you're using Autofac or Ninject) or as such if you're using native .net core DI:

```csharp
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args)
                .Build()
                .ActivateXRaySqlClientDiagnosticsLogging()
                .Run();
        }
```

```csharp
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();
            services.AddSqlClientXRayTracing(Configuration);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseXRay("SampleApp9000", Configuration);
```