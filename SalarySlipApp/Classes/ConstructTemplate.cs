using SalarySlipApp.Models;
using SalarySlipApp.SalarySlip.Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Resources;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using TemplateApplication.Classes;
using TemplateApplication.Interfaces;
using Humanizer;
using System.Globalization;

namespace TemplateApp.Classes
{
    public class ConstructTemplate
    {
        public string PopulateTemplate(EmployeeDetails employeeDetails, ICollection<Rules> employeePayDetails)
        {
            string templateBody = string.Empty;

            using(ITemplateProvider templateApplication = new TemplateProvider())
            {
                templateBody = templateApplication.SupplyTemplateStream().ReadToEnd();
            }
            
            templateBody = templateBody.Replace("$dateOfJoining", employeeDetails.DateOfJoining);
            templateBody = templateBody.Replace("$panNumber", employeeDetails.PanNumber);
            templateBody = templateBody.Replace("$name", employeeDetails.EmployeeName);
            templateBody = templateBody.Replace("$designation", employeeDetails.Designation);
            templateBody = templateBody.Replace("$accountNumber", employeeDetails.AccountNumber);
            templateBody = templateBody.Replace("$salary", employeeDetails.Salary);
            templateBody = templateBody.Replace("$month",employeeDetails.Month);
            templateBody = templateBody.Replace("$year",employeeDetails.Year);

            StringBuilder additionBuilder = new StringBuilder();
            StringBuilder subtractionBuilder = new StringBuilder();

            //Run for for all fields in final details

            foreach (var result in employeePayDetails)
            {
               if((result.ComputationName == ComputationVariety.ADDITION) && (result.RuleName != Constants.netPay))
                {
                    additionBuilder.Append(string.Format("<tr><td>{0}</td><td>{1}</td></tr>", result.RuleName, result.RuleValue));
                }

                if(result.ComputationName == ComputationVariety.SUBTRACTION)
                {
                    subtractionBuilder.Append(string.Format("<tr><td>{0}</td><td>{1}</td></tr>", result.RuleName, result.RuleValue));
                }
            }

            if(additionBuilder != null && additionBuilder.Length > 0)
            {
                templateBody = templateBody.Replace("$additionResult",additionBuilder.ToString());
            }
            else
            {
                templateBody = templateBody.Replace("$additionResult",string.Empty);
            }
            
            if(subtractionBuilder != null && subtractionBuilder.Length > 0)
            {
                templateBody = templateBody.Replace("$subtractionResult", subtractionBuilder.ToString());
            }
            else
            {
                templateBody = templateBody.Replace("$subtractionResult", string.Empty);
            }

            additionBuilder.Clear();
            subtractionBuilder.Clear();

            var details = employeePayDetails.Where(a => a.RuleName == Constants.netPay).Select(a => a).ToList();
            var ruleValue = details[0].RuleValue.ToString("#,#.##", System.Globalization.CultureInfo.CreateSpecificCulture("hi-IN"));
            var ruleValueinDecimal = Convert.ToDecimal(ruleValue);
            additionBuilder.Append(string.Format("<tr><td colspan=\"3\"><strong>{0}:</strong></td><td colspan=\"2\">{1}</td></tr>", details[0].RuleName, ruleValue));
            templateBody = templateBody.Replace("$netPay",additionBuilder.ToString());
            additionBuilder.Clear();
            var value = (NumberToWordsExtension.ToWords((long)ruleValueinDecimal)).Titleize();
            additionBuilder.Append(string.Format("<tr><td colspan=\"1\"><strong>Net Pay in Words:</strong></td><td colspan=\"4\">{0}</td></tr>", value));
            templateBody = templateBody.Replace("$payInWords", additionBuilder.ToString());
            additionBuilder.Clear();
            return templateBody;
        }
    }
}
