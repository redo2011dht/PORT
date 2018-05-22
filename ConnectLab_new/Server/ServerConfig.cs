using System;
using System.Collections.Generic;
using System.Text;

namespace ConnectLab
{
    [Serializable]
    public class ServerConfig:DOSQLConfig
    {
        private string appPass = "";

        public string AppPass
        {
            get { return appPass; }
            set { appPass = value; }
        }
        
        public ServerConfig()
            : base()
        {
            this.FileName = "server" + AppParams.ExtensionConfigFile;
        }
        public override object Read()
        {
            object obj = base.Read();
            if (string.IsNullOrEmpty(this.appPass))
                this.appPass = "65AF2BF5F5C7D6802D01BF967917E0CD";
            return this;
        }
      
        
    }
}
