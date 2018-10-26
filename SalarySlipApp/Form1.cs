using SalarySlipApp.Models;
using SalarySlipApp.RegularExpressionModule;
using SalarySlipApp.SalarySlip.Common;
using SalarySlipApp.Services;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TemplateApp.Classes;
using SalarySlipApp.ExtensionClasses;
using System.Globalization;

namespace SalarySlipApp
{

    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            dataGridView.Hide();
            addOuterPanel.Hide();
            deductOuterPanel.Hide();
            requiredName.Select();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            PopulateMonths();
            PopulateYears();
        }

        private void PopulateMonths()
        {
            month.DataSource = DateTime.Now.GetMonths();
            month.SelectedItem = CultureInfo.InvariantCulture.DateTimeFormat.MonthNames[DateTime.Now.AddMonths(-1).Month];
        }

        private void PopulateYears()
        {
            year.DataSource = Enumerable.Range(1950,DateTime.Now.Year - 1950 + 1).ToList();
            year.SelectedItem = DateTime.Now.Year;
        }

        private void generateButton_Click(object sender, EventArgs e)
        {
            if (salary.Text.ToString() != string.Empty)
            {
                decimal salaryAmount = Convert.ToDecimal(salary.Text);
                ISalaryService salaryService = new SalaryService();
                EmployeeDetails employeeDetails = new EmployeeDetails();
                employeeDetails.EmployeeName = requiredName.Text.ToString();
                employeeDetails.DateOfJoining = dateOfJoining.Text.ToString();
                employeeDetails.PanNumber = pan.Text.ToString();
                employeeDetails.AccountNumber = accountNumber.Text.ToString();
                employeeDetails.Designation = designation.Text.ToString();
                employeeDetails.Salary = salary.Text.ToString();
                employeeDetails.EmailId = email.Text.ToString();
                employeeDetails.Month = month.SelectedItem.ToString();
                employeeDetails.Year = year.SelectedItem.ToString();

                ICollection<Rules> userAdditionComponents = FetchUserComponents(addOuterPanel.Controls,ComputationVariety.ADDITION);
                ICollection<Rules> userDeductionComponents = FetchUserComponents(deductOuterPanel.Controls, ComputationVariety.SUBTRACTION);

                ICollection<Rules> computedRules = salaryService.ComputeRules(salaryAmount,userAdditionComponents,userDeductionComponents);
                ICollection<Rules> finalResults = PopulateGrid(computedRules,userAdditionComponents,userDeductionComponents);
                string templateContent = salaryService.CollectTemplateData(employeeDetails,finalResults);

                string pdfPath = salaryService.SendTemplate(employeeDetails,templateContent);
                salaryService.DeleteSalarySlips(pdfPath);
                //Mail Content;
            }

        }

        private List<Rules> FetchUserComponents(Control.ControlCollection componentCollection, ComputationVariety typeOfOperation)
        {
            StringBuilder componentBuilder = new StringBuilder();
            StringBuilder ruleHolder = new StringBuilder();
            List<Rules> userRules = new List<Rules>();
            if (componentCollection != null && componentCollection.Count > 0)
            {
                foreach(var component in componentCollection)
                {
                    string[] componentParts = componentBuilder.Append(component).ToString().Split('=');
                    componentParts = componentParts[0].Split(',');
                    if ((componentParts != null) && (componentParts.Count() > 0))
                    {
                        userRules.Add(new Rules()
                        {
                            ComputationName = typeOfOperation,
                            RuleName = componentParts[1].Split(':')[1],
                            RuleValue = Decimal.Round(Convert.ToDecimal(componentParts[2]),2)
                        }
                            );
                    }
                    componentBuilder.Clear();
                }
            }
            return userRules;
        }

        private void validateRuleName_Validation(object sender, CancelEventArgs e)
        {
            TextBox senderControl = (TextBox)sender;
            string textBoxInput = senderControl.Text.TrimStart(' ').TrimEnd(' '); //check line for first if case.
            if (string.IsNullOrEmpty(textBoxInput))
            {
                MessageBox.Show(string.Format("No value entered for text box {0}", senderControl.Name), "Salary Slip Application", MessageBoxButtons.OK, MessageBoxIcon.Error);
                senderControl.Focus();
            }
            else if(!(RegularExpressionValidator.IsValidComponentValuePair(textBoxInput)))
            {
                MessageBox.Show(string.Format("The Name,Value pair {0} of textbox {1} is not in proper format",senderControl.Text,senderControl.Name), "Salary Slip Application", MessageBoxButtons.OK, MessageBoxIcon.Error);
                senderControl.Focus();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="computedRules"></param>
        /// <param name="userAdditionComponents"></param>
        /// <param name="userDeductionComponents"></param>
        /// <returns></returns>
        private ICollection<Rules> PopulateGrid(ICollection<Rules> computedRules, ICollection<Rules> userAdditionComponents, ICollection<Rules> userDeductionComponents)
        {
            decimal additionSum = 0.0m;
            decimal subtractionSum = 0.0m;
            DataGridView dataGridView = this.dataGridView;
            var additionSectionCollection = ConfigurationManager.GetSection(Constants.additionSection) as NameValueCollection;
            var subtractionSectionCollection = ConfigurationManager.GetSection(Constants.subtractionSection) as NameValueCollection;

            if ((dataGridView != null))
            {
                DataTable dataTable = new DataTable();
                dataTable.Columns.AddRange(new DataColumn[4] {
                new DataColumn(Constants.addition,typeof(string)),
                new DataColumn(Constants.additionTotal, typeof(decimal)),
                new DataColumn(Constants.subtraction, typeof(string)),
                new DataColumn(Constants.subtractionTotal, typeof(decimal))});

                if((dataTable != null) && dataTable.Columns.Count > 0)
                {
                    if (dataTable.Columns.Contains(Constants.addition) && dataTable.Columns.Contains(Constants.additionTotal))
                    {
                        int totalCount = 0;
                        if((additionSectionCollection != null) && (additionSectionCollection.Count > 0))
                        {
                            totalCount =  additionSectionCollection.Count;
                        }
                        
                        if((userDeductionComponents != null) && (userDeductionComponents.Count > 0))
                        {
                            totalCount += userAdditionComponents.Count;
                        }

                        for(int i = 0; i< totalCount ; i++)
                        {
                            object[] additionArray = new object[2];
                            additionArray[0] = computedRules.Where(a => a.ComputationName == ComputationVariety.ADDITION).ElementAt(i).RuleName;
                            additionArray[1] = computedRules.Where(a => a.ComputationName == ComputationVariety.ADDITION).ElementAt(i).RuleValue;
                            additionSum += Convert.ToDecimal(additionArray[1]);
                            DataRow dataRow = dataTable.NewRow();
                            dataRow.ItemArray = additionArray;
                            dataTable.Rows.Add(dataRow);
                        }

                        //for(int j = 0 ; j < userAdditionComponents.Count; j++)
                        //{
                        //    var test = computedRules.Where(a => a.ComputationName == ComputationVariety.ADDITION && a.RuleName == userDeductionComponents.ElementAt(j).RuleName).Select(a => a.RuleName).ToArray();
                        //    object[] additionArray = new object[2];
                        //    additionArray[0] = computedRules.Where(a => a.ComputationName == ComputationVariety.ADDITION && a.RuleName == userDeductionComponents.ElementAt(j).RuleName).Select(a => a.RuleName).ToArray().ElementAt(0);
                        //    additionArray[1] = computedRules.Where(a => a.ComputationName == ComputationVariety.ADDITION).ElementAt(j).RuleValue;
                        //    additionSum += Convert.ToDecimal(additionArray[1]);
                        //    DataRow dataRow = dataTable.NewRow();
                        //    dataRow.ItemArray = additionArray;
                        //    dataTable.Rows.Add(dataRow);
                        //}
                    }

                    //End of addition.

                if (dataTable.Columns.Contains(Constants.subtraction) && dataTable.Columns.Contains(Constants.subtractionTotal))
                {
                    int computedRuleCounter = 0;
                    int totalCount = 0;
                    if ((subtractionSectionCollection != null) && (subtractionSectionCollection.Count > 0))
                    {
                        totalCount = subtractionSectionCollection.Count;
                    }

                    if ((userDeductionComponents != null) && (userDeductionComponents.Count > 0))
                    {
                        totalCount += userDeductionComponents.Count;
                    }
                    if (dataTable.Rows.Count != 0)
                    {
                        for (int i = 0; i < totalCount; i++)
                        {
                            //If there are more subtraction components than the addition component or there are only subtraction components.
                            if(dataTable.Rows.Count < totalCount)
                            {
                                break;
                            }
                            dataTable.Rows[i][Constants.subtraction] = computedRules.Where(a=> a.ComputationName == ComputationVariety.SUBTRACTION).ElementAt(i).RuleName;
                            dataTable.Rows[i][Constants.subtractionTotal] = computedRules.Where(a => a.ComputationName == ComputationVariety.SUBTRACTION).ElementAt(i).RuleValue;
                            computedRuleCounter++;
                            subtractionSum += Convert.ToDecimal(dataTable.Rows[i][Constants.subtractionTotal]);
                        }

                        if (computedRuleCounter != totalCount)
                        {
                            for (int i = computedRuleCounter; i < totalCount; i++)
                            {
                                DataRow dataRow = dataTable.NewRow();
                                dataRow[Constants.subtraction] = computedRules.Where(a => a.ComputationName == ComputationVariety.SUBTRACTION).ElementAt(i).RuleName;
                                dataRow[Constants.subtractionTotal] = computedRules.Where(a => a.ComputationName == ComputationVariety.SUBTRACTION).ElementAt(i).RuleValue;
                                subtractionSum += Convert.ToDecimal(dataRow[Constants.subtractionTotal]);
                                dataTable.Rows.Add(dataRow);
                            }
                        }
                    }
                    else
                    {
                        //If there are no existing rows in the table.
                        for (int i = 0; i < subtractionSectionCollection.Count; i++)
                        {
                            DataRow dataRow = dataTable.NewRow();
                            dataRow[Constants.subtraction] = computedRules.Where(a => a.ComputationName == ComputationVariety.SUBTRACTION).ElementAt(i).RuleName;
                            dataRow[Constants.subtractionTotal] = computedRules.Where(a => a.ComputationName == ComputationVariety.SUBTRACTION).ElementAt(i).RuleValue;
                            subtractionSum += Convert.ToDecimal(dataRow[Constants.subtractionTotal]);
                            dataTable.Rows.Add(dataRow);
                        }
                    }
                }
                    //End of subtraction.

                if(dataTable.Rows.Count > 0)
                {
                    DataRow totalDataRow = null;
                   // DataRow[] additionDataRows = dataTable.Select("Addition <> '' && AdditionTotal <> ''").TakeWhile(a=>a.ItemArray.ToList().Where(a=>a.ad));
                    var additionDataRows = dataTable.AsEnumerable()
                        .Where(w=>(w.Field<string>(Constants.addition) != null && w.Field<string>(Constants.addition) != string.Empty) 
                         && (w.Field<decimal>(Constants.additionTotal) != null))
                        .Select(a => a.Field<string>(Constants.addition)); //Complete

                    var subtractionDataRows = dataTable.AsEnumerable()
                        .Where(w => (w.Field<string>(Constants.subtraction) != null && w.Field<string>(Constants.subtraction) != string.Empty)
                         && (w.Field<decimal>(Constants.subtractionTotal) != null))
                        .Select(a => a.Field<string>(Constants.subtraction));

                    if (additionDataRows != null && additionDataRows.Count() > 0)
                    {
                        totalDataRow = dataTable.NewRow();
                        totalDataRow[Constants.addition] = Constants.grossSalary;
                        totalDataRow[Constants.additionTotal] = additionSum;
                        computedRules.Add(
                           new Rules
                           {
                               ComputationName = ComputationVariety.ADDITION,
                               RuleName = Constants.additionTotal,
                               RuleValue = additionSum
                           }
                         );
                        dataTable.Rows.Add(totalDataRow);
                    }
                    if(subtractionDataRows != null && subtractionDataRows.Count() > 0)
                    {

                        if(totalDataRow == null)
                        {
                            totalDataRow = dataTable.NewRow();
                            dataTable.Rows.Add(totalDataRow);
                        }
                        //totalDataRow[Constants.subtractionTotal] = additionSum - subtractionSum; //change
                        totalDataRow[Constants.subtraction] = Constants.totalDeduction;
                        totalDataRow[Constants.subtractionTotal] = subtractionSum;
                        computedRules.Add(
                            new Rules
                            {
                                ComputationName = ComputationVariety.SUBTRACTION,
                                RuleName = Constants.subtractionTotal,
                               // RuleValue = additionSum - subtractionSum // change
                                RuleValue = subtractionSum
                            });
                    }
                    if (additionSum >= 0 && subtractionSum >= 0)
                    {
                        DataRow netPayDataRow = dataTable.NewRow();
                        netPayDataRow[Constants.addition] = Constants.netPay;
                        netPayDataRow[Constants.additionTotal] = Decimal.Round(additionSum - subtractionSum,2);
                        dataTable.Rows.Add(netPayDataRow);
                        computedRules.Add(
                            new Rules
                            {
                                ComputationName = ComputationVariety.ADDITION,
                                RuleName = Constants.netPay,
                                RuleValue = Decimal.Round(additionSum - subtractionSum, 2)
                            });
                    }
                }
                dataGridView.DataSource = dataTable;
                dataGridView.Columns[Constants.addition].Width = 15;
                dataGridView.Columns[Constants.subtraction].Width = 15;
                this.Width = 583;
               // this.Height = 497;
                this.Height = 550;
                dataGridView.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
                dataGridView.Show();
             }  
                
            }
            return computedRules;
         }

        private void addComponent_Click(object sender, EventArgs e)
        {
            int numberOfTextBoxes = int.Parse(addComponentNumber.Text);
            //int xCoordinate = 30;
            //int yCoordinate = 40;
            int xCoordinate = 10;
            int yCoordinate = 10;
            addOuterPanel.Controls.Clear();
            for(int i = 0; i < numberOfTextBoxes; i++)
            {
                TextBox leftAddTextBox = new TextBox();
                leftAddTextBox.Location = new Point(xCoordinate, yCoordinate);
                leftAddTextBox.Size = new System.Drawing.Size(160,100);
                addOuterPanel.Controls.Add(leftAddTextBox);
                addOuterPanel.AutoScroll = true;
                addOuterPanel.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
                leftAddTextBox.Text = string.Format("Component Name{0},Value{0}", i + 1);
                leftAddTextBox.Name = string.Format("addComponentTextBox{0}", i + 1);
                //  leftAddTextBox.Validating += validateRuleName_Validation; //uncomment once fixed.
                addOuterPanel.Show();
                yCoordinate += 20;

            }
        }

        private void deductComponent_Click(object sender, EventArgs e)
        {
            int numberOfTextBoxes = int.Parse(deductComponentNumber.Text);
            int xCoordinate = 10;
            int yCoordinate = 10;
            deductOuterPanel.Controls.Clear();
            for (int i = 0; i < numberOfTextBoxes; i++)
            {
                TextBox leftDeductTextBox = new TextBox();
                leftDeductTextBox.Location = new Point(xCoordinate, yCoordinate);
                leftDeductTextBox.Size = new System.Drawing.Size(160, 100);
                deductOuterPanel.Controls.Add(leftDeductTextBox);
                deductOuterPanel.AutoScroll = true;
                deductOuterPanel.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
                leftDeductTextBox.Text = string.Format("Component Name{0},Value{0}", i + 1);
                leftDeductTextBox.Name = string.Format("deductComponentTextBox{0}", i + 1);
             //   leftDeductTextBox.Validating += validateRuleName_Validation; //uncomment once fixed.
                deductOuterPanel.Show();
                yCoordinate += 20;
            }
        }

        private void requiredName_Validating(object sender, CancelEventArgs e)
        {
            string textInput = requiredName.Text.TrimStart().TrimEnd();
            if(string.IsNullOrEmpty(textInput))
            {
                MessageBox.Show("No name has been entered", "Salary Slip Application", MessageBoxButtons.OK, MessageBoxIcon.Error);
                requiredName.Focus();
            }
            else if(!(RegularExpressionValidator.IsValidName(textInput)))
            {
                MessageBox.Show("The entered name is not in proper format","Salary Slip Application",MessageBoxButtons.OK,MessageBoxIcon.Error);
                requiredName.Focus();
            }
        }

        private void pan_Validating(object sender, CancelEventArgs e)
        {
            string textInput = pan.Text.TrimStart().TrimEnd();
            if (string.IsNullOrEmpty(textInput))
            {
                MessageBox.Show("No PAN has been entered", "Salary Slip Application", MessageBoxButtons.OK, MessageBoxIcon.Error);
                pan.Focus();
            } 
            else if (!(RegularExpressionValidator.IsValidPan(textInput)))
            {
                MessageBox.Show("The entered PAN is not in proper format", "Salary Slip Application", MessageBoxButtons.OK, MessageBoxIcon.Error);
                pan.Focus();
            }
        }

        private void accountNumber_Validating(object sender, CancelEventArgs e)
        {
            string textInput = accountNumber.Text.TrimStart().TrimEnd();
            if (string.IsNullOrEmpty(textInput))
            {
                MessageBox.Show("No account number has been entered", "Salary Slip Application", MessageBoxButtons.OK, MessageBoxIcon.Error);
                accountNumber.Focus();
            } 
            else if (!(RegularExpressionValidator.IsValidAccountNumber(textInput)))
            {
                MessageBox.Show("The entered account number is not in proper format", "Salary Slip Application", MessageBoxButtons.OK, MessageBoxIcon.Error);
                accountNumber.Focus();
            }
        }

        private void designation_Validating(object sender, CancelEventArgs e)
        {
            string textInput = designation.Text.TrimStart().TrimEnd();
            if (string.IsNullOrEmpty(textInput))
            {
                MessageBox.Show("No designation has been entered", "Salary Slip Application", MessageBoxButtons.OK, MessageBoxIcon.Error);
                designation.Focus();
            }
            else if (!(RegularExpressionValidator.IsValidDesignation(textInput)))
            {
                MessageBox.Show("The entered designation is not in proper format", "Salary Slip Application", MessageBoxButtons.OK, MessageBoxIcon.Error);
                designation.Focus();
            }
        }

        private void salary_Validating(object sender, CancelEventArgs e)
        {
            string textInput = salary.Text.TrimStart().TrimEnd();
            if (string.IsNullOrEmpty(textInput))
            {
                MessageBox.Show("No salary has been entered", "Salary Slip Application", MessageBoxButtons.OK, MessageBoxIcon.Error);
                salary.Focus();
            }
            else if (!(RegularExpressionValidator.IsValidSalary(textInput)))
            {
                MessageBox.Show("The entered salary is not in proper format", "Salary Slip Application", MessageBoxButtons.OK, MessageBoxIcon.Error);
                salary.Focus();
            }
        }

        //Currently Restricted to 10 components
        private void addComponentNumber_Validating(object sender, CancelEventArgs e)
        {
            string inputText = addComponentNumber.Text.TrimStart().TrimEnd();
            if (string.IsNullOrEmpty(inputText))
            {
                MessageBox.Show("No addition component count entered", "Salary Slip Application", MessageBoxButtons.OK, MessageBoxIcon.Error);
                addComponentNumber.Focus();
            }
            else if (!(RegularExpressionValidator.IsValidComponentCount(inputText)))
            {
                MessageBox.Show("Only 10 addition components are allowed", "Salary Slip Application", MessageBoxButtons.OK, MessageBoxIcon.Error);
                addComponentNumber.Focus();
            }
        }

        private void deductComponentNumber_Validating(object sender, CancelEventArgs e)
        {
            string inputText = deductComponentNumber.Text.TrimStart().TrimEnd();
            if (string.IsNullOrEmpty(inputText))
            {
                MessageBox.Show("No deduction component count entered", "Salary Slip Application", MessageBoxButtons.OK, MessageBoxIcon.Error);
                deductComponentNumber.Focus();
            }
            else if (!(RegularExpressionValidator.IsValidComponentCount(inputText)))
            {
                MessageBox.Show("Only 10 deduction components are allowed", "Salary Slip Application", MessageBoxButtons.OK, MessageBoxIcon.Error);
                deductComponentNumber.Focus();
            }
        }

    }
}
