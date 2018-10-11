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

        void SendTemplate(string templateContent);
    }
}
