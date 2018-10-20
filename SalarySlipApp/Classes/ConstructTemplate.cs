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
using System.Configuration;
using SalarySlipApp.ExtensionClasses;

namespace TemplateApp.Classes
{
    public class ConstructTemplate
    {
        public string PopulateTemplate(EmployeeDetails employeeDetails, ICollection<Rules> employeePayDetails)
        {
            int flag = -1;
            int componentCounter = 0;
            string templateBody = string.Empty;
            StringBuilder genericBuilder = new StringBuilder();

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

            
            //Run for for all fields in final details

            ////Uncomment if something goes's wrong ---Start
            //StringBuilder additionBuilder = new StringBuilder();
            //StringBuilder subtractionBuilder = new StringBuilder();
            //foreach (var result in employeePayDetails)
            //{
            //   if((result.ComputationName == ComputationVariety.ADDITION) && (result.RuleName != Constants.netPay))
            //    {
            //        additionBuilder.Append(string.Format("<tr><td colspan = \"2\">{0}</td><td colspan = \"2\">{1}</td></tr>", result.RuleName, result.RuleValue));
            //    }

            //    if(result.ComputationName == ComputationVariety.SUBTRACTION)
            //    {
            //        subtractionBuilder.Append(string.Format("<tr><td colspan = \"2\">{0}</td><td colspan = \"2\">{1}</td></tr>", result.RuleName, result.RuleValue));
            //    }
            //}

            //if(additionBuilder != null && additionBuilder.Length > 0)
            //{
            //    templateBody = templateBody.Replace("$additionResult",additionBuilder.ToString());
            //}
            //else
            //{
            //    templateBody = templateBody.Replace("$additionResult",string.Empty);
            //}
            
            //if(subtractionBuilder != null && subtractionBuilder.Length > 0)
            //{
            //    templateBody = templateBody.Replace("$subtractionResult", subtractionBuilder.ToString());
            //}
            //else
            //{
            //    templateBody = templateBody.Replace("$subtractionResult", string.Empty);
            //}


            //additionBuilder.Clear();
            //subtractionBuilder.Clear();
            ////Uncomment if something goes's wrong ---Stop

            //New set of code -- Start.

            StringBuilder additionBuilder = new StringBuilder();
            StringBuilder subtractionBuilder = new StringBuilder();
            //foreach (var result in employeePayDetails)
            //{
            //   if((result.ComputationName == ComputationVariety.ADDITION) && (result.RuleName != Constants.netPay))
            //    {
            //        genericBuilder.Append(string.Format("<tr><td colspan = \"2\">{0}</td><td colspan = \"2\">{1}</td>", result.RuleName, result.RuleValue));
            //    }

            //    if(result.ComputationName == ComputationVariety.SUBTRACTION)
            //    {
            //        genericBuilder.Append(string.Format("<td colspan = \"2\">{0}</td><td colspan = \"2\">{1}</td></tr>", result.RuleName, result.RuleValue));
            //    }
            //}

            var additionPayDetails = employeePayDetails.Where(a => (a.ComputationName == ComputationVariety.ADDITION) && (a.RuleName != Constants.netPay)).ToArray();
            var deductionPayDetails = employeePayDetails.Where(a => a.ComputationName == ComputationVariety.SUBTRACTION).ToArray();
            //var firstresult = additionPayDetails.Zip(deductionPayDetails, (f, s) => new { f, s });
            //var secondresult = deductionPayDetails.Zip(additionPayDetails, (f, s) => new { f, s });

            var interleavedList = additionPayDetails.Interleave(deductionPayDetails).ToList();

            //genericBuilder.Append(string.Format("<td colspan = \"2\"><div>{0}</div><div>{0}</div></td></tr>", string.Empty));
            int largerCount = interleavedList.Count();
            int beginCounter = -1;
            int endCounter = 0;
            for (int i = 0; i < interleavedList.Count; i++ )
            {
                if (endCounter != interleavedList.Count())
                {
                    if (beginCounter == -1)
                    {
                        genericBuilder.Append("<tr>");
                    }

                    genericBuilder.Append(string.Format("<td colspan = \"2\"><div>{0}</div><div>{1}</div></td>", interleavedList[i].RuleName, interleavedList[i].RuleValue));
                    beginCounter++;
                    if (beginCounter == 1)
                    {
                        genericBuilder.Append("</tr>");
                        beginCounter = -1;
                    }
                    endCounter++;
                }
                if (endCounter == interleavedList.Count())
                {
                    if (interleavedList[i].ComputationName == ComputationVariety.ADDITION)
                    {
                        genericBuilder.Append(string.Format("<td colspan = \"2\"><div>{0}</div><div>{1}</div></td><td colspan = \"2\"><div>{2}</div><div>{2}</div></td></tr>", interleavedList[i].RuleName, interleavedList[i].RuleValue, string.Empty));
                    }
                    else if (interleavedList[i].ComputationName == ComputationVariety.SUBTRACTION)
                    {
                        genericBuilder.Append(string.Format("<td colspan = \"2\"><div>{0}</div><div>{0}</div></td><td colspan = \"2\"><div>{1}</div><div>{2}</div></td></tr>", string.Empty, string.Empty, interleavedList[i].RuleName, interleavedList[i].RuleValue));
                    }
               }
            }
            if (genericBuilder != null && genericBuilder.Length > 0)
            {
                templateBody = templateBody.Replace("$additionAndDeductionComponents", genericBuilder.ToString());
            }
            else
            {
                templateBody = templateBody.Replace("$additionAndDeductionComponents", string.Empty);
            }
            genericBuilder.Clear();
            //New set of code -- Stop.


            var details = employeePayDetails.Where(a => a.RuleName == Constants.netPay).Select(a => a).ToList();
            var ruleValue = details[0].RuleValue.ToString("#,#.##", System.Globalization.CultureInfo.CreateSpecificCulture("hi-IN"));
            var ruleValueinDecimal = Convert.ToDecimal(ruleValue);
            genericBuilder.Append(string.Format("<tr><td colspan=\"3\"><strong>{0}:</strong></td><td colspan=\"2\">{1}</td></tr>", details[0].RuleName, ruleValue));
            templateBody = templateBody.Replace("$netPay", genericBuilder.ToString());
            genericBuilder.Clear();
            var value = (NumberToWordsExtension.ToWords((long)ruleValueinDecimal)).Titleize();
            genericBuilder.Append(string.Format("<tr><td colspan=\"1\"><strong>Net Pay in Words:</strong></td><td colspan=\"4\">{0}</td></tr>", value));
            templateBody = templateBody.Replace("$payInWords", genericBuilder.ToString());
            genericBuilder.Clear();
            templateBody = templateBody.Replace("$contentOfHeader", string.Format("<img src=\"{0}\" alt=\"{1}\" height=\"{2}\" width = \"{3}\">", ConfigurationManager.AppSettings[Constants.headerImage], "No Image Found", 50, 90));
            templateBody = templateBody.Replace("$contentOfFooter", FetchFooterContent() != null? FetchFooterContent():string.Empty);
            return templateBody;
        }

        public string FetchFooterContent()
        {
            StringBuilder footerContent = new StringBuilder();
            try
            {
                var fileToRead = ConfigurationManager.AppSettings[Constants.footerContent];
                if (File.Exists(fileToRead))
                {
                    using (StreamReader reader = new StreamReader(fileToRead))
                    {
                        footerContent.Append(reader.ReadToEnd());
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return footerContent.ToString();
        }
    }
}
