using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SalarySlipApp.Models
{
    public class Rules
    {
        public SalarySlip.Common.ComputationVariety ComputationName { get; set; }
        public string RuleName { get; set; }
        public decimal RuleValue { get; set; }
    }
}
