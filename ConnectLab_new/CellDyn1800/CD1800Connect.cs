using System;
using System.Collections.Generic;
using System.Text;
using System.Configuration;
using System.IO.Ports;
using System.Data;

namespace ConnectLab
{
    public class CD1800Connect : IDevice
    {
        private static CD1800Connect currentConnect;
        public static CD1800Connect CurrentConnect
        {
            get
            {
                return CD1800Connect.currentConnect;
            }
            set
            {
                CD1800Connect.currentConnect = value;
            }
        }


        SerialPort port;
        public CD1800Connect()
        {
            port = new SerialPort();
            port.Encoding = ASCIIEncoding.ASCII;
            port.ReadTimeout = 1000;
            port.Handshake = Handshake.XOnXOff;
            port.DataReceived += new SerialDataReceivedEventHandler(DataReceive);

            Connect();
        }

        // string InputData = "\"   \",\" CD1800\",\"------------\",0088,00,\"---\",\"11/01/11\",\"02:50\",\"         117\",\"    TRAN THI COI\",\" \",\"  /  /  \",\"    \",\"  /  \",\"  :  \",\"       \",00058,-----,00011,00009,00038,-----,00521,00109,00392,00753,00209,00278,00140,00160,00109,00017,00179,-----,00182,00160,00658,-----,-,-,0,0,0,0,1,0,1,-,-,-,-,-,-,-,-,-,-,-,-,-,00,00,04817,02130,06326,04723,00000,00000,1,\"O\",0,0,0,1,27";
        string[] result;
        string InputData = "";
        //string InputDataHuyetHoc = "";
        string tem = "";
        int index = 0;
        private void DataReceive(object obj, SerialDataReceivedEventArgs e)
        {
            try
            {
                tem = port.ReadExisting();
                index++;
                InputData += tem;
                //InputDataHuyetHoc += tem;
                if (tem.StartsWith(""))
                {
                    LogProgram.WriteBeginTranfer();
                    frmMain.formMain.m_notifyicon.ShowBalloonTip(3, "MKV", "Đang nhận kết quả từ máy cd 1800", System.Windows.Forms.ToolTipIcon.Info);

                }
                #region Sinh Hoa Mr Khoe
                //if (InputData.Contains("    E0     ") && tem.EndsWith(""))
                //{
                //    bool kt = LuuKetQuaSinhHoa(InputData);
                //    if (kt)
                //    {
                //        frmMain.formMain.m_notifyicon.ShowBalloonTip(3, "MKV", "Đã lưu kết quả từ máy EBLX200 " + InputData, System.Windows.Forms.ToolTipIcon.Info);
                //    }
                //    LogProgram.WriteEndTranfer();
                //    tem = "";
                //    InputData = "";
                //}
                #endregion
                //LogProgram.WriteEndTranfer();
                //}

                #region Huyết học Mr Khanh
                if ( tem.EndsWith(""))
                {
                    InputData = InputData.TrimStart('').TrimEnd('');
                    CD1800Result re = new CD1800Result(InputData);
                    if (UpdateToSQL(re) == false)
                    {
                        LogProgram.WriteSaveFail(re);
                        frmMain.formMain.m_notifyicon.ShowBalloonTip(3, "mkv", "lưu kết quả '" + re.SpecimenID + "' từ máy cd 1800 không thành công", System.Windows.Forms.ToolTipIcon.Error);

                    }
                    else
                    {
                        LogProgram.WriteSaveSucess(re);
                        frmMain.formMain.m_notifyicon.ShowBalloonTip(3, "mkv", "đã lưu kết quả '" + re.SpecimenID + "' từ máy cd 1800", System.Windows.Forms.ToolTipIcon.Info);

                    }
                    LogProgram.WriteEndTranfer();
                    tem = "";
                    InputData = "";
                }
                #endregion

                #region Huyết học Mr Khoe
                //if(tem.StartsWith("03"))
                //{
                //    LogProgram.WriteBeginTranfer();
                //    frmMain.formMain.m_notifyicon.ShowBalloonTip(3, "MKV", "Đang nhận kết quả từ máy Huyet Hoc LH500", System.Windows.Forms.ToolTipIcon.Info);

                //}
                //if(tem.EndsWith(""))
                //{
                //    bool okHH = LuKetQuaHuyetHoc(InputDataHuyetHoc);
                //    if (okHH == false)
                //    {
                //        LogProgram.WriteLog("***Fail", "Save Huyet Hoc LH500 Error: " + InputDataHuyetHoc, true);                        
                //        frmMain.formMain.m_notifyicon.ShowBalloonTip(3, "mkv", "lưu kết quả từ máy Huyet Hoc LH500 không thành công", System.Windows.Forms.ToolTipIcon.Error);

                //    }
                //    else
                //    {
                //        LogProgram.WriteLog("Success", "Save Huyet Hoc LH500 Success");
                //        frmMain.formMain.m_notifyicon.ShowBalloonTip(3, "mkv", "đã lưu kết quả từ máy Huyet Hoc LH500", System.Windows.Forms.ToolTipIcon.Info);

                //    }
                //    LogProgram.WriteEndTranfer();
                //    tem = "";
                //    InputDataHuyetHoc = "";
                //}
                #endregion
            }
            catch (Exception ex)
            {
                LogProgram.WriteLog("[CD1800]:", ex.ToString());
            }
        }


        public bool LuuKetQuaSinhHoa(string strChuoi)
        {
            index = 0;
            string mabenhnhan = "";
            string TenBenhNhan = "";
            string ngaythuchien = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss");
            string maketqua = "";
            string loaimay = "EBLX200";
            string stt = "";
            int space_last = strChuoi.LastIndexOf(" ");
            string chuoiTemp = strChuoi.Substring(strChuoi.IndexOf(" "), space_last - strChuoi.IndexOf(" ")).TrimStart();

            string Position = chuoiTemp.Substring(0, chuoiTemp.IndexOf(" "));
            chuoiTemp = chuoiTemp.Substring(chuoiTemp.IndexOf(" "), chuoiTemp.Length - chuoiTemp.IndexOf(" ")).TrimStart();

            stt = chuoiTemp.Substring(0, chuoiTemp.IndexOf(" "));
            chuoiTemp = chuoiTemp.Substring(chuoiTemp.IndexOf(" "), chuoiTemp.Length - chuoiTemp.IndexOf(" ")).TrimStart();

            chuoiTemp = chuoiTemp.Replace("    E0     ", "@");
            maketqua = chuoiTemp.Substring(0, chuoiTemp.IndexOf("@"));
            chuoiTemp = chuoiTemp.Substring(chuoiTemp.IndexOf("@") + 1, chuoiTemp.Length - (chuoiTemp.IndexOf("@") + 1));

            #region Luu Ket Qua
            string sqll = string.Format(@"INSERT INTO dbo.KetQuaTuMayCLS( mabenhnhan ,TenBenhNhan ,ngaythuchien , maketqua ,loaimay , STT)
                                   VALUES( '{0}' ,N'{1}' ,'{2}' ,'{3}' ,'{4}' , '{5}')",
                 " ", TenBenhNhan, ngaythuchien, maketqua, loaimay, stt);
            ServerConnect.I.BeginTran();
            if (ServerConnect.I.ExecSQL(sqll) == false)
            {
                ServerConnect.I.RollBack();
                return false;
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
                drow["Rerult"] = OneMore.Substring(3, 6).TrimStart();
                dtRerult.Rows.Add(drow);
            }
            #endregion

            #region Luu chitiet Ket Qua
            string newIdKQ = ServerConnect.I.GetTable("select max(idKetQuaTuMayCLS) from KetQuaTuMayCLS").Rows[0][0].ToString();
            string sqlChitietKetQua = "";
            if (dtRerult != null && dtRerult.Rows.Count > 0)
            {
                sqlChitietKetQua = "insert into KetQuaTuMayCLS_ct (idKetQuaTuMayCLS,tenthongso,giatri)";
                for (int i = 0; i < dtRerult.Rows.Count; i++)
                {
                    if (i > 0)
                        sqlChitietKetQua += @"
                        union all";
                    sqlChitietKetQua += @"
                        select idKetQuaTuMayCLS=" + newIdKQ + ", tenthongso=N'" + dtRerult.Rows[i]["Code"].ToString() + "',giatri='" + dtRerult.Rows[i]["Rerult"].ToString() + @"'
                        ";
                }
                if (ServerConnect.I.ExecSQL(sqlChitietKetQua) == false)
                {
                    ServerConnect.I.RollBack();
                    return false;
                }
                ServerConnect.I.Commit();
                return true;
            }
            else
                return false;
            #endregion
        }

        public bool LuKetQuaHuyetHoc(string strHuyetHoc)
        {
            try
            {
                string[] arrHH ={ "WBC", "NE%", "LY%", "MO%", "EO%", "BA%", "NE#", "LY#", "MO#", "EO#", "BA#", "RBC", "HGB", "HTC", "MCV", "MCH", "MCHC", "RDW", "PLT", "MPV", "RET%", "RET#" };
                #region MaBenhNhan
                int Id1Index = strHuyetHoc.IndexOf("ID1") + 4;
                string MaBenhNhan = strHuyetHoc.Substring(Id1Index, strHuyetHoc.IndexOf("\r\n", Id1Index) - Id1Index);
                string TenBenhNhan = "";
                #endregion
                #region Ngay Thuc Hien
                string NgayTH = DateTime.Now.ToString("yyyy/MM/dd");
                #endregion
                #region MaKetQua
                string MaKetQua = "";
                if (MaBenhNhan.Contains("PT-"))
                    MaKetQua = MaBenhNhan;
                else
                {
                    DataTable dtMakq = ServerConnect.I.GetTable("select top 1 MaPhieuCLS,mabenhnhan,TenBenhNhan from khambenhcanlamsan cls inner join benhnhan bn on bn.idbenhnhan= cls.idbenhnhan where mabenhnhan='" + MaBenhNhan + "' order by cls.ngaythu desc");
                    if (dtMakq != null && dtMakq.Rows.Count > 0)
                    {
                        MaKetQua = dtMakq.Rows[0]["MaPhieuCLS"].ToString();
                        TenBenhNhan = dtMakq.Rows[0]["TenBenhNhan"].ToString();
                    }
                }
                #endregion
                #region LoaiMay
                string LoaiMay = "LH500";
                #endregion


                #region Luu Ket Qua
                string sqll = string.Format(@"INSERT INTO dbo.KetQuaTuMayCLS( mabenhnhan ,TenBenhNhan ,ngaythuchien , maketqua ,loaimay)
                                   VALUES( '{0}' ,N'{1}' ,'{2}' ,'{3}' ,'{4}' )",
                     MaBenhNhan, TenBenhNhan, NgayTH, MaKetQua, LoaiMay);
                ServerConnect.I.BeginTran();
                if (ServerConnect.I.ExecSQL(sqll) == false)
                {
                    ServerConnect.I.RollBack();
                    return false;
                }
                #endregion
                #region Noi Dung Ket Qua
                string newIdKQ = ServerConnect.I.GetTable("select max(idKetQuaTuMayCLS) from KetQuaTuMayCLS").Rows[0][0].ToString();
                string sqlChitietKetQua = "insert into KetQuaTuMayCLS_ct (idKetQuaTuMayCLS,tenthongso,giatrichuoi)";
                for (int i = 0; i < arrHH.Length; i++)
                {
                    string tenThongSo = arrHH[i];
                    string giaTri = "";
                    int indexGiaTri = strHuyetHoc.IndexOf(tenThongSo);
                    if (indexGiaTri != -1)
                    {
                        try
                        {
                            indexGiaTri = indexGiaTri + tenThongSo.Length + 1;
                            giaTri = strHuyetHoc.Substring(indexGiaTri, strHuyetHoc.IndexOf("\r\n", indexGiaTri) - indexGiaTri);
                            giaTri = giaTri.Trim();
                        }
                        catch (Exception)
                        {
                            giaTri = "Error";
                        }
                    }
                    if (!string.IsNullOrEmpty(giaTri))
                    {
                        if (i > 0)
                            sqlChitietKetQua += @"
                        union all";
                        sqlChitietKetQua += @"
                        select idKetQuaTuMayCLS=" + newIdKQ + ", tenthongso=N'" + tenThongSo + "',giatri='" + giaTri + @"'
                        ";
                    }
                }
                if (ServerConnect.I.ExecSQL(sqlChitietKetQua) == false)
                {
                    ServerConnect.I.RollBack();
                    return false;
                }
                ServerConnect.I.Commit();
                return true;
                #endregion
            }
            catch (Exception)
            {
                return false;
            }
        }

        public bool Connect()
        {
            try
            {
                if (port.IsOpen)
                    port.Close();
                port.PortName = AppParams.cd1800Config.PortName;
                port.BaudRate = Convert.ToInt32(AppParams.cd1800Config.BaudRate);
                port.DataBits = Convert.ToInt32(AppParams.cd1800Config.DataBits);
                switch (AppParams.cd1800Config.ParityType)
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
                switch (AppParams.cd1800Config.StopBit)
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

        private bool UpdateToSQL(CD1800Result result)
        {
            try
            {
                string idKetQuaTuMayCLS = GetIdKetQuaTuMayCLS(result.SpecimenID);
                List<string> paramName = new List<string>();
                List<object> values = new List<object>();
                List<SqlDbType> sqlTypes = new List<SqlDbType>();
                #region thamso
                if (result.WBC != null)
                {
                    paramName.Add("@WBC");
                    values.Add(result.WBC);
                    sqlTypes.Add(SqlDbType.Float);

                }
                if (result.LYM != null)
                {
                    paramName.Add("@LYM");
                    values.Add(result.LYM);
                    sqlTypes.Add(SqlDbType.Float);

                }
                if (result.MID != null)
                {
                    paramName.Add("@MID");
                    values.Add(result.MID);
                    sqlTypes.Add(SqlDbType.Float);

                }
                if (result.GRAN != null)
                {
                    paramName.Add("@GRAN");
                    values.Add(result.GRAN);
                    sqlTypes.Add(SqlDbType.Float);

                }
                if (result.RBC != null)
                {
                    paramName.Add("@RBC");
                    values.Add(result.RBC);
                    sqlTypes.Add(SqlDbType.Float);

                }
                if (result.HGB != null)
                {
                    paramName.Add("@HGB");
                    values.Add(result.HGB);
                    sqlTypes.Add(SqlDbType.Float);

                }
                if (result.HCT != null)
                {
                    paramName.Add("@HCT");
                    values.Add(result.HCT);
                    sqlTypes.Add(SqlDbType.Float);

                }
                if (result.MCV != null)
                {
                    paramName.Add("@MCV");
                    values.Add(result.MCV);
                    sqlTypes.Add(SqlDbType.Float);

                }
                if (result.MCH != null)
                {
                    paramName.Add("@MCH");
                    values.Add(result.MCH);
                    sqlTypes.Add(SqlDbType.Float);

                }
                if (result.MCHC != null)
                {
                    paramName.Add("@MCHC");
                    values.Add(result.MCHC);
                    sqlTypes.Add(SqlDbType.Float);

                }
                if (result.RDW != null)
                {
                    paramName.Add("@RDW");
                    values.Add(result.RDW);
                    sqlTypes.Add(SqlDbType.Float);

                }
                if (result.PLT != null)
                {
                    paramName.Add("@PLT");
                    values.Add(result.PLT);
                    sqlTypes.Add(SqlDbType.Float);

                }

                if (result.MPV != null)
                {
                    paramName.Add("@MPV");
                    values.Add(result.MPV);
                    sqlTypes.Add(SqlDbType.Float);

                } if (result.PCT != null)
                {
                    paramName.Add("@PCT");
                    values.Add(result.PCT);
                    sqlTypes.Add(SqlDbType.Float);

                } if (result.PDW != null)
                {
                    paramName.Add("@PDW");
                    values.Add(result.PDW);
                    sqlTypes.Add(SqlDbType.Float);

                } if (result.LYM_PER != null)
                {
                    paramName.Add("@LYM_PER");
                    values.Add(result.LYM_PER);
                    sqlTypes.Add(SqlDbType.Float);

                }
                if (result.MID_PER != null)
                {
                    paramName.Add("@MID_PER");
                    values.Add(result.MID_PER);
                    sqlTypes.Add(SqlDbType.Float);

                }
                if (result.GRAN_PER != null)
                {
                    paramName.Add("@GRAN_PER");
                    values.Add(result.GRAN_PER);
                    sqlTypes.Add(SqlDbType.Float);

                }
#endregion
                if (idKetQuaTuMayCLS == "-1")//thêm mới
                {
                    string sql = string.Format(@"INSERT INTO dbo.KetQuaTuMayCLS( mabenhnhan ,TenBenhNhan ,ngaythuchien , maketqua ,loaimay , STT ,  mota    )
                                   VALUES( '{0}' ,N'{1}' ,'{2}' ,'{3}' ,'{4}' , '{5}' , N'{6}'  )",
                                   " ", result.Patient, result.Analyzed, result.SpecimenID, AppParams.cd1800Config.MaMayCLS, result.Sequence, "");
                    ServerConnect.I.BeginTran();
                    if (ServerConnect.I.ExecSQL(sql) == false)
                    {
                        ServerConnect.I.RollBack();
                        return false;
                    }
                    idKetQuaTuMayCLS = GetIdKetQuaTuMayCLS(result.SpecimenID);
                    string sqlCT = string.Format(@" INSERT INTO dbo.KetQuaTuMayCLS_CT ( idKetQuaTuMayCLS , tenthongso , giatri)
                                    VALUES  ( {0} , N'{1}' , {2})", idKetQuaTuMayCLS, AppParams.cd1800Config.WBC, result.WBC == null ? "NULL" : "@WBC");
                    sqlCT += Environment.NewLine + string.Format(@"INSERT INTO dbo.KetQuaTuMayCLS_CT ( idKetQuaTuMayCLS , tenthongso , giatri)
                                    VALUES  ( {0} , N'{1}' ,{2})", idKetQuaTuMayCLS, AppParams.cd1800Config.LYM, result.LYM == null ? "NULL" : "@LYM");
                    sqlCT += Environment.NewLine + string.Format(@"INSERT INTO dbo.KetQuaTuMayCLS_CT ( idKetQuaTuMayCLS , tenthongso , giatri)
                                    VALUES  ( {0} , N'{1}' ,{2} )", idKetQuaTuMayCLS, AppParams.cd1800Config.MID, result.MID == null ? "NULL" : "@MID");
                    sqlCT += Environment.NewLine + string.Format(@"INSERT INTO dbo.KetQuaTuMayCLS_CT ( idKetQuaTuMayCLS , tenthongso , giatri)
                                    VALUES  ( {0} , N'{1}' ,{2} )", idKetQuaTuMayCLS, AppParams.cd1800Config.GRAN, result.GRAN == null ? "NULL" : "@GRAN");
                    sqlCT += Environment.NewLine + string.Format(@"INSERT INTO dbo.KetQuaTuMayCLS_CT ( idKetQuaTuMayCLS , tenthongso , giatri)
                                    VALUES  ( {0} , N'{1}' ,{2} )", idKetQuaTuMayCLS, AppParams.cd1800Config.RBC, result.RBC == null ? "NULL" : "@RBC");
                    sqlCT += Environment.NewLine + string.Format(@"INSERT INTO dbo.KetQuaTuMayCLS_CT ( idKetQuaTuMayCLS , tenthongso , giatri)
                                    VALUES  ( {0} , N'{1}' ,{2} )", idKetQuaTuMayCLS, AppParams.cd1800Config.HGB, result.HGB == null ? "NULL" : "@HGB");
                    sqlCT += Environment.NewLine + string.Format(@"INSERT INTO dbo.KetQuaTuMayCLS_CT ( idKetQuaTuMayCLS , tenthongso , giatri)
                                    VALUES  ( {0} , N'{1}' ,{2} )", idKetQuaTuMayCLS, AppParams.cd1800Config.HCT, result.HCT == null ? "NULL" : "@HCT");
                    sqlCT += Environment.NewLine + string.Format(@"INSERT INTO dbo.KetQuaTuMayCLS_CT ( idKetQuaTuMayCLS , tenthongso , giatri)
                                    VALUES  ( {0} , N'{1}' ,{2} )", idKetQuaTuMayCLS, AppParams.cd1800Config.MCV, result.MCV == null ? "NULL" : "@MCV");
                    sqlCT += Environment.NewLine + string.Format(@"INSERT INTO dbo.KetQuaTuMayCLS_CT ( idKetQuaTuMayCLS , tenthongso , giatri)
                                    VALUES  ( {0} , N'{1}' ,{2} )", idKetQuaTuMayCLS, AppParams.cd1800Config.MCH, result.MCH == null ? "NULL" : "@MCH");
                    sqlCT += Environment.NewLine + string.Format(@"INSERT INTO dbo.KetQuaTuMayCLS_CT ( idKetQuaTuMayCLS , tenthongso , giatri)
                                    VALUES  ( {0} , N'{1}' ,{2} )", idKetQuaTuMayCLS, AppParams.cd1800Config.MCHC, result.MCHC == null ? "NULL" : "@MCHC");
                    sqlCT += Environment.NewLine + string.Format(@"INSERT INTO dbo.KetQuaTuMayCLS_CT ( idKetQuaTuMayCLS , tenthongso , giatri)
                                    VALUES  ( {0} , N'{1}' ,{2} )", idKetQuaTuMayCLS, AppParams.cd1800Config.RDW, result.RDW == null ? "NULL" : "@RDW");
                    sqlCT += Environment.NewLine + string.Format(@"INSERT INTO dbo.KetQuaTuMayCLS_CT ( idKetQuaTuMayCLS , tenthongso , giatri)
                                    VALUES  ( {0} , N'{1}' ,{2} )", idKetQuaTuMayCLS, AppParams.cd1800Config.PLT, result.PLT == null ? "NULL" : "@PLT");
                    sqlCT += Environment.NewLine + string.Format(@"INSERT INTO dbo.KetQuaTuMayCLS_CT ( idKetQuaTuMayCLS , tenthongso , giatri)
                                    VALUES  ( {0} , N'{1}' ,{2} )", idKetQuaTuMayCLS, AppParams.cd1800Config.MPV, result.MPV == null ? "NULL" : "@MPV");
                    sqlCT += Environment.NewLine + string.Format(@"INSERT INTO dbo.KetQuaTuMayCLS_CT ( idKetQuaTuMayCLS , tenthongso , giatri)
                                    VALUES  ( {0} , N'{1}' ,{2} )", idKetQuaTuMayCLS, AppParams.cd1800Config.PCT, result.PCT == null ? "NULL" : "@PCT");
                    sqlCT += Environment.NewLine + string.Format(@"INSERT INTO dbo.KetQuaTuMayCLS_CT ( idKetQuaTuMayCLS , tenthongso , giatri)
                                    VALUES  ( {0} , N'{1}' ,{2} )", idKetQuaTuMayCLS, AppParams.cd1800Config.PDW, result.PDW == null ? "NULL" : "@PDW");


                    sqlCT += Environment.NewLine + string.Format(@"INSERT INTO dbo.KetQuaTuMayCLS_CT ( idKetQuaTuMayCLS , tenthongso , giatri)
                                    VALUES  ( {0} , N'{1}' ,{2} )", idKetQuaTuMayCLS, AppParams.cd1800Config.LYMPer, result.LYM_PER == null ? "NULL" : "@LYM_PER");
                    sqlCT += Environment.NewLine + string.Format(@"INSERT INTO dbo.KetQuaTuMayCLS_CT ( idKetQuaTuMayCLS , tenthongso , giatri)
                                    VALUES  ( {0} , N'{1}' ,{2} )", idKetQuaTuMayCLS, AppParams.cd1800Config.MIDPer, result.MID_PER == null ? "NULL" : "@MID_PER");
                    sqlCT += Environment.NewLine + string.Format(@"INSERT INTO dbo.KetQuaTuMayCLS_CT ( idKetQuaTuMayCLS , tenthongso , giatri)
                                    VALUES  ( {0} , N'{1}' ,{2} )", idKetQuaTuMayCLS, AppParams.cd1800Config.GRANPer, result.GRAN_PER == null ? "NULL" : "@GRAN_PER");


                    if (ServerConnect.I.Exec(sqlCT, paramName.ToArray(), values.ToArray(), sqlTypes.ToArray()) <= 0)
                    {
                        ServerConnect.I.RollBack();
                        return false;
                    }
                    ServerConnect.I.Commit();
                }
                else
                {
                    string sql = string.Format(@"UPDATE dbo.KetQuaTuMayCLS_CT SET giatri={0} WHERE idKetQuaTuMayCLS={1} AND tenthongso='{2}'",
                                               result.WBC == null ? "NULL" : "@WBC", idKetQuaTuMayCLS, AppParams.cd1800Config.WBC);
                    sql += Environment.NewLine + string.Format(@"UPDATE dbo.KetQuaTuMayCLS_CT SET giatri={0} WHERE idKetQuaTuMayCLS={1} AND tenthongso='{2}'",
                                               result.LYM == null ? "NULL" : "@LYM", idKetQuaTuMayCLS, AppParams.cd1800Config.LYM);
                    sql += Environment.NewLine + string.Format(@"UPDATE dbo.KetQuaTuMayCLS_CT SET giatri={0} WHERE idKetQuaTuMayCLS={1} AND tenthongso='{2}'",
                                              result.MID == null ? "NULL" : "@MID", idKetQuaTuMayCLS, AppParams.cd1800Config.MID);
                    sql += Environment.NewLine + string.Format(@"UPDATE dbo.KetQuaTuMayCLS_CT SET giatri={0} WHERE idKetQuaTuMayCLS={1} AND tenthongso='{2}'",
                                              result.GRAN == null ? "NULL" : "@GRAN", idKetQuaTuMayCLS, AppParams.cd1800Config.GRAN);
                    sql += Environment.NewLine + string.Format(@"UPDATE dbo.KetQuaTuMayCLS_CT SET giatri={0} WHERE idKetQuaTuMayCLS={1} AND tenthongso='{2}'",
                                               result.RBC == null ? "NULL" : "@RBC", idKetQuaTuMayCLS, AppParams.cd1800Config.RBC);
                    sql += Environment.NewLine + string.Format(@"UPDATE dbo.KetQuaTuMayCLS_CT SET giatri={0} WHERE idKetQuaTuMayCLS={1} AND tenthongso='{2}'",
                                              result.HGB == null ? "NULL" : "@HGB", idKetQuaTuMayCLS, AppParams.cd1800Config.HGB);
                    sql += Environment.NewLine + string.Format(@"UPDATE dbo.KetQuaTuMayCLS_CT SET giatri={0} WHERE idKetQuaTuMayCLS={1} AND tenthongso='{2}'",
                                              result.HCT == null ? "NULL" : "@HCT", idKetQuaTuMayCLS, AppParams.cd1800Config.HCT);
                    sql += Environment.NewLine + string.Format(@"UPDATE dbo.KetQuaTuMayCLS_CT SET giatri={0} WHERE idKetQuaTuMayCLS={1} AND tenthongso='{2}'",
                                               result.MCV == null ? "NULL" : "@MCV", idKetQuaTuMayCLS, AppParams.cd1800Config.MCV);
                    sql += Environment.NewLine + string.Format(@"UPDATE dbo.KetQuaTuMayCLS_CT SET giatri={0} WHERE idKetQuaTuMayCLS={1} AND tenthongso='{2}'",
                                               result.MCH == null ? "NULL" : "@MCH", idKetQuaTuMayCLS, AppParams.cd1800Config.MCH);
                    sql += Environment.NewLine + string.Format(@"UPDATE dbo.KetQuaTuMayCLS_CT SET giatri={0} WHERE idKetQuaTuMayCLS={1} AND tenthongso='{2}'",
                                               result.MCHC == null ? "NULL" : "@MCHC", idKetQuaTuMayCLS, AppParams.cd1800Config.MCHC);
                    sql += Environment.NewLine + string.Format(@"UPDATE dbo.KetQuaTuMayCLS_CT SET giatri={0} WHERE idKetQuaTuMayCLS={1} AND tenthongso='{2}'",
                                               result.RDW == null ? "NULL" : "@RDW", idKetQuaTuMayCLS, AppParams.cd1800Config.RDW);
                    sql += Environment.NewLine + string.Format(@"UPDATE dbo.KetQuaTuMayCLS_CT SET giatri={0} WHERE idKetQuaTuMayCLS={1} AND tenthongso='{2}'",
                                              result.PLT == null ? "NULL" : "@PLT", idKetQuaTuMayCLS, AppParams.cd1800Config.PLT);
                    sql += Environment.NewLine + string.Format(@"UPDATE dbo.KetQuaTuMayCLS_CT SET giatri={0} WHERE idKetQuaTuMayCLS={1} AND tenthongso='{2}'",
                                              result.MPV == null ? "NULL" : "@MPV", idKetQuaTuMayCLS, AppParams.cd1800Config.MPV);
                    sql += Environment.NewLine + string.Format(@"UPDATE dbo.KetQuaTuMayCLS_CT SET giatri={0} WHERE idKetQuaTuMayCLS={1} AND tenthongso='{2}'",
                                              result.PCT == null ? "NULL" : "@PCT", idKetQuaTuMayCLS, AppParams.cd1800Config.PCT);
                    sql += Environment.NewLine + string.Format(@"UPDATE dbo.KetQuaTuMayCLS_CT SET giatri={0} WHERE idKetQuaTuMayCLS={1} AND tenthongso='{2}'",
                                             result.PDW == null ? "NULL" : "@PDW", idKetQuaTuMayCLS, AppParams.cd1800Config.PDW);

                    sql += Environment.NewLine + string.Format(@"UPDATE dbo.KetQuaTuMayCLS_CT SET giatri={0} WHERE idKetQuaTuMayCLS={1} AND tenthongso='{2}'",
                                              result.LYM_PER == null ? "NULL" : "@LYM_PER", idKetQuaTuMayCLS, AppParams.cd1800Config.LYMPer);
                    sql += Environment.NewLine + string.Format(@"UPDATE dbo.KetQuaTuMayCLS_CT SET giatri={0} WHERE idKetQuaTuMayCLS={1} AND tenthongso='{2}'",
                                             result.MID_PER == null ? "NULL" : "@MID_PER", idKetQuaTuMayCLS, AppParams.cd1800Config.MIDPer);
                    sql += Environment.NewLine + string.Format(@"UPDATE dbo.KetQuaTuMayCLS_CT SET giatri={0} WHERE idKetQuaTuMayCLS={1} AND tenthongso='{2}'",
                                             result.GRAN_PER == null ? "NULL" : "@GRAN_PER", idKetQuaTuMayCLS, AppParams.cd1800Config.GRANPer);

                    if (ServerConnect.I.Exec(sql, paramName.ToArray(), values.ToArray(), sqlTypes.ToArray()) <= 0)
                    {
                        return false;
                    }

                }

                return true;

            }
            catch
            {
                return false;
            }
        }
        private string GetIdKetQuaTuMayCLS(string SpecimenID)
        {
            string idKetQuaTuMayCLS = "-1";
            DataTable dt = ServerConnect.I.GetTable(string.Format(@"SELECT top 1 idKetQuaTuMayCLS FROM dbo.KetQuaTuMayCLS
                WHERE maketqua='{0}' AND loaimay='{1}' order by ngaythuchien desc", SpecimenID, AppParams.cd1800Config.MaMayCLS));
            if (dt != null && dt.Rows.Count > 0)
            {
                idKetQuaTuMayCLS = dt.Rows[0][0].ToString(); ;
            }
            return idKetQuaTuMayCLS;
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
