using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Reflection;

namespace ConnectLab
{
    [Serializable]
    public abstract class DOSQLConfig
    {

        protected string name = "";
        protected string dbName = "";
        protected string user = "";
        protected string pass = "";
        protected string FileName = "";
        protected string maMayCLS = "";

        public string MaMayCLS
        {
            get { return maMayCLS; }
            set { maMayCLS = value; }
        }
        public DOSQLConfig()
        {

        }
        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        public string DBName
        {
            get { return dbName; }
            set { dbName = value; }
        }

        public string User
        {
            get { return user; }
            set { user = value; }
        }

        public string Pass
        {
            get { return pass; }
            set { pass = value; }
        }
        public virtual bool Write()
        {
            return AppUntil.Write(this, AppParams.ConfigFolder + "\\" + this.FileName);
        }

        public virtual object Read()
        {
            return AppUntil.Read(this, AppParams.ConfigFolder + "\\" + this.FileName);
        }

        
    
   
    }
}
