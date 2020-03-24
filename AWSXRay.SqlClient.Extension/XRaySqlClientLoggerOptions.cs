using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("AWSXRay.SqlClient.Extension.Tests")]

namespace AWSXRay.SqlClient.Extension
{
    public class XRaySqlClientLoggerOptions
    {
        public List<Belonging> CaptureQueryParameters { get; set; } = new List<Belonging>();


        internal bool ShouldCaptureQueryParameters(string comparator)
        {
            return
                CaptureQueryParameters
                    .LastOrDefault(x => x.IsMatch(comparator)) is Include;
        }
    }
}
