using System;
using System.Globalization;

namespace RWA.Web.Application.Services.Workflow
{
    public static class DateParsingHelper
    {
        public static DateOnly? ParseFlexibleDate(string dateString)
        {
            if (string.IsNullOrWhiteSpace(dateString))
            {
                return null;
            }

            // First, try to parse as a double, which is what Excel uses for dates.
            if (double.TryParse(dateString, out double oaDate))
            {
                try
                {
                    return DateOnly.FromDateTime(DateTime.FromOADate(oaDate));
                }
                catch (ArgumentException)
                {
                    // Not a valid OADate, so proceed to string parsing.
                }
            }

            // Next, try parsing with a variety of common formats.
            var formats = new[]
            {
                "dd/MM/yyyy HH:mm:ss",
                "dd/MM/yyyy",
                "MM/dd/yyyy HH:mm:ss",
                "MM/dd/yyyy",
                "yyyy-MM-dd HH:mm:ss",
                "yyyy-MM-dd",
                "yyyyMMdd"
            };

            if (DateTime.TryParseExact(dateString, formats, CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime result))
            {
                return DateOnly.FromDateTime(result);
            }
            
            // Fallback for other potential formats
            if (DateTime.TryParse(dateString, CultureInfo.InvariantCulture, DateTimeStyles.None, out result))
            {
                 return DateOnly.FromDateTime(result);
            }


            return null;
        }
    }
}
