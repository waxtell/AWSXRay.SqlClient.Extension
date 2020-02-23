using Xunit;

namespace AWSXRay.SqlClient.Extension.Tests
{
    public class BelongingTests
    {
        [Fact]
        public void StoredProcedureMatchIncludedCaptureIsTrue()
        {
            var options = new XRaySqlClientLoggerOptions();
            options.CaptureQueryParameters.Add(new Include{ Expression = "[Schema].[StoredProc]", IsRegEx = false});

            Assert.True(options.ShouldCaptureQueryParameters("[Schema].[StoredProc]"));
        }

        [Fact]
        public void StoredProcedureMatchExcludedCaptureIsFalse()
        {
            var options = new XRaySqlClientLoggerOptions();
            options.CaptureQueryParameters.Add(new Exclude { Expression = "[Schema].[StoredProc]", IsRegEx = false });

            Assert.False(options.ShouldCaptureQueryParameters("[Schema].[StoredProc]"));
        }

        [Fact]
        public void IncludedAndExcludedMatchCaptureIsFalse()
        {
            var options = new XRaySqlClientLoggerOptions();
            options.CaptureQueryParameters.Add(new Include { Expression = "[Schema].[StoredProc]", IsRegEx = false });
            options.CaptureQueryParameters.Add(new Exclude { Expression = "[Schema].[StoredProc]", IsRegEx = false });

            Assert.False(options.ShouldCaptureQueryParameters("[Schema].[StoredProc]"));
        }
    }
}
