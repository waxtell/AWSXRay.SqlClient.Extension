using System.Text.RegularExpressions;
using System;
using ConfigurationSection.Inheritance.Extension;

namespace AWSXRay.SqlClient.Extension
{
    [KnownType(typeof(Include), "include")]
    [KnownType(typeof(Exclude), "exclude")]
    [TypeConverter("type")]
    public abstract class Belonging
    {
        public bool IsRegEx { get; set; } = false;
        public string Expression { get; set; }

        internal bool IsMatch(string comparator)
        {
            if (IsRegEx)
            {
                var regex = new Regex(Expression);

                return regex.IsMatch(comparator);
            }

            return 
                Expression
                    .Equals(comparator, StringComparison.OrdinalIgnoreCase);
        }
    }
}
