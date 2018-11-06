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
        /// <summary>
        /// This method is responsible for replacing placeholders in an html template called "FinalTemplate" with the respective values 
        /// such as employee details, salary breakup for both addition and deduction components, gross salary,total deductions and the net pay 
        /// in figures and words.
        /// </summary>
        /// <param name="employeeDetails">The employee's personal details and professional details.</param>
        /// <param name="employeePayDetails">The employee's salary breakup details divided into addition and deduction components</param>
        /// <returns>The html content having all the placeholders replaced with appropriate values.</returns>
        public string PopulateTemplate(EmployeeDetails employeeDetails, ICollection<Rules> employeePayDetails)
        {
            int beginCounter = -1;
            int endCounter = -1;
            int largerListCount = 0;
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
            templateBody = templateBody.Replace("$month",employeeDetails.Month.ToUpper());
            templateBody = templateBody.Replace("$year",employeeDetails.Year);

            //New set of code -- Start.

            var additionPayDetails = employeePayDetails.Where(a => (a.ComputationName == ComputationVariety.ADDITION) && (a.RuleName != Constants.netPay && a.RuleName != Constants.additionTotal)).ToArray();
            var deductionPayDetails = employeePayDetails.Where(a => (a.ComputationName == ComputationVariety.SUBTRACTION) && (a.RuleName != Constants.subtractionTotal)).ToArray();
            var additionTotal = employeePayDetails.Where(a => (a.ComputationName == ComputationVariety.ADDITION) && (a.RuleName == Constants.additionTotal)).ToArray();
            var deductionTotal = employeePayDetails.Where(a => (a.ComputationName == ComputationVariety.SUBTRACTION) && (a.RuleName == Constants.subtractionTotal)).ToArray();

           
            if((additionPayDetails != null && additionPayDetails.Count() > 0) && (deductionPayDetails != null && deductionPayDetails.Count() > 0))
            {
                largerListCount =  (additionPayDetails.Count() > deductionPayDetails.Count()) ? additionPayDetails.Count() : deductionPayDetails.Count();
            }

                for (int i = 0; i < largerListCount; i++)
                {
                    if(beginCounter == -1)
                    {
                        genericBuilder.Append("<tr class=\"alignment-style\">");
                        beginCounter ++;
                    }
                    if((beginCounter == 0))
                    {
                        if (i < additionPayDetails.Count() && i < deductionPayDetails.Count())
                        {
                            if (additionPayDetails[i] != null && deductionPayDetails[i] != null)
                            {
                                genericBuilder.Append(string.Format("<td colspan = \"1\">{0}</td><td colspan = \"1\">{1}</td><td colspan = \"1\">{2}</td><td colspan = \"1\">{3}</td></tr>", additionPayDetails[i].RuleName, additionPayDetails[i].RuleValue, deductionPayDetails[i].RuleName, deductionPayDetails[i].RuleValue));
                                
                                beginCounter = -1;
                                endCounter++;
                            }
                            else if(additionPayDetails[i] != null && deductionPayDetails[i] == null)
                            {
                                genericBuilder.Append(string.Format("<td colspan = \"1\">{0}</td><td colspan = \"1\">{1}</td><td colspan = \"1\">{2}</td><td colspan = \"1\">{2}</td></tr>", additionPayDetails[i].RuleName, additionPayDetails[i].RuleValue, string.Empty));
                                beginCounter = -1;
                                endCounter++;
                            }
                            else if (additionPayDetails[i] == null && deductionPayDetails[i] != null)
                            {
                                genericBuilder.Append(string.Format("<td colspan = \"1\">{0}</td><td colspan = \"1\">{0}</td><td colspan = \"2\">{1}</td><td colspan = \"1\">{2}</td></tr>", string.Empty, deductionPayDetails[i].RuleName, deductionPayDetails[i].RuleValue));
                                beginCounter = -1;
                                endCounter++;
                            }
                        }
                        else if(i < additionPayDetails.Count() && i == deductionPayDetails.Count())
                        {
                            if(additionPayDetails[i] != null)
                            {
                                genericBuilder.Append(string.Format("<td colspan = \"1\">{0}</td><td colspan = \"1\">{1}</td><td colspan = \"1\">{2}</td><td colspan = \"1\">{2}</td></tr>", additionPayDetails[i].RuleName, additionPayDetails[i].RuleValue, string.Empty));
                                beginCounter = -1;
                                endCounter++;
                            }
                        }
                        else if (i ==  additionPayDetails.Count() && i < deductionPayDetails.Count())
                        {
                            if(deductionPayDetails[i] != null)
                            {
                                genericBuilder.Append(string.Format("<td colspan = \"1\">{0}</td><td colspan = \"1\">{0}</td><td colspan = \"1\">{1}</td><td colspan = \"1\">{2}</td></tr>", string.Empty, deductionPayDetails[i].RuleName, deductionPayDetails[i].RuleValue));
                                beginCounter = -1;
                                endCounter++;
                            }
                        }
                  
                    }
                }
            if(endCounter == largerListCount - 1)
            {
                if((additionTotal != null && additionTotal.Count() > 0) && (deductionTotal != null && deductionTotal.Count() > 0))
                {
                    genericBuilder.Append(string.Format("<tr class=\"alignment-style\"><td colspan = \"1\">{0}</td><td colspan = \"1\">{1}</td><td colspan = \"1\">{2}</td><td colspan = \"1\">{3}</td></tr>", Constants.grossSalary, additionTotal[0].RuleValue, Constants.totalDeduction, deductionTotal[0].RuleValue));                    
                }
                else if ((additionTotal != null && additionTotal.Count() > 0) && (deductionTotal == null || deductionTotal.Count() == 0))
                {
                    genericBuilder.Append(string.Format("<tr class=\"alignment-style\"><td colspan = \"1\">{0}</td><td colspan = \"1\">{1}</td><td colspan = \"1\">{2}</td><td colspan = \"1\">{2}</td></tr>", Constants.grossSalary, additionPayDetails[0].RuleValue, string.Empty));                    
                }
                else if ((additionTotal == null || additionTotal.Count() == 0) && (deductionTotal != null && deductionTotal.Count() > 0))
                {
                    genericBuilder.Append(string.Format("<tr class=\"alignment-style\"><td colspan = \"1\">{0}</td><td colspan = \"1\">{0}</td><td colspan = \"1\">{1}</td><td colspan = \"1\">{2}</td></tr>", string.Empty, Constants.totalDeduction, deductionPayDetails[0].RuleValue));                    
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
            genericBuilder.Append(string.Format("<tr class=\"alignment-style\"><td colspan=\"3\">{0}:</td><td colspan=\"1\">{1}</td></tr>", details[0].RuleName, ruleValue));
            templateBody = templateBody.Replace("$netPay", genericBuilder.ToString());
            genericBuilder.Clear();
            var value = (NumberToWordsExtension.ToWords((long)ruleValueinDecimal)).Titleize();
            genericBuilder.Append(string.Format("<tr><td colspan=\"1\" class=\"left-alignment-style\">Net Pay in Words:</td><td colspan=\"3\" class=\"alignment-style-center\">{0}</td></tr>", value));
            templateBody = templateBody.Replace("$payInWords", genericBuilder.ToString());
            genericBuilder.Clear();
            templateBody = templateBody.Replace("$contentOfHeader", string.Format("<img src=\"{0}\" alt=\"{1}\">", ConfigurationManager.AppSettings[Constants.headerImage], "No Image Found"));
            templateBody = templateBody.Replace("$contentOfFooter", FetchFooterContent() != null? FetchFooterContent():string.Empty);
            return templateBody;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
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
