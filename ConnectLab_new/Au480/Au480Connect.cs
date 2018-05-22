﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Configuration;
using System.IO.Ports;
using System.Data;

namespace ConnectLab
{
    public class Au480Connect : IDevice
    {
        public static string[] arrSH = new string[] {         "002",
                                                            "003",
                                                            "004",
                                                            "005",
                                                            "006",
                                                            "007",
                                                            "008",
                                                            "009",
                                                            "010",
                                                            "011",
                                                            "013",
                                                            "015",
                                                            "016",
                                                            "017",
                                                            "020",
                                                            "097",
                                                            "098",
                                                            "099"
                                                    };
        private static Au480Connect currentConnect;
        public static Au480Connect CurrentConnect
        {
            get
            {
                return Au480Connect.currentConnect;
            }
            set
            {
                Au480Connect.currentConnect = value;
            }
        }


        SerialPort port;
        public Au480Connect()
        {
            port = new SerialPort();
            port.Encoding = ASCIIEncoding.ASCII;
           port.ReadTimeout = 1;
            //port.Handshake = Handshake.XOnXOff;
            port.DataReceived += new SerialDataReceivedEventHandler(DataReceive);

            Connect();
        }

        // string InputData = "\"   \",\" CD1800\",\"------------\",0088,00,\"---\",\"11/01/11\",\"02:50\",\"         117\",\"    TRAN THI COI\",\" \",\"  /  /  \",\"    \",\"  /  \",\"  :  \",\"       \",00058,-----,00011,00009,00038,-----,00521,00109,00392,00753,00209,00278,00140,00160,00109,00017,00179,-----,00182,00160,00658,-----,-,-,0,0,0,0,1,0,1,-,-,-,-,-,-,-,-,-,-,-,-,-,00,00,04817,02130,06326,04723,00000,00000,1,\"O\",0,0,0,1,27";
        string[] result;
        string InputData = "";
        string tem = "";
        int index = 0;
        private void DataReceive2(object obj, SerialDataReceivedEventArgs e)
        {
            tem = port.ReadExisting();
            index++;
            InputData += tem;
            Suport.hsTool.s_WriteFile(System.Windows.Forms.Application.StartupPath + "\\KETQUANE" + index.ToString() + ".txt", InputData);
        }
        private void DataReceive(object obj, SerialDataReceivedEventArgs e)
        {
            try
            {
                tem = port.ReadExisting();
                index++;
                InputData += tem;
                Suport.hsTool.s_WriteFile(System.Windows.Forms.Application.StartupPath + "\\KETQUANE"+index.ToString()+".txt",InputData);
                if (tem.StartsWith("") && InputData.Length > 10)
                {
                    LogProgram.WriteBeginTranfer();
                    frmMain.formMain.m_notifyicon.ShowBalloonTip(3, "MKV", "Đang nhận kết quả từ máy Au480 ", System.Windows.Forms.ToolTipIcon.Info);

                }
                if (tem.EndsWith("") && InputData.Length > 10)
                {
                    #region KhoaTD
                    try
                    {

                        string STX = ASCIIEncoding.ASCII.GetString(new byte[] { (Byte)0x02 });
                        string ETX = ASCIIEncoding.ASCII.GetString(new byte[] { (Byte)0x03 });
                        string[] arrPatient = System.Text.RegularExpressions.Regex.Split(InputData, ETX);
                        for (int l = 0; l < arrPatient.Length; l++)
                        {
                            if (arrPatient[l].Length > 3)
                            {
                                if (arrPatient[l].Substring(1, 1) == "R" && arrPatient[l].Length>10)
                                {
                                    string sample_id = arrPatient[l].Substring(14, 26);  //Barcode dán trên ống nghiệm
                                    string maketqua = null;
                                    string sqlGetMaPhieuCLS = @"select top 1 MaPhieuCLS,mabenhnhan,TenBenhNhan
                                                                 from khambenhcanlamsan cls 
	                                                                inner join benhnhan bn on bn.idbenhnhan= cls.idbenhnhan
	                                                                inner join banggiadichvu dv on cls.idcanlamsan=dv.idbanggiadichvu
                                                                  where ISNULL(CLS.dahuy,0)=0
	                                                                AND REPLACE(	REPLACE( REPLACE( MaPhieuCLS,'-',''),'PT',''),'CT','')+'99' =REPLACE(  REPLACE( REPLACE( '" + sample_id.Trim() + @"','-',''),'PT','') ,'CT','')
                                                                 and dv.tennhom like N'%sinh hóa%'
                                                                order by cls.ngaythu desc";
                                    DataTable dtMakq = ServerConnect.I.GetTable(sqlGetMaPhieuCLS);
                                    if (dtMakq != null && dtMakq.Rows.Count > 0)
                                    {
                                        maketqua = dtMakq.Rows[0]["MaPhieuCLS"].ToString();
                                        
                                    }
                                    else frmMain.formMain.m_notifyicon.ShowBalloonTip(3, "mkv", "Không kiểm tra được mã vạch ", System.Windows.Forms.ToolTipIcon.Info);



                                    string test = "";   
                                    string sqlDV = @"SELECT  CT.MACHITIET
                                                        FROM
                                                             KHAMBENHCANLAMSAN CLS
                                                            INNER join banggiadichvu dv ON CLS.IDCANLAMSAN=DV.IDBANGGIADICHVU
                                                            left join chitietdichvu ct on ct.idbanggiadichvu=dv.idbanggiadichvu
                                                            where CLS.MaPhieuCLS='" + maketqua + @"' 
                                                            AND DV.idphongkhambenh=22
                                                          AND DV.TENNHOM  like N'%sinh hóa%'
                                                        order by dv.tennhom,dv.stt,ct.stt
                                                        
                                    ";
                                    DataTable dtThongSo = ServerConnect.I.GetTable(sqlDV);
                                    if (dtThongSo != null && dtThongSo.Rows.Count > 0)
                                    {
                                        for (int k = 0; k < dtThongSo.Rows.Count; k++)
                                        {
                                            string Code = dtThongSo.Rows[k]["machitiet"].ToString();
                                            test += Code;
                                        }
                                        int ntemp = Suport.hsTool.int_Search(dtThongSo, "MACHITIET='018'");
                                        if (ntemp != -1 && Suport.hsTool.int_Search(dtThongSo, "MACHITIET='019'") == -1)
                                            test += "019";
                                    }
                                    else 
                                         frmMain.formMain.m_notifyicon.ShowBalloonTip(3, "mkv", "Không kiểm tra được chỉ định ", System.Windows.Forms.ToolTipIcon.Info);

                                    string textTran = arrPatient[l].Substring(3, 37);
                                    string textTranFinal = STX + "S " + textTran + "    E" + test + ETX;
                                    port.Write(textTranFinal);
                                    tem = "";
                                    InputData = "";
                                    return;
                                }
                            }
                        }
                    }
                    catch (Exception exBarCode) { ConnectLab.Au480Connect.SaveErro(exBarCode.ToString() + "-exBarCode", "Au480"); }
                    #endregion
                    #region LuuKetQuaSinhHoa
                    try
                    {
                        bool kt = LuuKetQuaSinhHoa(InputData);
                        if (!kt)
                        {
                            LogProgram.WriteLog("***Fail", "Save Reruls Au480 Error: " + InputData, true);
                            frmMain.formMain.m_notifyicon.ShowBalloonTip(3, "mkv", "lưu kết quả từ máy Au480 không thành công", System.Windows.Forms.ToolTipIcon.Error);

                        }
                        else
                        {
                            LogProgram.WriteLog("Success", "Save Reruls Au480 Success");
                            frmMain.formMain.m_notifyicon.ShowBalloonTip(3, "mkv", "đã lưu kết quả từ máy Au480", System.Windows.Forms.ToolTipIcon.Info);

                        }
                        tem = "";
                        InputData = "";
                    }
                    catch (Exception E) {
                        ConnectLab.Au480Connect.SaveErro(E.ToString(), "Au480");
                        tem = "";
                        InputData = "";
                    }
                    #endregion
                }
            }
            catch (Exception ex)
            {
                LogProgram.WriteLog("[Au480]:", ex.ToString());
                ConnectLab.Au480Connect.SaveErro(ex.ToString()+"-DataReceive", "Au480");
            }
        }

        public bool LuuKetQuaSinhHoa(string strChuoi)
        {
             
            #region Phân tích kết quả
                index = 0;
            string mabenhnhan = "";
            string TenBenhNhan = "";
            string ngaythuchien = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss");
            string maketqua = "";
            string loaimay = "Au480";
            string stt = "";
            int space_last = strChuoi.LastIndexOf(" ");
            string chuoiTemp = strChuoi.Substring(strChuoi.IndexOf(" "), space_last - strChuoi.IndexOf(" ")).TrimStart();

            string Position = chuoiTemp.Substring(0, chuoiTemp.IndexOf(" "));
            chuoiTemp = chuoiTemp.Substring(chuoiTemp.IndexOf(" "), chuoiTemp.Length - chuoiTemp.IndexOf(" ")).TrimStart();

            stt = chuoiTemp.Substring(0, chuoiTemp.IndexOf(" "));
            chuoiTemp = chuoiTemp.Substring(chuoiTemp.IndexOf(" "), chuoiTemp.Length - chuoiTemp.IndexOf(" "));

            //chuoiTemp = chuoiTemp.Replace("    E0     ", "@");
            //mabenhnhan = chuoiTemp.Substring(0, chuoiTemp.IndexOf("@"));
            mabenhnhan = chuoiTemp.Substring(0, 26).Trim();
            chuoiTemp = chuoiTemp.Replace(mabenhnhan, "").TrimStart();
            chuoiTemp = chuoiTemp.Replace("E", "").TrimStart();
            //chuoiTemp = chuoiTemp.Substring(34, chuoiTemp.Length-35).TrimStart();
            string sqlSelectBarCode = @"select top 1 MaPhieuCLS,mabenhnhan,TenBenhNhan
                                                                 from khambenhcanlamsan cls 
	                                                                inner join benhnhan bn on bn.idbenhnhan= cls.idbenhnhan
	                                                                inner join banggiadichvu dv on cls.idcanlamsan=dv.idbanggiadichvu
                                                                  where ISNULL(CLS.dahuy,0)=0
	                                                                AND REPLACE(	REPLACE( REPLACE( MaPhieuCLS,'-',''),'PT',''),'CT','')+'99' =REPLACE(  REPLACE( REPLACE( '"+mabenhnhan.Trim()+@"','-',''),'PT','') ,'CT','')
                                                                 and dv.tennhom like N'%sinh hóa%'
                                                                order by cls.ngaythu desc";

            DataTable dtMakq = ServerConnect.I.GetTable(sqlSelectBarCode);
            if (dtMakq != null && dtMakq.Rows.Count > 0)
            {
                maketqua = dtMakq.Rows[0]["MaPhieuCLS"].ToString();
                TenBenhNhan = dtMakq.Rows[0]["TenBenhNhan"].ToString();
                mabenhnhan = dtMakq.Rows[0]["mabenhnhan"].ToString();
            }
            chuoiTemp = chuoiTemp.Substring(chuoiTemp.IndexOf("@") + 1, chuoiTemp.Length - (chuoiTemp.IndexOf("@") + 1));
                #endregion
            #region Luu Ket Qua
            string newIdKQ = "1";
                string sqlCheckOldKetQua = @"SELECT idKetQuaTuMayCLS FROM KetQuaTuMayCLS WHERE maketqua='" + maketqua + "' AND loaimay='" + loaimay + "'";
                DataTable dtOldKetQua = ServerConnect.I.GetTable(sqlCheckOldKetQua);
                DataTable dtDetailOld =null;
                DataView dvDetail = null;
                if (dtOldKetQua != null && dtOldKetQua.Rows.Count > 0)
                {
                    newIdKQ = dtOldKetQua.Rows[0][0].ToString();
                    dtDetailOld = ServerConnect.I.GetTable(@"SELECT * FROM KetQuaTuMayCLS_ct WHERE idKetQuaTuMayCLS=" + newIdKQ);
                    dvDetail = new DataView(dtDetailOld);
                }
                else
                {
                    string sqll = string.Format(@"INSERT INTO dbo.KetQuaTuMayCLS( mabenhnhan ,TenBenhNhan ,ngaythuchien , maketqua ,loaimay , STT)
                                   VALUES( '{0}' ,N'{1}' ,'{2}' ,'{3}' ,'{4}' , '{5}')",
                         mabenhnhan, TenBenhNhan, ngaythuchien, maketqua, loaimay, stt);
                    ServerConnect.I.BeginTran();
                    if (ServerConnect.I.ExecSQL(sqll) == false)
                    {
                        ServerConnect.I.RollBack();
                        return false;
                    }
                     newIdKQ = ServerConnect.I.GetTable("select max(idKetQuaTuMayCLS) from KetQuaTuMayCLS").Rows[0][0].ToString();
                }
            #endregion
            #region Datatable Ket Qua
            DataTable dtRerult = new DataTable();
            dtRerult.Columns.Add("Code");
            dtRerult.Columns.Add("Rerult");
            string strRerult = chuoiTemp;
            while (strRerult.Length >= 10)
            {
                string OneMore = strRerult.Substring(0, 10);
                strRerult = strRerult.Substring(10, strRerult.Length - 10).TrimStart();
                DataRow drow = dtRerult.NewRow();
                drow["Code"] = OneMore.Substring(0, 3);
                string I_giatri = OneMore.Substring(3, 6).TrimStart().Replace("r", "");
                if (I_giatri.Length > 0 && char.IsNumber(I_giatri, I_giatri.Length - 1) == false)
                    I_giatri = I_giatri.Substring(0, I_giatri.Length - 1);

                drow["Rerult"] = I_giatri;
                dtRerult.Rows.Add(drow);
            }
            #endregion
            #region Luu chitiet Ket Qua

            string sqlInsertKetQuaDetail = "";
            string sqlUpdateDetail = "";
            if (dtRerult != null && dtRerult.Rows.Count > 0)
            {

                sqlInsertKetQuaDetail = "";
                for (int i = 0; i < dtRerult.Rows.Count; i++)
                {
                    string CodeI = dtRerult.Rows[i]["Code"].ToString();
                    if (CodeI == "018")
                    {
                        int tempp = Suport.hsTool.int_Search(dtRerult, "Code='019'");
                        int tempp_C = Suport.hsTool.int_Search(dtRerult, "Code='020'");
                        if (tempp != -1 && tempp_C!=-1)
                        {
                            string Rerult_A = dtRerult.Rows[i]["Rerult"].ToString();
                            string Rerult_B = dtRerult.Rows[tempp]["Rerult"].ToString();
                            try
                            {
                                double C = 91.5 * double.Parse(Rerult_A) / double.Parse(Rerult_B) + 2.15;
                                C = Math.Round(C, 1);
                                dtRerult.Rows[tempp_C]["Rerult"] = C;
                            }
                            catch (Exception e) { ConnectLab.Au480Connect.SaveErro(e.ToString()+"-018", "Au480"); }

                        }
                    }
                    bool IsInsertDetail = true;
                    if (dvDetail != null && dvDetail.Count > 0)
                    {
                        dvDetail.RowFilter = "tenthongso='" + dtRerult.Rows[i]["Code"].ToString() + "'";
                        if (dvDetail.Count > 0)
                        {
                            try
                            {
                                double d_temp = double.Parse(dtRerult.Rows[i]["Rerult"].ToString().Trim());
                            }
                            catch (Exception) {
                                dtRerult.Rows[i]["Rerult"]= "null";
                            
                            }
                            IsInsertDetail = false;
                            sqlUpdateDetail += @"
                                                       UpDate KetQuaTuMayCLS_ct       SET giatri=" + dtRerult.Rows[i]["Rerult"].ToString() + @"  WHERE    idKetQuaTuMayCLS_Detail=" + dvDetail[0]["idKetQuaTuMayCLS_Detail"].ToString();
                        }
                    }
                    if (IsInsertDetail)
                    {
                        try
                        {
                            double d_temp = double.Parse(dtRerult.Rows[i]["Rerult"].ToString().Trim());
                        }
                        catch (Exception)
                        {
                            dtRerult.Rows[i]["Rerult"]= "null";

                        }

                        if (i > 0)
                            sqlInsertKetQuaDetail += @"
                        " + (dtRerult.Rows[i]["Code"].ToString() != "019" ? " union all" : "");
                        if (dtRerult.Rows[i]["Code"].ToString() != "019")
                            sqlInsertKetQuaDetail += @"
                                select idKetQuaTuMayCLS=" + newIdKQ + ", tenthongso=N'" + dtRerult.Rows[i]["Code"].ToString() + "',giatri=" + dtRerult.Rows[i]["Rerult"].ToString() + @"
                                ";
                    }
                }
                if (sqlInsertKetQuaDetail != null && sqlInsertKetQuaDetail != "")
                {
                    sqlInsertKetQuaDetail = @"insert into KetQuaTuMayCLS_ct (idKetQuaTuMayCLS,tenthongso,giatri)
                                      "
                        + sqlInsertKetQuaDetail;

                    if (ServerConnect.I.ExecSQL(sqlInsertKetQuaDetail) == false)
                    {
                        ServerConnect.I.RollBack();
                        return false;
                    }
                }
                ServerConnect.I.Commit();
            
                #region Hải sơn add
                if (Program.IsPrintAuto)
                {
                    try
                    {

                        System.Threading.Thread thread = new System.Threading.Thread(new System.Threading.ThreadStart(delegate() { PrintKetQua(maketqua, newIdKQ); }));
                        thread.Start();


                    }
                    catch (Exception exPrint) { ConnectLab.Au480Connect.SaveErro(exPrint.ToString() + "-Print", "Au480"); }
                }
                #endregion

                return true;
            }
            else
                return false;
            #endregion
            
        }

        private void PrintKetQua(string MaPhieuCLS, string newIdKQ)
        {
            ConnectLab.PrintKetQuaXN P = new ConnectLab.PrintKetQuaXN();
            P.MaPhieuCLS = MaPhieuCLS;
            P.idKetQuaTuMayCLS = newIdKQ;
            P.IsTestOne = false;
            P.PrintKetQua();
        }


        public bool Connect()
        {
            try
            {
                if (port.IsOpen)
                    port.Close();
                port.PortName = AppParams.au480Config.PortName;
                port.BaudRate = Convert.ToInt32(AppParams.au480Config.BaudRate);
                port.DataBits = Convert.ToInt32(AppParams.au480Config.DataBits);

                switch (AppParams.au480Config.Handshake)
                {
                    case "None":
                        port.Handshake = Handshake.None;
                        break;
                    case "RequestToSend":
                        port.Handshake = Handshake.RequestToSend;
                        break;
                    case "RequestToSendXOnXOff":
                        port.Handshake = Handshake.RequestToSendXOnXOff;
                        break;
                    case "XOnXOff":
                        port.Handshake = Handshake.XOnXOff;
                        break;
                }
                port.RtsEnable = true;
                port.DtrEnable = true;
                switch (AppParams.au480Config.ParityType)
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
                switch (AppParams.au480Config.StopBit)
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
            catch (Exception eConnect)
            {
                ConnectLab.Au480Connect.SaveErro(eConnect.ToString() + "-ConnectData", "Au480");
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
        public static void SaveErro(string ERROCODE, string TypeMachine)
        {
            try
            {
                string sqlInsert = @"INSERT INTO zzErroXN(TypeMachine,ERROCODE,SAVEDATE) SELECT TypeMachine='" + TypeMachine + "',ERROCODE=N'" + ERROCODE + "',SAVEDATE=GETDATE()";
                DataAcess.Connect.ExecSQL(sqlInsert);
            }
            catch (Exception e) { }

        }
    }//end class
}//end name space
