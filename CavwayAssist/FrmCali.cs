using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;


namespace CavwayAssist
{

    public partial class FrmCali : Form
    {

        Vector bG1 = new Vector();
        Vector bG2 = new Vector();
        Vector bM1 = new Vector();
        Vector bM2 = new Vector();
        Matrix aG1 = new Matrix();
        Matrix aG2 = new Matrix();
        Matrix aM1 = new Matrix();
        Matrix aM2 = new Matrix();

        private bool bIsHasCaliInfo = false;
        private DateTime cali_time = new DateTime();
        private float cali_aver_err;
        private float cali_err_stddev;
        private float cali_max_err;
        private float cali_dip;

        private const int CALIINFO_FLAG = 0;
        private const int CALIINFO_VER = 1;
        private const int CALIINFO_DATE = 2;
        private const int CALIINFO_AVER_ERR = 6;
        private const int CALIINFO_ERR_VAR = 8;
        private const int CALIINFO_MAX_ERR = 10;
        private const int CALIINFO_DIP = 12;

        public FrmCali()
        {
            InitializeComponent();
        }


        private void btnDownCoeffs_Click(object sender, EventArgs e)
        {
            byte[] coeffs = new byte[128];
            byte[] cali_info = new byte[16];
            bIsHasCaliInfo = false;
            if(!UART.readMemory(0x9000 + 64, cali_info, 16))
            {
                MessageBox.Show("Download calibration information failed");
                return;
            }
            if (!UART.readMemory(0x9000+128, coeffs, 128))
            {
                MessageBox.Show("Download calibration coeffs failed");
                return;
            }
            
            if (cali_info[CALIINFO_FLAG] != 0x55)
            {
                bIsHasCaliInfo = false;
                lblinfo.Text += "No Calibration Information";
            }
            else
            {
                bIsHasCaliInfo = true;
                Int16 datatmp;
                UInt32 time_t = (UInt32)(cali_info[CALIINFO_DATE] | cali_info[CALIINFO_DATE + 1] << 8 | cali_info[CALIINFO_DATE + 2] << 16 | cali_info[CALIINFO_DATE + 3] << 24);
                cali_time = new DateTime(1970, 1, 1).AddSeconds(time_t);
                datatmp = (Int16)(cali_info[CALIINFO_AVER_ERR] | cali_info[CALIINFO_AVER_ERR + 1] << 8);
                cali_aver_err = (float)(datatmp / 100.0);
                datatmp = (Int16)(cali_info[CALIINFO_ERR_VAR] | cali_info[CALIINFO_ERR_VAR + 1] << 8);
                cali_err_stddev = (float)(datatmp / 100.0);
                datatmp = (Int16)(cali_info[CALIINFO_MAX_ERR] | cali_info[CALIINFO_MAX_ERR + 1] << 8);
                cali_max_err = (float)(datatmp / 100.0);
                datatmp = (Int16)(cali_info[CALIINFO_DIP] | cali_info[CALIINFO_DIP + 1] << 8);
                cali_dip = (float)(datatmp / 100.0);

            }

            byte[] sensor_coeffs = new byte[64];
            Array.Copy(coeffs, 0, sensor_coeffs, 0, 64);
            ParseCoeffs(sensor_coeffs, ref bG1, ref bM1, ref aG1, ref aM1);

            Array.Copy(coeffs, 64, sensor_coeffs, 0, 64);
            ParseCoeffs(sensor_coeffs, ref bG2, ref bM2, ref aG2, ref aM2);
            btnSaveCoeffs.Enabled = true;
            ShowCeofff();
            MessageBox.Show("Download calibration coeffs successful!");
        }

        void ShowCeofff()
        {
            lblinfo.Text = "Calibration info:\n";
            if (bIsHasCaliInfo)
            {
                lblinfo.Text += "Calibration Time: " + cali_time.ToString("yyyy-MM-dd HH:mm") + "\n";
                lblinfo.Text += "Average Error:" + cali_aver_err.ToString() + "\n";
                lblinfo.Text += "Error Stddev.:" + cali_err_stddev.ToString() + "\n";
                lblinfo.Text += "Max Error:" + cali_max_err.ToString() + "\n";
                lblinfo.Text += "Dip:" + cali_dip.ToString() + "\n";
            }
            lblCoeff.Text = "Sensor1:\n";
            lblCoeff.Text += "bG1:  " + bG1.x.ToString("f4") + "  " + bG1.y.ToString("f4") + "  " + bG1.z.ToString("f4") + "\n";
            lblCoeff.Text += "aG1:  " + aG1.x.x.ToString("f4") + "  " + aG1.x.y.ToString("f4") + "  " + aG1.x.z.ToString("f4") + "\n";
            lblCoeff.Text += "      " + aG1.y.x.ToString("f4") + "  " + aG1.y.y.ToString("f4") + "  " + aG1.y.z.ToString("f4") + "\n";
            lblCoeff.Text += "      " + aG1.z.x.ToString("f4") + "  " + aG1.z.y.ToString("f4") + "  " + aG1.z.z.ToString("f4") + "\n\n";

            lblCoeff.Text += "bM1:  " + bM1.x.ToString("f4") + "  " + bM1.y.ToString("f4") + "  " + bM1.z.ToString("f4") + "\n";
            lblCoeff.Text += "aM1:  " + aM1.x.x.ToString("f4") + "  " + aM1.x.y.ToString("f4") + "  " + aM1.x.z.ToString("f4") + "\n";
            lblCoeff.Text += "      " + aM1.y.x.ToString("f4") + "  " + aM1.y.y.ToString("f4") + "  " + aM1.y.z.ToString("f4") + "\n";
            lblCoeff.Text += "      " + aM1.z.x.ToString("f4") + "  " + aM1.z.y.ToString("f4") + "  " + aM1.z.z.ToString("f4") + "\n\n";

            lblCoeff.Text += "Sensor2:\n";

            lblCoeff.Text += "bG2:  " + bG2.x.ToString("f4") + "  " + bG2.y.ToString("f4") + "  " + bG2.z.ToString("f4") + "\n";
            lblCoeff.Text += "aG2:  " + aG2.x.x.ToString("f4") + "  " + aG2.x.y.ToString("f4") + "  " + aG2.x.z.ToString("f4") + "\n";
            lblCoeff.Text += "      " + aG2.y.x.ToString("f4") + "  " + aG2.y.y.ToString("f4") + "  " + aG2.y.z.ToString("f4") + "\n";
            lblCoeff.Text += "      " + aG2.z.x.ToString("f4") + "  " + aG2.z.y.ToString("f4") + "  " + aG2.z.z.ToString("f4") + "\n\n";

            lblCoeff.Text += "bM2:  " + bM2.x.ToString("f4") + "  " + bM2.y.ToString("f4") + "  " + bM2.z.ToString("f4") + "\n";
            lblCoeff.Text += "aM2:  " + aM2.x.x.ToString("f4") + "  " + aM2.x.y.ToString("f4") + "  " + aM2.x.z.ToString("f4") + "\n";
            lblCoeff.Text += "      " + aM2.y.x.ToString("f4") + "  " + aM2.y.y.ToString("f4") + "  " + aM2.y.z.ToString("f4") + "\n";
            lblCoeff.Text += "      " + aM2.z.x.ToString("f4") + "  " + aM2.z.y.ToString("f4") + "  " + aM2.z.z.ToString("f4") + "\n\n";
        }

        private void GetCoeff( byte[] data, int pos, ref double value )
        {
            short ival = (short)(((data[pos + 1]) << 8 ) | data[pos]);
            value = (double)(ival);
        }

        private void PutCoeff(byte[] coeff,int idx, double value)
        {
            short ival = (short)(Math.Round(value));
            coeff[idx] = (byte)(ival & 0xff);
            coeff[idx + 1] = (byte)((ival >> 8) & 0xff);
        }

        private void SetCaliInfoArray(byte[] cali_info)
        {
            Int16 tmpdata;
            cali_info[CALIINFO_FLAG] = 0x55;
            cali_info[CALIINFO_VER] = 0x01;  //version 1
            UInt32 cali_time_t = (UInt32)(cali_time.Subtract(new DateTime(1970, 1, 1)).TotalSeconds);
            cali_info[CALIINFO_DATE] = (byte)(cali_time_t);
            cali_info[CALIINFO_DATE + 1] = (byte)(cali_time_t >> 8);
            cali_info[CALIINFO_DATE + 2] = (byte)(cali_time_t >> 16);
            cali_info[CALIINFO_DATE + 3] = (byte)(cali_time_t >> 24);
            tmpdata = (Int16)(cali_aver_err * 100);
            cali_info[CALIINFO_AVER_ERR] = (byte)tmpdata;
            cali_info[CALIINFO_AVER_ERR + 1] = (byte)(tmpdata >> 8);
            tmpdata = (Int16)(cali_err_stddev * 100);
            cali_info[CALIINFO_ERR_VAR] = (byte)tmpdata;
            cali_info[CALIINFO_ERR_VAR + 1] = (byte)(tmpdata >> 8);
            tmpdata = (Int16)(cali_max_err * 100);
            cali_info[CALIINFO_MAX_ERR] = (byte)tmpdata;
            cali_info[CALIINFO_MAX_ERR + 1] = (byte)(tmpdata >> 8);
            tmpdata = (Int16)(cali_dip * 100);
            cali_info[CALIINFO_DIP] = (byte)tmpdata;
            cali_info[CALIINFO_DIP + 1] = (byte)(tmpdata >> 8);
        }

        private void SetCoeffArray( byte[] coeff, Vector bG, Vector bM, Matrix aG, Matrix aM)
        {
            double FV = GLOBAL.FV;
            double FM = GLOBAL.FM;
            PutCoeff(coeff, 0, bG.x * FV );
            PutCoeff(coeff, 2, aG.x.x * FM );
            PutCoeff(coeff, 4, aG.x.y * FM );
            PutCoeff(coeff, 6, aG.x.z * FM );
            PutCoeff(coeff, 8, bG.y * FV );
            PutCoeff(coeff, 10, aG.y.x * FM );
            PutCoeff(coeff, 12, aG.y.y * FM );
            PutCoeff(coeff, 14, aG.y.z * FM );
            PutCoeff(coeff, 16, bG.z * FV );
            PutCoeff(coeff, 18, aG.z.x * FM );
            PutCoeff(coeff, 20, aG.z.y * FM );
            PutCoeff(coeff, 22, aG.z.z * FM );

            PutCoeff(coeff, 24, bM.x * FV );
            PutCoeff(coeff, 26, aM.x.x * FM );
            PutCoeff(coeff, 28, aM.x.y * FM );
            PutCoeff(coeff, 30, aM.x.z * FM );
            PutCoeff(coeff, 32, bM.y * FV );
            PutCoeff(coeff, 34, aM.y.x * FM );
            PutCoeff(coeff, 36, aM.y.y * FM );
            PutCoeff(coeff, 38, aM.y.z * FM );
            PutCoeff(coeff, 40, bM.z * FV );
            PutCoeff(coeff, 42, aM.z.x * FM );
            PutCoeff(coeff, 44, aM.z.y * FM );
            PutCoeff(coeff, 46, aM.z.z * FM );
        }

        private void ParseCoeffs(byte[] coeff, ref Vector bG, ref Vector bM, ref Matrix aG, ref Matrix aM)
        {
            int ptr = 0;
            double v = 0;
            double FV = GLOBAL.FV;
            double FM = GLOBAL.FM;
            GetCoeff(coeff, 0, ref v); bG.x = v / FV;
            GetCoeff(coeff, 2, ref v); aG.x.x = v / FM;
            GetCoeff(coeff , 4, ref v); aG.x.y = v / FM;
            GetCoeff(coeff , 6, ref v); aG.x.z = v / FM;
            GetCoeff(coeff , 8, ref v); bG.y = v / FV;
            GetCoeff(coeff , 10, ref v); aG.y.x = v / FM;
            GetCoeff(coeff , 12, ref v); aG.y.y = v / FM;
            GetCoeff(coeff , 14, ref v); aG.y.z = v / FM;
            GetCoeff(coeff , 16, ref v); bG.z = v / FV;
            GetCoeff(coeff , 18, ref v); aG.z.x = v / FM;
            GetCoeff(coeff , 20, ref v); aG.z.y = v / FM;
            GetCoeff(coeff , 22, ref v); aG.z.z = v / FM;

            GetCoeff(coeff , 24, ref v); bM.x = v / FV;
            GetCoeff(coeff , 26, ref v); aM.x.x = v / FM;
            GetCoeff(coeff , 28, ref v); aM.x.y = v / FM;
            GetCoeff(coeff , 30, ref v); aM.x.z = v / FM;
            GetCoeff(coeff , 32, ref v); bM.y = v / FV;
            GetCoeff(coeff , 34, ref v); aM.y.x = v / FM;
            GetCoeff(coeff , 36, ref v); aM.y.y = v / FM;
            GetCoeff(coeff , 38, ref v); aM.y.z = v / FM;
            GetCoeff(coeff , 40, ref v); bM.z = v / FV;
            GetCoeff(coeff , 42, ref v); aM.z.x = v / FM;
            GetCoeff(coeff , 44, ref v); aM.z.y = v / FM;
            GetCoeff(coeff , 46, ref v); aM.z.z = v / FM;
        }

        private void btnSaveCoeffs_Click(object sender, EventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Title = "Please choose the path to save coeffs";
            saveFileDialog.Filter = "cavway coeff file|*.coe";
            if (saveFileDialog.ShowDialog() != DialogResult.OK) return;
            string path = saveFileDialog.FileName;//获得保存文件的路径
            log csv = new log(path);
            csv.ClearFile();
            csv.Write(bG1.x.ToString() + "," + bG1.y.ToString() + "," + bG1.z.ToString());
            csv.Write(bM1.x.ToString() + "," + bM1.y.ToString() + "," + bM1.z.ToString());
            csv.Write(aG1.x.x.ToString() + "," + aG1.x.y.ToString() + "," + aG1.x.z.ToString());
            csv.Write(aG1.y.x.ToString() + "," + aG1.y.y.ToString() + "," + aG1.y.z.ToString());
            csv.Write(aG1.z.x.ToString() + "," + aG1.z.y.ToString() + "," + aG1.z.z.ToString());
            csv.Write(aM1.x.x.ToString() + "," + aM1.x.y.ToString() + "," + aM1.x.z.ToString());
            csv.Write(aM1.y.x.ToString() + "," + aM1.y.y.ToString() + "," + aM1.y.z.ToString());
            csv.Write(aM1.z.x.ToString() + "," + aM1.z.y.ToString() + "," + aM1.z.z.ToString());

            csv.Write(bG2.x.ToString() + "," + bG2.y.ToString() + "," + bG2.z.ToString());
            csv.Write(bM2.x.ToString() + "," + bM2.y.ToString() + "," + bM2.z.ToString());
            csv.Write(aG2.x.x.ToString() + "," + aG2.x.y.ToString() + "," + aG2.x.z.ToString());
            csv.Write(aG2.y.x.ToString() + "," + aG2.y.y.ToString() + "," + aG2.y.z.ToString());
            csv.Write(aG2.z.x.ToString() + "," + aG2.z.y.ToString() + "," + aG2.z.z.ToString());
            csv.Write(aM2.x.x.ToString() + "," + aM2.x.y.ToString() + "," + aM2.x.z.ToString());
            csv.Write(aM2.y.x.ToString() + "," + aM2.y.y.ToString() + "," + aM2.y.z.ToString());
            csv.Write(aM2.z.x.ToString() + "," + aM2.z.y.ToString() + "," + aM2.z.z.ToString());
            if (bIsHasCaliInfo)
            {
                csv.Write(cali_time.ToString("yyyy-MM-dd HH:mm:ss"));
                csv.Write(cali_aver_err.ToString());
                csv.Write(cali_err_stddev.ToString());
                csv.Write(cali_max_err.ToString());
                csv.Write(cali_dip.ToString());
            }

            MessageBox.Show("Write Cavway coeffs file successful");

        }

        private void btnLoadCoeffs_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Title = "Please choose the proper coeff file";//设置对话框标题
            openFileDialog.Multiselect = false;  //设置对话框可以多选
            //openFileDialog.InitialDirectory = @"C:\Users\user1\Desktop\研发报告\basler拍照研究报告";//设置对话框的初始目录
            //设置对话框的文件类型
            openFileDialog.Filter = "cavway coeff file|*.coe";
            if (openFileDialog.ShowDialog() != DialogResult.OK) return;
            string path = openFileDialog.FileName;//获取选中的路径
            log csv = new log(path);
            List<String> coefflist = new List<string>();
            csv.ReadFile(ref coefflist);
            string[] split = coefflist[0].Split(',');
            bG1.x = double.Parse(split[0]); bG1.y = double.Parse(split[1]); bG1.z = double.Parse(split[2]);
            split = coefflist[1].Split(',');
            bM1.x = double.Parse(split[0]); bM1.y = double.Parse(split[1]); bM1.z = double.Parse(split[2]);
            split = coefflist[2].Split(',');
            aG1.x.x = double.Parse(split[0]); aG1.x.y = double.Parse(split[1]); aG1.x.z = double.Parse(split[2]);
            split = coefflist[3].Split(',');
            aG1.y.x = double.Parse(split[0]); aG1.y.y = double.Parse(split[1]); aG1.y.z = double.Parse(split[2]);
            split = coefflist[4].Split(',');
            aG1.z.x = double.Parse(split[0]); aG1.z.y = double.Parse(split[1]); aG1.z.z = double.Parse(split[2]);
            split = coefflist[5].Split(',');
            aM1.x.x = double.Parse(split[0]); aM1.x.y = double.Parse(split[1]); aM1.x.z = double.Parse(split[2]);
            split = coefflist[6].Split(',');
            aM1.y.x = double.Parse(split[0]); aM1.y.y = double.Parse(split[1]); aM1.y.z = double.Parse(split[2]);
            split = coefflist[7].Split(',');
            aM1.z.x = double.Parse(split[0]); aM1.z.y = double.Parse(split[1]); aM1.z.z = double.Parse(split[2]);

            split = coefflist[8].Split(',');
            bG2.x = double.Parse(split[0]); bG2.y = double.Parse(split[1]); bG2.z = double.Parse(split[2]);
            split = coefflist[9].Split(',');
            bM2.x = double.Parse(split[0]); bM2.y = double.Parse(split[1]); bM2.z = double.Parse(split[2]);
            split = coefflist[10].Split(',');
            aG2.x.x = double.Parse(split[0]); aG2.x.y = double.Parse(split[1]); aG2.x.z = double.Parse(split[2]);
            split = coefflist[11].Split(',');
            aG2.y.x = double.Parse(split[0]); aG2.y.y = double.Parse(split[1]); aG2.y.z = double.Parse(split[2]);
            split = coefflist[12].Split(',');
            aG2.z.x = double.Parse(split[0]); aG2.z.y = double.Parse(split[1]); aG2.z.z = double.Parse(split[2]);
            split = coefflist[13].Split(',');
            aM2.x.x = double.Parse(split[0]); aM2.x.y = double.Parse(split[1]); aM2.x.z = double.Parse(split[2]);
            split = coefflist[14].Split(',');
            aM2.y.x = double.Parse(split[0]); aM2.y.y = double.Parse(split[1]); aM2.y.z = double.Parse(split[2]);
            split = coefflist[15].Split(',');
            aM2.z.x = double.Parse(split[0]); aM2.z.y = double.Parse(split[1]); aM2.z.z = double.Parse(split[2]);

            if (coefflist.Count == 21)
            {
                bIsHasCaliInfo = true;
                DateTime.TryParse(coefflist[16], out cali_time);
                float.TryParse(coefflist[17], out cali_aver_err);
                float.TryParse(coefflist[18], out cali_err_stddev);
                float.TryParse(coefflist[19], out cali_max_err);
                float.TryParse(coefflist[20], out cali_dip);
            }
            else bIsHasCaliInfo = false;

            ShowCeofff();
            btnUploadCoeffs.Enabled = true;
        }

        private void btnUploadCoeffs_Click(object sender, EventArgs e)
        {
            byte[] coeff1 = new byte[48];
            byte[] coeff2 = new byte[48];
            byte[] coeff = new byte[128];
            byte[] cali_info = new byte[16];
            if (bIsHasCaliInfo)
            {           
                SetCaliInfoArray(cali_info);
            }
            else
            {
                for (int i = 0; i < 16; i++) cali_info[i] = 0xFF;
            }
            if (!UART.writeMemory(0x9000 + 64, cali_info, 16))
            {
                MessageBox.Show("Upload coeff params failed!");
                return;
            }

            SetCoeffArray(coeff1, bG1, bM1, aG1, aM1);
            SetCoeffArray(coeff2, bG2, bM2, aG2, aM2);
            for (int i = 0; i < 128; i++) coeff[i] = 0xff;
            Array.Copy(coeff1, 0, coeff, 0, 48);
            Array.Copy(coeff2, 0, coeff, 64, 48);
            if (UART.writeMemory(0x9000 + 128, coeff, 128))
                MessageBox.Show("Upload coeff params successful!");
            else MessageBox.Show("Upload coeff params failed!");
        }

        private void FrmCali_Load(object sender, EventArgs e)
        {

        }
    }
}
