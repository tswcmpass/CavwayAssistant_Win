namespace CavwayAssist
{
    partial class FrmMain
    {
        /// <summary>
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows 窗体设计器生成的代码

        /// <summary>
        /// 设计器支持所需的方法 - 不要修改
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FrmMain));
            this.btnConnect = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.lblSerial = new System.Windows.Forms.Label();
            this.lblPortStatus = new System.Windows.Forms.Label();
            this.grpCMD = new System.Windows.Forms.GroupBox();
            this.btnFirmware = new System.Windows.Forms.Button();
            this.btnCali = new System.Windows.Forms.Button();
            this.btnSyncTime = new System.Windows.Forms.Button();
            this.btnMeas = new System.Windows.Forms.Button();
            this.btnLaserOff = new System.Windows.Forms.Button();
            this.btnLaserON = new System.Windows.Forms.Button();
            this.grpHistoryData = new System.Windows.Forms.GroupBox();
            this.btnExportData = new System.Windows.Forms.Button();
            this.listData = new CavwayAssist.DoubleBufferListView();
            this.btnDownloadData = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.txtnumshot = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.groupBox1.SuspendLayout();
            this.grpCMD.SuspendLayout();
            this.grpHistoryData.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnConnect
            // 
            this.btnConnect.Font = new System.Drawing.Font("Arial Narrow", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnConnect.Location = new System.Drawing.Point(16, 23);
            this.btnConnect.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.btnConnect.Name = "btnConnect";
            this.btnConnect.Size = new System.Drawing.Size(94, 25);
            this.btnConnect.TabIndex = 0;
            this.btnConnect.Text = "Connect";
            this.btnConnect.UseVisualStyleBackColor = true;
            this.btnConnect.Click += new System.EventHandler(this.button1_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.lblSerial);
            this.groupBox1.Controls.Add(this.lblPortStatus);
            this.groupBox1.Controls.Add(this.btnConnect);
            this.groupBox1.Location = new System.Drawing.Point(10, 11);
            this.groupBox1.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Padding = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.groupBox1.Size = new System.Drawing.Size(131, 113);
            this.groupBox1.TabIndex = 1;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Device";
            // 
            // lblSerial
            // 
            this.lblSerial.AutoSize = true;
            this.lblSerial.Font = new System.Drawing.Font("Arial Narrow", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblSerial.Location = new System.Drawing.Point(13, 86);
            this.lblSerial.Name = "lblSerial";
            this.lblSerial.Size = new System.Drawing.Size(39, 17);
            this.lblSerial.TabIndex = 3;
            this.lblSerial.Text = "Serial.";
            // 
            // lblPortStatus
            // 
            this.lblPortStatus.AutoSize = true;
            this.lblPortStatus.Font = new System.Drawing.Font("Arial Narrow", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblPortStatus.ForeColor = System.Drawing.Color.Red;
            this.lblPortStatus.Location = new System.Drawing.Point(26, 60);
            this.lblPortStatus.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblPortStatus.Name = "lblPortStatus";
            this.lblPortStatus.Size = new System.Drawing.Size(74, 17);
            this.lblPortStatus.TabIndex = 2;
            this.lblPortStatus.Text = "Disconnected";
            // 
            // grpCMD
            // 
            this.grpCMD.Controls.Add(this.btnFirmware);
            this.grpCMD.Controls.Add(this.btnCali);
            this.grpCMD.Controls.Add(this.btnSyncTime);
            this.grpCMD.Controls.Add(this.btnMeas);
            this.grpCMD.Controls.Add(this.btnLaserOff);
            this.grpCMD.Controls.Add(this.btnLaserON);
            this.grpCMD.Location = new System.Drawing.Point(10, 130);
            this.grpCMD.Name = "grpCMD";
            this.grpCMD.Size = new System.Drawing.Size(131, 279);
            this.grpCMD.TabIndex = 2;
            this.grpCMD.TabStop = false;
            this.grpCMD.Text = "Commands";
            // 
            // btnFirmware
            // 
            this.btnFirmware.Font = new System.Drawing.Font("Arial Narrow", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnFirmware.Location = new System.Drawing.Point(16, 186);
            this.btnFirmware.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.btnFirmware.Name = "btnFirmware";
            this.btnFirmware.Size = new System.Drawing.Size(94, 25);
            this.btnFirmware.TabIndex = 5;
            this.btnFirmware.Text = "Firmware";
            this.btnFirmware.UseVisualStyleBackColor = true;
            this.btnFirmware.Click += new System.EventHandler(this.btnFirmware_Click);
            // 
            // btnCali
            // 
            this.btnCali.Font = new System.Drawing.Font("Arial Narrow", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnCali.Location = new System.Drawing.Point(16, 153);
            this.btnCali.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.btnCali.Name = "btnCali";
            this.btnCali.Size = new System.Drawing.Size(94, 25);
            this.btnCali.TabIndex = 3;
            this.btnCali.Text = "Calibration";
            this.btnCali.UseVisualStyleBackColor = true;
            this.btnCali.Click += new System.EventHandler(this.btnCali_Click);
            // 
            // btnSyncTime
            // 
            this.btnSyncTime.Font = new System.Drawing.Font("Arial Narrow", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnSyncTime.Location = new System.Drawing.Point(16, 122);
            this.btnSyncTime.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.btnSyncTime.Name = "btnSyncTime";
            this.btnSyncTime.Size = new System.Drawing.Size(94, 25);
            this.btnSyncTime.TabIndex = 8;
            this.btnSyncTime.Text = "Sync Time";
            this.btnSyncTime.UseVisualStyleBackColor = true;
            this.btnSyncTime.Click += new System.EventHandler(this.btnSyncTime_Click);
            // 
            // btnMeas
            // 
            this.btnMeas.Font = new System.Drawing.Font("Arial Narrow", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnMeas.Location = new System.Drawing.Point(16, 91);
            this.btnMeas.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.btnMeas.Name = "btnMeas";
            this.btnMeas.Size = new System.Drawing.Size(94, 25);
            this.btnMeas.TabIndex = 5;
            this.btnMeas.Text = "Measure";
            this.btnMeas.UseVisualStyleBackColor = true;
            this.btnMeas.Click += new System.EventHandler(this.btnMeas_Click);
            // 
            // btnLaserOff
            // 
            this.btnLaserOff.Font = new System.Drawing.Font("Arial Narrow", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnLaserOff.Location = new System.Drawing.Point(16, 60);
            this.btnLaserOff.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.btnLaserOff.Name = "btnLaserOff";
            this.btnLaserOff.Size = new System.Drawing.Size(94, 25);
            this.btnLaserOff.TabIndex = 4;
            this.btnLaserOff.Text = "Laser OFF";
            this.btnLaserOff.UseVisualStyleBackColor = true;
            this.btnLaserOff.Click += new System.EventHandler(this.btnLaserOff_Click);
            // 
            // btnLaserON
            // 
            this.btnLaserON.Font = new System.Drawing.Font("Arial Narrow", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnLaserON.Location = new System.Drawing.Point(16, 29);
            this.btnLaserON.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.btnLaserON.Name = "btnLaserON";
            this.btnLaserON.Size = new System.Drawing.Size(94, 25);
            this.btnLaserON.TabIndex = 3;
            this.btnLaserON.Text = "Laser ON";
            this.btnLaserON.UseVisualStyleBackColor = true;
            this.btnLaserON.Click += new System.EventHandler(this.btnLaserON_Click);
            // 
            // grpHistoryData
            // 
            this.grpHistoryData.Controls.Add(this.btnExportData);
            this.grpHistoryData.Controls.Add(this.listData);
            this.grpHistoryData.Controls.Add(this.btnDownloadData);
            this.grpHistoryData.Controls.Add(this.label2);
            this.grpHistoryData.Controls.Add(this.txtnumshot);
            this.grpHistoryData.Controls.Add(this.label1);
            this.grpHistoryData.Location = new System.Drawing.Point(147, 12);
            this.grpHistoryData.Name = "grpHistoryData";
            this.grpHistoryData.Size = new System.Drawing.Size(905, 397);
            this.grpHistoryData.TabIndex = 4;
            this.grpHistoryData.TabStop = false;
            this.grpHistoryData.Text = "History Data";
            // 
            // btnExportData
            // 
            this.btnExportData.Font = new System.Drawing.Font("Arial Narrow", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnExportData.Location = new System.Drawing.Point(427, 23);
            this.btnExportData.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.btnExportData.Name = "btnExportData";
            this.btnExportData.Size = new System.Drawing.Size(130, 25);
            this.btnExportData.TabIndex = 7;
            this.btnExportData.Text = "Export data";
            this.btnExportData.UseVisualStyleBackColor = true;
            this.btnExportData.Click += new System.EventHandler(this.btnExportData_Click);
            // 
            // listData
            // 
            this.listData.FullRowSelect = true;
            this.listData.Location = new System.Drawing.Point(6, 59);
            this.listData.Name = "listData";
            this.listData.Size = new System.Drawing.Size(886, 322);
            this.listData.TabIndex = 6;
            this.listData.UseCompatibleStateImageBehavior = false;
            // 
            // btnDownloadData
            // 
            this.btnDownloadData.Font = new System.Drawing.Font("Arial Narrow", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnDownloadData.Location = new System.Drawing.Point(279, 23);
            this.btnDownloadData.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.btnDownloadData.Name = "btnDownloadData";
            this.btnDownloadData.Size = new System.Drawing.Size(130, 25);
            this.btnDownloadData.TabIndex = 5;
            this.btnDownloadData.Text = "Download data";
            this.btnDownloadData.UseVisualStyleBackColor = true;
            this.btnDownloadData.Click += new System.EventHandler(this.btnDownloadData_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(224, 29);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(29, 15);
            this.label2.TabIndex = 5;
            this.label2.Text = "shots";
            // 
            // txtnumshot
            // 
            this.txtnumshot.Location = new System.Drawing.Point(132, 26);
            this.txtnumshot.Name = "txtnumshot";
            this.txtnumshot.Size = new System.Drawing.Size(77, 20);
            this.txtnumshot.TabIndex = 1;
            this.txtnumshot.Text = "1";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(16, 29);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(77, 15);
            this.label1.TabIndex = 0;
            this.label1.Text = "Download lastest";
            // 
            // FrmMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(5F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1064, 421);
            this.Controls.Add(this.grpHistoryData);
            this.Controls.Add(this.grpCMD);
            this.Controls.Add(this.groupBox1);
            this.Font = new System.Drawing.Font("Arial Narrow", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.MaximizeBox = false;
            this.Name = "FrmMain";
            this.Text = "Cavway Assistant V1.2 (Jan. 21st, 2026)";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FrmMain_FormClosing);
            this.Load += new System.EventHandler(this.FrmMain_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.grpCMD.ResumeLayout(false);
            this.grpHistoryData.ResumeLayout(false);
            this.grpHistoryData.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnConnect;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label lblPortStatus;
        private System.Windows.Forms.GroupBox grpCMD;
        private System.Windows.Forms.Button btnLaserOff;
        private System.Windows.Forms.Button btnLaserON;
        private System.Windows.Forms.Button btnMeas;
        private System.Windows.Forms.Button btnCali;
        private System.Windows.Forms.GroupBox grpHistoryData;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btnDownloadData;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox txtnumshot;
        private System.Windows.Forms.Button btnFirmware;
        private System.Windows.Forms.Button btnExportData;
        private System.Windows.Forms.Button btnSyncTime;
        private DoubleBufferListView listData;
        private System.Windows.Forms.Label lblSerial;
    }
}

