using System;
using System.Collections.Generic;
using System.Text;

namespace ConnectLab
{
    public class ServerConnect : SQLConnect
    {
        public static ServerConnect I = null;
        public ServerConnect()
            :
            base(AppParams.serverConfig.Name, AppParams.serverConfig.DBName,
            AppParams.serverConfig.User, AppParams.serverConfig.Pass)
        {

        }
        public override bool Connect()
        {
            base.ServerName = AppParams.serverConfig.Name;
            base.Databasename = AppParams.serverConfig.DBName;
            base.LoginName = AppParams.serverConfig.User;
            base.LoginPassword = AppParams.serverConfig.Pass;
            return base.Connect();
        }
      

    }
}
