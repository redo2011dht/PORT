using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Threading;
using System.Reflection;
using System.IO.Ports;

namespace ConnectLab
{
   public static class Program
    {
       public static bool IsPrintAuto = false;
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
           
            #region DataAcces Connect from file
            string file = Application.StartupPath + "\\CONF\\ServerConfig.txt";
            if (System.IO.File.Exists(file))
            {
                string s = Suport.hsTool.s_ReadFile(file);
                s = s.Replace("\r\n", ";");
                string[] arrS = s.Split(';');
                if (arrS.Length == 4)
                {
                    DataAcess.Connect.NewConnect(arrS[0], arrS[1], arrS[2], arrS[3]);
                }
            }
            #endregion
            #region IsPrint Auto 
            string file_IsPrint = Application.StartupPath + "\\CONF\\file_IsPrint.txt";
            if (System.IO.File.Exists(file))
            {
                string IsPrint = Suport.hsTool.s_ReadFile(file_IsPrint);
                if (IsPrint == "1") IsPrintAuto = true;
            }
            else IsPrintAuto = true;
            #endregion

            #region Hải sơn add
            //ConnectLab.PrintKetQuaXN P = new ConnectLab.PrintKetQuaXN();
            //P.MaPhieuCLS = "PT-170401224CT";
            //P.IsTestOne = true;
            //P.PrintKetQua();

            #endregion
            using (Mutex mutex = new Mutex(false, "Global\\" + Assembly.GetExecutingAssembly().GetType().GUID.ToString()))
            {
                if (!mutex.WaitOne(0, false))
                {
                    MessageBox.Show("Chương trình đã chạy rồi.", "MKV Soft :: Thông báo");
                    return;
                }
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                try
                {


                    ///
                    GC.Collect();
                    Application.Run(new frmMain());
                    GC.KeepAlive(mutex);

                }
                catch (Exception ex)
                {
                    MessageBox.Show("Lỗi chạy ứng dụng : " + ex.Message, "MKV Soft :: Thông báo");
                }
            }            
        }

        
    }
}