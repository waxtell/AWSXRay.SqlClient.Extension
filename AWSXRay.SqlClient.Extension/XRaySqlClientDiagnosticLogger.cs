using Microsoft.Extensions.DiagnosticAdapter;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using Amazon.XRay.Recorder.Core;
using Newtonsoft.Json;

namespace AWSXRay.SqlClient.Extension
{
    public class XRaySqlClientDiagnosticLogger : IObserver<DiagnosticListener>, IDisposable
    {
        private const string SqlClientDiagnosticListenerKey = "SqlClientDiagnosticListener";
        private readonly List<IDisposable> _subscription = new List<IDisposable>();
        private readonly XRaySqlClientLoggerOptions _options;

        public XRaySqlClientDiagnosticLogger(XRaySqlClientLoggerOptions options)
        {
            _subscription.Add(DiagnosticListener.AllListeners.Subscribe(this));
            _options = options;
        }

        [DiagnosticName("System.Data.SqlClient")]
        public void IsEnabled()
        {
        }

        [DiagnosticName("System.Data.SqlClient.WriteCommandBefore")]
        public void SqlClientWriteCommandBefore(Guid operationId, string operation, Guid connectionId, SqlCommand command)
        {
            string BuildSubSegmentName(DbCommand cmd)
                => cmd.Connection.Database + "@" +
                   SqlUtil.RemovePortNumberFromDataSource(cmd.Connection.DataSource);

            AWSXRayRecorder
                .Instance
                .With
                (
                    recorder =>
                    {
                        if (recorder.IsEntityPresent() && command != null)
                        {
                            recorder.BeginSubsegment(BuildSubSegmentName(command));
                            recorder.SetNamespace("remote");
                            recorder.AddSqlInformation("database_type", "sqlserver");
                            recorder.AddSqlInformation("database_version", command.Connection.ServerVersion);

                            var connectionStringBuilder = new SqlConnectionStringBuilder(command.Connection.ConnectionString);
                            connectionStringBuilder.Remove("Password");
                            if (!string.IsNullOrEmpty(connectionStringBuilder.UserID))
                            {
                                recorder.AddSqlInformation("user", connectionStringBuilder.UserID);
                            }

                            recorder.AddSqlInformation("connection_string", connectionStringBuilder.ToString());

                            if (recorder.XRayOptions.CollectSqlQueries)
                            {
                                recorder.AddSqlInformation("sanitized_query", command.CommandText);

                                if (command.Parameters.InputParameters().Any() && _options.ShouldCaptureQueryParameters(command.CommandText))
                                {
                                    foreach (SqlParameter p in command.Parameters)
                                    {
                                        if (p.Direction == ParameterDirection.Input ||
                                            p.Direction == ParameterDirection.InputOutput)
                                        {
                                            recorder
                                                .AddMetadata
                                                (
                                                    p.ParameterName,
                                                    JsonConvert.SerializeObject(p.Value)
                                                );
                                        }
                                    }
                                }
                            }
                        }
                    }
                );
        }

        [DiagnosticName("System.Data.SqlClient.WriteCommandAfter")]
        public void SqlClientWriteCommandAfter(Guid operationId, string operation, Guid connectionId, SqlCommand command)
        {
            AWSXRayRecorder
                .Instance
                .With
                (
                    recorder =>
                    {
                        if (recorder.IsEntityPresent() && command != null)
                        {
                            if (command.Parameters.OutputParameters().Any() && _options.ShouldCaptureQueryParameters(command.CommandText))
                            {
                                foreach (SqlParameter p in command.Parameters.OutputParameters())
                                {
                                    recorder
                                        .AddMetadata
                                        (
                                            p.ParameterName,
                                            JsonConvert.SerializeObject(p.Value)
                                        );
                                }
                            }
                        }

                        recorder.EndSubsegment();
                    }
                );
        }

        [DiagnosticName("System.Data.SqlClient.WriteCommandError")]
        public void SqlClientWriteCommandError(Guid operationId, string operation, Guid connectionId, SqlCommand command, Exception exception)
        {
            AWSXRayRecorder
                .Instance
                .With
                (
                    recorder =>
                    {
                        if (recorder.IsEntityPresent() && exception != null)
                        {
                            recorder
                                .AddException(exception);
                        }
                    }
                );
        }

        public void Dispose()
        {
            foreach (var sub in _subscription)
            {
                sub.Dispose();
            }
        }

        void IObserver<DiagnosticListener>.OnCompleted()
        {

        }

        void IObserver<DiagnosticListener>.OnError(Exception error)
        {

        }

        void IObserver<DiagnosticListener>.OnNext(DiagnosticListener value)
        {
            if (value.Name == SqlClientDiagnosticListenerKey)
            {
                _subscription.Add(value.SubscribeWithAdapter(this));
            }
        }
    }
}
