using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using System.IO.Ports;

namespace ConnectLab
{
    public class frmMain:ApplicationContext
    {

        public static frmMain formMain;
        public NotifyIcon m_notifyicon;
        private ContextMenu m_menu;
      
        public frmMain()
        {
            LogProgram.WriteStartProgram();

            AppParams.LoadAllConfig();

            ServerConnect.I = new ServerConnect();
            bool isServer =ServerConnect.I.Connect();


            Au480Connect.CurrentConnect = new Au480Connect();
            bool isAu480 = Au480Connect.CurrentConnect.IsConnectting;

            Lh500Connect.CurrentConnect = new Lh500Connect();
            bool isLh500 = Lh500Connect.CurrentConnect.IsConnectting;
            #region Temp ket qua 
            string InputTemp = @"0301





--------------
S02
CBC
DIFF
TCBC
04
G01
0A
DATE 10/19/16
TIME 09:35:05
ID1 19102016023
CASSPOS M      
ID1STATUS  
C/PSTATUS P
SASTATUS COMPLETE
INST Instrument 1
OPR LABADMIN
DFACT  1.00
G02
0A
WBC 8.6 
RBC 5.69 
HGB 161 163B02
HCT 48.1 
MCV 84.6 
MCH 28.3 
MCHC 33.5 
RDW 12.1 
PLT 232 
MPV 8.2 
G08
01
CONDITION AutoValidated
G0A
01
RP 01
TDIFF
02
G03
05
LY# 3.2 
MO# 0.7 
NE# 4.5 
EO# 0.2 
BA# 0.0 
G04
05
LY% 36.7 
MO% 8.1 
NE% 52.7 
EO% 2.3 
BA% 0.2CFAE03 L


--------------
   
                        
      ";
          //Lh500Connect.CurrentConnect.LuKetQuaHuyetHoc(InputTemp);
            #endregion

            //CD1800Connect.CurrentConnect = new CD1800Connect();
            //bool isCD1800 = CD1800Connect.CurrentConnect.IsConnectting;

            //EBLX200Connect.I = new EBLX200Connect();
            //bool isEBLX200 = EBLX200Connect.I.Connect();

            //LauraSmartConnect.CurrentConnect = new LauraSmartConnect();
            //bool isLaura = LauraSmartConnect.CurrentConnect.IsConnectting;

            //StagoStart4Connect.CurrentConnect = new StagoStart4Connect();
            //bool isStart4 = LauraSmartConnect.CurrentConnect.IsConnectting;

            //Elecsys2010Connect.CurrentConnect = new Elecsys2010Connect();
            //bool isES2010 = Elecsys2010Connect.CurrentConnect.IsConnectting;


        

            m_menu = new ContextMenu();
            MenuItem iConfig = new MenuItem("Cấu hình..", new System.EventHandler(Show_Click));
            iConfig.DefaultItem = true;
            m_menu.MenuItems.Add(0, iConfig);
           
            m_notifyicon = new NotifyIcon();
            m_notifyicon.Text = string.Format(@"MKV: Kết nối máy cận lâm sàng");
            m_notifyicon.Visible = true;
            m_notifyicon.BalloonTipClicked += new EventHandler(m_notifyicon_BalloonTipClicked);


            string error = "";
            string warningCLS = "";
            string successCLS = "";

            if (isServer)
            {
                LogProgram.WriteConnectServerSuccess();

            }
            else
            {
                LogProgram.WriteConnectServerFail();
                error = "Không thể kết nối cơ sở dữ liệu máy chủ";
            }

            
            if (isAu480)
            {
                LogProgram.WriteConnectAu480Success();
                successCLS += "Au480, ";
            }
            else
            {
                LogProgram.WriteConnectAu480Fail();
                warningCLS += "Au480,";
            }
            if (isLh500)
            {
                LogProgram.WriteConnectLh500Success();
                successCLS += "Lh500, ";
            }
            else
            {
                LogProgram.WriteConnectLh500Fail();
                warningCLS += "Lh500,";
            }
            //if (isCD1800)
            //{
            //    LogProgram.WriteConnectCD1800Success();
            //    successCLS += "CD 1800, ";
            //}
            //else
            //{
            //    LogProgram.WriteConnectCD1800Fail();
            //    warningCLS += "CD 1800,";
            //}


            //if (isEBLX200)
            //{
            //    LogProgram.WriteConnectEBLX200Success();
            //    successCLS += "EBLX 200,";
            //}
            //else
            //{
            //    LogProgram.WriteConnectEBLX200Fail();
            //    warningCLS += "EBLX 200,";
            //}
            //if (isLaura)
            //{
            //    LogProgram.WriteConnectLauraSmartSuccess();
            //    successCLS += "Laura Smart, ";
            //}
            //else
            //{
            //    LogProgram.WriteConnectLauraSmartFail();
            //    warningCLS += "Laura Smart,";
            //}
            //if (isStart4)
            //{
            //    LogProgram.WriteConnectStart4Success();
            //    successCLS += "Stago Start 4, ";
            //}
            //else
            //{
            //    LogProgram.WriteConnectStart4Fail();
            //    warningCLS += "Stago Start 4,";
            //}
            //if (isES2010)
            //{
            //    LogProgram.WriteConnectStart4Success();
            //    successCLS += "Elecsys 2010, ";
            //}
            //else
            //{
            //    LogProgram.WriteConnectStart4Fail();
            //    warningCLS += "Elecsys 2010,";
            //}
            if (error != "")
            {
                m_notifyicon.Icon = global::ConnectLab.Properties.Resources.disconnect;
              
                m_notifyicon.ShowBalloonTip(4, "MKV Soft - Lỗi", error, ToolTipIcon.Error);
            }
            else if (warningCLS != "")
            {
                m_notifyicon.Icon = global::ConnectLab.Properties.Resources.inconnect;
                m_notifyicon.ShowBalloonTip(4, "MKV Soft  - Cảnh báo", "Không thể kết nối máy cận: " + warningCLS.TrimEnd(','), ToolTipIcon.Warning);
            }
            else
            {
                m_notifyicon.Icon = global::ConnectLab.Properties.Resources.connect;
                m_notifyicon.ShowBalloonTip(4, "MKV Soft - Thông báo", "Kết nối thành công cơ sở dữ liệu máy chủ.\nKết nối thành công máy:  " + successCLS.TrimEnd(','), ToolTipIcon.Info);
            }


            m_notifyicon.ContextMenu = m_menu;
            formMain = this;

            this.ThreadExit += new EventHandler(frmMain_ThreadExit);
        }

        void frmMain_ThreadExit(object sender, EventArgs e)
        {
            LogProgram.WriteEndProgram();
        }


        void m_notifyicon_BalloonTipClicked(object sender, EventArgs e)
        {
            if (ServerConnect.I.IsConnectting == false
                //    || CD1800Connect.CurrentConnect.IsConnectting == false
                )
                Show_Click(sender, e);
        }

        //void frmMain_Activated(object sender, EventArgs e)
        //{
        //    this.Hide();
        //}
      
        //void frmMain_Load(object sender, EventArgs e)
        //{

        //}
       
      
        protected void Show_Click(Object sender, System.EventArgs e)
        {
            foreach (Form fr in Application.OpenForms)
            {

                if (fr.GetType().FullName == typeof(frmConfig).FullName)
                {
                    fr.Activate();
                    return;
                }
                else if (fr.GetType().FullName == typeof(frmPassword).FullName)
                {
                    fr.Activate();
                    return;
                }
                

                
            }
            frmPassword frm = new frmPassword();
            frm.Show();
        }
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                LogProgram.WriteEndProgram();
                this.m_notifyicon.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
