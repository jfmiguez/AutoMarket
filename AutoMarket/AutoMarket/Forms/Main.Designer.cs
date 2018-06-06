namespace AutoMarket
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
            this.btnTestYahoo = new System.Windows.Forms.Button();
            this.btnTestTDAmeritrade = new System.Windows.Forms.Button();
            this.txtUsername = new System.Windows.Forms.TextBox();
            this.txtPassword = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.btnTestGoogle = new System.Windows.Forms.Button();
            this.lstTestGoogle = new System.Windows.Forms.ListBox();
            this.lblCount = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.lstRealTimeQuotes = new System.Windows.Forms.ListBox();
            this.lblTitle = new System.Windows.Forms.Label();
            this.cboInterval = new System.Windows.Forms.ComboBox();
            this.dtDateFrom = new System.Windows.Forms.DateTimePicker();
            this.dtDateTo = new System.Windows.Forms.DateTimePicker();
            this.txtSymbol = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.butTestBases = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // btnTestYahoo
            // 
            this.btnTestYahoo.Location = new System.Drawing.Point(12, 22);
            this.btnTestYahoo.Name = "btnTestYahoo";
            this.btnTestYahoo.Size = new System.Drawing.Size(79, 23);
            this.btnTestYahoo.TabIndex = 0;
            this.btnTestYahoo.Text = "Test Yahoo";
            this.btnTestYahoo.UseVisualStyleBackColor = true;
            this.btnTestYahoo.Click += new System.EventHandler(this.btnTestYahoo_Click);
            // 
            // btnTestTDAmeritrade
            // 
            this.btnTestTDAmeritrade.Location = new System.Drawing.Point(362, 127);
            this.btnTestTDAmeritrade.Name = "btnTestTDAmeritrade";
            this.btnTestTDAmeritrade.Size = new System.Drawing.Size(113, 23);
            this.btnTestTDAmeritrade.TabIndex = 0;
            this.btnTestTDAmeritrade.Text = "Test TDAmeritrade";
            this.btnTestTDAmeritrade.UseVisualStyleBackColor = true;
            this.btnTestTDAmeritrade.Click += new System.EventHandler(this.btnTestTDAmeritrade_Click);
            // 
            // txtUsername
            // 
            this.txtUsername.Location = new System.Drawing.Point(481, 129);
            this.txtUsername.Name = "txtUsername";
            this.txtUsername.Size = new System.Drawing.Size(104, 20);
            this.txtUsername.TabIndex = 1;
            // 
            // txtPassword
            // 
            this.txtPassword.Location = new System.Drawing.Point(591, 130);
            this.txtPassword.Name = "txtPassword";
            this.txtPassword.PasswordChar = '\'';
            this.txtPassword.Size = new System.Drawing.Size(131, 20);
            this.txtPassword.TabIndex = 2;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(478, 113);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(55, 13);
            this.label1.TabIndex = 3;
            this.label1.Text = "Username";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(588, 113);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(53, 13);
            this.label2.TabIndex = 4;
            this.label2.Text = "Password";
            // 
            // btnTestGoogle
            // 
            this.btnTestGoogle.Location = new System.Drawing.Point(97, 22);
            this.btnTestGoogle.Name = "btnTestGoogle";
            this.btnTestGoogle.Size = new System.Drawing.Size(79, 23);
            this.btnTestGoogle.TabIndex = 5;
            this.btnTestGoogle.Text = "Test Google";
            this.btnTestGoogle.UseVisualStyleBackColor = true;
            this.btnTestGoogle.Click += new System.EventHandler(this.btnTestGoogle_Click);
            // 
            // lstTestGoogle
            // 
            this.lstTestGoogle.FormattingEnabled = true;
            this.lstTestGoogle.Location = new System.Drawing.Point(12, 113);
            this.lstTestGoogle.Name = "lstTestGoogle";
            this.lstTestGoogle.Size = new System.Drawing.Size(344, 264);
            this.lstTestGoogle.TabIndex = 6;
            // 
            // lblCount
            // 
            this.lblCount.AutoSize = true;
            this.lblCount.Location = new System.Drawing.Point(91, 97);
            this.lblCount.Name = "lblCount";
            this.lblCount.Size = new System.Drawing.Size(13, 13);
            this.lblCount.TabIndex = 7;
            this.lblCount.Text = "0";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(12, 97);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(50, 13);
            this.label3.TabIndex = 8;
            this.label3.Text = "Historical";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(12, 394);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(92, 13);
            this.label4.TabIndex = 8;
            this.label4.Text = "Real-Time Quotes";
            // 
            // lstRealTimeQuotes
            // 
            this.lstRealTimeQuotes.FormattingEnabled = true;
            this.lstRealTimeQuotes.Location = new System.Drawing.Point(12, 423);
            this.lstRealTimeQuotes.Name = "lstRealTimeQuotes";
            this.lstRealTimeQuotes.Size = new System.Drawing.Size(344, 95);
            this.lstRealTimeQuotes.TabIndex = 9;
            // 
            // lblTitle
            // 
            this.lblTitle.AutoSize = true;
            this.lblTitle.BackColor = System.Drawing.Color.DarkRed;
            this.lblTitle.Font = new System.Drawing.Font("Microsoft Sans Serif", 30F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblTitle.ForeColor = System.Drawing.Color.Yellow;
            this.lblTitle.Location = new System.Drawing.Point(340, 23);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new System.Drawing.Size(534, 46);
            this.lblTitle.TabIndex = 10;
            this.lblTitle.Text = "FORM FOR TESTING ONLY";
            // 
            // cboInterval
            // 
            this.cboInterval.FormattingEnabled = true;
            this.cboInterval.Items.AddRange(new object[] {
            "1m",
            "2m",
            "5m",
            "15m",
            "30m",
            "60m",
            "90m",
            "1h",
            "1d",
            "5d",
            "1wk",
            "1mo",
            "3mo"});
            this.cboInterval.Location = new System.Drawing.Point(67, 64);
            this.cboInterval.Name = "cboInterval";
            this.cboInterval.Size = new System.Drawing.Size(53, 21);
            this.cboInterval.TabIndex = 11;
            this.cboInterval.Text = "1d";
            // 
            // dtDateFrom
            // 
            this.dtDateFrom.Location = new System.Drawing.Point(126, 65);
            this.dtDateFrom.Name = "dtDateFrom";
            this.dtDateFrom.Size = new System.Drawing.Size(93, 20);
            this.dtDateFrom.TabIndex = 12;
            this.dtDateFrom.Value = new System.DateTime(2018, 1, 1, 0, 0, 0, 0);
            // 
            // dtDateTo
            // 
            this.dtDateTo.Location = new System.Drawing.Point(225, 65);
            this.dtDateTo.Name = "dtDateTo";
            this.dtDateTo.Size = new System.Drawing.Size(93, 20);
            this.dtDateTo.TabIndex = 12;
            // 
            // txtSymbol
            // 
            this.txtSymbol.Location = new System.Drawing.Point(11, 64);
            this.txtSymbol.Name = "txtSymbol";
            this.txtSymbol.Size = new System.Drawing.Size(50, 20);
            this.txtSymbol.TabIndex = 13;
            this.txtSymbol.Text = "ABCB";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(12, 48);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(41, 13);
            this.label5.TabIndex = 14;
            this.label5.Text = "Symbol";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(64, 48);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(42, 13);
            this.label6.TabIndex = 14;
            this.label6.Text = "Interval";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(123, 48);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(56, 13);
            this.label7.TabIndex = 14;
            this.label7.Text = "Date From";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(222, 48);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(46, 13);
            this.label8.TabIndex = 14;
            this.label8.Text = "Date To";
            // 
            // butTestBases
            // 
            this.butTestBases.Location = new System.Drawing.Point(182, 22);
            this.butTestBases.Name = "butTestBases";
            this.butTestBases.Size = new System.Drawing.Size(79, 23);
            this.butTestBases.TabIndex = 15;
            this.butTestBases.Text = "Test Bases";
            this.butTestBases.UseVisualStyleBackColor = true;
            this.butTestBases.Click += new System.EventHandler(this.butTestBases_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(886, 527);
            this.Controls.Add(this.butTestBases);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.txtSymbol);
            this.Controls.Add(this.dtDateTo);
            this.Controls.Add(this.dtDateFrom);
            this.Controls.Add(this.cboInterval);
            this.Controls.Add(this.lblTitle);
            this.Controls.Add(this.lstRealTimeQuotes);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.lblCount);
            this.Controls.Add(this.lstTestGoogle);
            this.Controls.Add(this.btnTestGoogle);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.txtPassword);
            this.Controls.Add(this.txtUsername);
            this.Controls.Add(this.btnTestTDAmeritrade);
            this.Controls.Add(this.btnTestYahoo);
            this.Name = "Form1";
            this.Text = "AutoMarket";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnTestYahoo;
        private System.Windows.Forms.Button btnTestTDAmeritrade;
        private System.Windows.Forms.TextBox txtUsername;
        private System.Windows.Forms.TextBox txtPassword;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button btnTestGoogle;
        private System.Windows.Forms.ListBox lstTestGoogle;
        private System.Windows.Forms.Label lblCount;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.ListBox lstRealTimeQuotes;
        private System.Windows.Forms.Label lblTitle;
        private System.Windows.Forms.ComboBox cboInterval;
        private System.Windows.Forms.DateTimePicker dtDateFrom;
        private System.Windows.Forms.DateTimePicker dtDateTo;
        private System.Windows.Forms.TextBox txtSymbol;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Button butTestBases;
    }
}

