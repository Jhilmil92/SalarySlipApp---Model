using SalarySlipApp.Models;
using SalarySlipApp.SalarySlip.Common;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using TemplateApp.Classes;

namespace SalarySlipApp.Services
{
    public class SalaryService:ISalaryService
    {
        public ICollection<Rules> ComputeRules(decimal salary, ICollection<Rules> userAdditionComponents, ICollection<Rules> userDeductionComponents)
        {
            decimal grossSalary = 0.0m;
            decimal componentAmount = 0.0m;
            StringBuilder componentValueAsString = new StringBuilder();
            ICollection<Rules> computedRules = new List<Rules>();
            var additionSectionCollection = ConfigurationManager.GetSection(Constants.additionSection) as NameValueCollection;
            if ((additionSectionCollection != null) && (additionSectionCollection.Count > 0))
            {

                foreach (var component in additionSectionCollection.AllKeys)
                {
                    if (component != Constants.balance)
                    {
                        //Check percent or actual amount
                        if (additionSectionCollection[component].EndsWith("%"))
                        {
                            componentValueAsString.Append(additionSectionCollection[component]);
                            componentAmount = Convert.ToDecimal(componentValueAsString.Remove(componentValueAsString.Length - 1, 1).ToString());
                            componentAmount = ((componentAmount) / 100) * salary;
                        }
                        computedRules.Add(new Rules
                            {
                                ComputationName = ComputationVariety.ADDITION,
                                RuleName = component,
                                RuleValue = componentAmount
                            });
                        grossSalary += componentAmount;
                        componentValueAsString.Clear();
                    }

                }

                if (computedRules != null && computedRules.Count > 0)
                {
                    //if (computedRules.Any(a => a.ComputationName == ComputationVariety.ADDITION && a.RuleName == Constants.basicPay)
                    //    && computedRules.Any(a => a.ComputationName == ComputationVariety.ADDITION && a.RuleName == Constants.specialAllowance)
                    //    && computedRules.Any(a => a.ComputationName == ComputationVariety.ADDITION && a.RuleName == Constants.houseRentAllowance))
                    //{
                        //decimal deductionAmount = Convert.ToDecimal(computedRules.Where(a => a.ComputationName == ComputationVariety.ADDITION && a.RuleName == Constants.basicPay).Select(a=>a.RuleValue).ToArray().ElementAt(0))
                        //    + Convert.ToDecimal(computedRules.Where(a => a.ComputationName == ComputationVariety.ADDITION && a.RuleName == Constants.specialAllowance).Select(a=>a.RuleValue).ToArray().ElementAt(0))
                        //    + Convert.ToDecimal(computedRules.Where(a => a.ComputationName == ComputationVariety.ADDITION && a.RuleName == Constants.houseRentAllowance).Select(a=>a.RuleValue).ToArray().ElementAt(0));
                        //decimal remainingSalary = salary - (deductionAmount);
                        decimal remainingSalary = salary - grossSalary;
                        computedRules.Add(new Rules
                            {
                                ComputationName = ComputationVariety.ADDITION,
                                RuleName = Constants.balance,
                                RuleValue = remainingSalary
                            });
                        grossSalary += remainingSalary;
                    //}
                }

                //Add additional components to the gross salary.
                if((userAdditionComponents != null) && (userAdditionComponents.Count > 0))
                {
                    decimal userAdditionComponentTotal = 0.0m;
                    foreach(var component in userAdditionComponents)
                    {
                        computedRules.Add(new Rules
                        {
                            ComputationName = component.ComputationName,
                            RuleName = component.RuleName,
                            RuleValue = component.RuleValue
                        }
                            );
                        userAdditionComponentTotal += component.RuleValue;
                    }
                    grossSalary += userAdditionComponentTotal;
                }


            }

            var subtractionSectionCollection = ConfigurationManager.GetSection(Constants.subtractionSection) as NameValueCollection;
            if ((subtractionSectionCollection != null) && (subtractionSectionCollection.Count > 0))
            {
                foreach (var component in subtractionSectionCollection.AllKeys)
                {
                    //componentAmount = Convert.ToDecimal(subtractionSectionCollection[component]);
                    if (subtractionSectionCollection[component].EndsWith("%"))
                    {
                        componentValueAsString.Append(subtractionSectionCollection[component]);
                        componentAmount = Convert.ToDecimal(componentValueAsString.Remove(componentValueAsString.Length - 1, 1).ToString());
                        componentAmount = ((componentAmount) / 100) * grossSalary;
                    }
                    computedRules.Add(new Rules
                        {
                            ComputationName = ComputationVariety.SUBTRACTION,
                            RuleName = component,
                            RuleValue = componentAmount
                        });
                    componentValueAsString.Clear();
                }

                if((userDeductionComponents != null) && (userDeductionComponents.Count > 0))
                {
                    decimal userDeductionComponentTotal = 0.0m;
                    foreach (var component in userDeductionComponents)
                    {
                        computedRules.Add(new Rules
                            {
                                ComputationName = component.ComputationName,
                                RuleName = component.RuleName,
                                RuleValue = component.RuleValue
                            });
                        userDeductionComponentTotal += component.RuleValue;
                    }
                }
            }
            return computedRules;
        }

        public string CollectTemplateData(EmployeeDetails employeeDetails, ICollection<Rules> employeePayDetails)
        {
            //Use and manipulate template
            ConstructTemplate template = new ConstructTemplate();
            string templateContent = template.PopulateTemplate(employeeDetails, employeePayDetails);
            return templateContent;
        }


        public void SendTemplate(string templateContent)
        {
            //RemoteCertificateValidationCallback orgCallback = ServicePointManager.ServerCertificateValidationCallback;
            //ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback(OnValidateCertificate);
            //ServicePointManager.Expect100Continue = true;
            //MailAddress addressFrom = new MailAddress("jhilmil@vtecsys.com", "Jhilmil");
            //MailAddress addressTo = new MailAddress("uttam@vtecsys.com");
            //MailMessage message = new MailMessage(addressFrom, addressTo);
            //message.Subject = "Salary Slip";
            //message.IsBodyHtml = true;
            //message.Body = templateContent;
            //message.BodyEncoding = System.Text.Encoding.UTF8;
            //message.DeliveryNotificationOptions = DeliveryNotificationOptions.OnFailure;

            ////SmtpClient client = new SmtpClient();
            ////client.Port = 587;
            ////client.Host = "smtp.gmail.com";
            ////client.EnableSsl = false;
            ////client.DeliveryMethod = SmtpDeliveryMethod.Network;
            ////client.UseDefaultCredentials = false;
            ////client.Credentials = new System.Net.NetworkCredential("jhilmil@vtecsys.com","Jhilmil@92");
            ////client.Send(message);

            //using (SmtpClient smtpserver = new SmtpClient())
            //{
            //   // smtpserver.Timeout = 5 * 1000;
            //    smtpserver.DeliveryMethod = SmtpDeliveryMethod.Network;
            //    smtpserver.Host = "smtp.gmail.com";
            //    smtpserver.Port = Convert.ToInt32(587);
            //    smtpserver.Credentials = new System.Net.NetworkCredential("jhilmil@vtecsys.com", "Jhilmil@92");
            //    smtpserver.EnableSsl = true;
            //    smtpserver.Send(message);
            //}
            string senderID = "jhilmil.basu92@gmail.com";
            string senderPassword = "Jhilmil@12111992";
            RemoteCertificateValidationCallback orgCallback = ServicePointManager.ServerCertificateValidationCallback;
            string body = "Test";
            try
            {
                ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback(OnValidateCertificate);
                ServicePointManager.Expect100Continue = true;
                MailMessage mail = new MailMessage();
               // mail.To.Add("uttam@vtecsys.com");
                mail.To.Add("jhilmil@vtecsys.com");
                mail.From = new MailAddress(senderID);
                mail.Subject = "My Test Email!";
                mail.Body = templateContent;
                mail.IsBodyHtml = true;
                SmtpClient smtp = new SmtpClient();
                smtp.Host = "smtp.gmail.com";
                smtp.Credentials = new System.Net.NetworkCredential(senderID, senderPassword);
                smtp.Port = 587;
                smtp.EnableSsl = true;
                smtp.Send(mail);
                Console.WriteLine("Email Sent Successfully");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }  
        }
        private bool OnValidateCertificate(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
        {
            return true;
        }
    }
}
