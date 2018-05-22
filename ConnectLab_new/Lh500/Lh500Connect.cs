using System;
using System.Collections.Generic;
using System.Text;
using System.Configuration;
using System.IO.Ports;
using System.Data;

namespace ConnectLab
{
    public class Lh500Connect : IDevice
    {
        public static string[] arrHH ={ "WBC", "NE%", "LY%", "MO%", "EO%", "BA%", "NE#", "LY#", "MO#", "EO#", "BA#", "RBC", "HGB", "HCT", "MCV", "MCH", "MCHC", "RDW", "PLT", "MPV", "RET%", "RET#" };
        private static Lh500Connect currentConnect;
        public static Lh500Connect CurrentConnect
        {
            get
            {
                return Lh500Connect.currentConnect;
            }
            set
            {
                Lh500Connect.currentConnect = value;
            }
        }


        SerialPort port;
        public Lh500Connect()
        {




            ////////
            port = new SerialPort();
            //port.Encoding = ASCIIEncoding.ASCII;
            port.ReadTimeout = 1000;
            //port.Handshake = Handshake.XOnXOff;
            port.DataReceived += new SerialDataReceivedEventHandler(DataReceive);
            Connect();

        }

        string[] result;
        string InputDataHuyetHoc = "";
        string tem = "";
        private void DataReceive(object obj, SerialDataReceivedEventArgs e)
        {
            try
            {
                tem = port.ReadExisting();
                InputDataHuyetHoc += tem;

                #region Huyết học Mr Khoe
                if (tem.StartsWith("03"))
                {
                    LogProgram.WriteBeginTranfer();
                    frmMain.formMain.m_notifyicon.ShowBalloonTip(3, "MKV", "Đang nhận kết quả từ máy Huyet Hoc LH500", System.Windows.Forms.ToolTipIcon.Info);

                }
                if (tem.EndsWith(""))
                {
                    bool okHH = LuKetQuaHuyetHoc(InputDataHuyetHoc);
                    if (okHH == false)
                    {
                        LogProgram.WriteLog("***Fail", "Save Huyet Hoc LH500 Error: " + InputDataHuyetHoc, true);
                        frmMain.formMain.m_notifyicon.ShowBalloonTip(3, "mkv", "lưu kết quả từ máy Huyet Hoc LH500 không thành công", System.Windows.Forms.ToolTipIcon.Error);

                    }
                    else
                    {
                        LogProgram.WriteLog("Success", "Save Huyet Hoc LH500 Success");
                        frmMain.formMain.m_notifyicon.ShowBalloonTip(3, "mkv", "đã lưu kết quả từ máy Huyet Hoc LH500", System.Windows.Forms.ToolTipIcon.Info);

                    }
                    LogProgram.WriteEndTranfer();
                    tem = "";
                    InputDataHuyetHoc = "";
                }
                #endregion
            }
            catch (Exception ex)
            {
                InputDataHuyetHoc = "";
                LogProgram.WriteLog("[LH500]:", ex.ToString());
                ConnectLab.Au480Connect.SaveErro(ex.ToString()+"-DataReceive", "LH500");
            }
        }

        public System.IO.Ports.SerialPort RS232;
        public bool LuKetQuaHuyetHoc(string strHuyetHoc)
        {
            try
            {
               
                #region MaBenhNhan
                int Id1Index = strHuyetHoc.IndexOf("ID1") + 4;
                string BARCODE = strHuyetHoc.Substring(Id1Index, strHuyetHoc.IndexOf("\r\n", Id1Index) - Id1Index);
                string TenBenhNhan = "";
                #endregion
                #region Ngay Thuc Hien
                string NgayTH = DateTime.Now.ToString("yyyy/MM/dd");
                #endregion
                #region MaKetQua
                string MaKetQua = "";
                string sqlGetMaPhieuCLS = @"select top 1 MaPhieuCLS,mabenhnhan,TenBenhNhan
                                                                 from khambenhcanlamsan cls 
	                                                                inner join benhnhan bn on bn.idbenhnhan= cls.idbenhnhan
	                                                                inner join banggiadichvu dv on cls.idcanlamsan=dv.idbanggiadichvu
                                                                  where ISNULL(CLS.dahuy,0)=0
	                                                                AND REPLACE(	REPLACE( REPLACE( MaPhieuCLS,'-',''),'PT',''),'CT','')+'99' =REPLACE(  REPLACE( REPLACE( '" + BARCODE.Trim() + @"','-',''),'PT','') ,'CT','')
                                                                  and dv.tennhom like N'%huyết học%'
                                                                order by cls.ngaythu desc";
                DataTable dtMakq = ServerConnect.I.GetTable(sqlGetMaPhieuCLS);
                string MaBenhNhan = "";
                if (dtMakq != null && dtMakq.Rows.Count > 0)
                {
                    MaKetQua = dtMakq.Rows[0]["MaPhieuCLS"].ToString();
                    TenBenhNhan = dtMakq.Rows[0]["TenBenhNhan"].ToString();
                    MaBenhNhan = dtMakq.Rows[0]["mabenhnhan"].ToString();
                }
                #endregion
                #region LoaiMay
                string LoaiMay = "LH500";
                #endregion

                //KhoaTD
                string STX = ASCIIEncoding.ASCII.GetString(new byte[] { (Byte)0x02 }); 
                string[] arrDataReceive = System.Text.RegularExpressions.Regex.Split(strHuyetHoc, STX);
                string datareceived = "";
                datareceived = arrDataReceive[1].Remove(arrDataReceive[1].Length - 5, 5) + arrDataReceive[2].Remove(0, 2);
                datareceived = datareceived.Remove(datareceived.Length - 5, 5);
                datareceived += arrDataReceive[3].Remove(0, 2);
                datareceived = datareceived.Remove(datareceived.Length - 5, 5);
                //

                #region Luu Ket Qua
                string newIdKQ = "1";
                string sqlCheckOldKetQua = @"SELECT idKetQuaTuMayCLS FROM KetQuaTuMayCLS WHERE maketqua='" + MaKetQua + "' AND loaimay='" + LoaiMay + "'";
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
                    string sqll = string.Format(@"INSERT INTO dbo.KetQuaTuMayCLS( mabenhnhan ,TenBenhNhan ,ngaythuchien , maketqua ,loaimay)
                                   VALUES( '{0}' ,N'{1}' ,'{2}' ,'{3}' ,'{4}' )",
                         MaBenhNhan, TenBenhNhan, NgayTH, MaKetQua, LoaiMay);
                    ServerConnect.I.BeginTran();
                    if (ServerConnect.I.ExecSQL(sqll) == false)
                    {
                        ServerConnect.I.RollBack();
                        return false;
                    }
                    DataTable DTTEMP = ServerConnect.I.GetTable("select max(idKetQuaTuMayCLS) from KetQuaTuMayCLS");
                    if (DTTEMP != null && DTTEMP.Rows.Count > 0)
                        newIdKQ = DTTEMP.Rows[0][0].ToString();
                }
                
                #endregion
                
                #region Noi Dung Ket Qua
                string sqlUpdateDetail = "";
                
                string sqlInsertKetQuaDetail = "";
                for (int i = 0; i < arrHH.Length; i++)
                {
                    string tenThongSo = arrHH[i];
                    string giaTri = "";
                    //int indexGiaTri = strHuyetHoc.IndexOf(tenThongSo);
                    //KhoaTD
                    int indexGiaTri = datareceived.IndexOf(tenThongSo);
                    //
                    if (indexGiaTri != -1)
                    {
                        if (tenThongSo == "HGB") 
                        { 
                        
                        }
                        try
                        {
                            indexGiaTri = indexGiaTri + tenThongSo.Length + 1;
                            giaTri = datareceived.Substring(indexGiaTri, datareceived.IndexOf("\r\n", indexGiaTri) - indexGiaTri);
                            giaTri = giaTri.Trim();
                            if (giaTri.Length > 0 && char.IsNumber(giaTri, giaTri.Length - 1) == false)
                                giaTri = giaTri.Substring(0, giaTri.Length - 1);
                        }
                        catch (Exception ex)
                        {
                            giaTri = "Error";
                            ConnectLab.Au480Connect.SaveErro(ex.ToString() + "-Phantich gia tri", "LH500");
                        }
                    }
                    if (!string.IsNullOrEmpty(giaTri))
                    {
                        bool IsInsertDetail = true;
                        if (dvDetail != null && dvDetail.Count > 0   )
                        {
                            dvDetail.RowFilter = "tenthongso='" + tenThongSo + "'";
                            if (dvDetail.Count > 0)
                            {
                                IsInsertDetail = false;
                                sqlUpdateDetail += @"
                                                       UpDate KetQuaTuMayCLS_ct       SET giatri='" + GetNumberValue(giaTri) + @"'  WHERE    idKetQuaTuMayCLS_Detail=" + dvDetail[0]["idKetQuaTuMayCLS_Detail"].ToString();
                            }
                        }
                        if (IsInsertDetail)
                        {
                            if (i > 0)
                                sqlInsertKetQuaDetail += @"
                                            union all";
                            sqlInsertKetQuaDetail += @"
                                    select idKetQuaTuMayCLS=" + newIdKQ + ", tenthongso=N'" + tenThongSo + "',giatri='" + GetNumberValue(giaTri) + @"'
                                    ";
                        }
                    }
                }
                if (sqlInsertKetQuaDetail != null && sqlInsertKetQuaDetail != "")
                {
                    sqlInsertKetQuaDetail = "insert into KetQuaTuMayCLS_ct (idKetQuaTuMayCLS,tenthongso,giatrichuoi)" + sqlInsertKetQuaDetail;
                    if (ServerConnect.I.ExecSQL(sqlInsertKetQuaDetail) == false)
                    {
                        ServerConnect.I.RollBack();
                        return false;
                    }
                }
                if (sqlUpdateDetail != null && sqlUpdateDetail != "")
                {
                    if (ServerConnect.I.ExecSQL(sqlUpdateDetail) == false)
                    {
                        ServerConnect.I.RollBack();
                        return false;
                    }
                }
                ServerConnect.I.Commit();
                #endregion
                #region In kết quả tự đổng
                #region Hải sơn add
                if (Program.IsPrintAuto)
                {
                    try
                    {
                        System.Threading.Thread thread = new System.Threading.Thread(new System.Threading.ThreadStart(delegate() { PrintKetQua(MaKetQua, newIdKQ); }));
                        thread.Start();


                    }
                    catch (Exception ex) { ConnectLab.Au480Connect.SaveErro(ex.ToString() + "-Print", "LH500"); }
                }
                #endregion
                #endregion
                return true;
               
            }
            catch (Exception ex)
            {
                ConnectLab.Au480Connect.SaveErro(ex.ToString() + "-LuuKetQua", "LH500");
                return false;
            }
        }

        private string GetNumberValue(string value)
        {
            string s = "";
            for (int i = 0; i < value.Length; i++)
            {
                if (char.IsNumber(value[i]) == true || value[i] == '.')
                    s = s + value[i];
            }
            return s;
        }

        private void PrintKetQua(string MaPhieuCLS, string newIdKQ)
        {
            ConnectLab.PrintKetQuaXN P = new ConnectLab.PrintKetQuaXN();
            P.MaPhieuCLS = MaPhieuCLS;
            P.IsTestOne = false;
            P.idKetQuaTuMayCLS = newIdKQ;
            P.PrintKetQua();
        }

        public bool Connect()
        {
            try
            {
                if (port.IsOpen)
                    port.Close();
                port.PortName = AppParams.lh500Config.PortName;
                port.BaudRate = Convert.ToInt32(AppParams.lh500Config.BaudRate);
                port.DataBits = Convert.ToInt32(AppParams.lh500Config.DataBits);

                switch (AppParams.lh500Config.Handshake)
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
                switch (AppParams.lh500Config.ParityType)
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
                switch (AppParams.lh500Config.StopBit)
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
            catch (Exception ex)
            {
                ConnectLab.Au480Connect.SaveErro(ex.ToString() + "-Connect", "LH500");
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
