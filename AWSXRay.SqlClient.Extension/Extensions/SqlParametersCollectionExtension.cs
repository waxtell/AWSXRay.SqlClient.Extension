using System.Collections.Generic;
using System.Data;
using System.Linq;
using Microsoft.Data.SqlClient;

// ReSharper disable once CheckNamespace
namespace AWSXRay.SqlClient.Extension
{
    internal static class SqlParametersCollectionExtension
    {
        public static IEnumerable<SqlParameter> OutputParameters(this SqlParameterCollection parameters)
        {
            return
                parameters
                    .Cast<SqlParameter>()
                    .Where(p => p.Direction == ParameterDirection.Output || p.Direction == ParameterDirection.ReturnValue);
        }
        public static IEnumerable<SqlParameter> InputParameters(this SqlParameterCollection parameters)
        {
            return
                parameters
                    .Cast<SqlParameter>()
                    .Where(p => p.Direction == ParameterDirection.InputOutput || p.Direction == ParameterDirection.Input);
        }
    }
}
