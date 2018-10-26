using SalarySlipApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SalarySlipApp.Services
{
    public interface ISalaryService
    {
        ICollection<Rules> ComputeRules(decimal salary, ICollection<Rules> userAdditionComponents, ICollection<Rules> userDeductionComponents);
        string CollectTemplateData(EmployeeDetails employeeDetails, ICollection<Rules> employeePayDetails);
        string SendTemplate(EmployeeDetails employeeDetails,string templateContent);
        void DeleteSalarySlips(string pdfFilePath);
    }
}
