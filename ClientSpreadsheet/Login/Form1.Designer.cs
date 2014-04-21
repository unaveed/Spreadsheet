namespace Login
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
            this.label2 = new System.Windows.Forms.Label();
            this.ServerIPAddress = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.Password = new System.Windows.Forms.TextBox();
            this.LoginBtn = new System.Windows.Forms.Button();
            this.InvalidLogin = new System.Windows.Forms.Label();
            this.CancelBtn = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.BackColor = System.Drawing.Color.PaleGreen;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 20F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Underline))));
            this.label1.ForeColor = System.Drawing.Color.LavenderBlush;
            this.label1.Location = new System.Drawing.Point(90, 34);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(192, 31);
            this.label1.TabIndex = 0;
            this.label1.Text = "Please Log In";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 14F, System.Drawing.FontStyle.Bold);
            this.label2.ForeColor = System.Drawing.Color.LavenderBlush;
            this.label2.Location = new System.Drawing.Point(45, 31);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(249, 24);
            this.label2.TabIndex = 1;
            this.label2.Text = "Enter Server\'s iP Address";
            // 
            // ServerIPAddress
            // 
            this.ServerIPAddress.Location = new System.Drawing.Point(100, 69);
            this.ServerIPAddress.Name = "ServerIPAddress";
            this.ServerIPAddress.Size = new System.Drawing.Size(123, 20);
            this.ServerIPAddress.TabIndex = 2;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 14F, System.Drawing.FontStyle.Bold);
            this.label3.ForeColor = System.Drawing.Color.LavenderBlush;
            this.label3.Location = new System.Drawing.Point(109, 129);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(100, 24);
            this.label3.TabIndex = 3;
            this.label3.Text = "Password";
            // 
            // Password
            // 
            this.Password.Location = new System.Drawing.Point(100, 167);
            this.Password.Name = "Password";
            this.Password.Size = new System.Drawing.Size(123, 20);
            this.Password.TabIndex = 4;
            this.Password.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // LoginBtn
            // 
            this.LoginBtn.BackColor = System.Drawing.Color.LightCyan;
            this.LoginBtn.Location = new System.Drawing.Point(12, 342);
            this.LoginBtn.Name = "LoginBtn";
            this.LoginBtn.Size = new System.Drawing.Size(75, 48);
            this.LoginBtn.TabIndex = 5;
            this.LoginBtn.Text = "Login";
            this.LoginBtn.UseVisualStyleBackColor = false;
            this.LoginBtn.Click += new System.EventHandler(this.LoginBtn_Click);
            // 
            // InvalidLogin
            // 
            this.InvalidLogin.AutoSize = true;
            this.InvalidLogin.Location = new System.Drawing.Point(176, 319);
            this.InvalidLogin.Name = "InvalidLogin";
            this.InvalidLogin.Size = new System.Drawing.Size(0, 13);
            this.InvalidLogin.TabIndex = 6;
            // 
            // CancelBtn
            // 
            this.CancelBtn.BackColor = System.Drawing.Color.LightCyan;
            this.CancelBtn.Location = new System.Drawing.Point(294, 342);
            this.CancelBtn.Name = "CancelBtn";
            this.CancelBtn.Size = new System.Drawing.Size(75, 48);
            this.CancelBtn.TabIndex = 7;
            this.CancelBtn.Text = "Cancel";
            this.CancelBtn.UseVisualStyleBackColor = false;
            this.CancelBtn.Click += new System.EventHandler(this.CancelBtn_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.BackColor = System.Drawing.Color.PaleGreen;
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.ServerIPAddress);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.Password);
            this.groupBox1.Location = new System.Drawing.Point(23, 90);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(335, 226);
            this.groupBox1.TabIndex = 8;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = " ";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Green;
            this.ClientSize = new System.Drawing.Size(381, 402);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.CancelBtn);
            this.Controls.Add(this.InvalidLogin);
            this.Controls.Add(this.LoginBtn);
            this.Controls.Add(this.label1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Name = "Form1";
            this.Text = "LOGIN";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox ServerIPAddress;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox Password;
        private System.Windows.Forms.Button LoginBtn;
        private System.Windows.Forms.Label InvalidLogin;
        private System.Windows.Forms.Button CancelBtn;
        private System.Windows.Forms.GroupBox groupBox1;
    }
}

