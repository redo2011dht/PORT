using System;
using System.Collections.Generic;
using System.Text;
using System.Configuration;
using System.IO.Ports;
using System.Data;
using ConnectLab.Elecsys2010;
using System.Windows.Forms;

namespace ConnectLab
{
    public class Elecsys2010Connect:IDevice
    {
        private static Elecsys2010Connect currentConnect;
        public static Elecsys2010Connect CurrentConnect
        {
            get
            {
                return Elecsys2010Connect.currentConnect;
            }
            set
            {
                Elecsys2010Connect.currentConnect = value;
            }
        }


        public SerialPort port;
        public Elecsys2010Connect()
        {
            port = new SerialPort();
            port.ReadTimeout = 1000;
            port.DataBits = 8;
            port.DataReceived += new SerialDataReceivedEventHandler(port_DataReceived);
            Connect();
        }
        
        AstmMessage currMassage = null;
        AstmRecord currRecord = null;
        string inputdata;
        string frameStr;
        //string test;
        public void port_DataReceived(object obj, SerialDataReceivedEventArgs e)
        {
            try
            {
                inputdata = Elecsys2010Connect.CurrentConnect.port.ReadExisting();

                switch (ASTM.detectPhase(inputdata))
                {
                    case ASTM.ESTABLISHMENT_PHASE:
                        if (currMassage == null) // Have no a receiving message
                        {
                            currMassage = new AstmMessage();
                            AstmFrame.nextFrameIndex = 1;
                            ASTM.writeACK(Elecsys2010Connect.CurrentConnect.port);
                            frmMain.formMain.m_notifyicon.ShowBalloonTip(3, "MKV Soft:: Thông báo", "Elecsys2010: bắt đầu nhận message", ToolTipIcon.Info);
                        }
                        else
                        {
                            currMassage=new AstmMessage();
                            frmMain.formMain.m_notifyicon.ShowBalloonTip(3, "MKV Soft:: Thông báo", "Elecsys2010: một message chưa nhận xong bị hủy", ToolTipIcon.Warning);
                        }
                        //LogProgram.WriteLog("E2010", "----BEGIN MESSAGE----");
                        break;
                    case ASTM.TRANSFER_PHASE:
                        if (currMassage != null) // Receive frame only when a receiving message initialized
                        {
                            frameStr += inputdata;
                            if (frameStr.StartsWith(ASTM.STX.ToString()) && frameStr.EndsWith(ASTM.CR.ToString() + ASTM.LF.ToString()))
                            {
                                //LogProgram.WriteLog("Eframe:", ASTM.Translate(frameStr));
                                AstmFrame frame = new AstmFrame(frameStr);
                                if (frame.isValidFrame)
                                {
                                    if (currRecord == null)
                                        currRecord = new AstmRecord();
                                    if (currRecord.AddFrame(frame))
                                    {
                                        if (currRecord.isFinished)
                                        {
                                            currMassage.AddRecord(currRecord);
                                            currRecord = null;
                                        }
                                        ASTM.writeACK(Elecsys2010Connect.CurrentConnect.port);
                                    }
                                    else ASTM.writeNAK(Elecsys2010Connect.CurrentConnect.port);
                                }
                                // process for add new frame failure
                                else
                                    ASTM.writeNAK(Elecsys2010Connect.CurrentConnect.port);
                                frameStr = ""; // Reset to receiver new frame
                            }
                        }
                        break;
                    case ASTM.TERMINATION_PHASE:
                        if (currMassage.isFinished)
                        {
                            // Process message here
                            currMassage.Build();
                            currMassage.UpdateToSQL();
                        }
                        else
                        {
                            // Receiving a message fail
                        }
                        //LogProgram.WriteLog("E2010", "----END MESSAGE----");
                        currMassage = null;
                        break;
                }
            }
            catch (Exception ex)
            {
                LogProgram.WriteLog("[Elecsys2010]:", ex.ToString());
            }
            //string a = frameStr.ToString();
            //currMassage = null;
        }

        public bool Connect()
        {
            try
            {
                if (port.IsOpen)
                    port.Close();
                port.PortName = AppParams.elecsys2010Config.PortName;
                port.BaudRate = Convert.ToInt32(AppParams.elecsys2010Config.BaudRate);
                port.DataBits = Convert.ToInt32(AppParams.elecsys2010Config.DataBits);
                switch (AppParams.elecsys2010Config.ParityType)
                {
                    case "Odd":
                        port.Parity = Parity.Odd;
                        break;
                    case "None":
                        port.Parity = Parity.None;
                        break;
                    case "Even":
                        port.Parity = Parity.Even;
                        break;
                }
                switch (AppParams.elecsys2010Config.StopBit)
                {
                    case "1":
                        port.StopBits = StopBits.One;
                        break;
                    case "1.5":
                        port.StopBits = StopBits.OnePointFive;
                        break;
                    case "2":
                        port.StopBits = StopBits.Two;
                        break;
                }
                port.Open();
            }
            catch
            {
                return false;
            }
            return true;
        }

        public void CloseCOM()
        {
            if (port.IsOpen)
                port.Close();
        }

        public bool IsConnectting
        {
            get
            {
                return port.IsOpen;
            }
        }

        #region IDevice Members


        public bool DeviceName
        {
            get { throw new Exception("The method or operation is not implemented."); }
        }

        public bool DeviceID
        {
            get { throw new Exception("The method or operation is not implemented."); }
        }

        #endregion

        #region IDevice Members


        public bool IsUsing
        {
            get
            {
                throw new Exception("The method or operation is not implemented.");
            }
            set
            {
                throw new Exception("The method or operation is not implemented.");
            }
        }

        #endregion
    }
}
