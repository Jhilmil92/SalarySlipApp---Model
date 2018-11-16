using SalarySlipApp.Models;
using SalarySlipApp.SalarySlip.Common;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using TemplateApp.Classes;
using System.Web;
using System.Web.Hosting;
using System.Security.Principal;
using System.Security.AccessControl;
using System.Net.Mime;
using SalarySlipApp.ExtensionClasses;
using NReco.PdfGenerator;

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
                            componentAmount = Decimal.Round(((componentAmount) / 100) * salary,2);
                        }
                        else if (!(char.IsLetter(additionSectionCollection[component].ToString(), 0)))
                        {
                            componentAmount = Convert.ToDecimal(additionSectionCollection[component]);
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
                        componentAmount = Decimal.Round(((componentAmount) / 100) * grossSalary,2);
                    }
                    else if (!(char.IsLetter(subtractionSectionCollection[component].ToString(),0)))
                    {
                        componentAmount = Convert.ToDecimal(subtractionSectionCollection[component]);
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


        public string SendTemplate(EmployeeDetails employeeDetails,string templateContent)
        {
            string pdfFilePath = @"e:\SalarySlips\";
            string pdfFileName = string.Format("{0}{1:dd-MMM-yyyy HH-mm-ss-fff}{2}", "SalarySlip", DateTime.Now, ".pdf");
            string finalPdfPath = Path.Combine(pdfFilePath, pdfFileName);
            HtmlToPdfConverter(pdfFilePath,pdfFileName,finalPdfPath,templateContent);
            string senderID = "jhilmil.basu92@gmail.com";
            string senderPassword = "Jhilmil@12111992";
            RemoteCertificateValidationCallback orgCallback = ServicePointManager.ServerCertificateValidationCallback;
            string body = "Test";
            try
            {
                ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback(OnValidateCertificate);
                ServicePointManager.Expect100Continue = true;
                MailMessage mail = new MailMessage();

                Attachment attachment = new Attachment(finalPdfPath,MediaTypeNames.Application.Octet);
                ContentDisposition disposition = attachment.ContentDisposition;
                disposition.CreationDate = File.GetCreationTime(finalPdfPath);
                disposition.ModificationDate = File.GetLastWriteTime(finalPdfPath);
                disposition.ReadDate = File.GetLastAccessTime(finalPdfPath);
                disposition.FileName = Path.GetFileName(finalPdfPath);
                disposition.Size = new FileInfo(finalPdfPath).Length;
                disposition.DispositionType = DispositionTypeNames.Attachment;
                mail.Attachments.Add(attachment);

                mail.To.Add(employeeDetails.EmailId);
                mail.From = new MailAddress(senderID);
                mail.Subject = "My Test Email!";
                mail.Body = "Salary Slip";
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
            return pdfFilePath;
        }

        private void SetPathPermission(string pdfFilePath)
        {
            var directoryInfo = new DirectoryInfo(pdfFilePath);
            var accessControl = directoryInfo.GetAccessControl();
            var userIdentity = WindowsIdentity.GetCurrent();
            var permissions = new FileSystemAccessRule(userIdentity.Name,
                                                  FileSystemRights.FullControl,
                                                  InheritanceFlags.ObjectInherit |
                                                  InheritanceFlags.ContainerInherit,
                                                  PropagationFlags.None,
                                                  AccessControlType.Allow);
            accessControl.AddAccessRule(permissions);
            directoryInfo.SetAccessControl(accessControl);
        }

        public void HtmlToPdfConverter(string pdfFilePath,string pdfFileName,string finalPdfPath,string templateContent)
        {
            if (!(Directory.Exists(pdfFilePath)))
            {
                Directory.CreateDirectory(pdfFilePath);
            }

            SetPathPermission(pdfFilePath);
            var htmlToPdf = new NReco.PdfGenerator.HtmlToPdfConverter();
            htmlToPdf.CustomWkHtmlArgs = "--disable-smart-shrinking";
           // htmlToPdf.PageHeight = 297;
            htmlToPdf.Size = PageSize.A4;
           // htmlToPdf.PageWidth = 210;
            htmlToPdf.Orientation = NReco.PdfGenerator.PageOrientation.Portrait;
           // htmlToPdf.PageHeaderHtml = string.Format("<img src=\"{0}\" alt=\"{1}\" height=\"{2}\" width = \"{3}\">", ConfigurationManager.AppSettings[Constants.headerImage], "No Image Found", 50, 90);
            var pdfBytes = htmlToPdf.GeneratePdf(templateContent);
            if (pdfBytes != null)
            {
                using (FileStream fileStream = new FileStream(finalPdfPath, FileMode.OpenOrCreate))
                {
                    fileStream.Write(pdfBytes, 0, pdfBytes.Length);
                    fileStream.Close();
                }
            }
        }

        public void DeleteSalarySlips(string pdfFilePath)
        {
            try
            {
                if(Directory.Exists(pdfFilePath))
                {
                    var directory = new DirectoryInfo(pdfFilePath);
                    foreach(var file in directory.GetFiles())
                    {
                        if(!(IsFileLocked(file)))
                        {
                            file.Delete();
                        }
                    }
                }
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        private bool OnValidateCertificate(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
        {
            return true;
        }

        public static Boolean IsFileLocked(FileInfo file)
        {
            FileStream stream = null;

            try
            {
                //Don't change FileAccess to ReadWrite, 
                //because if a file is in readOnly, it fails.
                stream = file.Open
                (
                    FileMode.Open,
                    FileAccess.Read,
                    FileShare.None
                );
            }
            catch (IOException)
            {
                //the file is unavailable because it is:
                //still being written to
                //or being processed by another thread
                //or does not exist (has already been processed)
                return true;
            }
            finally
            {
                if (stream != null)
                    stream.Close();
            }

            //file is not locked
            return false;
        }
    }
}
