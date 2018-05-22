using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace ConnectLab
{
    public class LogProgram
    {

        public static void WriteLog(string caption, string log)
        {
            WriteLog(caption, log, false);
        }
        public static void WriteLog(string caption, string log, bool notify)
        {

           
            string logFilePath =AppParams.LogFolder+"\\MKV_LOG_" + DateTime.Today.ToString("yyyy_MM_dd") + ".txt";
            if (Directory.Exists(AppParams.LogFolder) == false)
            {
                Directory.CreateDirectory(AppParams.LogFolder);
            } 
            if(File.Exists(logFilePath)==false)
            {
                File.Create(logFilePath);
            }
             StreamWriter sw=null;
            try
            {
               sw = new StreamWriter(logFilePath, true);

                if (notify)
                {
                    sw.WriteLine("***[" + caption + "][" + DateTime.Now.ToString("dd-MM-yyyy hh:mm:ss") + "]:" + log);
                }
                else
                {
                    sw.WriteLine("[" + caption + "][" + DateTime.Now.ToString("dd-MM-yyyy hh:mm:ss") + "]:" + log);
                }
            }
            catch (Exception)
            {

            }

            finally
            {

                if (sw != null)
                {
                
                    sw.Flush();
                    sw.Close();
                    sw.Dispose();
                    sw = null;

                }

            }
        }

        public static void WriteStartProgram()
        {
            LogProgram.WriteLog("Success", "Start program");
        }
        public static void WriteEndProgram()
        {
            LogProgram.WriteLog("Success", "Exit program");
        }

        public static void WriteConnectServerSuccess()
        {
            LogProgram.WriteLog("Success", "Connect to SQL Server");
        }
        public static void WriteConnectServerFail()
        {
            LogProgram.WriteLog("***Fail", "Connect to SQL Server",true);
        }

        public static void WriteConnectAu480Success()
        {
            LogProgram.WriteLog("Success", "Connect to Au480");
        }
        public static void WriteConnectAu480Fail()
        {
            LogProgram.WriteLog("***Fail", "Connect to Au480", true);
        }
        public static void WriteConnectLh500Success()
        {
            LogProgram.WriteLog("Success", "Connect to Lh500");
        }
        public static void WriteConnectLh500Fail()
        {
            LogProgram.WriteLog("***Fail", "Connect to Lh500", true);
        }
        public static void WriteConnectCD1800Success()
        {
            LogProgram.WriteLog("Success", "Connect to Cell-Dyn 1800");
        }
        public static void WriteConnectCD1800Fail()
        {
            LogProgram.WriteLog("***Fail", "Connect to Cell-Dyn 1800",true);
        }

        public static void WriteConnectEBLX200Success()
        {
            LogProgram.WriteLog("Success", "Connect to ERBA LX 200");
        }
        public static void WriteConnectEBLX200Fail()
        {
            LogProgram.WriteLog("***Fail", "Connect to ERBA LX 200", true);
        }
        public static void WriteConnectStart4Success()
        {
            LogProgram.WriteLog("Success", "Connect to Stago Start 4");
        }
        public static void WriteConnectStart4Fail()
        {
            LogProgram.WriteLog("***Fail", "Connect toStago Start 4", true);
        }
        public static void WriteConnectLauraSmartSuccess()
        {
            LogProgram.WriteLog("Success", "Connect to Laura Smart");
        }
        public static void WriteConnectLauraSmartFail()
        {
            LogProgram.WriteLog("***Fail", "Connect to Laura Smart", true);
        }
        public static void WriteStartFormConfig()
        {
            LogProgram.WriteLog("Success", "Start form config");
        }
        public static void WriteCloseFormConfig()
        {
            LogProgram.WriteLog("Success", "Close form config");
        }
        public static void WriteSaveConfigSuccess()
        {
            LogProgram.WriteLog("Success", "Changed config");
        }
        public static void WriteSaveConfigFail()
        {
            LogProgram.WriteLog("***Fail", "Changed config");
        }
        public static void WriteBeginTranfer()
        {
            LogProgram.WriteLog("Success", "Begin recieved from Cell-Dyn 1800 ");
        }
        public static void WriteEndTranfer()
        {
            LogProgram.WriteLog("Success", "End recieved from Cell-Dyn 1800 ");
        }

        public static void WriteSaveSucess(CD1800Result re)
        {
            LogProgram.WriteLog("Success", "Save SpecimenID: " + re.SpecimenID + "; Patient: " + re.Patient + "; Sequence: " + re.Sequence);
        }
        public static void WriteSaveFail(CD1800Result re)
        {
            LogProgram.WriteLog("***Fail", "Save SpecimenID: " + re.SpecimenID + "; Patient: " + re.Patient + "; Sequence: " + re.Sequence,true);
        }

        public static void WriteBeginReloadConfig()
        {
            LogProgram.WriteLog("Success", "Begin reload all config");
        }
        public static void WriteEndReloadConfig()
        {
            LogProgram.WriteLog("Success", "End reload all config");
        }
    }
}
