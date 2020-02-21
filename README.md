# AWSXRay.SqlClient.Extension
Non-invasive AWS XRay tracing for SQL Client

This code is based on the POC provided by [travisgosselin](https://github.com/travisgosselin), which may be found [here](https://github.com/aws/aws-xray-sdk-dotnet/issues/6#issuecomment-439515991).  This project also includes some features provided by the AWS TraceableSqlCommand.

This code/package provides a non-invasive mechanism for tracing sql calls.  Non-invasive, in this context, means not having to introduce TraceableSqlCommand in to your data access layer (Dapper FTW!).

Commands are included/excluded in accordance with rules defined in appsettings.json
```
{
  "XRaySqlClientLoggerOptions": {
    "CaptureQueryParameters": [
      { "type": "include", "Expression": "[Test].[Whatever]", "IsRegEx": false },
      { "type": "exclude", "Expression": "[Test].[Sensitive]", "IsRegEx": false }
    ]
  }
}

```