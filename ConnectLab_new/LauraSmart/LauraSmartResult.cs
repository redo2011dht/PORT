using System;
using System.Collections.Generic;
using System.Text;

namespace ConnectLab
{
    public class LauraSmartResult
    {
        public string ID = "";
        public string Patient = "";
        public string Comments = "";
        public string DateExcute = "";
        public string Sequence = "";
        public decimal? BLD;
        public string BLDStr;
        public decimal? LEU;
        public string LEUStr;
        public decimal? BIL;
        public string BILStr;
        public decimal? UBG;
        public string UBGStr;
        public decimal? KET;
        public string KETStr;
        public decimal? GLU;
        public string GLUStr;
        public decimal? PRO;
        public string PROStr;
        public decimal? PH;
        public string PHStr;
        public decimal? NIT;
        public string NITStr;
        public decimal? SG;
        public string SGStr;

        private string[] opArray;
        public LauraSmartResult()
        {
        }
        string tem = @"MEDILAB

        DekaPHAN LAURA
        Seq.No:  0001
        Pat.ID:  03122011101

        2011.12.03       8:40
        ........................

         BLD    NEG
         LEU    NEG
         BIL    NEG
         UBG   NORM
         KET    NEG
         GLU    NEG
         PRO    NEG
         pH     6.5
         NIT    NEG
         SG   1.015            
        ------------------------

        ";

        public LauraSmartResult(string ouputFromLaura)
        {
            //ouputFromLaura = tem;
            ouputFromLaura = ouputFromLaura.Replace("\r\n", "$");
            opArray = ouputFromLaura.Split('$');
            if (opArray.Length >= 10)
            {
                this.Sequence = opArray[3].Trim().Substring(7).Trim();
                this.ID = opArray[4].Trim().Substring(7).Trim();
                string date = opArray[6].Trim();
                this.DateExcute = date.Substring(0, 10).Replace(".", "-") + " " + date.Substring(11).Trim();
                for (int i = 9; i < opArray.Length; i++)
                {
                    string outText = opArray[i].TrimStart('*', ' ');
                    if (outText.StartsWith("BLD"))
                    {
                        GetResult(opArray[i], out this.BLDStr, out this.BLD);
                    }
                    else if (outText.StartsWith("LEU"))
                    {
                        GetResult(opArray[i], out this.LEUStr, out this.LEU);

                    }
                    else if (outText.StartsWith("BIL"))
                    {
                        GetResult(opArray[i], out this.BILStr, out this.BIL);
                    }
                    else if (outText.StartsWith("UBG"))
                    {
                        GetResult(opArray[i], out this.UBGStr, out this.UBG);

                    }
                    else if (outText.StartsWith("KET"))
                    {
                        GetResult(opArray[i], out this.KETStr, out this.KET);

                    }
                    else if (outText.StartsWith("GLU"))
                    {
                        GetResult(opArray[i], out this.GLUStr, out this.GLU);
                    }
                    else if (outText.StartsWith("PRO"))
                    {
                        GetResult(opArray[i], out this.PROStr, out this.PRO);
                    }
                    else if (outText.StartsWith("pH"))
                    {
                        GetResult(opArray[i], out this.PHStr, out this.PH);
                    }
                    else if (outText.StartsWith("NIT"))
                    {
                        GetResult(opArray[i], out this.NITStr, out this.NIT);
                    }
                    else if (outText.StartsWith("SG"))
                    {
                        GetResult(opArray[i], out this.SGStr, out this.SG);
                    }
                }
            }
        }

        private void GetResult(string outText, out string sResult, out decimal? dResult)
        {
            outText = outText.TrimStart('*', ' ').Remove(0, 3).Trim();
            int indexSP = outText.IndexOf(' ');
            if (indexSP > -1)
            {
                sResult = outText.Substring(0, indexSP).Trim();
            }
            else
            {
                sResult = outText;
            }
            dResult = GetDecimal(sResult);
            if (dResult != null) sResult = null;

        }
        private void GetResult(string outText, out decimal? dResult)
        {
            string sResult;
            outText = outText.TrimStart('*', ' ');
            sResult = outText.Substring(3, 5).Trim();
            dResult = GetDecimal(sResult);

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

                if (agr == "")
                {
                    return null;
                }
                agr = agr.Replace(".", System.Windows.Forms.Application.CurrentCulture.NumberFormat.NumberDecimalSeparator);

                num = decimal.Parse(agr);//,System.Windows.Forms.Application.CurrentCulture.NumberFormat);               
                return num;
            }
            catch
            {
            }
            return null;

        }


    }
}
