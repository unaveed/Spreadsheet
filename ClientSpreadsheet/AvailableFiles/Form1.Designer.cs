namespace AvailableFiles
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
            this.DM = new System.Windows.Forms.Label();
            this.NewBtn = new System.Windows.Forms.Button();
            this.newSSName = new System.Windows.Forms.TextBox();
            this.CancelBtn = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.SuspendLayout();
            // 
            // DM
            // 
            this.DM.AutoSize = true;
            this.DM.ForeColor = System.Drawing.Color.Cornsilk;
            this.DM.Location = new System.Drawing.Point(13, 13);
            this.DM.Name = "DM";
            this.DM.Size = new System.Drawing.Size(35, 13);
            this.DM.TabIndex = 0;
            this.DM.Text = "label1";
            // 
            // NewBtn
            // 
            this.NewBtn.Location = new System.Drawing.Point(16, 486);
            this.NewBtn.Name = "NewBtn";
            this.NewBtn.Size = new System.Drawing.Size(75, 30);
            this.NewBtn.TabIndex = 1;
            this.NewBtn.Text = "New";
            this.NewBtn.UseVisualStyleBackColor = true;
            this.NewBtn.Click += new System.EventHandler(this.NewBtn_Click);
            // 
            // newSSName
            // 
            this.newSSName.ForeColor = System.Drawing.SystemColors.MenuText;
            this.newSSName.Location = new System.Drawing.Point(118, 493);
            this.newSSName.Name = "newSSName";
            this.newSSName.Size = new System.Drawing.Size(253, 20);
            this.newSSName.TabIndex = 2;
            this.newSSName.Text = "Provide a name for the new spreadsheet";
            // 
            // CancelBtn
            // 
            this.CancelBtn.Location = new System.Drawing.Point(396, 486);
            this.CancelBtn.Name = "CancelBtn";
            this.CancelBtn.Size = new System.Drawing.Size(75, 30);
            this.CancelBtn.TabIndex = 3;
            this.CancelBtn.Text = "Cancel";
            this.CancelBtn.UseVisualStyleBackColor = true;
            this.CancelBtn.Click += new System.EventHandler(this.CancelBtn_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.BackColor = System.Drawing.Color.Green;
            this.groupBox1.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBox1.ForeColor = System.Drawing.Color.Cornsilk;
            this.groupBox1.Location = new System.Drawing.Point(16, 43);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(446, 421);
            this.groupBox1.TabIndex = 4;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Files";
            // 
            // Form1
            // 
            this.BackColor = System.Drawing.Color.Green;
            this.ClientSize = new System.Drawing.Size(483, 525);
            this.Controls.Add(this.CancelBtn);
            this.Controls.Add(this.newSSName);
            this.Controls.Add(this.NewBtn);
            this.Controls.Add(this.DM);
            this.Controls.Add(this.groupBox1);
            this.Name = "Form1";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button NewSpreadsheet;
        private System.Windows.Forms.Label DisplayMessage;
        private System.Windows.Forms.Label DM;
        private System.Windows.Forms.Button NewBtn;
        private System.Windows.Forms.TextBox newSSName;
        private System.Windows.Forms.Button CancelBtn;
        private System.Windows.Forms.GroupBox groupBox1;

    }
}

