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
            var exclude = CaptureQueryParameters
                            ?.OfType<Exclude>()
                            .Any(x => x.IsMatch(comparator));

            var include = CaptureQueryParameters
                            ?.OfType<Include>()
                            .Any(x => x.IsMatch(comparator));

            return 
            (
                (include.HasValue && include.Value) && 
                (!exclude.HasValue || !exclude.Value)
            );
        }
    }
}
