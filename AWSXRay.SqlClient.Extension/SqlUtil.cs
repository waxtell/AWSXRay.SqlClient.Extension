using System.Text.RegularExpressions;

namespace AWSXRay.SqlClient.Extension
{
    internal class SqlUtil
    {
        private static readonly Regex PortNumberRegex = new Regex(@",\d+$");

        public static string RemovePortNumberFromDataSource(string dataSource)
        {
            return PortNumberRegex.Replace(dataSource, string.Empty);
        }
    }
}
