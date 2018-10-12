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

       public static List<String> GetMonths(this DateTime dateTime)
       {
            return CultureInfo.InvariantCulture.DateTimeFormat.MonthNames.Take(12).ToList();
       }
    }
}
