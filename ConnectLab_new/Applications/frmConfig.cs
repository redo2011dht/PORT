using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.IO.Ports;
using System.Xml;
using ConnectLab.Elecsys2010;
// Bắt đầu code
namespace ConnectLab
{
    public partial class frmConfig : Form
    {
        SerialPort P = new SerialPort(); // Khai báo 1 Object SerialPort mới.
        string InputData = String.Empty; // Khai báo string buff dùng cho hiển thị dữ liệu sau này.
        delegate void SetTextCallback(string text); // Khai bao delegate SetTextCallBack voi tham so string
        bool? saveSuccess = null;
        string[] BaudRate = { "1200", "2400", "4800", "9600", "19200", "38400", "57600", "115200" };
        string[] Databits = { "6", "7", "8" };
        string[] Parity = { "None", "Odd", "Even" };
        string[] Handshake = { "None", "RequestToSend", "RequestToSendXOnXOff", "XOnXOff" };
        string[] stopbit = { "1", "1.5", "2" };
        string[] ports;
        public frmConfig()
        {
            LogProgram.WriteStartFormConfig();
            InitializeComponent();
            List<string> portTemp = new List<string>(SerialPort.GetPortNames());
            portTemp.Add("NONE");
            ports = portTemp.ToArray();
            P.ReadTimeout = 1000;

            InitControls();
            UpdateControls();
            this.FormClosed += new FormClosedEventHandler(frmConfig_FormClosed);
        }


        private void InitControls()
        {
            InitAu480();
            InitLh500();
            InitCD1800();
            InitLauraSmart();
            InitStart4();
            InitElecsys2010();
        }

        private void InitAu480()
        {
            cboAu480_Com.Items.AddRange(ports);
            cboAu480_Rate.Items.AddRange(BaudRate);
            cboAu480_DataBit.Items.AddRange(Databits);
            cboAu480_Parity.Items.AddRange(Parity);
            cboAu480_StopBit.Items.AddRange(stopbit);
            cboAu480_Handshake.Items.AddRange(Handshake);
        }
        private void InitLh500()
        {
            cboLh500_Com.Items.AddRange(ports);
            cboLh500_Rate.Items.AddRange(BaudRate);
            cboLh500_DataBit.Items.AddRange(Databits);
            cboLh500_Parity.Items.AddRange(Parity);
            cboLh500_StopBit.Items.AddRange(stopbit);
            cboLh500_Handshake.Items.AddRange(Handshake);
        }
        private void InitElecsys2010()
        {           
            cbxElecsys2010_Com.Items.AddRange(ports); 
            cbxElecsys2010_BaudRate.Items.AddRange(BaudRate);
            cbxElecsys2010_DataBit.Items.AddRange(Databits);
            cbxElecsys2010_Parity.Items.AddRange(Parity);
            cbxElecsys2010_StopBit.Items.AddRange(stopbit);
        }
        private void InitCD1800()
        {            
            cbCD1800_Com.Items.AddRange(ports);
            cbCD1800_Rate.Items.AddRange(BaudRate);
            cbCD1800_DataBits.Items.AddRange(Databits);
            cbCD1800_Parity.Items.AddRange(Parity);
            cbCD1800_StopBit.Items.AddRange(stopbit);
        }
        private void InitLauraSmart()
        {          
            cboLASM_Com.Items.AddRange(ports); 
            cboLASM_Rate.Items.AddRange(BaudRate);
            cboLASM_DataBit.Items.AddRange(Databits);
            cboLASM_Parity.Items.AddRange(Parity);
            cboLASM_StopBit.Items.AddRange(stopbit);
        }

        private void InitStart4()
        {           
            cboStart4_Com.Items.AddRange(ports);
            cboStart4_BaudRate.Items.AddRange(BaudRate);
            cboStart4_DataBits.Items.AddRange(Databits);
            cboStart4_Parity.Items.AddRange(Parity);
            cboStart4_StopBit.Items.AddRange(stopbit);
        }


        private void UpdateControls()
        {

            //Server
            txtSever.Text = AppParams.serverConfig.Name;
            txtDataBase.Text = AppParams.serverConfig.DBName;
            txtUser.Text = AppParams.serverConfig.User;
            txtPass.Text = AppParams.serverConfig.Pass;

            // Au480
            cboAu480_Com.SelectedItem = AppParams.au480Config.PortName;
            cboAu480_DataBit.SelectedItem = AppParams.au480Config.DataBits;
            cboAu480_Parity.SelectedItem = AppParams.au480Config.ParityType;
            cboAu480_Rate.SelectedItem = AppParams.au480Config.BaudRate;
            cboAu480_StopBit.SelectedItem = AppParams.au480Config.StopBit;
            cboAu480_Handshake.SelectedItem = AppParams.au480Config.Handshake;
            txtAu480_MaMay.Text = AppParams.au480Config.MaMayCLS;

            // Lh500
            cboLh500_Com.SelectedItem = AppParams.lh500Config.PortName;
            cboLh500_DataBit.SelectedItem = AppParams.lh500Config.DataBits;
            cboLh500_Parity.SelectedItem = AppParams.lh500Config.ParityType;
            cboLh500_Rate.SelectedItem = AppParams.lh500Config.BaudRate;
            cboLh500_StopBit.SelectedItem = AppParams.lh500Config.StopBit;
            cboLh500_Handshake.SelectedItem = AppParams.lh500Config.Handshake;
            txtLh500_MaMay.Text = AppParams.lh500Config.MaMayCLS;

            //cell-dyn 1800
            cbCD1800_Com.SelectedItem = AppParams.cd1800Config.PortName;
            cbCD1800_DataBits.SelectedItem = AppParams.cd1800Config.DataBits;
            cbCD1800_Parity.SelectedItem = AppParams.cd1800Config.ParityType;
            cbCD1800_Rate.SelectedItem = AppParams.cd1800Config.BaudRate;
            cbCD1800_StopBit.SelectedItem = AppParams.cd1800Config.StopBit;
            txtCD1800_MaMay.Text = AppParams.cd1800Config.MaMayCLS;
            txtCD1800_GRAN.Text = AppParams.cd1800Config.GRAN;
            txtCD1800_MID.Text = AppParams.cd1800Config.MID;
            txtCD1800_LYM.Text = AppParams.cd1800Config.LYM;
            txtCD1800_RBC.Text = AppParams.cd1800Config.RBC;
            txtCD1800_GRANPer.Text = AppParams.cd1800Config.GRANPer;
            txtCD1800_MIDPer.Text = AppParams.cd1800Config.MIDPer;
            txtCD1800_HCT.Text = AppParams.cd1800Config.HCT;
            txtCD1800_HGB.Text = AppParams.cd1800Config.HGB;
            txtCD1800_LYMPer.Text = AppParams.cd1800Config.LYMPer;
            txtCD1800_WBC.Text = AppParams.cd1800Config.WBC;
            txtCD1800_MPV.Text = AppParams.cd1800Config.MPV;
            txtCD1800_PLT.Text = AppParams.cd1800Config.PLT;
            txtCD1800_MCH.Text = AppParams.cd1800Config.MCH;
            txtCD1800_RDW.Text = AppParams.cd1800Config.RDW;
            txtCD1800_MCV.Text = AppParams.cd1800Config.MCV;
            txtCD1800_MCHC.Text = AppParams.cd1800Config.MCHC;
            txtCD1800_PDW.Text = AppParams.cd1800Config.PDW;
            txtCD1800_PCT.Text = AppParams.cd1800Config.PCT;


            // Erba lx 200

            txtEBLX200_Server.Text = AppParams.eblx200Config.Name;
            txtEBLX200_DBName.Text = AppParams.eblx200Config.DBName;
            txtEBLX200_User.Text = AppParams.eblx200Config.User;
            txtEBLX200_Pass.Text = AppParams.eblx200Config.Pass;
            txtEBLX200_MaMay.Text = AppParams.eblx200Config.MaMayCLS;

            // Laura smart
            cboLASM_Com.SelectedItem = AppParams.lauraSmartConfig.PortName;
            cboLASM_DataBit.SelectedItem = AppParams.lauraSmartConfig.DataBits;
            cboLASM_Parity.SelectedItem = AppParams.lauraSmartConfig.ParityType;
            cboLASM_Rate.SelectedItem = AppParams.lauraSmartConfig.BaudRate;
            cboLASM_StopBit.SelectedItem = AppParams.lauraSmartConfig.StopBit;
            txtLASM_MaMay.Text = AppParams.lauraSmartConfig.MaMayCLS;
            txtLASM_BLD.Text = AppParams.lauraSmartConfig.BLD;
            txtLASM_LEU.Text = AppParams.lauraSmartConfig.LEU;
            txtLASM_BIL.Text = AppParams.lauraSmartConfig.BIL;
            txtLASM_UBG.Text = AppParams.lauraSmartConfig.UBG;
            txtLASM_KET.Text = AppParams.lauraSmartConfig.KET;
            txtLASM_GLU.Text = AppParams.lauraSmartConfig.GLU;
            txtLASM_PRO.Text = AppParams.lauraSmartConfig.PRO;
            txtLASM_PH.Text = AppParams.lauraSmartConfig.PH;
            txtLASM_NIT.Text = AppParams.lauraSmartConfig.NIT;
            txtLASM_SG.Text = AppParams.lauraSmartConfig.SG;


            //Start 4


            cboStart4_Com.Text = AppParams.stagoStart4Config.PortName;
            cboStart4_BaudRate.Text = AppParams.stagoStart4Config.BaudRate;
            cboStart4_DataBits.Text = AppParams.stagoStart4Config.DataBits;
            cboStart4_Parity.Text = AppParams.stagoStart4Config.ParityType;
            cboStart4_StopBit.Text = AppParams.stagoStart4Config.StopBit;
            txtStart4_MaMay.Text = AppParams.stagoStart4Config.MaMayCLS;

            txtStart4_FIB.Text = AppParams.stagoStart4Config.FIBRINOGENE;
            txtStart4_INR.Text = AppParams.stagoStart4Config.INR;
            txtStart4_PROTH.Text = AppParams.stagoStart4Config.PROTHROMBINE;
            txtStart4_TCK.Text = AppParams.stagoStart4Config.TCK;
            txtStart4_TQ.Text = AppParams.stagoStart4Config.TQ;

            //Elecsys 2010
            cbxElecsys2010_Com.Text = AppParams.elecsys2010Config.PortName;
            cbxElecsys2010_BaudRate.Text = AppParams.elecsys2010Config.BaudRate;
            cbxElecsys2010_DataBit.Text = AppParams.elecsys2010Config.DataBits;
            cbxElecsys2010_Parity.Text = AppParams.elecsys2010Config.ParityType;
            cbxElecsys2010_StopBit.Text = AppParams.elecsys2010Config.StopBit;
            txtElecsys2010_MaMay.Text = AppParams.elecsys2010Config.MaMayCLS;
        }




        private void btKetNoi_Click(object sender, EventArgs e)
        {

            #region Test
            //            CD1800Connect a = new CD1800Connect();
//            string ChuoiHuyetHoc = @"0301
//
//
//
//
//
//--------------
//S02
//CBC
//DIFF
//TCBC
//04
//G01
//0A
//DATE 08/12/14
//TIME 16:14:22
//ID1 TRAN CHAU LAI
//CASSPOS M      
//ID1STATUS  
//C/PSTATUS P
//SASTATUS COMPLETE
//INST Instrument 1
//OPR LABADMIN
//DFACT  1.00
//G02
//0A
//WBC 6.6 
//RBC 4.98 
//HGB 144BD0026 
//HCT 44.9 
//MCV 90.1 
//MCH 29.3 
//MCHC 32.5 L
//RDW 13.3 
//PLT 269 
//MPV 6.5 L
//G08
//01
//CONDITION AutoValidated
//G0A
//01
//RP 02
//TDIFF
//02
//G03
//05
//LY# 1.8 
//MO# 0.4 
//NE# 3.9 
//EO# 0.4 
//BA# 0.1 
//G04
//05
//LY% 26.9 
//MO% 5.9 
//NE% 59.5 
//EO% 6.6 H
//BA4EC503% 1.1 
//
//
//--------------
//                                                                                                                                                                                                                                    6AD3";
//            a.LuKetQuaHuyetHoc(ChuoiHuyetHoc);
            //            return;
            #endregion

            string error = "";

            AppParams.serverConfig.Name = txtSever.Text.Trim();
            AppParams.serverConfig.DBName = txtDataBase.Text.Trim();
            AppParams.serverConfig.User = txtUser.Text.Trim();
            AppParams.serverConfig.Pass = txtPass.Text.Trim();
            AppParams.serverConfig.Write();
            if (ServerConnect.I.Connect() == false)
            {
                if (error != "") error += Environment.NewLine;
                error += "Không thể kết nối với cơ sở dữ liệu trên server.!";
            }

            if (listView1.Items[0].Checked)
            {
                AppParams.cd1800Config.PortName = cbCD1800_Com.Text.Trim();
                AppParams.cd1800Config.BaudRate = cbCD1800_Rate.Text.Trim();
                AppParams.cd1800Config.DataBits = cbCD1800_DataBits.Text.Trim();
                AppParams.cd1800Config.ParityType = cbCD1800_Parity.Text.Trim();
                AppParams.cd1800Config.StopBit = cbCD1800_StopBit.Text.Trim();
                AppParams.cd1800Config.MaMayCLS = txtCD1800_MaMay.Text.Trim();
                AppParams.cd1800Config.GRAN = txtCD1800_GRAN.Text;
                AppParams.cd1800Config.MID = txtCD1800_MID.Text;
                AppParams.cd1800Config.LYM = txtCD1800_LYM.Text;
                AppParams.cd1800Config.RBC = txtCD1800_RBC.Text;
                AppParams.cd1800Config.GRANPer = txtCD1800_GRANPer.Text;
                AppParams.cd1800Config.MIDPer = txtCD1800_MIDPer.Text;
                AppParams.cd1800Config.HCT = txtCD1800_HCT.Text;
                AppParams.cd1800Config.HGB = txtCD1800_HGB.Text;
                AppParams.cd1800Config.LYMPer = txtCD1800_LYMPer.Text;
                AppParams.cd1800Config.WBC = txtCD1800_WBC.Text;
                AppParams.cd1800Config.MPV = txtCD1800_MPV.Text;
                AppParams.cd1800Config.PLT = txtCD1800_PLT.Text;
                AppParams.cd1800Config.MCH = txtCD1800_MCH.Text;
                AppParams.cd1800Config.RDW = txtCD1800_RDW.Text;
                AppParams.cd1800Config.MCV = txtCD1800_MCV.Text;
                AppParams.cd1800Config.MCHC = txtCD1800_MCHC.Text;
                AppParams.cd1800Config.PDW = txtCD1800_PDW.Text;
                AppParams.cd1800Config.PCT = txtCD1800_PCT.Text;

                AppParams.cd1800Config.Write();
                if (CD1800Connect.CurrentConnect.Connect() == false)
                {
                    if (error != "") error += Environment.NewLine;
                    error += "Không thể kết nối với máy Cell - Dyn 1800.!";
                    this.saveSuccess = false;
                }
            }

            if (listView1.Items[1].Checked)
            {
                AppParams.eblx200Config.Name = txtEBLX200_Server.Text.Trim();
                AppParams.eblx200Config.DBName = txtEBLX200_DBName.Text.Trim();
                AppParams.eblx200Config.User = txtEBLX200_User.Text.Trim();
                AppParams.eblx200Config.Pass = txtEBLX200_Pass.Text.Trim();
                AppParams.eblx200Config.MaMayCLS = txtEBLX200_MaMay.Text;
                AppParams.eblx200Config.Write();
                if (EBLX200Connect.I.Connect() == false)
                {
                    if (error != "") error += Environment.NewLine;
                    error += "Không thể kết nối với máy EBLX 200.!";
                    this.saveSuccess = false;
                }
            }

            if (listView1.Items[2].Checked)
            {
                AppParams.lauraSmartConfig.PortName = cboLASM_Com.Text.Trim();
                AppParams.lauraSmartConfig.DataBits = cboLASM_DataBit.Text.Trim();
                AppParams.lauraSmartConfig.ParityType = cboLASM_Parity.Text.Trim();
                AppParams.lauraSmartConfig.BaudRate = cboLASM_Rate.Text.Trim();
                AppParams.lauraSmartConfig.StopBit = cboLASM_StopBit.Text.Trim();
                AppParams.lauraSmartConfig.MaMayCLS = txtLASM_MaMay.Text.Trim();
                AppParams.lauraSmartConfig.BLD = txtLASM_BLD.Text;
                AppParams.lauraSmartConfig.LEU = txtLASM_LEU.Text;
                AppParams.lauraSmartConfig.BIL = txtLASM_BIL.Text;
                AppParams.lauraSmartConfig.UBG = txtLASM_UBG.Text;
                AppParams.lauraSmartConfig.KET = txtLASM_KET.Text;
                AppParams.lauraSmartConfig.GLU = txtLASM_GLU.Text;
                AppParams.lauraSmartConfig.PRO = txtLASM_PRO.Text;
                AppParams.lauraSmartConfig.PH = txtLASM_PH.Text;
                AppParams.lauraSmartConfig.NIT = txtLASM_NIT.Text;
                AppParams.lauraSmartConfig.SG = txtLASM_SG.Text;
                AppParams.lauraSmartConfig.Write();
                if (LauraSmartConnect.CurrentConnect.Connect() == false)
                {
                    if (error != "") error += Environment.NewLine;
                    error += "Không thể kết nối với máy Laura Smart.!";
                    this.saveSuccess = false;
                }
            }

            if (listView1.Items[3].Checked)
            {
                AppParams.stagoStart4Config.PortName = cboStart4_Com.Text;
                AppParams.stagoStart4Config.BaudRate = cboStart4_BaudRate.Text;
                AppParams.stagoStart4Config.DataBits = cboStart4_DataBits.Text;
                AppParams.stagoStart4Config.ParityType = cboStart4_Parity.Text;
                AppParams.stagoStart4Config.StopBit = cboStart4_StopBit.Text;
                AppParams.stagoStart4Config.MaMayCLS = txtStart4_MaMay.Text;
                AppParams.stagoStart4Config.FIBRINOGENE = txtStart4_FIB.Text;
                AppParams.stagoStart4Config.INR = txtStart4_INR.Text;
                AppParams.stagoStart4Config.PROTHROMBINE = txtStart4_PROTH.Text;
                AppParams.stagoStart4Config.TCK = txtStart4_TCK.Text;
                AppParams.stagoStart4Config.TQ = txtStart4_TQ.Text;
                AppParams.stagoStart4Config.Write();
                if (StagoStart4Connect.CurrentConnect.Connect() == false)
                {
                    if (error != "") error += Environment.NewLine;
                    error += "Không thể kết nối với máy Stago Start4.!";
                    this.saveSuccess = false;
                }
            }

            if (listView1.Items[4].Checked)
            {
                AppParams.elecsys2010Config.PortName = cbxElecsys2010_Com.Text;
                AppParams.elecsys2010Config.BaudRate = cbxElecsys2010_BaudRate.Text;
                AppParams.elecsys2010Config.DataBits = cbxElecsys2010_DataBit.Text;
                AppParams.elecsys2010Config.ParityType = cbxElecsys2010_Parity.Text;
                AppParams.elecsys2010Config.StopBit = cbxElecsys2010_StopBit.Text;
                AppParams.elecsys2010Config.MaMayCLS = txtElecsys2010_MaMay.Text;
                AppParams.elecsys2010Config.Write();
                if (Elecsys2010Connect.CurrentConnect.Connect() == false)
                {
                    if (error != "") error += Environment.NewLine;
                    error += "Không thể kết nối với máy Elecsys 2010.!";
                    this.saveSuccess = false;
                }
            }

            if (listView1.Items[5].Checked)
            {
                AppParams.au480Config.PortName = cboAu480_Com.Text.Trim();
                AppParams.au480Config.DataBits = cboAu480_DataBit.Text.Trim();
                AppParams.au480Config.ParityType = cboAu480_Parity.Text.Trim();
                AppParams.au480Config.BaudRate = cboAu480_Rate.Text.Trim();
                AppParams.au480Config.StopBit = cboAu480_StopBit.Text.Trim();
                AppParams.au480Config.Handshake = cboAu480_Handshake.Text.Trim();
                AppParams.au480Config.MaMayCLS = txtAu480_MaMay.Text.Trim();
                AppParams.au480Config.Write();
                if (Au480Connect.CurrentConnect.Connect() == false)
                {
                    if (error != "") error += Environment.NewLine;
                    error += "Không thể kết nối với máy Au 480 !";
                    this.saveSuccess = false;
                }
            }

            if (listView1.Items[6].Checked)
            {
                AppParams.lh500Config.PortName = cboLh500_Com.Text.Trim();
                AppParams.lh500Config.DataBits = cboLh500_DataBit.Text.Trim();
                AppParams.lh500Config.ParityType = cboLh500_Parity.Text.Trim();
                AppParams.lh500Config.BaudRate = cboLh500_Rate.Text.Trim();
                AppParams.lh500Config.StopBit = cboLh500_StopBit.Text.Trim();
                AppParams.lh500Config.Handshake = cboLh500_Handshake.Text.Trim();
                AppParams.lh500Config.MaMayCLS = txtLh500_MaMay.Text.Trim();
                AppParams.lh500Config.Write();
                if (Lh500Connect.CurrentConnect.Connect() == false)
                {
                    if (error != "") error += Environment.NewLine;
                    error += "Không thể kết nối với máy Lh 500 !";
                    this.saveSuccess = false;
                }
            }
            if (error == "")
            {
                this.saveSuccess = true;
                LogProgram.WriteSaveConfigSuccess();
                MessageBox.Show("Kết nối thành công tới thiết bị.");
            }
            else
            {
                this.saveSuccess = false;
                LogProgram.WriteSaveConfigFail();
                MessageBox.Show(error + Environment.NewLine + "Vui lòng kiểm tra lại thông tin kết nối!");

            }
        }
        void frmConfig_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (saveSuccess != null)
            {
                LogProgram.WriteBeginReloadConfig();
                this.Hide();

                //if (this.saveSuccess == false)
                //{

                //    // app
                //    AppParams.serverConfig.Read();
                //    AppParams.cd1800Config.Read();
                //    AppParams.eblx200Config.Read();
                //    ServerConnect.I.ReadConfidFromAppParams();
                //    CD1800Connect.CurrentConnect.ReadConfidFromAppParams();
                //    EBLX200Connect.I.ReadConfidFromAppParams();

                //}
                string error = "";
                string warningCLS = "";
                string successCLS = "";
                bool isServer = ServerConnect.I.Connect();
                bool isCD1800 = false;// CD1800Connect.CurrentConnect.Connect();
                bool isEBLX200 = false;// EBLX200Connect.I.Connect();
                bool isLaura = false;// LauraSmartConnect.CurrentConnect.Connect();
                bool isStago = false;// StagoStart4Connect.CurrentConnect.Connect();
                bool isE2010 = false;// Elecsys2010Connect.CurrentConnect.Connect();
                if (isServer)
                {
                    LogProgram.WriteConnectServerSuccess();
                }
                else
                {
                    LogProgram.WriteConnectServerFail();
                    error = "Không thể kết nối cơ sở dữ liệu máy chủ";
                }
                if (isCD1800)
                {
                    LogProgram.WriteConnectCD1800Success();
                    successCLS += "CD 1800,";
                }
                else
                {
                    LogProgram.WriteConnectCD1800Fail();
                    warningCLS += "CD 1800,";
                }
                if (isEBLX200)
                {
                    LogProgram.WriteConnectEBLX200Success();
                    successCLS += "EBLX 200,";
                }
                else
                {
                    LogProgram.WriteConnectEBLX200Fail();
                    warningCLS += "EBLX 200,";
                }
                if (isEBLX200)
                {
                    LogProgram.WriteConnectLauraSmartSuccess();
                    successCLS += "Laura Smart,";
                }
                else
                {
                    LogProgram.WriteConnectLauraSmartFail();
                    warningCLS += "Laura Smart,,";
                }
                if (isStago)
                {
                    LogProgram.WriteConnectStart4Success();
                    successCLS += "Stago Start 4,";
                }
                else
                {
                    LogProgram.WriteConnectStart4Fail();
                    warningCLS += "Stago Start 4,";
                }

                if (isE2010)
                {
                    LogProgram.WriteConnectStart4Success();
                    successCLS += "Elecsys 2010,";
                }
                else
                {
                    LogProgram.WriteConnectStart4Fail();
                    warningCLS += "Elecsys 2010,";
                }

                if (error != "")
                {
                    frmMain.formMain.m_notifyicon.Icon = global::ConnectLab.Properties.Resources.disconnect;

                    frmMain.formMain.m_notifyicon.ShowBalloonTip(4, "MKV Soft - Lỗi", error, ToolTipIcon.Error);
                }
                else if (warningCLS != "")
                {
                    frmMain.formMain.m_notifyicon.Icon = global::ConnectLab.Properties.Resources.inconnect;
                    frmMain.formMain.m_notifyicon.ShowBalloonTip(4, "MKV Soft  - Cảnh báo", "Không thể kết nối máy xét nghiệm: " + warningCLS.TrimEnd(','), ToolTipIcon.Warning);
                }
                else
                {
                    frmMain.formMain.m_notifyicon.Icon = global::ConnectLab.Properties.Resources.connect;
                    frmMain.formMain.m_notifyicon.ShowBalloonTip(4, "MKV Soft - Thông báo", "Kết nối thành công cơ sở dữ liệu máy chủ.\nKết nối thành công máy:  " + successCLS.TrimEnd(','), ToolTipIcon.Info);
                }
                LogProgram.WriteEndReloadConfig();
            }
            LogProgram.WriteCloseFormConfig();
        }

        private void btThoat_Click(object sender, EventArgs e)
        {

            //CD1800Connect a = new CD1800Connect();
            //a.LuuKetQuaSinhHoa("D 001101 0001       PT-140700202CT    E0     002 91.84r 007 68.83r 009375.09r 010774.73r DE");

            this.Close();
        }


        private void llbleChangePass_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            frmChanePassword frm = new frmChanePassword();
            frm.Show(this);
        }

       

    }
}
