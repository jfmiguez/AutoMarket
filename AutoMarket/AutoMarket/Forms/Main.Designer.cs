﻿namespace AutoMarket
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
            this.SuspendLayout();
            // 
            // btnTestYahoo
            // 
            this.btnTestYahoo.Location = new System.Drawing.Point(12, 22);
            this.btnTestYahoo.Name = "btnTestYahoo";
            this.btnTestYahoo.Size = new System.Drawing.Size(113, 23);
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
            this.btnTestGoogle.Location = new System.Drawing.Point(12, 51);
            this.btnTestGoogle.Name = "btnTestGoogle";
            this.btnTestGoogle.Size = new System.Drawing.Size(113, 23);
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
            this.lblCount.Location = new System.Drawing.Point(131, 56);
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
            this.lblTitle.Location = new System.Drawing.Point(188, 22);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new System.Drawing.Size(534, 46);
            this.lblTitle.TabIndex = 10;
            this.lblTitle.Text = "FORM FOR TESTING ONLY";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(746, 527);
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
            this.Text = "Form1";
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
    }
}

