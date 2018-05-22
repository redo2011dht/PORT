using System;
using System.Collections.Generic;
using System.Text;

namespace ConnectLab
{

    [Serializable]
    public class Lh500Config : DOCOMConfig
    {
        public Lh500Config()
            : base()
        {
            base.portName = "COM4";
            base.baudRate = "19200";
            base.dataBits = "8";
            base.handshake = "XOnXOff";
            base.maMayCLS = "LH500";
            base.FileName = "LH500" + AppParams.ExtensionConfigFile;
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
