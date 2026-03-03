using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CavwayAssist
{
    class RAW
    {
        public int X = 0;
        public int Y = 0;
        public int Z = 0;
    }
    class Shot
    {
        public const int G_SCALE = 667;
        public const int M_SCALE = 4876;

        public bool isValid = true;
        public DateTime dtShotTime = new DateTime();
        public byte Flgshot = 0xFF;
        public byte flags = 0xFF;
        public bool isLeg = false;
        public bool isCali = false;
        public float distance = 0;
        public float azimuth = 0;
        public float inclination = 0;
        public float roll = 0;
        public float absG = 1;
        public float absM = 0;
        public float dip = 0;
        public RAW[] RawG = new RAW[2];
        public RAW[] RawM = new RAW[2];
        public byte[] errorbytes = new byte[9];
        public string errorinfo = "";

        private const float ABSSCALE = 10000;
        private const float ANGLESCALE = (float)(360.0 / 0xFFFF);

        private string parseErrInfo(byte[] errbytes)
        {
            string res = "";
            //string error_type = "";
            if (errbytes[0] == 0xFF)
            {
                res = "No error";
                return res;
            }
            int err_cnt = 0;
            if (((errbytes[0] >> 7) & 0x1) == 0x00)  //absG error
            {
                res += "absG error:";
                res += " absG1:" + ((errbytes[4 * err_cnt + 2] << 8 | errbytes[4 * err_cnt + 1]) / ABSSCALE).ToString("F4");
                res += " ";
                res += " absG2:" + ((errbytes[4 * err_cnt + 4] << 8 | errbytes[4 * err_cnt + 3]) / ABSSCALE).ToString("F4");
                res += " ";
                err_cnt++;
                if (err_cnt == 2) return res;
            }
            if (((errbytes[0] >> 6) & 0x1) == 0x00)  //absM error
            {
                res += "absM error:";
                res += " absM1:" + ((errbytes[4 * err_cnt + 2] << 8 | errbytes[4 * err_cnt + 1]) / ABSSCALE).ToString("F4");
                res += " ";
                res += " absM2:" + ((errbytes[4 * err_cnt + 4] << 8 | errbytes[4 * err_cnt + 3]) / ABSSCALE).ToString("F4");
                res += " ";
                err_cnt++;
                if (err_cnt == 2) return res;
            }
            if (((errbytes[0] >> 5) & 0x1) == 0x00)  //dip error
            {
                res += "dip error:";
                res += " err1:" + ((Int16)(errbytes[4 * err_cnt + 2] << 8 | errbytes[4 * err_cnt + 1]) * ANGLESCALE).ToString("F2");
                res += " ";
                res += " err2:" + ((Int16)(errbytes[4 * err_cnt + 4] << 8 | errbytes[4 * err_cnt + 3]) * ANGLESCALE).ToString("F2");
                res += " ";
                err_cnt++;
                if (err_cnt == 2) return res;
            }
            if (((errbytes[0] >> 4) & 0x1) == 0x00)  //angle error
            {
                res += "angle error:";
                res += ((errbytes[4 * err_cnt + 2] << 8 | errbytes[4 * err_cnt + 1]) * ANGLESCALE).ToString("F2");
                res += " ";
                err_cnt++;
                if (err_cnt == 2) return res;
            }
            return res;
        }
        
        public Shot(byte[] store)
        {
            if (store[0] == 0xFF)
            {
                isValid = false;
                return;
            }
            Flgshot = store[1];
            isLeg = ((Flgshot & 0x80) == 0x00) ? true : false;
            isCali = ((Flgshot & 0x01) == 0x00) ? true : false;
            flags = (byte)((Flgshot >> 1) & 0x07);
            int i_distance = store[2] << 16 | store[4] << 8 | store[3];
            distance = i_distance / 1000.0f;
            int i_azm = (int)(store[6] << 8 | store[5]);
            azimuth = (float)(i_azm * 360 / 65535.0);
            int i_theta = (Int16)(store[8] << 8 | store[7]);
            inclination = (float)(i_theta * 360 / 65535.0);
            absG = (float)(((store[11] | store[12] << 8) * G_SCALE) / 1000.0 / GLOBAL.FM);
            absM = (float)(((store[13] | store[14] << 8) * M_SCALE) / 100.0 / GLOBAL.FM);
            int i_dip = (int)(store[15] | store[16] << 8);
            dip = (float)(i_dip * 360 / 65535.0);
            for(int i = 0; i < 2; i++)
            {
                RawG[i] = new RAW();
                RawM[i] = new RAW();
                RawG[i].X = (Int16)(store[21 + i * 12] | store[22 + i * 12] << 8);
                RawG[i].Y = (Int16)(store[23 + i * 12] | store[24 + i * 12] << 8);
                RawG[i].Z = (Int16)(store[25 + i * 12] | store[26 + i * 12] << 8);
                RawM[i].X = (Int16)(store[27 + i * 12] | store[28 + i * 12] << 8);
                RawM[i].Y = (Int16)(store[29 + i * 12] | store[30 + i * 12] << 8);
                RawM[i].Z = (Int16)(store[31 + i * 12] | store[32 + i * 12] << 8);
            }
            UInt32 time_t = (UInt32)(store[20] << 24 | store[19] << 16 | store[18] << 8 | store[17]);
            DateTime dtRes = TimeZone.CurrentTimeZone.ToUniversalTime(new DateTime(1970, 1, 1, 0, 0, 0));
            dtShotTime = dtRes.AddSeconds(time_t).ToLocalTime();
            for (int i = 0; i < 9; i++)
                errorbytes[i] = store[45 + i];
            errorinfo = parseErrInfo(errorbytes);
        }       
    }
}
