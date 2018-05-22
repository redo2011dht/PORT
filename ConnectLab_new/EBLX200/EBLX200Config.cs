using System;
using System.Collections.Generic;
using System.Text;

namespace ConnectLab
{
    [Serializable]
    public class EBLX200Config:DOSQLConfig
    {
        public EBLX200Config()
            : base()
        {
            this.FileName = "EBLX200" + AppParams.ExtensionConfigFile;
        }
        
       
    }
}
