using System;
using System.Collections.Generic;
using System.Text;

namespace ConnectLab
{
    [Serializable]
    public class CD1800Config : DOCOMConfig
    {

        private string gran="GRAN";
        private string mid="MID";
        private string lym="LYM";
        private string rbc="RBC";
        private string granper="GRAN %";
        private string midper="MID %";
        private string hct="HCT";
        private string hgb="HGB";
        private string lymper="LYM %";
        private string wbc="WBC";
        private string mpv="MPV";
        private string plt="PLT";
        private string mch="MCH";
        private string rdw="RDW";
        private string mcv="MCV";
        private string mchc="MCHC";
        private string pdw="PDW";
        private string pct="PCT";

        public string GRAN
        {
            get { return gran; }
            set { gran = value; }
        }
        public string MID
        {
            get { return mid; }
            set { mid = value; }
        }
        public string LYM
        {
            get { return lym; }
            set { lym = value; }
        }
        public string RBC
        {
            get { return rbc; }
            set { rbc = value; }
        }
        public string GRANPer
        {
            get { return granper; }
            set { granper = value; }
        }
        public string MIDPer
        {
            get { return midper; }
            set { midper = value; }
        }
        public string HCT
        {
            get { return hct; }
            set { hct = value; }
        }
        public string HGB
        {
            get { return hgb; }
            set { hgb = value; }
        }
        public string LYMPer
        {
            get { return lymper; }
            set { lymper = value; }
        }
        public string WBC
        {
            get { return wbc; }
            set { wbc = value; }
        }
        public string MPV
        {
            get { return mpv; }
            set { mpv = value; }
        }
        public string PLT
        {
            get { return plt; }
            set { plt = value; }
        }
        public string MCH
        {
            get { return mch; }
            set { mch = value; }
        }
        public string RDW
        {
            get { return rdw; }
            set { rdw = value; }
        }
        public string MCV
        {
            get { return mcv; }
            set { mcv = value; }
        }
        public string MCHC
        {
            get { return mchc; }
            set { mchc = value; }
        }
        public string PDW
        {
            get { return pdw; }
            set { pdw = value; }
        }
        public string PCT
        {
            get { return pct; }
            set { pct = value; }
        }

        public CD1800Config()
            : base()
        {
            base.FileName ="CD1800" + AppParams.ExtensionConfigFile;
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
