using System;
using System.Collections;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Management;
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
            List<PORT_INFO> ports = new List<PORT_INFO>();
            try
            {
                string[] queries = new string[]
                {
                    "SELECT * FROM Win32_SerialPort",
                    "SELECT * FROM Win32_PnPEntity WHERE Name LIKE '%(COM%'"
                };

                foreach (string query in queries)
                {
                    using (var searcher = new ManagementObjectSearcher(query))
                    {
                        foreach (ManagementObject obj in searcher.Get())
                        {
                            string portName = GetPortName(obj);
                            if (string.IsNullOrEmpty(portName) || portName.ToUpper().Contains("LPT"))
                                continue;

                            string description = obj["Description"]?.ToString();
                            if (string.IsNullOrEmpty(description))
                                description = obj["Caption"]?.ToString() ?? obj["Name"]?.ToString() ?? string.Empty;

                            string hardwareId = GetHardwareId(obj);

                            if (ports.Any(p => string.Equals(p.port_name, portName, StringComparison.OrdinalIgnoreCase)))
                                continue;

                            ports.Add(new PORT_INFO
                            {
                                port_name = portName,
                                description = description,
                                hardware_id = hardwareId
                            });
                        }
                    }
                }
            }
            catch
            {
                // WMI 查询失败时，不抛异常，返回空列表
            }
            return ports;
        }

        private static string GetPortName(ManagementObject obj)
        {
            string deviceId = obj["DeviceID"]?.ToString();
            if (!string.IsNullOrEmpty(deviceId) && deviceId.StartsWith("COM", StringComparison.OrdinalIgnoreCase))
                return deviceId;

            string name = obj["Name"]?.ToString();
            if (!string.IsNullOrEmpty(name))
            {
                int start = name.LastIndexOf("(COM", StringComparison.OrdinalIgnoreCase);
                if (start >= 0)
                {
                    int end = name.IndexOf(')', start);
                    if (end > start)
                        return name.Substring(start + 1, end - start - 1);
                }
            }

            if (!string.IsNullOrEmpty(deviceId) && deviceId.ToUpperInvariant().Contains("COM"))
            {
                int idx = deviceId.ToUpperInvariant().IndexOf("COM");
                if (idx >= 0)
                {
                    int end = deviceId.IndexOf('&', idx);
                    if (end < 0) end = deviceId.Length;
                    return deviceId.Substring(idx, end - idx);
                }
            }

            return string.Empty;
        }

        private static string GetHardwareId(ManagementObject obj)
        {
            object pnpIdObj = obj["PNPDeviceID"];
            if (pnpIdObj != null)
                return pnpIdObj.ToString();

            object hardwareIdObj = obj["HardwareID"];
            string[] hardwareIds = hardwareIdObj as string[];
            if (hardwareIds != null && hardwareIds.Length > 0)
                return string.Join(";", hardwareIds);

            string hardwareId = hardwareIdObj as string;
            if (!string.IsNullOrEmpty(hardwareId))
                return hardwareId;

            return string.Empty;
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
            string vidToken = "VID_" + vid.ToString("X4");
            string pidToken = "PID_" + pid.ToString("X4");
            ArrayList list = new ArrayList();
            foreach (var port in COM_ports)
            {
                string hwid = port.hardware_id.ToUpperInvariant();
                if (hwid.Contains(vidToken) && hwid.Contains(pidToken))
                {
                    list.Add(port.port_name);
                }
            }
            return (string[])list.ToArray(typeof(string));
        }

        public static Boolean connect()
        {
            string[] strCOM = GetCOMFromPIDVID(0x55d3, 0x1a86);
            if (strCOM.Length == 0)
            {
                string[] allPorts = SerialPort.GetPortNames();
                if (allPorts.Length == 1)
                {
                    strCOM = allPorts;
                }
                else
                {
                    var ports = list_ports();
                    foreach (var p in ports)
                    {
                        if (!string.IsNullOrEmpty(p.description) &&
                            (p.description.IndexOf("CH343", StringComparison.OrdinalIgnoreCase) >= 0 ||
                             p.description.IndexOf("USB-SERIAL", StringComparison.OrdinalIgnoreCase) >= 0 ||
                             p.description.IndexOf("USB Serial", StringComparison.OrdinalIgnoreCase) >= 0 ||
                             p.description.IndexOf("USB 到 串口", StringComparison.OrdinalIgnoreCase) >= 0))
                        {
                            strCOM = new string[] { p.port_name };
                            break;
                        }
                    }
                }
            }

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

        public static int readSerial()
        {
            byte[] serialbuff = new byte[4] { 0, 0, 0, 0 };
            UART.readMemory(0x8008, serialbuff, 4);
            int serial = serialbuff[0] | (serialbuff[1] << 8);
            return serial;
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
