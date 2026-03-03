namespace CavwayAssist
{
    partial class FrmCali
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
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.btnUploadCoeffs = new System.Windows.Forms.Button();
            this.btnLoadCoeffs = new System.Windows.Forms.Button();
            this.lblCoeff = new System.Windows.Forms.Label();
            this.btnSaveCoeffs = new System.Windows.Forms.Button();
            this.btnDownCoeffs = new System.Windows.Forms.Button();
            this.lblinfo = new System.Windows.Forms.Label();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.lblinfo);
            this.groupBox1.Controls.Add(this.btnUploadCoeffs);
            this.groupBox1.Controls.Add(this.btnLoadCoeffs);
            this.groupBox1.Controls.Add(this.lblCoeff);
            this.groupBox1.Controls.Add(this.btnSaveCoeffs);
            this.groupBox1.Controls.Add(this.btnDownCoeffs);
            this.groupBox1.Location = new System.Drawing.Point(9, 9);
            this.groupBox1.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Padding = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.groupBox1.Size = new System.Drawing.Size(360, 395);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Calibration Coeffs";
            // 
            // btnUploadCoeffs
            // 
            this.btnUploadCoeffs.Enabled = false;
            this.btnUploadCoeffs.Font = new System.Drawing.Font("Arial Narrow", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnUploadCoeffs.Location = new System.Drawing.Point(234, 191);
            this.btnUploadCoeffs.Margin = new System.Windows.Forms.Padding(1, 2, 1, 2);
            this.btnUploadCoeffs.Name = "btnUploadCoeffs";
            this.btnUploadCoeffs.Size = new System.Drawing.Size(112, 37);
            this.btnUploadCoeffs.TabIndex = 5;
            this.btnUploadCoeffs.Text = "Upload Coeffs";
            this.btnUploadCoeffs.UseVisualStyleBackColor = true;
            this.btnUploadCoeffs.Click += new System.EventHandler(this.btnUploadCoeffs_Click);
            // 
            // btnLoadCoeffs
            // 
            this.btnLoadCoeffs.Font = new System.Drawing.Font("Arial Narrow", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnLoadCoeffs.Location = new System.Drawing.Point(234, 143);
            this.btnLoadCoeffs.Margin = new System.Windows.Forms.Padding(1, 2, 1, 2);
            this.btnLoadCoeffs.Name = "btnLoadCoeffs";
            this.btnLoadCoeffs.Size = new System.Drawing.Size(112, 37);
            this.btnLoadCoeffs.TabIndex = 6;
            this.btnLoadCoeffs.Text = "Load Coeffs";
            this.btnLoadCoeffs.UseVisualStyleBackColor = true;
            this.btnLoadCoeffs.Click += new System.EventHandler(this.btnLoadCoeffs_Click);
            // 
            // lblCoeff
            // 
            this.lblCoeff.Location = new System.Drawing.Point(9, 118);
            this.lblCoeff.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblCoeff.Name = "lblCoeff";
            this.lblCoeff.Size = new System.Drawing.Size(213, 257);
            this.lblCoeff.TabIndex = 1;
            this.lblCoeff.Text = "Calibration Coeffs:";
            // 
            // btnSaveCoeffs
            // 
            this.btnSaveCoeffs.Enabled = false;
            this.btnSaveCoeffs.Font = new System.Drawing.Font("Arial Narrow", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnSaveCoeffs.Location = new System.Drawing.Point(234, 94);
            this.btnSaveCoeffs.Margin = new System.Windows.Forms.Padding(1, 2, 1, 2);
            this.btnSaveCoeffs.Name = "btnSaveCoeffs";
            this.btnSaveCoeffs.Size = new System.Drawing.Size(112, 37);
            this.btnSaveCoeffs.TabIndex = 5;
            this.btnSaveCoeffs.Text = "Save Coeffs";
            this.btnSaveCoeffs.UseVisualStyleBackColor = true;
            this.btnSaveCoeffs.Click += new System.EventHandler(this.btnSaveCoeffs_Click);
            // 
            // btnDownCoeffs
            // 
            this.btnDownCoeffs.Font = new System.Drawing.Font("Arial Narrow", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnDownCoeffs.Location = new System.Drawing.Point(234, 42);
            this.btnDownCoeffs.Margin = new System.Windows.Forms.Padding(1, 2, 1, 2);
            this.btnDownCoeffs.Name = "btnDownCoeffs";
            this.btnDownCoeffs.Size = new System.Drawing.Size(112, 37);
            this.btnDownCoeffs.TabIndex = 4;
            this.btnDownCoeffs.Text = "Download Coeffs";
            this.btnDownCoeffs.UseVisualStyleBackColor = true;
            this.btnDownCoeffs.Click += new System.EventHandler(this.btnDownCoeffs_Click);
            // 
            // lblinfo
            // 
            this.lblinfo.Location = new System.Drawing.Point(9, 16);
            this.lblinfo.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblinfo.Name = "lblinfo";
            this.lblinfo.Size = new System.Drawing.Size(213, 94);
            this.lblinfo.TabIndex = 7;
            this.lblinfo.Text = "Calibration Information:";
            // 
            // FrmCali
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(379, 415);
            this.Controls.Add(this.groupBox1);
            this.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.MaximizeBox = false;
            this.Name = "FrmCali";
            this.Text = "Calibration";
            this.Load += new System.EventHandler(this.FrmCali_Load);
            this.groupBox1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button btnDownCoeffs;
        private System.Windows.Forms.Label lblCoeff;
        private System.Windows.Forms.Button btnSaveCoeffs;
        private System.Windows.Forms.Button btnLoadCoeffs;
        private System.Windows.Forms.Button btnUploadCoeffs;
        private System.Windows.Forms.Label lblinfo;
    }
}