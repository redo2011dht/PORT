using System;
using System.Collections.Generic;
using System.Text;

namespace ConnectLab
{

    [Serializable]
    public class Au480Config : DOCOMConfig
    {
        public Au480Config()
            : base()
        {
            base.portName = "COM1";
            base.baudRate = "9600";
            base.dataBits = "7";
            base.handshake = "XOnXOff";
            base.maMayCLS = "AU480";
            base.FileName = "Au480" + AppParams.ExtensionConfigFile;
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
