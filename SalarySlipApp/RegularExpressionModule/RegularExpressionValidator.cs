using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SalarySlipApp.RegularExpressionModule
{
    public class RegularExpressionValidator
    {
        public static bool IsValidName(string input)
        {
            bool isValidInput = false;
            Regex regularExpression = new Regex("^[a-zA-Z]+[a-zA-Z\\s]+[a-zA-Z]$");
            Match match = regularExpression.Match(input);
            if(match.Success)
            {
                isValidInput = true;
            }
            return isValidInput;
        }

        public static bool IsValidPan(string input)
        {
            bool isValidInput = false;
            Regex regularExpression = new Regex("[a-zA-Z]{5}[\\d]{4}[a-zA-Z]$");
            Match match = regularExpression.Match(input);
            if (match.Success)
            {
                isValidInput = true;
            }
            return isValidInput;
        }

        public static bool IsValidAccountNumber(string input)
        {
            bool isValidInput = false;
            Regex regularExpression = new Regex("^[\\d]+$");
            Match match = regularExpression.Match(input);
            if (match.Success)
            {
                isValidInput = true;
            }
            return isValidInput;
        }

        public static bool IsValidDesignation(string input)
        {
            bool isValidInput = false;
            Regex regularExpression = new Regex("^[a-zA-Z]+[a-zA-Z\\s]+[a-zA-Z\\d]$");
            Match match = regularExpression.Match(input);
            if (match.Success)
            {
                isValidInput = true;
            }
            return isValidInput;
        }
        //Upto 99 lakhs.
        public static bool IsValidSalary(string input)
        {
            bool isValidInput = false;
            Regex regularExpression = new Regex("^[1-9][\\d]{2,6}$");
            Match match = regularExpression.Match(input);
            if (match.Success)
            {
                isValidInput = true;
            }
            return isValidInput;
        }

        public static bool IsValidComponentCount(string input)
        {
            bool isValidInput = false;
            Regex regularExpression = new Regex("^[1-9][0]?$");
            Match match = regularExpression.Match(input);
            if (match.Success)
            {
                isValidInput = true;
            }
            return isValidInput;
        }

        public static bool IsValidComponentValuePair(string input)
        {
            bool isValidInput = false;
            Regex regularExpression = new Regex("^[a-zA-Z][a-zA-Z\\s]+[a-zA-Z\\d]$");
            Match match = regularExpression.Match(input);
            if (match.Success)
            {
                isValidInput = true;
            }
            return isValidInput;
        }

    }
}
