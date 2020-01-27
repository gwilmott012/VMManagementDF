using System;
using System.Collections.Generic;
using System.Text;

namespace VMManagementDF.Helpers
{
    public static class StringHelpers { public static string ToTitleCase(this string value) => System.Globalization.CultureInfo.CurrentCulture.TextInfo.ToTitleCase(value); }
}
