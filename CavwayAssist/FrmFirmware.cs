using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Resources;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CavwayAssist
{
    public partial class FrmFirmware : Form
    {
        public FrmFirmware()
        {
            InitializeComponent();
        }

        private string firmpathname = "";
        Boolean isInternalFirm = false;


        private void btnUpgrade_Click(object sender, EventArgs e)
        {
            //string filepathname = "";
            //if (firm_rd1.Checked) filepathname = "CVWY_X1.bin";
            if(firmpathname == "")
            {
                MessageBox.Show("No firmware selected!");
                return;
            }
            progbar.Value = 0;
            btnUpgrade.Enabled = false;
            Thread ThreadDS = new Thread(new ParameterizedThreadStart(ThreadFWUpgrade));
            ThreadDS.Priority = ThreadPriority.Normal;
            ThreadDS.Start(firmpathname);
        }


        private void ThreadFWUpgrade(object obj)
        {
            Stream stream;
            if (isInternalFirm)
            {
                Assembly assm = Assembly.GetExecutingAssembly();
                string path = assm.GetName().Name.ToString() + ".Resources." + (string)obj;
                stream = assm.GetManifestResourceStream(path);
            }
            else
            {
                stream = new FileStream((string)obj, FileMode.Open, FileAccess.Read);
            }
            long filelen = stream.Length - 256;

            if (!UART.isConnected())
            {
                this.BeginInvoke((EventHandler)(delegate
                {
                    btnUpgrade.Enabled = true;
                    MessageBox.Show("Device not connected!");
                }));
                return;
            }
            if (!UART.sendFWUpdateStart())
            {
                this.BeginInvoke((EventHandler)(delegate
                {
                    btnUpgrade.Enabled = true;
                    MessageBox.Show("Upgrade fail.");
                }));
                return;      //send firmware begin command
            }
            this.BeginInvoke((EventHandler)(delegate
            {
                progbar.Value = 10;
            }));

            byte[] buff = new byte[128];

            int offset = 0;
            int res;
            int packet_idx = 0;
            uint crc = 0;
            UInt32 checksum = 0;
            
            do
            {
                for (int i = 0; i < 128; i++) buff[i] = 0xff;  //init buff with all 0xff
                res = stream.Read(buff, 0, buff.Length);       //read from bin file
                if(offset >= 256)   //skip header zone, 256 bytes record the bin file identity
                {
                    if(!UART.sendFWPacket(packet_idx, buff, ref crc))   //send data packet
                    {
                        MessageBox.Show("Upgrade Fail");
                        this.BeginInvoke((EventHandler)(delegate
                        {
                            btnUpgrade.Enabled = true;
                        }));
                        return;
                    }
                    checksum += crc;
                    packet_idx++;
                    this.BeginInvoke((EventHandler)(delegate
                    {
                        progbar.Value = (int)(10 + packet_idx * 80 / (filelen / 128));
                    }));
                }
                offset += buff.Length;
            } while (res == buff.Length);

            stream.Close();


            if(UART.sendFWUpdateEnd(checksum))           //send firmware end command
            {
                this.BeginInvoke((EventHandler)(delegate
                {
                    progbar.Value = 100;
                    MessageBox.Show("upgrade success");
                    btnUpgrade.Enabled = true;
                }));
            }
            else
            {
                this.BeginInvoke((EventHandler)(delegate
                {
                    progbar.Value = 0;
                    MessageBox.Show("upgrade failed");
                    btnUpgrade.Enabled = true;
                }));
            }
        }

        private bool checkValid(string filepath, ref string firm_ver, ref DateTime bin_date)
        {
            Stream stream;
            byte[] bin_ID = new byte[8] {0x11,0x23,0x55,0x6e,0x7c,0xef,0x6d,0x5b }; 
            stream = new FileStream(filepath, FileMode.Open, FileAccess.Read);
            byte[] buff = new byte[256];
            byte[] bin_ID_read = new byte[8];
            int res = stream.Read(buff, 0, buff.Length);       //read 256 bytes header from bin file
            stream.Close();

            Array.Copy(buff, bin_ID_read, 8);
            if (!bin_ID_read.SequenceEqual(bin_ID)) return false;
            firm_ver = buff[12].ToString() + "." + buff[13].ToString() + "." + buff[14].ToString();
            long Timestamp = (buff[8] << 24) + (buff[9] << 16) + (buff[10] << 8) + buff[11];
            bin_date = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc).AddSeconds(Timestamp);
            return true;
        }

        private void btnBrowse_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Title = "Please choose the proper coeff file";//设置对话框标题
            openFileDialog.Multiselect = false;  //设置对话框可以多选
            //openFileDialog.InitialDirectory = @"C:\Users\user1\Desktop\研发报告\basler拍照研究报告";//设置对话框的初始目录
            //设置对话框的文件类型
            openFileDialog.Filter = "cavway firmware file|*.bin";
            if (openFileDialog.ShowDialog() != DialogResult.OK) return;
            isInternalFirm = false;
            string path = openFileDialog.FileName;//获取选中的路径
            DateTime modificationTime = File.GetLastWriteTime(path);
            DateTime binTime = new DateTime();
            string firm_ver = "";
            if (checkValid(path, ref firm_ver, ref binTime))
            {
                lblFirm.Text = "Selected:\n" + path + "\nFirmware release time:" + binTime.ToString("yyyy-MM-dd HH:mm") + "\nFirmware Version:" + firm_ver;
                firmpathname = path;
            }
            else
            {
                lblFirm.Text = "Invalid Firmware file";
                MessageBox.Show("Invalid Firmware file!");
                firmpathname = "";
            }
        }

        private void firm_rd1_CheckedChanged(object sender, EventArgs e)
        {
            /*if (firm_rd1.Checked)
            {
                isInternalFirm = true;
                firmpathname = "CVWY_X1.bin";
                lblFirm.Text = "Selected: Internal Firmware 1.0 (2024/11/15)";
            }
            else*/
            {
                isInternalFirm = false;
                lblFirm.Text = "No Firmware Selected.";
            }
        }

        private void FrmFirmware_Load(object sender, EventArgs e)
        {
            //firm_rd1.Checked = false;
        }
    }
}
