﻿using SalarySlipApp.Models;
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

                ICollection<Rules> userAdditionComponents = FetchUserComponents(addOuterPanel.Controls,ComputationVariety.ADDITION);
                ICollection<Rules> userDeductionComponents = FetchUserComponents(deductOuterPanel.Controls, ComputationVariety.SUBTRACTION);

                ICollection<Rules> computedRules = salaryService.ComputeRules(salaryAmount,userAdditionComponents,userDeductionComponents);
                ICollection<Rules> finalResults = PopulateGrid(computedRules,userAdditionComponents,userDeductionComponents);
                string templateContent = salaryService.CollectTemplateData(employeeDetails,finalResults);

                salaryService.SendTemplate(templateContent);
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
                    //string ruleValidationInput = componentParts[0].Split(':')[1]; //may not be needed
                    componentParts = componentParts[0].Split(',');
                    //ruleHolder.Append(ruleValidationInput);//may not be needed
                    ((TextBox)component).Validating += validateRuleName_Validation;
                    ruleHolder.Clear();
                    if ((componentParts != null) && (componentParts.Count() > 0))
                    {
                        userRules.Add(new Rules()
                        {
                            ComputationName = typeOfOperation,
                            RuleName = componentParts[1].Split(':')[1], //change
                            RuleValue = Convert.ToDecimal(componentParts[2]) //change
                        }
                            );
                    }
                    componentBuilder.Clear();
                }
            }
            return userRules;
        }

        private void validateRuleName_Validation(object sender, EventArgs e)
        {
            TextBox senderControl = (TextBox)sender;
            
        }


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
                        totalDataRow[Constants.addition] = "Gross Salary";
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
                        netPayDataRow[Constants.additionTotal] = additionSum - subtractionSum;
                        dataTable.Rows.Add(netPayDataRow);
                        computedRules.Add(
                            new Rules
                            {
                                ComputationName = ComputationVariety.ADDITION,
                                RuleName = Constants.netPay,
                                RuleValue = additionSum - subtractionSum
                            });
                    }
                }
                dataGridView.DataSource = dataTable;
                dataGridView.Columns[Constants.addition].Width = 15;
                dataGridView.Columns[Constants.subtraction].Width = 15;
                this.Width = 583;
                this.Height = 497;
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
                //TextBox rightAddTextBox = new TextBox();
                //rightAddTextBox.Size = new System.Drawing.Size(60, 200);
                leftAddTextBox.Location = new Point(xCoordinate, yCoordinate);
                leftAddTextBox.Size = new System.Drawing.Size(160,100);
                //rightAddTextBox.Location = new Point(xCoordinate + 110, yCoordinate);
                addOuterPanel.Controls.Add(leftAddTextBox);
                //addOuterPanel.Controls.Add(rightAddTextBox);
                addOuterPanel.AutoScroll = true;
                addOuterPanel.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
                leftAddTextBox.Text = string.Format("Component Name{0},Value{0}", i + 1);
                leftAddTextBox.Name = string.Format("addComponentTextBox{0}", i + 1);
                //rightAddTextBox.Text = string.Format("Value{0}",i+1);
                //rightAddTextBox.Name = string.Format("rightTextBox{0}", i + 1);
                addOuterPanel.Show();
                yCoordinate += 20;

            }
        }

        private void deductComponent_Click(object sender, EventArgs e)
        {
            int numberOfTextBoxes = int.Parse(deductComponentNumber.Text);
            //int xCoordinate = 30;
            //int yCoordinate = 40;
            int xCoordinate = 10;
            int yCoordinate = 10;
            deductOuterPanel.Controls.Clear();
            for (int i = 0; i < numberOfTextBoxes; i++)
            {
                TextBox leftDeductTextBox = new TextBox();
                //TextBox rightDeductTextBox = new TextBox();
                //rightDeductTextBox.Size = new System.Drawing.Size(60, 200);
                leftDeductTextBox.Location = new Point(xCoordinate, yCoordinate);
                leftDeductTextBox.Size = new System.Drawing.Size(160, 100);
                //rightDeductTextBox.Location = new Point(xCoordinate + 110, yCoordinate);
                deductOuterPanel.Controls.Add(leftDeductTextBox);
                //deductOuterPanel.Controls.Add(rightDeductTextBox);
                deductOuterPanel.AutoScroll = true;
                deductOuterPanel.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
                leftDeductTextBox.Text = string.Format("Component Name{0},Value{0}", i + 1);
                leftDeductTextBox.Name = string.Format("deductComponentTextBox{0}", i + 1);
                //rightDeductTextBox.Text = string.Format("Value{0}", i + 1);
                //rightDeductTextBox.Name = string.Format("rightTextBox{0}",i+1);
                deductOuterPanel.Show();
                yCoordinate += 20;
            }
        }

        private void requiredName_Validating(object sender, CancelEventArgs e)
        {
            if(!(RegularExpressionValidator.IsValidName(requiredName.Text)))
            {
                MessageBox.Show("The entered name is not in proper format","Salary Slip Application",MessageBoxButtons.OK,MessageBoxIcon.Error);
                requiredName.Focus();
            }
        }

        private void pan_Validating(object sender, CancelEventArgs e)
        {
            if (!(RegularExpressionValidator.IsValidPan(pan.Text)))
            {
                MessageBox.Show("The entered PAN is not in proper format", "Salary Slip Application", MessageBoxButtons.OK, MessageBoxIcon.Error);
                pan.Focus();
            }
        }

        private void accountNumber_Validating(object sender, CancelEventArgs e)
        {
            if (!(RegularExpressionValidator.IsValidAccountNumber(accountNumber.Text)))
            {
                MessageBox.Show("The entered account number is not in proper format", "Salary Slip Application", MessageBoxButtons.OK, MessageBoxIcon.Error);
                accountNumber.Focus();
            }
        }

        private void designation_Validating(object sender, CancelEventArgs e)
        {
            if (!(RegularExpressionValidator.IsValidDesignation(designation.Text)))
            {
                MessageBox.Show("The entered designation is not in proper format", "Salary Slip Application", MessageBoxButtons.OK, MessageBoxIcon.Error);
                designation.Focus();
            }
        }

        private void salary_Validating(object sender, CancelEventArgs e)
        {
            if (!(RegularExpressionValidator.IsValidSalary(salary.Text)))
            {
                MessageBox.Show("The entered salary is not in proper format", "Salary Slip Application", MessageBoxButtons.OK, MessageBoxIcon.Error);
                salary.Focus();
            }
        }

        //Currently Restricted to 10 components
        private void addComponentNumber_Validating(object sender, CancelEventArgs e)
        {
            if (!(RegularExpressionValidator.IsValidComponentCount(addComponentNumber.Text)))
            {
                MessageBox.Show("Only 10 addition components are allowed", "Salary Slip Application", MessageBoxButtons.OK, MessageBoxIcon.Error);
                addComponentNumber.Focus();
            }
        }

        private void deductComponentNumber_Validating(object sender, CancelEventArgs e)
        {
            if (!(RegularExpressionValidator.IsValidComponentCount(deductComponentNumber.Text)))
            {
                MessageBox.Show("Only 10 deduction components are allowed", "Salary Slip Application", MessageBoxButtons.OK, MessageBoxIcon.Error);
                deductComponentNumber.Focus();
            }
        }
    }
}