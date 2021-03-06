using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Data;

namespace ConnectLab
{
    public class EBLX200Connect : SQLConnect,IDevice
    {
        Timer timer;
        public static EBLX200Connect I = null;
        private string lastTime = "2011-12-16 01:10:00.000";
        public EBLX200Connect()
            :
            base(AppParams.eblx200Config.Name, AppParams.eblx200Config.DBName,
            AppParams.eblx200Config.User, AppParams.eblx200Config.Pass)
        {
            timer = new Timer();            
            timer.Interval = 1000;
            //timer.Tick += new EventHandler(timer_Tick);
            try
            {
                lastTime = this.d_SystemDate().ToString("yyyy-MM-dd hh:mm:ss.sss");
            }
            catch
            {
                lastTime = DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss.sss");
            }
        }
        void timer_Tick(object sender, EventArgs e)
        {
            try
            {
                if (EBLX200Connect.I.IsConnectting == false)
                {
                    EBLX200Connect.I.Connect();
                }
                string sql = string.Format(@"SELECT vr.Patient TenBenhNhan, 
            CONVERT(VARCHAR(10), CONVERT(DATETIME, vr.[Result Date],120),120) ngaythuchien,
            CONVERT(VARCHAR(30),CONVERT(DATETIME ,vr.[Result Date],121),121) resultdate,
            vr.SampleId maketqua,vr.Test,
            CASE WHEN  ISNUMERIC(vr.Result)=0 THEN NULL ELSE convert(float, vr.Result) end Result,
            CASE WHEN  ISNUMERIC(vr.Result)=1 THEN NULL ELSE vr.Result END resultStr
            FROM dbo.VWResults vr
            WHERE CONVERT(DATETIME ,vr.[Result Date],120)>'{0}' 
            order by CONVERT(DATETIME ,vr.[Result Date],120) ", lastTime);

                DataTable dt = EBLX200Connect.I.GetTable(sql);
                if (dt == null || dt.Rows.Count == 0) return;
                DataTable dtKQCLS = dt.DefaultView.ToTable(true, "TenBenhNhan", "ngaythuchien", "maketqua");

                if (dtKQCLS == null || dtKQCLS.Rows.Count < 1) return;

                frmMain.formMain.m_notifyicon.ShowBalloonTip(3, "MKV Soft:: Thông báo", "Cập nhật kết quả từ máy Erba XL-200", ToolTipIcon.Info);
                foreach (DataRow r in dtKQCLS.Rows)
                {
                    ServerConnect.I.BeginTran();
                    string sqll = string.Format(@"INSERT INTO dbo.KetQuaTuMayCLS( mabenhnhan ,TenBenhNhan ,ngaythuchien , maketqua ,loaimay , STT ,  mota    )
                                   VALUES( '{0}' ,N'{1}' ,'{2}' ,'{3}' ,'{4}' , '{5}' , N'{6}' )",
                                     " ", r["TenBenhNhan"], r["ngaythuchien"], r["maketqua"], AppParams.eblx200Config.MaMayCLS, "NULL", "");
                    if (ServerConnect.I.ExecSQL(sqll) == false)
                    {
                        ServerConnect.I.RollBack();
                        continue;
                    }
                    string sqlCT = "";
                    string idKetQuaTuMayCLS = GetIdKetQuaTuMayCLS(r["maketqua"].ToString());

                    DataRow[] temps = dt.Select(string.Format("maketqua='{0}' and TenBenhNhan='{1}' and ngaythuchien='{2}'",
                        r["maketqua"], r["TenBenhNhan"], r["ngaythuchien"]));
                    List<string> paramNames = new List<string>();
                    List<object> values = new List<object>();
                    List<SqlDbType> sqlTypes = new List<SqlDbType>();

                    for (int j = 0; j < temps.Length; j++)
                    {
                        DataRow row = temps[j];
                        float rs;
                        if (float.TryParse(row["Result"].ToString(), out rs) == false)
                        {
                            sqlCT += string.Format(@" INSERT INTO dbo.KetQuaTuMayCLS_CT ( idKetQuaTuMayCLS , tenthongso , giatrichuoi)
                                    VALUES  ( {0} , N'{1}' , '{2}')", idKetQuaTuMayCLS, row["Test"], row["resultStr"]);
                        }
                        else
                        {
                            string paramName = "@" + row["Test"];
                            paramNames.Add(paramName);
                            values.Add(Math.Round(rs, 2));
                            sqlTypes.Add(SqlDbType.Float);
                            sqlCT += string.Format(@" INSERT INTO dbo.KetQuaTuMayCLS_CT ( idKetQuaTuMayCLS , tenthongso , giatri)
                                    VALUES  ( {0} , N'{1}' , {2})", idKetQuaTuMayCLS, row["Test"], paramName);

                        }

                        sqlCT += Environment.NewLine;


                    }
                    if (sqlCT != "")
                    {
                        if (ServerConnect.I.Exec(sqlCT, paramNames.ToArray(), values.ToArray(), sqlTypes.ToArray()) <= 0)
                        {
                            ServerConnect.I.RollBack();
                            continue;
                        }
                    }
                    ServerConnect.I.Commit();

                }
                lastTime = dt.Rows[dt.Rows.Count - 1]["resultdate"].ToString();
            }
            catch
            {
                ServerConnect.I.RollBack();
            }
        }
        private string GetIdKetQuaTuMayCLS(string SpecimenID)
        {
            string idKetQuaTuMayCLS = "-1";
            DataTable dt = ServerConnect.I.GetTable(string.Format(@"SELECT top 1 idKetQuaTuMayCLS FROM dbo.KetQuaTuMayCLS
                WHERE maketqua='{0}' AND loaimay='{1}' order by idKetQuaTuMayCLS desc", SpecimenID,AppParams.eblx200Config.MaMayCLS));
            if (dt != null && dt.Rows.Count > 0)
            {
                idKetQuaTuMayCLS = dt.Rows[0][0].ToString(); ;
            }
            return idKetQuaTuMayCLS;
        }
        public override bool Connect()
        {
            base.ServerName = AppParams.eblx200Config.Name;
            base.Databasename = AppParams.eblx200Config.DBName;
            base.LoginName = AppParams.eblx200Config.User;
            base.LoginPassword = AppParams.eblx200Config.Pass;

            bool ok = base.Connect();
            timer.Start();
            return ok;
        }

    
        public void ReadConfidFromAppParams()
        {
            this.ServerName = AppParams.eblx200Config.Name;
            this.Databasename = AppParams.eblx200Config.DBName;
            this.LoginName = AppParams.eblx200Config.User;
            this.LoginPassword = AppParams.eblx200Config.Pass;
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
