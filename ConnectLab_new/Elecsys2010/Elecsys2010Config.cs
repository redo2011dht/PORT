using System;
using System.Collections.Generic;
using System.Text;

namespace ConnectLab
{
    [Serializable]
    public class Elecsys2010Config : DOCOMConfig
    {        
        public Elecsys2010Config()
            : base()
        {
            base.FileName ="Elecsys2010" + AppParams.ExtensionConfigFile;
        }


        public override bool Write()
        {
            return AppUntil.Write(this, AppParams.ConfigFolder + "\\" + this.FileName);
        }

        public override object Read()
        {
            return AppUntil.Read(this, AppParams.ConfigFolder + "\\" + this.FileName);
        }
    }
}
