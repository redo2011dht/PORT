using System;
using System.Collections.Generic;
using System.Text;
using System.Configuration;
using System.IO.Ports;
using System.Data;
using System.IO;

namespace ConnectLab
{
    public class StagoStart4Connect:IDevice
    {
        private static StagoStart4Connect currentConnect;
        public static StagoStart4Connect CurrentConnect
        {
            get
            {
                return StagoStart4Connect.currentConnect;
            }
            set
            {
                StagoStart4Connect.currentConnect = value;
            }
        }


        SerialPort port;
        public StagoStart4Connect()
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

        //ST4  TR 1 1INR    13.281 2          14S  14.500  83.400   1.120          15S  15.700  71.600   1.240m

        private void DataReceive(object obj, SerialDataReceivedEventArgs e)
        {

            try
            {
                tem = port.ReadExisting();
                LogProgram.WriteLog("[Start4: COM]: ", tem);
                if (tem.Length > 20)
                {
                    InputData += tem.Trim();
                }
                else
                {
                    if (InputData.Trim().EndsWith("ST4"))
                    {
                        InputData += "  " + tem.Trim();
                    }
                    else
                    {
                        if (tem.EndsWith("S") && !tem.EndsWith("S"))
                        {
                            InputData += tem.ToString() + " ";
                        }
                        else if (tem.EndsWith("INR"))
                            InputData += tem.ToString() + " ";
                        else
                            InputData += tem.ToString();
                    }
                }
                //if (tem.Trim().ToString()!="")
                //{
                //    InputData = InputData.Trim();
                //}
                string time = DateTime.Now.ToString("ddMMyyyy") + DateTime.Now.Minute.ToString()+DateTime.Now.Second.ToString();
                if (tem.StartsWith(""))
                {
                    //WriteFile("NhanDuLieuLuc" + time + ".txt",InputData);
                   // File.Create("Step1.txt");
                    LogProgram.WriteBeginTranfer();
                    frmMain.formMain.m_notifyicon.ShowBalloonTip(3, "MKV", "Đang nhận kết quả từ máy Stago Start 4", System.Windows.Forms.ToolTipIcon.Info);

                }
                if (tem.Trim().TrimEnd('\0').EndsWith(""))
                {
                    string a = InputData.Trim().ToString();
                    InputData = a;
                    LogProgram.WriteLog("Stago Result:", InputData);
                    string[] temp = InputData.Split(new char[] { '', '' }, StringSplitOptions.RemoveEmptyEntries);
                    //WriteFile("DuLieuXuliLuc" + time + ".txt", InputData.ToString());
                    foreach (string t in temp)
                    {
                        StagoStart4Result re = new StagoStart4Result(t);
                        if (UpdateToSQL(re) == false)
                        {
                            //WriteFile("XuliThatBaiLuc" + time + ".txt", InputData.ToString());
                            //LogProgram.WriteSaveFail(re);
                            frmMain.formMain.m_notifyicon.ShowBalloonTip(3, "MKV", "Lưu kết quả '" + re.ID + "' từ máy Stago Start 4 không thành công", System.Windows.Forms.ToolTipIcon.Error);

                        }
                        else
                        {
                            //WriteFile("XuliThanhCongLuc" + time + ".txt", InputData.ToString());
                            //LogProgram.WriteSaveSucess(re);
                            frmMain.formMain.m_notifyicon.ShowBalloonTip(3, "MKV", "Đã lưu kết quả '" + re.ID + "' từ máy Stago Start 4", System.Windows.Forms.ToolTipIcon.Info);

                        }
                    }
                    LogProgram.WriteEndTranfer();
                    tem = "";
                    InputData = "";
                }
            }
            catch(Exception ex)
            {
                LogProgram.WriteLog("[Start4:] ", ex.ToString());
                tem = "";
                InputData = "";
            }
           
            
        }
        public void test(string a)
        {
            try
            {
                tem = port.ReadExisting();
                LogProgram.WriteLog("[Start4: COM]: ", tem);
                InputData += tem.Trim();
                if (tem.StartsWith(""))
                {
                    File.Create("Step1.txt");
                    LogProgram.WriteBeginTranfer();
                    frmMain.formMain.m_notifyicon.ShowBalloonTip(3, "MKV", "Đang nhận kết quả từ máy Stago Start 4", System.Windows.Forms.ToolTipIcon.Info);

                }
                if (tem.Trim().TrimEnd('\0').EndsWith(""))
                {
                    File.Create("Step2.txt");
                    InputData = InputData.TrimStart('').TrimEnd('\0', '');
                    InputData = InputData.Replace("", "$");

                    string[] temp = InputData.Split('$');
                    foreach (string t in temp)
                    {

                        StagoStart4Result re = new StagoStart4Result(t);
                        if (UpdateToSQL(re) == false)
                        {
                            File.Create("Step3.txt");
                            //LogProgram.WriteSaveFail(re);
                            frmMain.formMain.m_notifyicon.ShowBalloonTip(3, "MKV", "Lưu kết quả '" + re.ID + "' từ máy Stago Start 4 không thành công", System.Windows.Forms.ToolTipIcon.Error);

                        }
                        else
                        {
                            //LogProgram.WriteSaveSucess(re);
                            frmMain.formMain.m_notifyicon.ShowBalloonTip(3, "MKV", "Đã lưu kết quả '" + re.ID + "' từ máy Stago Start 4", System.Windows.Forms.ToolTipIcon.Info);

                        }
                    }
                    LogProgram.WriteEndTranfer();
                    tem = "";
                    InputData = "";
                }
            }
            catch (Exception ex)
            {
                LogProgram.WriteLog("[Start4:] ", ex.ToString());
            }
        }
        public bool Connect()
        {
            try
            {
                if (port.IsOpen)
                    port.Close();
                port.PortName = AppParams.stagoStart4Config.PortName;
                port.BaudRate = Convert.ToInt32(AppParams.stagoStart4Config.BaudRate);
                port.DataBits = Convert.ToInt32(AppParams.stagoStart4Config.DataBits);
                switch (AppParams.stagoStart4Config.ParityType)
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
                switch (AppParams.stagoStart4Config.StopBit)
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
                port.DtrEnable = true;
                
                
                if (port.IsOpen)
                {
                    //File.Create("OK.txt");
                    //WriteFile("KetNoi"+DateTime.Now.ToString("ddMMyyyy")+".txt","Da nhan cong COM vao luc : "+DateTime.Now.ToString());
                }
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

        public bool UpdateToSQL(StagoStart4Result result)
        {
            try
            {
               
                while (result != null)
                {
                    ServerConnect.I.BeginTran();
                    string idkqua = DateTime.Today.ToString("yyMMdd") + "-" + result.ID;
                    string idKetQuaTuMayCLS = GetIdKetQuaTuMayCLS(idkqua);
                    List<string> paramName = new List<string>();
                    List<object> values = new List<object>();
                    List<SqlDbType> sqlTypes = new List<SqlDbType>();

                    if (result.FIBRINOGENE != null)
                    {
                        paramName.Add("@FIB");
                        values.Add(result.FIBRINOGENE.Value);
                        sqlTypes.Add(SqlDbType.Float);
                    }
                    if (result.INR != null)
                    {
                        paramName.Add("@INR");
                        values.Add(result.INR.Value);
                        sqlTypes.Add(SqlDbType.Float);
                    }
                    if (result.PROTHROMBINE != null)
                    {
                        paramName.Add("@PROT");
                        values.Add(result.PROTHROMBINE.Value);
                        sqlTypes.Add(SqlDbType.Float);
                    }
                    if (result.TCK != null)
                    {
                        paramName.Add("@TCK");
                        values.Add(result.TCK.Value);
                        sqlTypes.Add(SqlDbType.Float);
                    }
                    if (result.TQ!= null)
                    {
                        paramName.Add("@TQ");
                        values.Add(result.TQ.Value);
                        sqlTypes.Add(SqlDbType.Float);
                    }
                    if (idKetQuaTuMayCLS == "-1")//thêm mới
                    {
                        string sql = string.Format(@"INSERT INTO dbo.KetQuaTuMayCLS( mabenhnhan ,TenBenhNhan ,ngaythuchien , maketqua ,loaimay , STT ,  mota    )
                                   VALUES( '{0}' ,N'{1}' ,{2} ,'{3}' ,'{4}' , '{5}' , N'{6}'  )",
                                       " ", "", "GetDate()", idkqua, AppParams.stagoStart4Config.MaMayCLS, "", result.TypeName);
                        if (ServerConnect.I.ExecSQL(sql) == false)
                        {
                            ServerConnect.I.RollBack();
                            return false;
                        }
                        idKetQuaTuMayCLS = GetIdKetQuaTuMayCLS(idkqua);
                        string sqlCT = "";
                        if (result.FIBRINOGENE != null)
                            sqlCT += string.Format(@" INSERT INTO dbo.KetQuaTuMayCLS_CT ( idKetQuaTuMayCLS , tenthongso , giatri, giatrichuoi)
                                    VALUES  ( {0} , N'{1}' , {2},{3})", idKetQuaTuMayCLS, AppParams.stagoStart4Config.FIBRINOGENE, "@FIB", "NULL");
                        if (result.INR != null)
                            sqlCT += Environment.NewLine + string.Format(@"INSERT INTO dbo.KetQuaTuMayCLS_CT ( idKetQuaTuMayCLS , tenthongso , giatri, giatrichuoi)
                                    VALUES  ( {0} , N'{1}' ,{2},{3})", idKetQuaTuMayCLS, AppParams.stagoStart4Config.INR, "@INR", "NULL");
                        if (result.PROTHROMBINE != null)
                            sqlCT += Environment.NewLine + string.Format(@"INSERT INTO dbo.KetQuaTuMayCLS_CT ( idKetQuaTuMayCLS , tenthongso , giatri, giatrichuoi)
                                    VALUES  ( {0} , N'{1}' ,{2},{3} )", idKetQuaTuMayCLS, AppParams.stagoStart4Config.PROTHROMBINE, "@PROT", "NULL");
                        if (result.TCK != null)
                            sqlCT += Environment.NewLine + string.Format(@"INSERT INTO dbo.KetQuaTuMayCLS_CT ( idKetQuaTuMayCLS , tenthongso , giatri, giatrichuoi)
                                    VALUES  ( {0} , N'{1}' ,{2},{3} )", idKetQuaTuMayCLS, AppParams.stagoStart4Config.TCK, "@TCK", "NULL");
                        if (result.TQ != null)
                            sqlCT += Environment.NewLine + string.Format(@"INSERT INTO dbo.KetQuaTuMayCLS_CT ( idKetQuaTuMayCLS , tenthongso , giatri, giatrichuoi)
                                    VALUES  ( {0} , N'{1}' ,{2},{3} )", idKetQuaTuMayCLS, AppParams.stagoStart4Config.TQ, "@TQ", "NULL");
                        if (ServerConnect.I.Exec(sqlCT, paramName.ToArray(), values.ToArray(), sqlTypes.ToArray()) <= 0)
                        {
                            ServerConnect.I.RollBack();
                            return false;
                        }

                    }
                    else
                    {
                        string sql = string.Format(@"UPDATE dbo.KetQuaTuMayCLS_CT SET giatri={0} WHERE idKetQuaTuMayCLS={1} AND tenthongso='{2}'",
                                                   result.FIBRINOGENE == null ? "NULL" : "@FIB", idKetQuaTuMayCLS, AppParams.stagoStart4Config.FIBRINOGENE);
                        sql += Environment.NewLine + string.Format(@"UPDATE dbo.KetQuaTuMayCLS_CT SET giatri={0}  WHERE idKetQuaTuMayCLS={1} AND tenthongso='{2}'",
                                                  result.INR == null ? "NULL" : "@INR", idKetQuaTuMayCLS, AppParams.stagoStart4Config.INR);
                        sql += Environment.NewLine + string.Format(@"UPDATE dbo.KetQuaTuMayCLS_CT SET giatri={0}  WHERE idKetQuaTuMayCLS={1} AND tenthongso='{2}'",
                                                  result.PROTHROMBINE == null ? "NULL" : "@PROT", idKetQuaTuMayCLS, AppParams.stagoStart4Config.PROTHROMBINE);
                        sql += Environment.NewLine + string.Format(@"UPDATE dbo.KetQuaTuMayCLS_CT SET giatri={0}  WHERE idKetQuaTuMayCLS={1} AND tenthongso='{2}'",
                                                  result.TCK == null ? "NULL" : "@TCK", idKetQuaTuMayCLS, AppParams.stagoStart4Config.TCK);
                        sql += Environment.NewLine + string.Format(@"UPDATE dbo.KetQuaTuMayCLS_CT SET giatri={0}  WHERE idKetQuaTuMayCLS={1} AND tenthongso='{2}'",
                                                 result.TQ == null ? "NULL" : "@TQ", idKetQuaTuMayCLS, AppParams.stagoStart4Config.TQ);
                        if (ServerConnect.I.Exec(sql, paramName.ToArray(), values.ToArray(), sqlTypes.ToArray()) <= 0)
                        {
                            ServerConnect.I.RollBack();
                            return false;
                        }

                    }
                    ServerConnect.I.Commit();
                    result = result.SecondResult;

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
                WHERE maketqua='{0}' AND loaimay='{1}' order by ngaythuchien desc", SpecimenID, AppParams.stagoStart4Config.MaMayCLS));
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

        bool IDevice.Connect()
        {
            throw new Exception("The method or operation is not implemented.");
        }

        bool IDevice.IsConnectting
        {
            get { throw new Exception("The method or operation is not implemented."); }
        }

        bool IDevice.DeviceName
        {
            get { throw new Exception("The method or operation is not implemented."); }
        }

        bool IDevice.DeviceID
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
        public void WriteFile(string fileName, string noidung)
        {
            if (File.Exists(fileName)==false)File.Delete(fileName);
           
            FileStream myFile = new FileStream(fileName, FileMode.Create);
           
            StreamWriter sw = new StreamWriter(myFile);
            sw.WriteLine(noidung);
            // sw.writea
            sw.Flush();
            sw.Close();
            myFile.Close();
        }
    }
}
