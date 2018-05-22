using System;
using System.Collections.Generic;
using System.Text;

namespace ConnectLab
{

    [Serializable]
    public class StagoStart4Config : DOCOMConfig
    {
        private string tq = "TQ";
        private string prothrombine = "PROTH";
        private string inr = "INR";
        private string tck = "TCK";
        private string fibrinogene = "FIB";
        public string TQ
        {
            get { return tq; }
            set { tq = value; }
        }
        public string PROTHROMBINE
        {
            get { return prothrombine; }
            set { prothrombine = value; }
        }
        public string INR
        {
            get { return inr; }
            set { inr = value; }
        }
        public string TCK
        {
            get { return tck; }
            set { tck = value; }
        }
        public string FIBRINOGENE
        {
            get { return fibrinogene; }
            set { fibrinogene = value; }
        }
        public StagoStart4Config()
            : base()
        {
            base.portName = "COM3";
            base.baudRate = "9600";
            base.maMayCLS = "START4";
            base.FileName = "Start4" + AppParams.ExtensionConfigFile;
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
