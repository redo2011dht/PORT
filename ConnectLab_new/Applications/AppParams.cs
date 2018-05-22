using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace ConnectLab
{
    public class AppParams
    {
        public static ServerConfig serverConfig = null;
        public static Au480Config au480Config = null;
        public static Lh500Config lh500Config = null;
        public static CD1800Config cd1800Config = null;
        public static EBLX200Config eblx200Config = null;
        public static LauraSmartConfig lauraSmartConfig = null;
        public static StagoStart4Config stagoStart4Config = null;
        public static Elecsys2010Config elecsys2010Config = null;
        public static string ExtensionConfigFile = ".conf";
        public static string ConfigFolder = Application.StartupPath + "\\CONF";
        public static string LogFolder = Application.StartupPath + "\\LOG";

      

        public static void LoadAllConfig()
        {
            AppParams.serverConfig = new ServerConfig();
            AppParams.serverConfig.Read();

            AppParams.au480Config = new Au480Config();
            AppParams.au480Config.Read();

            AppParams.lh500Config = new Lh500Config();
            AppParams.lh500Config.Read();

            AppParams.cd1800Config = new CD1800Config();
            AppParams.cd1800Config.Read();
            
            AppParams.eblx200Config = new EBLX200Config();
            AppParams.eblx200Config.Read();

            AppParams.lauraSmartConfig = new LauraSmartConfig();
            AppParams.lauraSmartConfig.Read();

            AppParams.stagoStart4Config = new StagoStart4Config();
            AppParams.stagoStart4Config.Read();

            AppParams.elecsys2010Config = new Elecsys2010Config();
            AppParams.elecsys2010Config.Read();
        }
       

    }
}
