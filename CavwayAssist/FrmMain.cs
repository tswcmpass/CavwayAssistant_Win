using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Management;
using System.Threading;

namespace CavwayAssist
{
    public partial class FrmMain : Form
    {
        List<Shot> listshot = new List<Shot>();
        

        public FrmMain()
        {
            InitializeComponent();
            disableControls();
        }

        private void readSerial()
        {
            byte[] serialbuff = new byte[4] { 0,0,0,0 };
            UART.readMemory(0x8008, serialbuff, 4);
            int Serial = serialbuff[0] | (serialbuff[1] << 8);
            lblSerial.Text = "Serial. " + Serial.ToString().PadLeft(4, '0');
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (!UART.isConnected())
            {
                if (UART.connect())
                {
                    
                    lblPortStatus.Text = "Connected";
                    lblPortStatus.ForeColor = Color.Green;
                    btnConnect.Text = "Disconnect";
                    enableControls();
                    readSerial();
                }
                else
                {
                    MessageBox.Show("Connect Device Failed.");
                }
            }
            else
            {
                UART.Disconnect();
                lblPortStatus.Text = "Disconnected";
                lblPortStatus.ForeColor = Color.Red;
                btnConnect.Text = "Connect";
                disableControls();
            }
        }

        private void enableControls()
        {
            grpCMD.Enabled = true;
            btnCali.Enabled = true;
            btnDownloadData.Enabled = true;
            btnFirmware.Enabled = true;
        }

        private void disableControls()
        {
            grpCMD.Enabled = false;
            btnCali.Enabled = false;
            btnDownloadData.Enabled = false;
            btnFirmware.Enabled = false;
        }

        private void btnLaserON_Click(object sender, EventArgs e)
        {
            UART.sendCommand(Command.LaserOn);
        }

        private void btnLaserOff_Click(object sender, EventArgs e)
        {
            UART.sendCommand(Command.LaserOff);
        }

        private void btnMeas_Click(object sender, EventArgs e)
        {
            UART.sendCommand(Command.LaserTrig);
        }

        private void FrmMain_Load(object sender, EventArgs e)
        {
            ColumnHeader ch = new ColumnHeader();
            ch.Text = "Time"; ch.Width = 150;
            listData.Columns.Add(ch);
            ch = new ColumnHeader();
            ch.Text = "Flag"; ch.Width = 120;
            listData.Columns.Add(ch);
            ch = new ColumnHeader();
            ch.Text = "Distance/m";ch.Width = 80;
            listData.Columns.Add(ch);
            ch = new ColumnHeader();
            ch.Text = "Azimuth/deg";ch.Width = 80;
            listData.Columns.Add(ch);
            ch = new ColumnHeader();
            ch.Text = "Inclination/deg"; ch.Width = 80;
            listData.Columns.Add(ch);
            ch = new ColumnHeader();
            ch.Text = "absG/g"; ch.Width = 80;
            listData.Columns.Add(ch);
            ch = new ColumnHeader();
            ch.Text = "absM/uT"; ch.Width = 80;
            listData.Columns.Add(ch);
            ch = new ColumnHeader();
            ch.Text = "dip/deg"; ch.Width = 80;
            listData.Columns.Add(ch);
            ch = new ColumnHeader();
            ch.Text = "GX1"; ch.Width = 60;
            listData.Columns.Add(ch);
            ch = new ColumnHeader();
            ch.Text = "GY1"; ch.Width = 60;
            listData.Columns.Add(ch);
            ch = new ColumnHeader();
            ch.Text = "GZ1"; ch.Width = 60;
            listData.Columns.Add(ch);
            ch = new ColumnHeader();
            ch.Text = "MX1"; ch.Width = 60;
            listData.Columns.Add(ch);
            ch = new ColumnHeader();
            ch.Text = "MY1"; ch.Width = 60;
            listData.Columns.Add(ch);
            ch = new ColumnHeader();
            ch.Text = "MZ1"; ch.Width = 60;
            listData.Columns.Add(ch);
            ch = new ColumnHeader();
            ch.Text = "GX2"; ch.Width = 60;
            listData.Columns.Add(ch);
            ch = new ColumnHeader();
            ch.Text = "GY2"; ch.Width = 60;
            listData.Columns.Add(ch);
            ch = new ColumnHeader();
            ch.Text = "GZ2"; ch.Width = 60;
            listData.Columns.Add(ch);
            ch = new ColumnHeader();
            ch.Text = "MX2"; ch.Width = 60;
            listData.Columns.Add(ch);
            ch = new ColumnHeader();
            ch.Text = "MY2"; ch.Width = 60;
            listData.Columns.Add(ch);
            ch = new ColumnHeader();
            ch.Text = "MZ2"; ch.Width = 60;
            listData.Columns.Add(ch);
            ch = new ColumnHeader();
            ch.Text = "Error Infos"; ch.Width = 300;
            listData.Columns.Add(ch);
            listData.View = View.Details;

            Thread ThreadDetectCommOff = new Thread(ThreadDetectComm);
            ThreadDetectCommOff.Priority = ThreadPriority.BelowNormal;
            ThreadDetectCommOff.Start();

        }

        private int ThreadRunning = 1;
        private void ThreadDetectComm()
        {
            while(ThreadRunning == 1)
            {
                if(!UART.isConnected())
                {
                    this.BeginInvoke((EventHandler)(delegate
                    {
                        lblPortStatus.Text = "Disconnected";
                        lblPortStatus.ForeColor = Color.Red;
                        btnConnect.Text = "Connect";
                        disableControls();
                    }));
                }
                Thread.Sleep(100);
            }
            ThreadRunning = -1;
        }

        private void btnCali_Click(object sender, EventArgs e)
        {
            FrmCali frm = new FrmCali();
            frm.ShowDialog();
        }

        private void btnDownloadData_Click(object sender, EventArgs e)
        {
            int shot_num = 0;
            bool res = int.TryParse(txtnumshot.Text, out shot_num);
            if(!res || shot_num < 0 || shot_num > 1000)
            {
                MessageBox.Show("Please input valid number");
                return;
            }
            listData.Items.Clear();   //clear listview before downloading
            curr_idx_rd = 0;
            Thread ThreadDS = new Thread(new ParameterizedThreadStart(ThreadDataComm));
            ThreadDS.Priority = ThreadPriority.AboveNormal;
            ThreadDS.Start(shot_num);
            btnDownloadData.Enabled = false;
        }

        private int curr_idx_rd = 0;
        private void Updatelistview()
        {
            while(curr_idx_rd < listshot.Count)
            {
                int i = curr_idx_rd;
                ListViewItem lvi = new ListViewItem();
                lvi.Text = listshot[i].dtShotTime.ToString();
                string flg = "";
                if (listshot[i].isLeg) flg = "leg";
                else if (listshot[i].isCali) flg = "cali";
                else flg = "splay";
                if (listshot[i].flags == 0) flg += " (feature)";
                else if (listshot[i].flags == 1) flg += " (ridge)";
                else if (listshot[i].flags == 2) flg += " (backsight)";
                else if (listshot[i].flags == 3) flg += " (generic)";
                lvi.SubItems.Add(flg);
                lvi.SubItems.Add(listshot[i].distance.ToString("F2"));
                lvi.SubItems.Add(listshot[i].azimuth.ToString("F1"));
                lvi.SubItems.Add(listshot[i].inclination.ToString("F1"));
                lvi.SubItems.Add(listshot[i].absG.ToString("F3"));
                lvi.SubItems.Add(listshot[i].absM.ToString("F2"));
                lvi.SubItems.Add(listshot[i].dip.ToString("F2"));
                for(int j = 0; j < 2; j++)
                {
                    lvi.SubItems.Add(listshot[i].RawG[j].X.ToString());
                    lvi.SubItems.Add(listshot[i].RawG[j].Y.ToString());
                    lvi.SubItems.Add(listshot[i].RawG[j].Z.ToString());
                    lvi.SubItems.Add(listshot[i].RawM[j].X.ToString());
                    lvi.SubItems.Add(listshot[i].RawM[j].Y.ToString());
                    lvi.SubItems.Add(listshot[i].RawM[j].Z.ToString());
                }
                lvi.SubItems.Add(listshot[i].errorinfo);
                listData.BeginUpdate();
                listData.Items.Add(lvi);
                listData.EndUpdate();
                curr_idx_rd++;
            }
        }

        private void ThreadDataComm(object obj)
        {
            byte[] data_buf = new byte[64];
            int shot_num = (int)obj;
            listshot.Clear();
            for (int i = 0; i < shot_num; i++)
            {
                if(!UART.readMemory(i, data_buf, 64))
                {
                    MessageBox.Show("Download failed");
                    break;
                }
                Shot shotdata = new Shot(data_buf);
                if (!shotdata.isValid) break;
                listshot.Add(shotdata);
                this.BeginInvoke((EventHandler)(delegate
                {
                    btnDownloadData.Text = "downloading " + (i + 1).ToString() + "/" + shot_num.ToString();
                    Updatelistview();
                }));
            }
            this.BeginInvoke((EventHandler)(delegate
            {
                btnDownloadData.Text = "download";
                btnDownloadData.Enabled = true;
            }));
        }

        private void btnExportData_Click(object sender, EventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Title = "Please choose the path to save coeffs";
            saveFileDialog.Filter = "csv file|*.csv";
            if (saveFileDialog.ShowDialog() != DialogResult.OK) return;
            string path = saveFileDialog.FileName;//获得保存文件的路径
            log csv = new log(path);
            string tmpstr = "";
            for (int i = 0; i < listData.Columns.Count; i++)
            {
                tmpstr += listData.Columns[i].Text;
                if (i != listData.Columns.Count) tmpstr += ",";
            }
            csv.Write(tmpstr);
            for(int i = 0;i < listshot.Count;i++)
            {
                tmpstr = listshot[i].dtShotTime.ToString() + "," + listData.Items[i].SubItems[1].Text + "," +
                    listshot[i].distance.ToString("F3") + "," + listshot[i].azimuth.ToString("F2") +"," + listshot[i].inclination.ToString("F2") + "," +
                    listshot[i].absG.ToString("F3") + "," + listshot[i].absM.ToString("F2") + "," + listshot[i].dip.ToString("F2") + ",";
                for(int j = 0;j < 2; j++)
                {
                    tmpstr += listshot[i].RawG[j].X + "," + listshot[i].RawG[j].Y + "," + listshot[i].RawG[j].Z + "," +
                        listshot[i].RawM[j].X + "," + listshot[i].RawM[j].Y + "," + listshot[i].RawM[j].Z + ",";
                }
                tmpstr += listshot[i].errorinfo;
                csv.Write(tmpstr);
            }
            MessageBox.Show("Write data file successful");

        }

        private void btnFirmware_Click(object sender, EventArgs e)
        {
            FrmFirmware frm = new FrmFirmware();
            frm.ShowDialog();
        }

        private void FrmMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            ThreadRunning = 0;
            while (ThreadRunning != -1) ;
        }

        private void btnSyncTime_Click(object sender, EventArgs e)
        {
            DateTime dtDevice = DateTime.Now;
            dtDevice = DateTime.SpecifyKind(dtDevice, DateTimeKind.Utc);
            UInt32 time_t = (UInt32)(dtDevice.Subtract(DateTime.SpecifyKind(new DateTime(1970, 1, 1, 0, 0, 0), DateTimeKind.Utc)).TotalSeconds);
            byte[] buf = new byte[4];
            buf[0] = (byte)(time_t);
            buf[1] = (byte)(time_t>>8);
            buf[2] = (byte)(time_t>>16);
            buf[3] = (byte)(time_t>>24);
            if(UART.writeMemory(0x8000, buf, 4))
            {
                MessageBox.Show("sync time successful!");
            } else MessageBox.Show("sync time failed");
        }
    }

    //对ListView进行继承重写：
    public class DoubleBufferListView : ListView
    {
        public DoubleBufferListView()
        {
            SetStyle(ControlStyles.DoubleBuffer |
              ControlStyles.OptimizedDoubleBuffer |
              ControlStyles.AllPaintingInWmPaint, true);
            UpdateStyles();
        }
    }
}
