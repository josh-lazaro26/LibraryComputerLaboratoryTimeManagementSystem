using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraryComputerLaboratoryTimeManagementSystem.Frontend.HelperClasses
{
    public static class TimeHelper
    {
        public static TimeSpan ParseApiDuration(string durationStr)
        {
            if (!TimeSpan.TryParse(durationStr, out var ts))
                return TimeSpan.Zero;

            return ts.Duration();
        }

        public static string ToDisplayFormat(TimeSpan ts)
        {
            if (ts <= TimeSpan.Zero) return "00:00";

            if (ts.TotalHours >= 1)
                return ts.ToString(@"hh\:mm\:ss");

            return ts.ToString(@"mm\:ss");
        }
    }
}
