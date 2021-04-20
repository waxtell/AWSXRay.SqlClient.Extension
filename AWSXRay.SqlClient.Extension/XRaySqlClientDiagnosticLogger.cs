using Microsoft.Extensions.DiagnosticAdapter;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Diagnostics;
using System.Linq;
using Amazon.XRay.Recorder.Core;
using Microsoft.Data.SqlClient;

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

        [DiagnosticName("Microsoft.Data.SqlClient")]
        public void IsEnabled()
        {
        }

        private static void CreateSubSegmentForConnection(DbConnection connection, IAWSXRayRecorder recorder)
        {
            string BuildSubSegmentName() => connection.Database + "@" + SqlUtil.RemovePortNumberFromDataSource(connection.DataSource);

            recorder.BeginSubsegment(BuildSubSegmentName());
            recorder.SetNamespace("remote");
            recorder.AddSqlInformation("database_type", "sqlserver");
            recorder.AddSqlInformation("database_version", connection.State == ConnectionState.Open ? connection.ServerVersion : "Not Available");

            var connectionStringBuilder = new SqlConnectionStringBuilder(connection.ConnectionString);
            connectionStringBuilder.Remove("Password");
            if (!string.IsNullOrEmpty(connectionStringBuilder.UserID))
            {
                recorder.AddSqlInformation("user", connectionStringBuilder.UserID);
            }

            recorder.AddSqlInformation("connection_string", connectionStringBuilder.ToString());
        }

        private static void AddCommandDetails(SqlCommand command, IAWSXRayRecorder recorder, bool captureParameters)
        {
            recorder.AddSqlInformation("sanitized_query", command.CommandText);

            if (command.Parameters.InputParameters().Any() && captureParameters)
            {
                foreach (var p in command.Parameters.InputParameters())
                {
                    recorder
                        .AddMetadata
                        (
                            p.ParameterName,
                            p.Value
                        );
                }
            }
        }

        [DiagnosticName("Microsoft.Data.SqlClient.WriteCommandBefore")]
        public void SqlClientWriteCommandBefore(Guid operationId, string operation, Guid _, SqlCommand command)
        {
            AWSXRayRecorder
                .Instance
                .With
                (
                    recorder =>
                    {
                        if (recorder.IsEntityPresent() && command != null)
                        {
                            CreateSubSegmentForConnection(command.Connection, recorder);

                            if (recorder.XRayOptions.CollectSqlQueries)
                            {
                                AddCommandDetails(command, recorder, _options.ShouldCaptureQueryParameters(command.CommandText));
                            }
                        }
                    }
                );
        }

        [DiagnosticName("Microsoft.Data.SqlClient.WriteCommandAfter")]
        public void SqlClientWriteCommandAfter(Guid operationId, string operation, Guid _, SqlCommand command)
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
                                foreach (var p in command.Parameters.OutputParameters())
                                {
                                    recorder
                                        .AddMetadata
                                        (
                                            p.ParameterName,
                                            p.Value
                                        );
                                }
                            }
                        }

                        recorder.EndSubsegment();
                    }
                );
        }

        [DiagnosticName("Microsoft.Data.SqlClient.WriteCommandError")]
        public void SqlClientWriteCommandError(Guid operationId, string operation, Guid _, SqlCommand command, Exception exception)
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

        [DiagnosticName("Microsoft.Data.SqlClient.WriteConnectionOpenError")]
        public void SqlClientWriteConnectionOpenError(SqlConnection connection, Exception exception)
        {
            AWSXRayRecorder
                .Instance
                .With
                (
                    recorder =>
                    {
                        if (recorder.IsEntityPresent() && exception != null)
                        {
                            CreateSubSegmentForConnection(connection, recorder);

                            recorder
                                .AddException(exception);
                        }
                    }
                );
        }

        [DiagnosticName("Microsoft.Data.SqlClient.WriteConnectionCloseError")]
        public void SqlClientWriteConnectionCloseError(SqlConnection _, Exception exception)
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
