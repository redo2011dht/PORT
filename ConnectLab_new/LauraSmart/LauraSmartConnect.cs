using System;
using System.Collections.Generic;
using System.Text;
using System.Configuration;
using System.IO.Ports;
using System.Data;

namespace ConnectLab
{
    public class LauraSmartConnect:IDevice
    {
        private static LauraSmartConnect currentConnect;
        public static LauraSmartConnect CurrentConnect
        {
            get
            {
                return LauraSmartConnect.currentConnect;
            }
            set
            {
                LauraSmartConnect.currentConnect = value;
            }
        }


        SerialPort port;
        public LauraSmartConnect()
        {
            port = new SerialPort();
            port.ReadTimeout = 1000;
            port.DataReceived += new SerialDataReceivedEventHandler(DataReceive);
            Connect();
        }
        // string InputData = "\"   \",\" CD1800\",\"------------\",0088,00,\"---\",\"11/01/11\",\"02:50\",\"         117\",\"    TRAN THI COI\",\" \",\"  /  /  \",\"    \",\"  /  \",\"  :  \",\"       \",00058,-----,00011,00009,00038,-----,00521,00109,00392,00753,00209,00278,00140,00160,00109,00017,00179,-----,00182,00160,00658,-----,-,-,0,0,0,0,1,0,1,-,-,-,-,-,-,-,-,-,-,-,-,-,00,00,04817,02130,06326,04723,00000,00000,1,\"O\",0,0,0,1,27";
        string[] result;
        string InputData = "";
        string tem = "";

        // MEDILAB

        //DekaPHAN LAURA
        //Seq.No:  0001
        //Pat.ID:  03122011101

        //2011.12.03       8:40
        //........................

        // BLD    NEG
        // LEU    NEG
        // BIL    NEG
        // UBG   NORM
        // KET    NEG
        // GLU    NEG
        // PRO    NEG
        // pH     6.5
        // NIT    NEG
        // SG   1.015
        //------------------------ 22 dòng

        private void DataReceive(object obj, SerialDataReceivedEventArgs e)
        {
            try
            {
                tem = port.ReadExisting();
                InputData += tem;
                if (tem.StartsWith(""))
                {
                    LogProgram.WriteBeginTranfer();
                    frmMain.formMain.m_notifyicon.ShowBalloonTip(3, "MKV", "Đang nhận kết quả từ máy Laura Smart", System.Windows.Forms.ToolTipIcon.Info);

                }
                if (tem.EndsWith(""))
                {
                    InputData = InputData.TrimStart('').TrimEnd('');
                    InputData = InputData.Replace("", "$");

                    string[] temp = InputData.Split('$');
                    foreach (string t in temp)
                    {

                        LauraSmartResult re = new LauraSmartResult(t);
                        if (UpdateToSQL(re) == false)
                        {
                            //LogProgram.WriteSaveFail(re);
                            frmMain.formMain.m_notifyicon.ShowBalloonTip(3, "MKV", "Lưu kết quả '" + re.ID + "' từ máy Laura Smartkhông thành công", System.Windows.Forms.ToolTipIcon.Error);

                        }
                        else
                        {
                            //LogProgram.WriteSaveSucess(re);
                            frmMain.formMain.m_notifyicon.ShowBalloonTip(3, "MKV", "Đã lưu kết quả '" + re.ID + "' từ máy Laura Smart", System.Windows.Forms.ToolTipIcon.Info);

                        }
                    }
                    LogProgram.WriteEndTranfer();
                    tem = "";
                    InputData = "";
                }
            }
            catch(Exception ex)
            {
                LogProgram.WriteLog("[Laura]:", ex.ToString());
            }
        }








        public bool Connect()
        {
            try
            {
                if (port.IsOpen)
                    port.Close();
                port.PortName = AppParams.lauraSmartConfig.PortName;
                port.BaudRate = Convert.ToInt32(AppParams.lauraSmartConfig.BaudRate);
                port.DataBits = Convert.ToInt32(AppParams.lauraSmartConfig.DataBits);
                switch (AppParams.lauraSmartConfig.ParityType)
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
                switch (AppParams.lauraSmartConfig.StopBit)
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

        private bool UpdateToSQL(LauraSmartResult result)
        {
            try
            {
                string idKetQuaTuMayCLS = GetIdKetQuaTuMayCLS(result.ID);
                List<string> paramName = new List<string>();
                List<object> values = new List<object>();
                List<SqlDbType> sqlTypes = new List<SqlDbType>();


                if (result.BLD != null)
                {
                    paramName.Add("@BLD");
                    values.Add(result.BLD.Value);
                    sqlTypes.Add(SqlDbType.Float);
                }
                if (result.LEU != null)
                {
                    paramName.Add("@LEU");
                    values.Add(result.LEU.Value);
                    sqlTypes.Add(SqlDbType.Float);
                }
                if (result.BIL != null)
                {
                    paramName.Add("@BIL");
                    values.Add(result.BIL.Value);
                    sqlTypes.Add(SqlDbType.Float);
                }
                if (result.UBG != null)
                {
                    paramName.Add("@UBG");
                    values.Add(result.UBG.Value);
                    sqlTypes.Add(SqlDbType.Float);
                }
                if (result.KET != null)
                {
                    paramName.Add("@KET");
                    values.Add(result.KET.Value);
                    sqlTypes.Add(SqlDbType.Float);
                }
                if (result.GLU != null)
                {
                    paramName.Add("@GLU");
                    values.Add(result.GLU.Value);
                    sqlTypes.Add(SqlDbType.Float);
                }
                if (result.PRO != null)
                {
                    paramName.Add("@PRO");
                    values.Add(result.PRO.Value);
                    sqlTypes.Add(SqlDbType.Float);
                }
                if (result.PH != null)
                {
                    paramName.Add("@PH");
                    values.Add(result.PH.Value);
                    sqlTypes.Add(SqlDbType.Float);
                }
                if (result.NIT != null)
                {
                    paramName.Add("@NIT");
                    values.Add(result.NIT.Value);
                    sqlTypes.Add(SqlDbType.Float);
                }
                if (result.SG != null)
                {
                    paramName.Add("@SG");
                    values.Add(result.SG.Value);
                    sqlTypes.Add(SqlDbType.Float);
                }
                if (idKetQuaTuMayCLS == "-1")//thêm mới
                {
                    string sql = string.Format(@"INSERT INTO dbo.KetQuaTuMayCLS( mabenhnhan ,TenBenhNhan ,ngaythuchien , maketqua ,loaimay , STT ,  mota    )
                                   VALUES( '{0}' ,N'{1}' ,'{2}' ,'{3}' ,'{4}' , '{5}' , N'{6}'  )",
                                   " ", result.Patient, result.DateExcute, result.ID, AppParams.lauraSmartConfig.MaMayCLS, result.Sequence, "");
                    ServerConnect.I.BeginTran();
                    if (ServerConnect.I.ExecSQL(sql) == false)
                    {
                        ServerConnect.I.RollBack();
                        return false;
                    }
                    idKetQuaTuMayCLS = GetIdKetQuaTuMayCLS(result.ID);
                    string sqlCT = string.Format(@" INSERT INTO dbo.KetQuaTuMayCLS_CT ( idKetQuaTuMayCLS , tenthongso , giatri, giatrichuoi)
                                    VALUES  ( {0} , N'{1}' , {2},{3})", idKetQuaTuMayCLS, AppParams.lauraSmartConfig.BLD, result.BLD == null ? "NULL" : "@BLD", string.IsNullOrEmpty(result.BLDStr) ? "NULL" : "'" + result.BLDStr + "'");
                    sqlCT += Environment.NewLine + string.Format(@"INSERT INTO dbo.KetQuaTuMayCLS_CT ( idKetQuaTuMayCLS , tenthongso , giatri, giatrichuoi)
                                    VALUES  ( {0} , N'{1}' ,{2},{3})", idKetQuaTuMayCLS, AppParams.lauraSmartConfig.LEU, result.LEU == null ? "NULL" : "@LEU", string.IsNullOrEmpty(result.LEUStr) ? "NULL" : "'" + result.LEUStr + "'");
                    sqlCT += Environment.NewLine + string.Format(@"INSERT INTO dbo.KetQuaTuMayCLS_CT ( idKetQuaTuMayCLS , tenthongso , giatri, giatrichuoi)
                                    VALUES  ( {0} , N'{1}' ,{2},{3} )", idKetQuaTuMayCLS, AppParams.lauraSmartConfig.BIL, result.BIL == null ? "NULL" : "@BIL", string.IsNullOrEmpty(result.BILStr) ? "NULL" : "'" + result.BILStr + "'");
                    sqlCT += Environment.NewLine + string.Format(@"INSERT INTO dbo.KetQuaTuMayCLS_CT ( idKetQuaTuMayCLS , tenthongso , giatri, giatrichuoi)
                                    VALUES  ( {0} , N'{1}' ,{2},{3} )", idKetQuaTuMayCLS, AppParams.lauraSmartConfig.UBG, result.UBG == null ? "NULL" : "@UBG", string.IsNullOrEmpty(result.UBGStr) ? "NULL" : "'" + result.UBGStr + "'");
                    sqlCT += Environment.NewLine + string.Format(@"INSERT INTO dbo.KetQuaTuMayCLS_CT ( idKetQuaTuMayCLS , tenthongso , giatri, giatrichuoi)
                                    VALUES  ( {0} , N'{1}' ,{2},{3} )", idKetQuaTuMayCLS, AppParams.lauraSmartConfig.KET, result.KET == null ? "NULL" : "@KET", string.IsNullOrEmpty(result.KETStr) ? "NULL" : "'" + result.KETStr + "'");
                    sqlCT += Environment.NewLine + string.Format(@"INSERT INTO dbo.KetQuaTuMayCLS_CT ( idKetQuaTuMayCLS , tenthongso , giatri, giatrichuoi)
                                    VALUES  ( {0} , N'{1}' ,{2},{3} )", idKetQuaTuMayCLS, AppParams.lauraSmartConfig.GLU, result.GLU == null ? "NULL" : "@GLU", string.IsNullOrEmpty(result.GLUStr) ? "NULL" : "'" + result.GLUStr + "'");
                    sqlCT += Environment.NewLine + string.Format(@"INSERT INTO dbo.KetQuaTuMayCLS_CT ( idKetQuaTuMayCLS , tenthongso , giatri, giatrichuoi)
                                    VALUES  ( {0} , N'{1}' ,{2},{3} )", idKetQuaTuMayCLS, AppParams.lauraSmartConfig.PRO, result.PRO == null ? "NULL" : "@PRO", string.IsNullOrEmpty(result.PROStr) ? "NULL" : "'" + result.PROStr + "'");
                    sqlCT += Environment.NewLine + string.Format(@"INSERT INTO dbo.KetQuaTuMayCLS_CT ( idKetQuaTuMayCLS , tenthongso , giatri, giatrichuoi)
                                    VALUES  ( {0} , N'{1}' ,{2},{3} )", idKetQuaTuMayCLS, AppParams.lauraSmartConfig.PH, result.PH == null ? "NULL" : "@PH", string.IsNullOrEmpty(result.PHStr) ? "NULL" : "'" + result.PHStr + "'");
                    sqlCT += Environment.NewLine + string.Format(@"INSERT INTO dbo.KetQuaTuMayCLS_CT ( idKetQuaTuMayCLS , tenthongso , giatri, giatrichuoi)
                                    VALUES  ( {0} , N'{1}' ,{2},{3} )", idKetQuaTuMayCLS, AppParams.lauraSmartConfig.NIT, result.NIT == null ? "NULL" : "@NIT", string.IsNullOrEmpty(result.NITStr) ? "NULL" : "'" + result.NITStr + "'");
                    sqlCT += Environment.NewLine + string.Format(@"INSERT INTO dbo.KetQuaTuMayCLS_CT ( idKetQuaTuMayCLS , tenthongso , giatri, giatrichuoi)
                                    VALUES  ( {0} , N'{1}' ,{2},{3} )", idKetQuaTuMayCLS, AppParams.lauraSmartConfig.SG, result.SG == null ? "NULL" : "@SG", string.IsNullOrEmpty(result.SGStr) ? "NULL" : "'" + result.SGStr + "'");



                    if (ServerConnect.I.Exec(sqlCT, paramName.ToArray(), values.ToArray(), sqlTypes.ToArray()) <= 0)
                    {
                        ServerConnect.I.RollBack();
                        return false;
                    }
                    ServerConnect.I.Commit();
                }
                else
                {
                    string sql = string.Format(@"UPDATE dbo.KetQuaTuMayCLS_CT SET giatri={0}, giatrichuoi={3} WHERE idKetQuaTuMayCLS={1} AND tenthongso='{2}'",
                                               result.BLD == null ? "NULL" : "@BLD", idKetQuaTuMayCLS, AppParams.lauraSmartConfig.BLD, string.IsNullOrEmpty(result.BLDStr) ? "NULL" : "'" + result.BLDStr + "'");
                    sql += Environment.NewLine + string.Format(@"UPDATE dbo.KetQuaTuMayCLS_CT SET giatri={0}, giatrichuoi={3}  WHERE idKetQuaTuMayCLS={1} AND tenthongso='{2}'",
                                              result.LEU == null ? "NULL" : "@LEU", idKetQuaTuMayCLS, AppParams.lauraSmartConfig.LEU, string.IsNullOrEmpty(result.LEUStr) ? "NULL" : "'" + result.LEUStr + "'");
                    sql += Environment.NewLine + string.Format(@"UPDATE dbo.KetQuaTuMayCLS_CT SET giatri={0}, giatrichuoi={3}  WHERE idKetQuaTuMayCLS={1} AND tenthongso='{2}'",
                                              result.BIL == null ? "NULL" : "@BIL", idKetQuaTuMayCLS, AppParams.lauraSmartConfig.BIL, string.IsNullOrEmpty(result.BILStr) ? "NULL" : "'" + result.BILStr + "'");
                    sql += Environment.NewLine + string.Format(@"UPDATE dbo.KetQuaTuMayCLS_CT SET giatri={0}, giatrichuoi={3}  WHERE idKetQuaTuMayCLS={1} AND tenthongso='{2}'",
                                              result.UBG == null ? "NULL" : "@UBG", idKetQuaTuMayCLS, AppParams.lauraSmartConfig.UBG, string.IsNullOrEmpty(result.UBGStr) ? "NULL" : "'" + result.UBGStr + "'");
                    sql += Environment.NewLine + string.Format(@"UPDATE dbo.KetQuaTuMayCLS_CT SET giatri={0}, giatrichuoi={3}  WHERE idKetQuaTuMayCLS={1} AND tenthongso='{2}'",
                                             result.KET == null ? "NULL" : "@KET", idKetQuaTuMayCLS, AppParams.lauraSmartConfig.KET, string.IsNullOrEmpty(result.KETStr) ? "NULL" : "'" + result.KETStr + "'");
                    sql += Environment.NewLine + string.Format(@"UPDATE dbo.KetQuaTuMayCLS_CT SET giatri={0}, giatrichuoi={3}  WHERE idKetQuaTuMayCLS={1} AND tenthongso='{2}'",
                                              result.GLU == null ? "NULL" : "@GLU", idKetQuaTuMayCLS, AppParams.lauraSmartConfig.GLU, string.IsNullOrEmpty(result.GLUStr) ? "NULL" : "'" + result.GLUStr + "'");
                    sql += Environment.NewLine + string.Format(@"UPDATE dbo.KetQuaTuMayCLS_CT SET giatri={0}, giatrichuoi={3}  WHERE idKetQuaTuMayCLS={1} AND tenthongso='{2}'",
                                              result.PRO == null ? "NULL" : "@PRO", idKetQuaTuMayCLS, AppParams.lauraSmartConfig.PRO, string.IsNullOrEmpty(result.PROStr) ? "NULL" : "'" + result.PROStr + "'");
                    sql += Environment.NewLine + string.Format(@"UPDATE dbo.KetQuaTuMayCLS_CT SET giatri={0}, giatrichuoi={3}  WHERE idKetQuaTuMayCLS={1} AND tenthongso='{2}'",
                                              result.PH == null ? "NULL" : "@PH", idKetQuaTuMayCLS, AppParams.lauraSmartConfig.PH, string.IsNullOrEmpty(result.PHStr) ? "NULL" : "'" + result.PHStr + "'");
                    sql += Environment.NewLine + string.Format(@"UPDATE dbo.KetQuaTuMayCLS_CT SET giatri={0}, giatrichuoi={3}  WHERE idKetQuaTuMayCLS={1} AND tenthongso='{2}'",
                                               result.NIT == null ? "NULL" : "@NIT", idKetQuaTuMayCLS, AppParams.lauraSmartConfig.NIT, string.IsNullOrEmpty(result.NITStr) ? "NULL" : "'" + result.NITStr + "'");
                    sql += Environment.NewLine + string.Format(@"UPDATE dbo.KetQuaTuMayCLS_CT SET giatri={0}, giatrichuoi={3}  WHERE idKetQuaTuMayCLS={1} AND tenthongso='{2}'",
                                              result.SG == null ? "NULL" : "@SG", idKetQuaTuMayCLS, AppParams.lauraSmartConfig.SG, string.IsNullOrEmpty(result.SGStr) ? "NULL" : "'" + result.SGStr + "'");


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
                WHERE maketqua='{0}' AND loaimay='{1}' order by ngaythuchien desc", SpecimenID, AppParams.lauraSmartConfig.MaMayCLS));
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
