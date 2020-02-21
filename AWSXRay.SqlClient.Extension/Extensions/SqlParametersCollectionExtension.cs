using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace AWSXRay.SqlClient.Extension.Extensions
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
