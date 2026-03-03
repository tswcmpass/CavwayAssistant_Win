namespace CavwayAssist
{
    partial class FrmFirmware
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FrmFirmware));
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.btnUpgrade = new System.Windows.Forms.Button();
            this.progbar = new System.Windows.Forms.ProgressBar();
            this.btnBrowse = new System.Windows.Forms.Button();
            this.lblFirm = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.Location = new System.Drawing.Point(12, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(625, 44);
            this.label1.TabIndex = 1;
            this.label1.Text = "Before firmware upgrade, let Cavway X1 enter into boot mode:\r\nPress the LEFT and " +
    "RIGHT while switching on with MEAS key.";
            // 
            // label2
            // 
            this.label2.ForeColor = System.Drawing.Color.Red;
            this.label2.Location = new System.Drawing.Point(12, 62);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(550, 27);
            this.label2.TabIndex = 2;
            this.label2.Text = "ATTENTION: ALL DATA STORED WILL BE ERASED DURING UPGRADE";
            // 
            // btnUpgrade
            // 
            this.btnUpgrade.Location = new System.Drawing.Point(452, 212);
            this.btnUpgrade.Name = "btnUpgrade";
            this.btnUpgrade.Size = new System.Drawing.Size(162, 39);
            this.btnUpgrade.TabIndex = 3;
            this.btnUpgrade.Text = "Upgrade";
            this.btnUpgrade.UseVisualStyleBackColor = true;
            this.btnUpgrade.Click += new System.EventHandler(this.btnUpgrade_Click);
            // 
            // progbar
            // 
            this.progbar.Location = new System.Drawing.Point(12, 176);
            this.progbar.Name = "progbar";
            this.progbar.Size = new System.Drawing.Size(625, 30);
            this.progbar.TabIndex = 4;
            // 
            // btnBrowse
            // 
            this.btnBrowse.Location = new System.Drawing.Point(12, 212);
            this.btnBrowse.Name = "btnBrowse";
            this.btnBrowse.Size = new System.Drawing.Size(162, 39);
            this.btnBrowse.TabIndex = 5;
            this.btnBrowse.Text = "Browse file";
            this.btnBrowse.UseVisualStyleBackColor = true;
            this.btnBrowse.Click += new System.EventHandler(this.btnBrowse_Click);
            // 
            // lblFirm
            // 
            this.lblFirm.AutoSize = true;
            this.lblFirm.Location = new System.Drawing.Point(15, 96);
            this.lblFirm.Name = "lblFirm";
            this.lblFirm.Size = new System.Drawing.Size(170, 18);
            this.lblFirm.TabIndex = 6;
            this.lblFirm.Text = "Firmware Selected:";
            // 
            // FrmFirmware
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 18F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(649, 261);
            this.Controls.Add(this.lblFirm);
            this.Controls.Add(this.btnBrowse);
            this.Controls.Add(this.progbar);
            this.Controls.Add(this.btnUpgrade);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.Name = "FrmFirmware";
            this.Text = "Firmware Upgrade";
            this.Load += new System.EventHandler(this.FrmFirmware_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button btnUpgrade;
        private System.Windows.Forms.ProgressBar progbar;
        private System.Windows.Forms.Button btnBrowse;
        private System.Windows.Forms.Label lblFirm;
    }
}