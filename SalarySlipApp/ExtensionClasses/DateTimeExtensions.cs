using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SalarySlipApp.ExtensionClasses
{
    static class DateTimeExtensions
    {
       public static string ToMonthName(this DateTime dateTime)
        {
            return CultureInfo.CreateSpecificCulture("en-IN").DateTimeFormat.GetMonthName(dateTime.Month);
        }
    }
}
