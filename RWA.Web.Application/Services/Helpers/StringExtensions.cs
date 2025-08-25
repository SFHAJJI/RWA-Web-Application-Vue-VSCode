using System;

namespace RWA.Web.Application.Services.Helpers
{
    public static class StringExtensions
    {
        public static bool TrimmedEquals(this string str, string other, StringComparison comparison = StringComparison.OrdinalIgnoreCase)
        {
            if (str is null && other is null)
            {
                return true;
            }

            if (str is null || other is null)
            {
                return false;
            }

            return str.Trim().Equals(other.Trim(), comparison);
        }
    }
}
