using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CavwayAssist
{
    class log
    {
        private string logpath;
        private string logtext;

        public log(string LogPath)
        {
            logpath = LogPath;
            logtext = "";
        }

        public bool Write(string LogText)
        {
            bool success = false;
            logtext = LogText;

            FileStream fs = null;
            StreamWriter sw = null;
            try
            {
                fs = File.Open(logpath, FileMode.Append);
                sw = new StreamWriter(fs);
                sw.WriteLine(logtext);

                success = true;
            }
            catch (Exception)
            {
                success = false;
            }
            finally
            {
                if (sw != null)
                {
                    sw.Close();
                }
                if (fs != null)
                {
                    fs.Close();
                }
            }

            return success;
        }

        public bool ReadFile(ref List<string> loglist)
        {
            FileStream fs = null;
            StreamReader sr = null;

            bool success = false;

            loglist.Clear();
            try
            {
                fs = File.OpenRead(logpath);
                sr = new StreamReader(fs);

                string strline;
                while ((strline = sr.ReadLine()) != null)
                {
                    loglist.Add(strline);
                }
                success = true;
            }
            catch (Exception)
            {
                success = false;
            }
            finally
            {
                if (sr != null)
                    sr.Close();
                if (fs != null)
                    fs.Close();
            }

            return success;
        }

        public void ClearFile()
        {
            FileStream fs = File.Open(logpath, FileMode.Create);
            fs.Close();
        }
    }
}
