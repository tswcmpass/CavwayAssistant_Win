using System;
using System.Collections;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CavwayAssist
{
    enum Command
    {
        EnterCali = 0x30,
        QuitCali = 0x31,
        LaserOn = 0x36,
        LaserOff = 0x37,
        LaserTrig = 0x38
    }

    

    class UART
    {
        public const int HEADER_DATA_PACKET = 0x01;
        public const int HEADER_CALI_PACKET = 0x02;
        public const int HEADER_MEM_READ_REPLY_PACKET = 0x3d;

        private static SerialPort port = new SerialPort();
        public struct PORT_INFO
        {
            public string port_name;
            public string description;
            public string hardware_id;
        }
        private const short INVALID_HANDLE_VALUE = -1;
        public const uint DIGCF_DEFAULT = 0x00000001;  // only valid with DIGCF_DEVICEINTERFACE
        public const uint DIGCF_PRESENT = 0x00000002;
        public const uint DIGCF_ALLCLASSES = 0x00000004;
        public const uint DIGCF_PROFILE = 0x00000008;
        public const uint DIGCF_DEVICEINTERFACE = 0x00000010;
        private const int MAX_DEV_LEN = 256;//返回值最大长度
        private const int SPDRP_FRIENDLYNAME = (0x0000000C);// FriendlyName (R/W)
        private const int SPDRP_DEVICEDESC = (0x00000000);// DeviceDesc (R/W)
        private const int SPDRP_HARDWAREID = (0x00000001);// HardwareID (R/W)
        private const int DICS_FLAG_GLOBAL = 0x00000001;
        public struct SP_DEVINFO_DATA
        {
            public uint cbSize;
            public Guid ClassGuid;
            public uint DevInst;
            public IntPtr Reserved;
        }
        [DllImport("SetupAPI.dll")]
        public static extern bool SetupDiEnumDeviceInfo(
            IntPtr DeviceInfoSet,
            uint MemberIndex,
            ref SP_DEVINFO_DATA DeviceInfoData
        );
        [DllImport("SetupAPI.dll")]
        public static extern IntPtr SetupDiGetClassDevs(
            ref Guid ClassGuid,
            uint Enumerator,
            IntPtr hwndParent,
            uint Flags
        );
        [DllImport("SetupAPI.dll")]
        private static extern bool SetupDiDestroyDeviceInfoList(IntPtr DeviceInfoSet);
        [DllImport("setupapi.dll")]
        private static extern IntPtr SetupDiOpenDevRegKey(
            IntPtr DeviceInfoSet,
            ref SP_DEVINFO_DATA DeviceInfoData,
            uint Scope,
            uint HwProfile,
            uint KeyType,
            uint samDesired
            );
        [DllImport("setupapi.dll")]
        private static extern bool SetupDiGetDeviceRegistryPropertyW(
            IntPtr DeviceInfoSet,
            ref SP_DEVINFO_DATA DeviceInfoData,
            uint Property,
            ref uint PropertyRegDataType,
            byte[] PropertyBuffer,
            uint PropertyBufferSize,
            IntPtr RequiredSize
            );
        [DllImport("ADVAPI32.dll", CharSet = CharSet.Unicode, BestFitMapping = false)]
        internal static extern int RegQueryValueEx(IntPtr hKey, string lpValueName,
                    int[] lpReserved, ref int lpType, [Out] byte[] lpData,
                    ref int lpcbData);

        [DllImport("ADVAPI32.dll", SetLastError = false)]
        internal static extern int RegCloseKey(IntPtr handle);

        public static List<PORT_INFO> list_ports()
        {
            Guid GUID_DEVCLASS_PORTS = new Guid("4d36e978-e325-11ce-bfc1-08002be10318");
            Guid GUID_DEVCLASS_MODEM = new Guid("4d36e96d-e325-11ce-bfc1-08002be10318");
            Guid GUID_DEVINTERFACE_COMPORT = new Guid("86E0D1E0-8089-11D0-9CE4-08003E301F73");
            Guid GUID_DEVINTERFACE_MODEM = new Guid("2c7089aa-2e0e-11d1-b114-00c04fc2aae4");
            List<PORT_INFO> ports = new List<PORT_INFO>();
            Guid[] ClassGuid = new Guid[4];
            ClassGuid[0] = GUID_DEVCLASS_PORTS;
            ClassGuid[1] = GUID_DEVCLASS_MODEM;
            ClassGuid[2] = GUID_DEVINTERFACE_COMPORT;
            ClassGuid[3] = GUID_DEVINTERFACE_MODEM;
            uint[] dwFlag = new uint[4];
            dwFlag[0] = DIGCF_PRESENT;
            dwFlag[1] = DIGCF_PRESENT;
            dwFlag[2] = DIGCF_PRESENT | DIGCF_DEVICEINTERFACE;
            dwFlag[3] = DIGCF_PRESENT | DIGCF_DEVICEINTERFACE;
            List<string> temp_portname_list = new List<string>();//用于判断是否有重复的
            for (int index = 0; index < ClassGuid.Length; index++)
            {
                IntPtr hDevInfo = (IntPtr)INVALID_HANDLE_VALUE;
                //函数返回一个包含本机上所有被请求的设备信息的设备信息集句柄
                hDevInfo = SetupDiGetClassDevs(ref ClassGuid[index], 0, IntPtr.Zero, dwFlag[index]);
                if (hDevInfo == (IntPtr)INVALID_HANDLE_VALUE)
                {
                    return ports;
                }
                SP_DEVINFO_DATA DeviceInfoData = new SP_DEVINFO_DATA();
                DeviceInfoData.cbSize = (uint)Marshal.SizeOf(new SP_DEVINFO_DATA());
                //在64位系统上cbSize 是32 32位系统上是28，如果不对则会读取出错
                DeviceInfoData.DevInst = 0;
                DeviceInfoData.ClassGuid = Guid.Empty;
                DeviceInfoData.Reserved = IntPtr.Zero;
                //枚举设备信息
                uint i = 0;
                while (SetupDiEnumDeviceInfo(hDevInfo, i++, ref DeviceInfoData))
                {
                    //Get port name
                    const uint DIREG_DEV = 0x00000001;
                    const uint KEY_READ = 0x00020019;
                    //打开枚举成功的设备注册表句柄hkey，并获取该设备对应的相关信息 DeviceInfoData
                    IntPtr hkey = SetupDiOpenDevRegKey(
                        hDevInfo,
                        ref DeviceInfoData,
                        DICS_FLAG_GLOBAL,
                        0,
                        DIREG_DEV,
                        KEY_READ);
                    //根据句柄 和关键词获取 portName
                    string portName = GetStringValue(hkey, "PortName");
                    //关闭注册表句柄
                    RegCloseKey(hkey);
                    if (portName.Length == 0 || portName.ToString().Contains("LPT")) continue;//并口跳过
                    if (temp_portname_list.IndexOf(portName) >= 0) continue;//重复的跳过
                    temp_portname_list.Add(portName);
                    // Get port friendly name 从DeviceInfoData中获取DeviceName信息
                    byte[] DeviceName = new byte[MAX_DEV_LEN];
                    uint PropertyRegDataType = 0;
                    if (!SetupDiGetDeviceRegistryPropertyW(hDevInfo, ref DeviceInfoData, SPDRP_FRIENDLYNAME,
                        ref PropertyRegDataType, DeviceName, MAX_DEV_LEN, IntPtr.Zero))
                    {
                        DeviceName = new byte[MAX_DEV_LEN];
                    }
                    // Get DeviceDsc 从DeviceInfoData中获取DeviceDsc信息
                    byte[] DeviceDsc = new byte[MAX_DEV_LEN];
                    if (!SetupDiGetDeviceRegistryPropertyW(hDevInfo, ref DeviceInfoData, SPDRP_DEVICEDESC,
                        ref PropertyRegDataType, DeviceDsc, MAX_DEV_LEN, IntPtr.Zero))
                    {
                        DeviceDsc = new byte[MAX_DEV_LEN];
                    }
                    // Get hardware ID 从DeviceInfoData中获取 hardware ID 信息
                    byte[] hardwareID = new byte[MAX_DEV_LEN];
                    if (!SetupDiGetDeviceRegistryPropertyW(hDevInfo, ref DeviceInfoData, SPDRP_HARDWAREID,
                        ref PropertyRegDataType, hardwareID, MAX_DEV_LEN, IntPtr.Zero))
                    {
                        hardwareID = new byte[MAX_DEV_LEN];
                    }
                    PORT_INFO temp_port;
                    temp_port.port_name = portName.Split('\0').Length > 0 ? portName.Split('\0')[0] : "";
                    var str_arry = Encoding.Unicode.GetString(DeviceDsc).Split('\0');
                    temp_port.description = str_arry.Length > 0 ? str_arry[0] : "";
                    str_arry = Encoding.Unicode.GetString(hardwareID).Split('\0');
                    temp_port.hardware_id = str_arry.Length > 0 ? str_arry[0] : "";
                    ports.Add(temp_port);
                }
                SetupDiDestroyDeviceInfoList(hDevInfo);//
            }
            return ports;
        }


        public static string GetStringValue(IntPtr hkey, string valName)
        {
            int type = 0;
            int datasize = 0;
            int ret = RegQueryValueEx(hkey, valName, null, ref type, null, ref datasize);
            if (ret == 0)
                if (type == 1)
                {
                    byte[] blob = new byte[datasize];
                    ret = RegQueryValueEx(hkey, valName, null, ref type, blob, ref datasize);
                    UnicodeEncoding unicode = new UnicodeEncoding();
                    return unicode.GetString(blob);
                }
            return null;
        }


        /// <summary>
        /// 根据给定的PID VID 搜寻 对应的串口
        /// </summary>
        /// <param name="pid"></param> PID 注意进制，如 0x5656
        /// <param name="vid"></param> VID 注意进制，如 0x5656
        /// <returns></returns> 返回的是符合 PID 和VID 的串口名称列表
        public static string[] GetCOMFromPIDVID(int pid, int vid)
        {
            List<PORT_INFO> COM_ports = list_ports();
            string pid_s = pid.ToString("X4");
            string vid_s = vid.ToString("X4");
            ArrayList list = new ArrayList();
            foreach (var port in COM_ports)
            {
                if (port.hardware_id.Contains(pid_s) && port.hardware_id.Contains(vid_s))
                {
                    list.Add(port.port_name);
                }
            }
            return (string[])list.ToArray(typeof(string));
        }

        public static Boolean connect()
        {
            string[] strCOM = GetCOMFromPIDVID(0x55d3, 0x1a86);
            if (strCOM.Length == 0) return false;
            //port = new SerialPort();
            port.PortName = strCOM[0];
            port.BaudRate = 115200;
            port.Parity = Parity.None;
            port.DataBits = 8;
            port.StopBits = StopBits.One;
            try {
                port.Open();
            }catch(Exception e)
            {
                return false;
            }
            if (port.IsOpen) return true;
            else return false;
        }

        //len: numbers of byte to read at addr
        public static bool readMemory(int addr, byte[] buffIn, int len)
        {
            byte[] cmd = new byte[4];
            cmd[0] = 0x3d;
            cmd[1] = (byte)(addr & 0xFF);
            cmd[2] = (byte)((addr >> 8) & 0xFF);
            cmd[3] = (byte)(len);
            if (!sendPacket(cmd, 4)) return false;
            Thread.Sleep((int)Math.Max(8 + 2.5 * len, 200));
            if (port.BytesToRead < len + 4)
            {
                return false;
            }
            Byte[] ReceivedData = new Byte[port.BytesToRead];//创建接收字节数组
            port.Read(ReceivedData, 0, port.BytesToRead);//读取所接收到的数据
            int ptr = 0;
            if(ReceivedData[0] == HEADER_DATA_PACKET || ReceivedData[0] == HEADER_CALI_PACKET)
            {
                while(ptr < ReceivedData.Length)
                {
                    if (ptr > 2)
                    {
                        if (ReceivedData[ptr] == HEADER_MEM_READ_REPLY_PACKET && ReceivedData[ptr - 1] == '\n' && ReceivedData[ptr - 2] == '\r')
                        {
                            break;
                        }
                    }
                    ptr++;
                }
                if (ptr == ReceivedData.Length) return false;
            }
            int actual_len = ReceivedData.Length - ptr;
            if (actual_len < len + 4) return false;
            if (ReceivedData[ptr] != 0x3d) return false;
            if (ReceivedData[ptr + 1] != cmd[1] || ReceivedData[ptr + 2] != cmd[2]) return false;
            for(int i = 0; i < len; i++)
            {
                buffIn[i] = ReceivedData[ptr + i + 4];
            }
            return true;
        }

        //len: numbers of byte to write at addr
        public static bool writeMemory(int addr, byte[] buffOut, int len)
        {
            byte[] buffIn = new byte[len];
            byte[] cmd = new byte[len+4];
            cmd[0] = 0x3e;
            cmd[1] = (byte)(addr & 0xFF);
            cmd[2] = (byte)((addr >> 8) & 0xFF);
            cmd[3] = (byte)(len);
            for (int i = 0; i < len; i++)
                cmd[i + 4] = buffOut[i];
            if (!sendPacket(cmd, len + 4))
                return false;
            Thread.Sleep((int)Math.Max(8 + 2.5 * (len + 4), 200));
            if (port.BytesToRead < len + 4)
            {
                return false;
            }
            Byte[] ReceivedData = new Byte[port.BytesToRead];//创建接收字节数组
            port.Read(ReceivedData, 0, port.BytesToRead);//读取所接收到的数据
            int ptr = 0;
            if (ReceivedData[0] == HEADER_DATA_PACKET || ReceivedData[0] == HEADER_CALI_PACKET)
            {
                while (ptr < ReceivedData.Length)
                {
                    if (ptr > 2)
                    {
                        if (ReceivedData[ptr] == HEADER_MEM_READ_REPLY_PACKET && ReceivedData[ptr - 1] == '\n' && ReceivedData[ptr - 2] == '\r')
                        {
                            break;
                        }
                    }
                    ptr++;
                }
                if (ptr == ReceivedData.Length)
                    return false;
            }
            int actual_len = ReceivedData.Length - ptr;
            if (actual_len < len + 4)
                return false;
            if (ReceivedData[ptr] != 0x3d)
                return false;
            if (ReceivedData[ptr + 1] != cmd[1] || ReceivedData[ptr + 2] != cmd[2])
                return false;
            for (int i = 0; i < len; i++)
            {
                buffIn[i] = ReceivedData[ptr + i + 4];
            }
            for(int i = 0; i < len; i++)  //compare buffin and buffout
            {
                if (buffIn[i] != buffOut[i])
                    return false;
            }
            return true;
        }

        //time_out: max millisecondes
        private static int waitResp(int time_out, int bytestoread)
        {
            int cnt = 0;
            while (port.BytesToRead != bytestoread)
            {
                Thread.Sleep(10);
                cnt++;
                if (cnt > time_out / 10)
                {
                    port.DiscardInBuffer();
                    return -1;
                }
            }
            return cnt * 10;
        }

        //send FW upgrade begin command before sending data packet
        public static bool sendFWUpdateStart()
        {
            byte[] cmd = new byte[1];
            byte[] buff = new byte[2];
            cmd[0] = 0x4B;
            sendPacket(cmd, 1);
            if (waitResp(3000, 2) == -1) return false; //time out
            port.Read(buff, 0, 2);
            if (buff[0] != 0x4B || buff[1] != 0x01) return false;
            return true;
        }

        //send FW upgrade end command after sending data packet
        //checksum: the sum of all packet's crc
        public static bool sendFWUpdateEnd(UInt32 checksum)
        {
            byte[] cmd = new byte[5];
            byte[] buff = new byte[2];
            cmd[0] = 0x4D;
            cmd[1] = (byte)(checksum);
            cmd[2] = (byte)(checksum >> 8);
            cmd[3] = (byte)(checksum >> 16);
            cmd[4] = (byte)(checksum >> 24);
            sendPacket(cmd, 5);
            if (waitResp(8000, 2) == -1) return false; //time out
            port.Read(buff, 0, 2);
            if (buff[0] != 0x4D || buff[1] != 0x00) return false;
            return true;
        }

        private static uint crc16_modbus(byte[] modbusdata, uint Length)//Length为modbusdata的长度
        {
            uint i, j;
            uint crc16 = 0xFFFF;
            for (i = 0; i < Length; i++)
            {
                crc16 ^= modbusdata[i]; // CRC = BYTE xor CRC  
                for (j = 0; j < 8; j++)
                {
                    if ((crc16 & 0x01) == 1) //如果CRC最后一位为1右移一位后carry=1则将CRC右移一位后再与POLY16=0xA001进行xor运算 
                        crc16 = (crc16 >> 1) ^ 0xA001;
                    else                     //如果CRC最后一位为0则只将CRC右移一位 
                        crc16 = crc16 >> 1;
                }
            }
            return crc16;
        }

        //Send data packet, command code 0x4C
        //data: 128 bytes length buffer
        //crc: crc calculated of this packet
        public static bool sendFWPacket(int packet_idx, byte[] data, ref uint crc)
        {
            byte[] buff = new byte[133];
            byte[] inbuff = new byte[6];
            crc = crc16_modbus(data, 128);
            buff[0] = 0x4C;
            buff[1] = (byte)(packet_idx & 0xFF);
            buff[2] = (byte)(packet_idx >> 8);
            for(int i = 0;i < 128; i++)
            {
                buff[i + 3] = data[i];
            }
            buff[131] = (byte)(crc & 0xFF);
            buff[132] = (byte)(crc >> 8); 
            if(!sendPacket(buff, 133)) return false;
            if (waitResp(500, 6) == -1) return false; //time out
            port.Read(inbuff, 0, 6);
            int crc_return = inbuff[5] << 8 | inbuff[4];
            if (crc == crc_return && inbuff[3] == 0x00)
                return true;
            else return false;
        }

        //send data by UART
        public static bool sendPacket(byte[] payload, int len)
        {
            if (!port.IsOpen) return false;
            byte[] sendbuf = new byte[len + 8];
            sendbuf[0] = (byte)'d';
            sendbuf[1] = (byte)'a';
            sendbuf[2] = (byte)'t';
            sendbuf[3] = (byte)'a';
            sendbuf[4] = (byte)':';
            sendbuf[5] = (byte)payload.Length;
            int i = 0;
            for (i = 0; i < len; i++)
            {
                sendbuf[i + 6] = payload[i];
            }
            sendbuf[i + 6] = (byte)'\r';
            sendbuf[i + 7] = (byte)'\n';
            try
            {
                port.DiscardInBuffer();
                port.Write(sendbuf, 0, sendbuf.Length);
            }catch(Exception e)
            {

            }
            return true;
        }

        public static bool sendCommand(Command cmd)
        {
            byte[] buf = new byte[1];
            buf[0] = (byte)cmd;
            return sendPacket(buf, 1);
        }

        public static Boolean isConnected()
        {
            return port.IsOpen;
        }

        public static void Disconnect()
        {
            port.Close();
        }

        public static void DownloadCoeffs()
        {

        }
    }
}
