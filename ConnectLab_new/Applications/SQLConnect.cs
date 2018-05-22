using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Drawing;
using System.Windows.Forms;
namespace ConnectLab
{
    public class SQLConnect
    {
        protected SqlCommand Cmd = null;
        public SqlConnection Conn = null;
        private int countBeginTran = 0;
        private int countCommittran = 0;
        protected SqlDataAdapter Da;
        public string Databasename;
        public bool IsConnectByMdfFile = false;
        private bool IsConnecting = false;
        protected bool isRunTransac = false;
        public string LoginName;
        public string LoginPassword;
        public string MdfFileName = null;
        public string ProductName;
        public string ServerName;
        protected SqlTransaction Tran;

        public bool IsConnectting
        {

            get
            {
                return this.IsConnecting;
            }
        }

        public SQLConnect(string sSer, string sDataBaseName, string sLoginName, string sLoginPass)
        {
            try
            {
                Init();
                ServerName = sSer.Trim();
                Databasename = sDataBaseName.Trim();
                LoginName = sLoginName.Trim();
                LoginPassword = sLoginPass;
                Conn.ConnectionString = "Data Source=" + ServerName + ";Initial Catalog=" + Databasename + ";User ID=" + LoginName + "; Password=" + LoginPassword + ";Connect Timeout=1";
                if (LoginName == "")
                {
                    Conn.ConnectionString = "Data Source=" + ServerName + ";Initial Catalog=" + Databasename + ";Integrated Security=True;Connect Timeout=1";
                }
            }
            catch { }
        }
      
        public virtual bool Connect()
        {
            if (Conn == null)
            {
                Init();
            }
            if (ServerName == null)
            {
                return false;
            }
            if ((Conn != null) && (Conn.State == ConnectionState.Open))
            {
                Conn.Close();
            }
            if (IsConnectByMdfFile)
            {
                Conn.ConnectionString = "Data Source=" + ServerName + ";AttachDbFilename=" + MdfFileName + ";Integrated Security=True;Connect Timeout=30;User Instance=True";
            }
            else
            {
                Conn.ConnectionString = "Data Source=" + ServerName + ";Initial Catalog=" + Databasename + ";User ID=" + LoginName + "; Password=" + LoginPassword + ";Connect Timeout=1";
                if (LoginName == "")
                {
                    Conn.ConnectionString = "Data Source=" + ServerName + ";Initial Catalog=" + Databasename + ";Integrated Security=True;Connect Timeout=1";
                }
            }
            try
            {
                Conn.Open();
                Conn.Close();
                IsConnecting = true;
                return true;
            }
            catch (Exception)
            {
                IsConnecting = false;
                return false;
            }
        }

        public void BeginTran()
        {
            return;
            countBeginTran++;
            if (!isRunTransac && IsConnect())
            {
                if (Conn.State == ConnectionState.Closed)
                {
                    Conn.Open();
                }
                Tran = Conn.BeginTransaction();
                Cmd.Transaction = Tran;
                isRunTransac = true;
            }
        }
        public bool BackupDB(bool IsDefaultFileName, ref string OutputFile)
        {
            OutputFile = "Không thành công";
            if (IsDefaultFileName)
            {
                DataTable table = GetTable("select physical_name  from sys.database_files");
                if ((table == null) || (table.Rows.Count == 0))
                {
                    return false;
                }
                string str = table.Rows[0][0].ToString();
                int length = str.LastIndexOf(@"\");
                if (length == -1)
                {
                    return false;
                }
                str = str.Substring(0, length);
                table = GetTable("select name from sysobjects where name like 'BackupDB' and type='p'");
                if (table == null)
                {
                    return false;
                }
                if (table.Rows.Count == 0)
                {
                    string strCommandText = " Create Procedure BackupDB(@Path as nvarchar(500))\r\n as\r\n begin\r\n declare @DBName as nvarchar(100),\r\n \t\t@FileName as nvarchar(100),\r\n \t\t@MediaName as nvarchar(500),\r\n \t\t@Disk as nvarchar(500),\r\n \t\t@Name as nvarchar(100),\r\n \t\t@Date as datetime,\r\n \t\t@Day as nvarchar(2),\r\n \t\t@Month as nvarchar(2) \r\n \t\tset @Date=(select getdate())\r\n \t\tset @Day=convert(nvarchar(2),day(@Date))\r\n \t\tset @Month=convert(nvarchar(2),month(@Date))\r\n \t\tif(len(@Day)=1) set @Day='0'+@Day\r\n \t\tif(len(@Month)=1) set @Month='0'+@Month\r\n \t\tset @DBName=DB_Name()\r\n \t\tset @FileName=@DBName +convert(nvarchar(4),year(@Date))+ @Month+@day+'.bak'\r\n set @MediaName=replace(@Path,':\\','_')\r\n set @Disk=@Path +'\\'+ @FileName\r\n set @Name='Full Backup of '+@DBName\r\n \t\tBACKUP DATABASE @DBName\r\n \t\tTO DISK =@Disk\r\n \t\t   WITH FORMAT,\r\n \t\t\t  MEDIANAME = @MediaName,\r\n \t\t\t  NAME = @Name\r\n end\r\n ";
                    if (!ExecSQL(strCommandText))
                    {
                        return false;
                    }
                }
                bool flag2 = ExecSQL("EXEC BackupDB  '" + str + "'");
                if (flag2)
                {
                    OutputFile = str + @"\" + Databasename + d_SystemDate().ToString("yyyyMMdd") + ".bak";
                }
                return flag2;
            }
            FolderBrowserDialog dialog = new FolderBrowserDialog();
            dialog.ShowDialog();
            string selectedPath = dialog.SelectedPath;
            DateTime time = d_SystemDate();
            string databasename = Databasename;
            string str5 = databasename + time.ToString("yyyyMMdd") + ".bak";
            string str6 = selectedPath.Replace(@":\", "_").Replace(@"\", "_");
            string str7 = selectedPath + @"\" + databasename;
            string str8 = "Full Backup of " + databasename;
            bool flag3 = ExecSQL("  BACKUP DATABASE " + databasename + "\r\n TO DISK ='" + str7 + "'\r\n    WITH FORMAT,\r\n \t  MEDIANAME = '" + str6 + "',\r\n \t  NAME = '" + str8 + "'");
            if (flag3)
            {
                OutputFile = selectedPath + @"\" + Databasename + d_SystemDate().ToString("yyyyMMdd") + ".bak";
            }
            return flag3;
        }



        public void Commit()
        {
            return;
            countCommittran++;
            if ((countCommittran == countBeginTran) && isRunTransac)
            {
                try
                {
                    Tran.Commit();
                    countBeginTran = 0;
                    countCommittran = 0;
                }
                catch (Exception)
                {
                }
                if (Conn.State == ConnectionState.Open)
                {
                    Conn.Close();
                }
                isRunTransac = false;
            }
        }

        public void ConnectByMdfFile(string sSer, string sDataBaseName, string mdffilename)
        {
            if (Conn == null)
            {
                Init();
            }
            ServerName = sSer.Trim();
            Databasename = sDataBaseName.Trim();
            MdfFileName = mdffilename;
            IsConnectByMdfFile = true;
            Conn.ConnectionString = Conn.ConnectionString = @"Data Source=.\SQLEXPRESS;AttachDbFilename=E:\Data\MeKongViet\QLPhongMachTu\PHONGMACH.mdf;Integrated Security=True;Connect Timeout=30;User Instance=True";
        }

        public DateTime d_SystemDate()
        {
            DataTable table = GetTable("SELECT GETDATE()");
            if (table == null)
            {
                return DateTime.Now;
            }
            return DateTime.Parse(table.Rows[0][0].ToString());
        }


        public int Exec(string strCommandText, ref string ErrorMessage)
        {
            int num = -1;
            try
            {
                if (!IsConnect())
                {
                    return -1;
                }
                Cmd.CommandText = strCommandText;
                Cmd.CommandType = CommandType.Text;
                Cmd.Parameters.Clear();
                if (Conn.State == ConnectionState.Closed)
                {
                    Conn.Open();
                }
                num = Cmd.ExecuteNonQuery();
                if (!((Conn.State != ConnectionState.Open) || isRunTransac))
                {
                    Conn.Close();
                }
            }
            catch (Exception exception)
            {
                ErrorMessage = exception.ToString();
            }
            if (!((Conn.State != ConnectionState.Open) || isRunTransac))
            {
                Conn.Close();
            }
            return num;
        }

        public int Exec(string strCommandText, int timeout)
        {
            int num = -1;
            try
            {
                if (!IsConnect())
                {
                    return -1;
                }
                Cmd.CommandText = strCommandText;
                Cmd.CommandTimeout = timeout;
                Cmd.CommandType = CommandType.Text;
                Cmd.Parameters.Clear();
                if (Conn.State == ConnectionState.Closed)
                {
                    Conn.Open();
                }
                num = Cmd.ExecuteNonQuery();
                if (!((Conn.State != ConnectionState.Open) || isRunTransac))
                {
                    Conn.Close();
                }
            }
            catch (Exception)
            {
            }
            if (!((Conn.State != ConnectionState.Open) || isRunTransac))
            {
                Conn.Close();
            }
            return num;
        }

        public int Exec(string strCommandText, string[] arrParameterName, object[] arrParameterValue)
        {
            int num = -1;
            try
            {
                if (!IsConnect())
                {
                    return -1;
                }
                if (Conn.State == ConnectionState.Closed)
                {
                    Conn.Open();
                }
                Cmd.CommandText = strCommandText;
                Cmd.CommandType = CommandType.Text;
                Cmd.Parameters.Clear();
                for (int i = 0; i < arrParameterName.Length; i++)
                {
                    Cmd.Parameters.Add(arrParameterName[i], arrParameterValue[i]);
                }
                num = Cmd.ExecuteNonQuery();
                if (!((Conn.State != ConnectionState.Open) || isRunTransac))
                {
                    Conn.Close();
                }
            }
            catch (Exception)
            {
            }
            if (!((Conn.State != ConnectionState.Open) || isRunTransac))
            {
                Conn.Close();
            }
            return num;
        }

        public int Exec(string strCommandText, string[] arrParameterName, object[] arrParameterValue, SqlDbType[] types)
        {
            int num = -1;
            try
            {
                if (!IsConnect())
                {
                    return -1;
                }
                if (Conn.State == ConnectionState.Closed)
                {
                    Conn.Open();
                }
                Cmd.CommandText = strCommandText;
                Cmd.CommandType = CommandType.Text;
                Cmd.Parameters.Clear();
                for (int i = 0; i < arrParameterName.Length; i++)
                {
                    SqlParameter s = new SqlParameter(arrParameterName[i], arrParameterValue[i]);
                    s.SqlDbType = types[i];
                    Cmd.Parameters.Add(s);
                }
                num = Cmd.ExecuteNonQuery();
                if (!((Conn.State != ConnectionState.Open) || isRunTransac))
                {
                    Conn.Close();
                }
            }
            catch (Exception)
            {
            }
            if (!((Conn.State != ConnectionState.Open) || isRunTransac))
            {
                Conn.Close();
            }
            return num;
        }
        public int Exec(string strCommandText, string[] arrParameterName, object[] arrParameterValue, int timeout)
        {
            int num = -1;
            try
            {
                if (!IsConnect())
                {
                    return -1;
                }
                if (Conn.State == ConnectionState.Closed)
                {
                    Conn.Open();
                }
                Cmd.CommandText = strCommandText;
                Cmd.CommandType = CommandType.Text;
                Cmd.CommandTimeout = timeout;
                Cmd.Parameters.Clear();
                for (int i = 0; i < arrParameterName.Length; i++)
                {
                    Cmd.Parameters.Add(arrParameterName[i], arrParameterValue[i]);
                }
                if (Conn.State == ConnectionState.Closed)
                {
                    Conn.Open();
                }
                num = Cmd.ExecuteNonQuery();
                if (!((Conn.State != ConnectionState.Open) || isRunTransac))
                {
                    Conn.Close();
                }
            }
            catch (Exception)
            {
            }
            if (!((Conn.State != ConnectionState.Open) || isRunTransac))
            {
                Conn.Close();
            }
            return num;
        }

        public bool ExecSQL(string strCommandText)
        {
            bool flag = false;
            try
            {
                if (!IsConnect())
                {
                    return false;
                }
                Cmd.CommandText = strCommandText;
                Cmd.CommandType = CommandType.Text;
                Cmd.Parameters.Clear();
                if (Conn.State == ConnectionState.Closed)
                {
                    Conn.Open();
                }
                Cmd.ExecuteNonQuery();
                if (!((Conn.State != ConnectionState.Open) || isRunTransac))
                {
                    Conn.Close();
                }
                flag = true;
            }
            catch (Exception)
            {
                flag = false;
            }
            if (!((Conn.State != ConnectionState.Open) || isRunTransac))
            {
                Conn.Close();
            }
            return flag;
        }

        public bool ExecSQL(string strCommandText, int timeOut)
        {
            return (Exec(strCommandText, timeOut) >= 1);
        }

        public bool ExecSQL(string strCommandText, string[] arrParameterName, object[] arrParameterValue)
        {
            return (Exec(strCommandText, arrParameterName, arrParameterValue) >= 1);
        }

        public bool ExecSQL(string strCommandText, string[] arrParameterName, object[] arrParameterValue, int timeOut)
        {
            return (Exec(strCommandText, arrParameterName, arrParameterValue, timeOut) >= 1);
        }

        public Bitmap getBitmap(string sqlSelect, string sFileFilter)
        {
            return getBitmap(sqlSelect, sFileFilter, false);
        }

        public Bitmap getBitmap(string sqlSelect, string sFileFilter, bool IsFileName)
        {
            try
            {
                StreamReader reader = new StreamReader(sGetFileName_ImageField(sqlSelect, sFileFilter, IsFileName));
                BinaryReader reader2 = new BinaryReader(reader.BaseStream, Encoding.Default);
                Bitmap bitmap = (Bitmap)Image.FromStream(reader2.BaseStream);
                reader2.Close();
                reader2.Close();
                return bitmap;
            }
            catch
            {
                return null;
            }
        }

        public byte[] GetByte(string sqlSelect)
        {
            if (!IsConnect())
            {
                return null;
            }
            byte[] buffer = null;
            try
            {
                if (!((Conn.State != ConnectionState.Closed) || isRunTransac))
                {
                    Conn.Open();
                }
                Cmd.CommandText = sqlSelect;
                buffer = (byte[])Cmd.ExecuteScalar();
                if (!((Conn.State != ConnectionState.Open) || isRunTransac))
                {
                    Conn.Close();
                }
            }
            catch (Exception)
            {
                if (Conn.State == ConnectionState.Open)
                {
                    Conn.Close();
                }
                return null;
            }
            return buffer;
        }

        public DataSet GetDataSet(string strSelect)
        {
            if (!IsConnect())
            {
                return null;
            }
            DataSet dataSet = new DataSet();
            try
            {
                strSelect = strSelect.Replace("= NULL", " IS NULL");
                strSelect = strSelect.Replace("=NULL", " IS NULL");
                Cmd.CommandText = strSelect;
                Da.SelectCommand = Cmd;
                Da.Fill(dataSet);
                if (!((Conn.State != ConnectionState.Open) || isRunTransac))
                {
                    Conn.Close();
                }
            }
            catch (Exception)
            {
                if (Conn.State == ConnectionState.Open)
                {
                    Conn.Close();
                }
                return null;
            }
            return dataSet;
        }

        public Image getImage(string sqlSelect, string sFileFilter)
        {
            return getImage(sqlSelect, sFileFilter, false);
        }

        public Image getImage(string sqlSelect, string sFileFilter, bool IsFileName)
        {
            try
            {
                StreamReader reader = new StreamReader(sGetFileName_ImageField(sqlSelect, sFileFilter, IsFileName));
                BinaryReader reader2 = new BinaryReader(reader.BaseStream, Encoding.Default);
                Image image = Image.FromStream(reader2.BaseStream);
                reader2.Close();
                return image;
            }
            catch
            {
                return null;
            }
        }

        public DataTable GetTable(string strSelect)
        {
            if (!IsConnect())
            {
                return null;
            }
            DataTable dataTable = new DataTable();
            try
            {
                Cmd.CommandText = strSelect;
                Da.SelectCommand = Cmd;
                Da.Fill(dataTable);
                if (!((Conn.State != ConnectionState.Open) || isRunTransac))
                {
                    Conn.Close();
                }
            }
            catch (Exception)
            {
                if (Conn.State == ConnectionState.Open)
                {
                    Conn.Close();
                }
                return null;
            }
            return dataTable;
        }

        public DataTable GetTable(string strSelect, string TableName)
        {
            if (!IsConnect())
            {
                return null;
            }
            DataTable dataTable = new DataTable();
            try
            {
                Cmd.CommandText = strSelect;
                Da.SelectCommand = Cmd;
                Da.Fill(dataTable);
                dataTable.TableName = TableName;
                if (!((Conn.State != ConnectionState.Open) || isRunTransac))
                {
                    Conn.Close();
                }
            }
            catch (Exception)
            {
                if (Conn.State == ConnectionState.Open)
                {
                    Conn.Close();
                }
                return null;
            }
            return dataTable;
        }

        protected void Init()
        {
            Conn = new SqlConnection();
            Cmd = new SqlCommand();
            Da = new SqlDataAdapter();
            Cmd.Connection = Conn;
        }

        public bool IsConnect()
        {
            if ((((Conn == null) || (Conn.ConnectionString == null)) || (Cmd == null)) || (Conn.ConnectionString == ""))
            {
                if (((((ServerName == null) || (Databasename == null)) || ((LoginName == null) || (LoginPassword == null))) || (ServerName == "")) || (Databasename == ""))
                {
                    return false;
                }
                bool flag = Connect();

                return flag;
            }
            return true;
        }






        public void RollBack()
        {
            return;
            if (isRunTransac)
            {
                try
                {
                    Tran.Rollback();
                    if (Conn.State == ConnectionState.Open)
                    {
                        Conn.Close();
                    }
                    isRunTransac = false;
                    countBeginTran = 0;
                    countCommittran = 0;
                }
                catch (Exception)
                {
                }
            }
        }

        public string s_SystemDate()
        {
            DataTable table = GetTable("SELECT GETDATE()");
            if (table != null)
            {
                return DateTime.Parse(table.Rows[0][0].ToString()).ToString("yyyy-MM-dd HH:mm:ss");
            }
            return DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        }

        public string sGetFileName_ImageField(string sqlSelect, string sfileFilter)
        {
            return sGetFileName_ImageField(sqlSelect, sfileFilter, false);
        }

        public string sGetFileName_ImageField(string sqlSelect, string sfileFilter, bool IsFileName)
        {
            byte[] @byte = GetByte(sqlSelect);
            if (@byte == null)
            {
                return null;
            }
            string path = "";
            if (!IsFileName)
            {
                path = Application.StartupPath + @"\hsOpenImageFile." + sfileFilter;
            }
            else
            {
                path = sfileFilter;
            }
            if (File.Exists(path))
            {
                File.Delete(path);
            }
            FileStream stream = new FileStream(path, FileMode.CreateNew);
            stream.Write(@byte, 0, @byte.Length);
            stream.Close();
            return path;
        }






    }





}
