using System;
using System.Collections.Generic;
using System.Text;

namespace ConnectLab
{
  
    [Serializable]
    public class LauraSmartConfig : DOCOMConfig
    {
        private string bld="BLD";
        private string leu="LEU";
        private string bil="BIL";
        private string ubg="UBG";
        private string ket="KET";
        private string glu="GLU";
        private string pro="PRO";
        private string ph="PH";
        private string nit="NIT";
        private string sg="SG";

        public string BLD
        {
            get { return bld; }
            set { bld = value; }
        }
        public string LEU
        {
            get { return leu; }
            set { leu = value; }
        }
        public string BIL
        {
            get { return bil; }
            set { bil = value; }
        }
        public string UBG
        {
            get { return ubg; }
            set { ubg = value; }
        }
        public string KET
        {
            get { return ket; }
            set { ket = value; }
        }
        public string GLU
        {
            get { return glu; }
            set { glu = value; }
        }
        public string PRO
        {
            get { return pro; }
            set { pro = value; }
        }
        public string PH
        {
            get { return ph; }
            set { ph = value; }
        }
        public string NIT
        {
            get { return nit; }
            set { nit = value; }
        }

        public string SG
        {
            get { return sg; }
            set { sg = value; }
        }
        public LauraSmartConfig()
            : base()
        {
            base.portName = "COM4";
            base.baudRate = "19200";
            base.maMayCLS = "LASM";
            base.FileName = "LauraSmart" + AppParams.ExtensionConfigFile;
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
