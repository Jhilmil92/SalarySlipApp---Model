namespace SalarySlipApp
{
    partial class Form1
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.label1 = new System.Windows.Forms.Label();
            this.requiredName = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.dateOfJoining = new System.Windows.Forms.DateTimePicker();
            this.label3 = new System.Windows.Forms.Label();
            this.pan = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.accountNumber = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.designation = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.salary = new System.Windows.Forms.TextBox();
            this.generateButton = new System.Windows.Forms.Button();
            this.dataGridView = new System.Windows.Forms.DataGridView();
            this.addOuterPanel = new System.Windows.Forms.Panel();
            this.deductOuterPanel = new System.Windows.Forms.Panel();
            this.addInnerPanel = new System.Windows.Forms.Panel();
            this.addComponent = new System.Windows.Forms.Button();
            this.addComponentNumber = new System.Windows.Forms.TextBox();
            this.deductInnerPanel = new System.Windows.Forms.Panel();
            this.deductComponent = new System.Windows.Forms.Button();
            this.deductComponentNumber = new System.Windows.Forms.TextBox();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView)).BeginInit();
            this.addInnerPanel.SuspendLayout();
            this.deductInnerPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(22, 27);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(35, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Name";
            // 
            // requiredName
            // 
            this.requiredName.Location = new System.Drawing.Point(63, 24);
            this.requiredName.Name = "requiredName";
            this.requiredName.Size = new System.Drawing.Size(163, 20);
            this.requiredName.TabIndex = 1;
            this.requiredName.Validating += new System.ComponentModel.CancelEventHandler(this.requiredName_Validating);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(261, 27);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(78, 13);
            this.label2.TabIndex = 2;
            this.label2.Text = "Date of Joining";
            // 
            // dateOfJoining
            // 
            this.dateOfJoining.Location = new System.Drawing.Point(355, 24);
            this.dateOfJoining.Name = "dateOfJoining";
            this.dateOfJoining.Size = new System.Drawing.Size(200, 20);
            this.dateOfJoining.TabIndex = 3;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(22, 72);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(29, 13);
            this.label3.TabIndex = 4;
            this.label3.Text = "PAN";
            // 
            // pan
            // 
            this.pan.Location = new System.Drawing.Point(63, 69);
            this.pan.Name = "pan";
            this.pan.Size = new System.Drawing.Size(163, 20);
            this.pan.TabIndex = 5;
            this.pan.Validating += new System.ComponentModel.CancelEventHandler(this.pan_Validating);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(261, 72);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(87, 13);
            this.label4.TabIndex = 6;
            this.label4.Text = "Account Number";
            // 
            // accountNumber
            // 
            this.accountNumber.Location = new System.Drawing.Point(355, 69);
            this.accountNumber.Name = "accountNumber";
            this.accountNumber.Size = new System.Drawing.Size(200, 20);
            this.accountNumber.TabIndex = 7;
            this.accountNumber.Validating += new System.ComponentModel.CancelEventHandler(this.accountNumber_Validating);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(22, 116);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(63, 13);
            this.label5.TabIndex = 8;
            this.label5.Text = "Designation";
            // 
            // designation
            // 
            this.designation.Location = new System.Drawing.Point(91, 113);
            this.designation.Name = "designation";
            this.designation.Size = new System.Drawing.Size(160, 20);
            this.designation.TabIndex = 9;
            this.designation.Validating += new System.ComponentModel.CancelEventHandler(this.designation_Validating);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(276, 120);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(36, 13);
            this.label6.TabIndex = 10;
            this.label6.Text = "Salary";
            // 
            // salary
            // 
            this.salary.Location = new System.Drawing.Point(355, 116);
            this.salary.Name = "salary";
            this.salary.Size = new System.Drawing.Size(134, 20);
            this.salary.TabIndex = 11;
            this.salary.Validating += new System.ComponentModel.CancelEventHandler(this.salary_Validating);
            // 
            // generateButton
            // 
            this.generateButton.Location = new System.Drawing.Point(219, 271);
            this.generateButton.Name = "generateButton";
            this.generateButton.Size = new System.Drawing.Size(120, 31);
            this.generateButton.TabIndex = 12;
            this.generateButton.Text = "Generate";
            this.generateButton.UseVisualStyleBackColor = true;
            this.generateButton.Click += new System.EventHandler(this.generateButton_Click);
            // 
            // dataGridView
            // 
            this.dataGridView.AllowUserToAddRows = false;
            this.dataGridView.AllowUserToDeleteRows = false;
            this.dataGridView.BackgroundColor = System.Drawing.SystemColors.ButtonHighlight;
            this.dataGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView.Location = new System.Drawing.Point(13, 308);
            this.dataGridView.Name = "dataGridView";
            this.dataGridView.ReadOnly = true;
            this.dataGridView.Size = new System.Drawing.Size(542, 191);
            this.dataGridView.TabIndex = 13;
            // 
            // addOuterPanel
            // 
            this.addOuterPanel.Location = new System.Drawing.Point(25, 179);
            this.addOuterPanel.Name = "addOuterPanel";
            this.addOuterPanel.Size = new System.Drawing.Size(200, 86);
            this.addOuterPanel.TabIndex = 14;
            // 
            // deductOuterPanel
            // 
            this.deductOuterPanel.Location = new System.Drawing.Point(340, 179);
            this.deductOuterPanel.Name = "deductOuterPanel";
            this.deductOuterPanel.Size = new System.Drawing.Size(200, 86);
            this.deductOuterPanel.TabIndex = 15;
            // 
            // addInnerPanel
            // 
            this.addInnerPanel.Controls.Add(this.addComponent);
            this.addInnerPanel.Controls.Add(this.addComponentNumber);
            this.addInnerPanel.Location = new System.Drawing.Point(25, 148);
            this.addInnerPanel.Name = "addInnerPanel";
            this.addInnerPanel.Size = new System.Drawing.Size(200, 31);
            this.addInnerPanel.TabIndex = 0;
            // 
            // addComponent
            // 
            this.addComponent.Location = new System.Drawing.Point(141, 3);
            this.addComponent.Name = "addComponent";
            this.addComponent.Size = new System.Drawing.Size(56, 23);
            this.addComponent.TabIndex = 1;
            this.addComponent.Text = "AC";
            this.addComponent.UseVisualStyleBackColor = true;
            this.addComponent.Click += new System.EventHandler(this.addComponent_Click);
            // 
            // addComponentNumber
            // 
            this.addComponentNumber.Location = new System.Drawing.Point(3, 5);
            this.addComponentNumber.Name = "addComponentNumber";
            this.addComponentNumber.Size = new System.Drawing.Size(111, 20);
            this.addComponentNumber.TabIndex = 0;
            this.addComponentNumber.Validating += new System.ComponentModel.CancelEventHandler(this.addComponentNumber_Validating);
            // 
            // deductInnerPanel
            // 
            this.deductInnerPanel.Controls.Add(this.deductComponent);
            this.deductInnerPanel.Controls.Add(this.deductComponentNumber);
            this.deductInnerPanel.Location = new System.Drawing.Point(340, 148);
            this.deductInnerPanel.Name = "deductInnerPanel";
            this.deductInnerPanel.Size = new System.Drawing.Size(200, 31);
            this.deductInnerPanel.TabIndex = 1;
            // 
            // deductComponent
            // 
            this.deductComponent.Location = new System.Drawing.Point(141, 5);
            this.deductComponent.Name = "deductComponent";
            this.deductComponent.Size = new System.Drawing.Size(56, 23);
            this.deductComponent.TabIndex = 2;
            this.deductComponent.Text = "DC";
            this.deductComponent.UseVisualStyleBackColor = true;
            this.deductComponent.Click += new System.EventHandler(this.deductComponent_Click);
            // 
            // deductComponentNumber
            // 
            this.deductComponentNumber.Location = new System.Drawing.Point(3, 5);
            this.deductComponentNumber.Name = "deductComponentNumber";
            this.deductComponentNumber.Size = new System.Drawing.Size(111, 20);
            this.deductComponentNumber.TabIndex = 2;
            this.deductComponentNumber.Validating += new System.ComponentModel.CancelEventHandler(this.deductComponentNumber_Validating);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(567, 502);
            this.Controls.Add(this.deductInnerPanel);
            this.Controls.Add(this.addInnerPanel);
            this.Controls.Add(this.deductOuterPanel);
            this.Controls.Add(this.addOuterPanel);
            this.Controls.Add(this.dataGridView);
            this.Controls.Add(this.generateButton);
            this.Controls.Add(this.salary);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.designation);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.accountNumber);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.pan);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.dateOfJoining);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.requiredName);
            this.Controls.Add(this.label1);
            this.Name = "Form1";
            this.Text = "Salary Slip Application";
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView)).EndInit();
            this.addInnerPanel.ResumeLayout(false);
            this.addInnerPanel.PerformLayout();
            this.deductInnerPanel.ResumeLayout(false);
            this.deductInnerPanel.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox requiredName;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.DateTimePicker dateOfJoining;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox pan;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox accountNumber;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox designation;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox salary;
        private System.Windows.Forms.Button generateButton;
        private System.Windows.Forms.DataGridView dataGridView;
        private System.Windows.Forms.Panel addOuterPanel;
        private System.Windows.Forms.Panel deductOuterPanel;
        private System.Windows.Forms.Panel addInnerPanel;
        private System.Windows.Forms.Panel deductInnerPanel;
        private System.Windows.Forms.Button addComponent;
        private System.Windows.Forms.TextBox addComponentNumber;
        private System.Windows.Forms.Button deductComponent;
        private System.Windows.Forms.TextBox deductComponentNumber;
    }
}

