using System.Text.RegularExpressions;

namespace AWSXRay.SqlClient.Extension
{
    internal class SqlUtil
    {
        private static readonly Regex _portNumberRegex = new Regex(@",\d+$");

        public static string RemovePortNumberFromDataSource(string dataSource)
        {
            return _portNumberRegex.Replace(dataSource, string.Empty);
        }
    }
}
