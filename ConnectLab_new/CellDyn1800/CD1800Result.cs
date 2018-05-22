using System;
using System.Collections.Generic;
using System.Text;

namespace ConnectLab
{
    public class CD1800Result
    { 
        public string SpecimenID = "";
        public string Patient = "";
        public string Sex = "";
        public string DOB = "";
        public string Physician;
        public string Comments = "";
        public string Analyzed = "";
        public string OperatorID = "";
        public string Sequence= "";
        public string Mode;
        public decimal? WBC;
        public decimal? LYM;
        public decimal? MID;
        public decimal? GRAN;
        public decimal? RBC;
        public decimal? HGB;
        public decimal? HCT;
        public decimal? MCV;
        public decimal? MCH;
        public decimal? MCHC;
        public decimal? RDW;
        public decimal? PLT;
        public decimal? MPV;
        public decimal? PCT;
        public decimal? PDW;
        public decimal? LYM_PER;
        public decimal? MID_PER;
        public decimal? GRAN_PER;
        private string[] opArray;
        public CD1800Result()
        {
        }
        public CD1800Result(string ouputFromCD)
        {
            opArray = ouputFromCD.Split(',');
            if (opArray.Length > 0)
            {
                this.SpecimenID = GetString(opArray[8]);
                this.Patient = GetString(opArray[9]);
                this.Sex = GetString(opArray[10]);//?
                this.DOB = GetDate(opArray[11], "");
                this.Physician = "";// GetString(opArray[13]);//?
                this.Comments = "";
                this.Analyzed = GetDate(opArray[6], opArray[7]);
                this.OperatorID = "";
                this.Sequence = GetString(opArray[3]);
                this.Mode = "";
                this.WBC = GetDecimal(opArray[16]);
                this.LYM = GetDecimal(opArray[18]);
                this.MID = GetDecimal(opArray[19]);
                this.GRAN = GetDecimal(opArray[20]);
                this.RBC = GetDecimal(opArray[22]);
                if (RBC != null) RBC = RBC / 10;
                this.HGB = GetDecimal(opArray[23]);
                this.HCT = GetDecimal(opArray[24]);
                this.MCV = GetDecimal(opArray[25]);
                this.MCH = GetDecimal(opArray[26]);
                this.MCHC = GetDecimal(opArray[27]);
                this.RDW = GetDecimal(opArray[28]);
                this.PLT = GetDecimal(opArray[29]);
                if (PLT != null) PLT = PLT * 10;
                this.MPV = GetDecimal(opArray[30]);
                this.PCT = GetDecimal(opArray[31]);
                this.LYM_PER = GetDecimal(opArray[34]);
                this.MID_PER = GetDecimal(opArray[35]);
                this.GRAN_PER = GetDecimal(opArray[36]);
                if (this.PCT != null) this.PCT = this.PCT / 10;
                this.PDW = GetDecimal(opArray[32]);

            }
        }
        private string GetString(string value)
        {
            if(value==null) return "";
            value = value.Replace("\"", "").TrimStart(' ', '-').TrimEnd(' ', '-');
           
            return value;
        }
        public decimal? GetDecimal(string agr)
        {
            if (agr == null)
            {
                return null;
            }          
            decimal num = -79228162514264337593543950335M;
            try
            {
                agr = GetString(agr);
                if (agr == "")
                {
                    return null;
                }
                num = decimal.Parse(agr.ToString());
                num = num / 10;
                return num;
            }
            catch
            {
            }
            return null ;

        }

        public string GetDate(string date, string time)
        {

            if (date == null || date == "") return "";
            if (time == null) time = "";
            string[] dateArr = date.Replace("\"","").Split('/');
            string[] timeArr = time.Replace("\"","").Split('/');
         
            string dd = "";
            string MM = "";
            string yy = "";
            string hh = "00";
            string mm = "00";
            if (dateArr.Length != 3) return "";
            MM = dateArr[0].Trim();
            dd = dateArr[1].Trim();
            yy = dateArr[2].Trim();
            if (yy == "") return "";
            if (yy.Length == 2)
                yy = "20" + yy;
            else if (yy.Length == 3)
                yy = "2" + yy;

            if (MM == "") MM = "01";
            if (dd == "") dd = "01";
            if (timeArr.Length >=2)
            {
                hh = timeArr[0].Trim();
                mm = timeArr[1].Trim();
                if (hh == "") hh = "00";
                if (mm == "") mm = "00";
            }

            return yy + "-" + MM + "-" + dd + " " + hh + ":" + mm + ":00";




        }

    }
}
