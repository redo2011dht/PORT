using System;
using System.Collections.Generic;
using System.Text;

namespace ConnectLab
{
    public class StagoStart4Result
    {
        public string ID = "";
        public string DateExcute = "";
        public string TypeID = "";
        public string TypeName = "";
        public string C = "";
        public decimal? TQ;
        public decimal? PROTHROMBINE;
        public decimal? INR;
        public decimal? TCK;
        public decimal? FIBRINOGENE;
        public StagoStart4Result SecondResult;

        public StagoStart4Result()
        {

        }


        public StagoStart4Result(string ouputFromLaura)
        {
            ouputFromLaura = ouputFromLaura.Trim();
            string[] opArray = ouputFromLaura.Split(' ');
            List<string> opList = new List<string>();
            
            foreach (string str in opArray)
            {
                if (str.Trim() == "") continue;
                opList.Add(str.Trim());             

            }
            if (opList.Count>=5)
            {
                if (opList[5].EndsWith("S"))
                {
                    opList.Insert(5, "1");
                }
            }
           
            int count;
           if(int.TryParse(opList[5],out count)==false)
           {
               count=1;
           }

            this.ID = GetID(opList[6]);
            this.TypeID = opList[3].Substring(0, 1);
            this.C = opList[2];
            if (TypeID == "1" && C == "1")
            {
                this.TypeName = "PT";
                this.TQ = GetDecimal(opList[7]);
                this.PROTHROMBINE = GetDecimal(opList[8]);
                this.INR = GetDecimal(opList[9]);
                if (count >= 2)
                {
                    this.SecondResult = new StagoStart4Result();
                    this.SecondResult.TypeID = this.TypeID;
                    this.SecondResult.TypeName = this.TypeName;
                    this.SecondResult.C = this.C;
                    this.SecondResult.ID = GetID(opList[10]);
                    this.SecondResult.TQ = GetDecimal(opList[11]);
                    this.SecondResult.PROTHROMBINE = GetDecimal(opList[12]);
                    this.SecondResult.INR = GetDecimal(opList[13]);
                    if (count >= 3)
                    {
                        this.SecondResult.SecondResult = new StagoStart4Result();
                        this.SecondResult.SecondResult.TypeID = this.TypeID;
                        this.SecondResult.SecondResult.TypeName = this.TypeName;
                        this.SecondResult.SecondResult.C = this.C;
                        this.SecondResult.SecondResult.ID = GetID(opList[14]);
                        this.SecondResult.SecondResult.TQ = GetDecimal(opList[15]);
                        this.SecondResult.SecondResult.PROTHROMBINE = GetDecimal(opList[16]);
                        this.SecondResult.SecondResult.INR = GetDecimal(opList[17]);
                        if (count >= 4)
                        {
                            this.SecondResult.SecondResult.SecondResult = new StagoStart4Result();
                            this.SecondResult.SecondResult.SecondResult.TypeID = this.TypeID;
                            this.SecondResult.SecondResult.SecondResult.TypeName = this.TypeName;
                            this.SecondResult.SecondResult.SecondResult.C = this.C;
                            this.SecondResult.SecondResult.SecondResult.ID = GetID(opList[18]);
                            this.SecondResult.SecondResult.SecondResult.TQ = GetDecimal(opList[19]);
                            this.SecondResult.SecondResult.SecondResult.PROTHROMBINE = GetDecimal(opList[20]);
                            this.SecondResult.SecondResult.SecondResult.INR = GetDecimal(opList[21]);
                        }
                    }
                }
                else if (TypeID == "2" && C == "1")
                {
                    this.TypeName = "APTT";
                    this.TCK = GetDecimal(opList[7]);
                    if (count >=2 )
                    {
                        this.SecondResult = new StagoStart4Result();
                        this.SecondResult.TypeID = this.TypeID;
                        this.SecondResult.TypeName = this.TypeName;
                        this.SecondResult.C = this.C;
                        this.SecondResult.ID = GetID(opList[10]);
                        this.SecondResult.TCK = GetDecimal(opList[11]);
                        if (count >= 3)
                        {
                            this.SecondResult.SecondResult = new StagoStart4Result();
                            this.SecondResult.SecondResult.TypeID = this.TypeID;
                            this.SecondResult.SecondResult.TypeName = this.TypeName;
                            this.SecondResult.SecondResult.C = this.C;
                            this.SecondResult.SecondResult.ID = GetID(opList[14]);
                            this.SecondResult.SecondResult.TCK = GetDecimal(opList[15]);
                            if (count >= 4)
                            {
                                this.SecondResult.SecondResult.SecondResult = new StagoStart4Result();
                                this.SecondResult.SecondResult.SecondResult.TypeID = this.TypeID;
                                this.SecondResult.SecondResult.SecondResult.TypeName = this.TypeName;
                                this.SecondResult.SecondResult.SecondResult.C = this.C;
                                this.SecondResult.SecondResult.SecondResult.ID = GetID(opList[18]);
                                this.SecondResult.SecondResult.SecondResult.TCK = GetDecimal(opList[19]);
                            }
                        }

                    }
                }
                else if (TypeID == "3" && C == "1")
                {
                    this.TypeName = "FIBRIOGEN";
                    this.FIBRINOGENE = GetDecimal(opList[7]);
                    if (count >= 2)
                    {
                        this.SecondResult = new StagoStart4Result();
                        this.SecondResult.TypeID = this.TypeID;
                        this.SecondResult.TypeName = this.TypeName;
                        this.SecondResult.C = this.C;
                        this.SecondResult.ID = GetID(opList[10]);
                        this.SecondResult.FIBRINOGENE = GetDecimal(opList[11]);
                        if (count >= 3)
                        {
                            this.SecondResult.SecondResult = new StagoStart4Result();
                            this.SecondResult.SecondResult.TypeID = this.TypeID;
                            this.SecondResult.SecondResult.TypeName = this.TypeName;
                            this.SecondResult.SecondResult.C = this.C;
                            this.SecondResult.SecondResult.ID = GetID(opList[14]);
                            this.SecondResult.SecondResult.FIBRINOGENE = GetDecimal(opList[15]);
                            if (count >= 4)
                            {
                                this.SecondResult.SecondResult.SecondResult = new StagoStart4Result();
                                this.SecondResult.SecondResult.SecondResult.TypeID = this.TypeID;
                                this.SecondResult.SecondResult.SecondResult.TypeName = this.TypeName;
                                this.SecondResult.SecondResult.SecondResult.C = this.C;
                                this.SecondResult.SecondResult.SecondResult.ID = GetID(opList[18]);
                                this.SecondResult.SecondResult.SecondResult.FIBRINOGENE = GetDecimal(opList[19]);

                            }
                        }
                    }
                }
            }
        }

        private string GetID(string agr)
        {
            string id = agr.TrimEnd('S');
            int IDtemp;
            if (int.TryParse(id.Split('.')[0], out IDtemp))
            {
                id = IDtemp.ToString();
                if (id.Length < 3)
                    id = IDtemp.ToString("000");
            }
            return id;
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
                if (char.IsLetter(agr[agr.Length - 1]))
                {
                    agr = agr.Remove(agr.Length - 1);
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
