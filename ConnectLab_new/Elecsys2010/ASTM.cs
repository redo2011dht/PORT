using System;
using System.Collections.Generic;
using System.Text;
using System.IO.Ports;
using System.Data.SqlClient;
using System.Data;

namespace ConnectLab.Elecsys2010
{
    class ASTM
    {
        // Defines
        public const char ENQ = '\x05';
        public const char EOT = '\x04';
        public const char ACK = '\x06';
        public const char NAK = '\x15';
        public const char STX = '\x02';
        public const char ETB = '\x17';
        public const char ETX = '\x03';
        public const char CR = '\x0D';
        public const char LF = '\x0A';
        // Properties
        string Message = "";
        public bool isFullMessage = false;
        // Constructors
        // Methods
        public static string Translate(string inputStr)
        {
            inputStr = inputStr.Replace(ENQ.ToString(), "[ENQ]");
            inputStr = inputStr.Replace(EOT.ToString(), "[EOT]");
            inputStr = inputStr.Replace(ACK.ToString(), "[ACK]");
            inputStr = inputStr.Replace(NAK.ToString(), "[NAK]");
            inputStr = inputStr.Replace(STX.ToString(), "[STX]");
            inputStr = inputStr.Replace(ETB.ToString(), "[ETB]");
            inputStr = inputStr.Replace(ETX.ToString(), "[ETX]");
            inputStr = inputStr.Replace(CR.ToString(), "[CR]");
            inputStr = inputStr.Replace(LF.ToString(), "[LF]");
            return inputStr;
        }
        public const int ESTABLISHMENT_PHASE = 1;
        public const int TRANSFER_PHASE = 2;
        public const int TERMINATION_PHASE = 3;
        public static int detectPhase(string inputStr)
        {
            if (inputStr == null || inputStr == "") return 0;
            char[] keyWord = new char[1];
            inputStr.CopyTo(0, keyWord, 0, 1);
            switch (keyWord[0])
            {
                case ASTM.ENQ:
                    return ASTM.ESTABLISHMENT_PHASE;
                    break;                
                case ASTM.EOT:
                    return ASTM.TERMINATION_PHASE;
                    break;                
                default:
                    return ASTM.TRANSFER_PHASE;
                    break;
            }
        }        

        public static void writeACK(SerialPort port)
        {
            port.Write(ASTM.ACK.ToString());            
        }
        public static void writeNAK(SerialPort port)
        {
            port.Write(ASTM.NAK.ToString());
        }
        public static void writeEOT(SerialPort port)
        {
            port.Write(ASTM.EOT.ToString());
        }
    }

    class AstmMessage
    {
        List<AstmRecord> recordList;
        public bool isFinished=false;
        bool haveHeader = false;
        bool haveTerminal = false;

        public AstmMessage()
        {
            recordList = new List<AstmRecord>();
        }

        public void AddRecord(AstmRecord record)
        {
            if (record.getRecordType() == 'H') haveHeader = true;
            if (record.getRecordType() == 'L') haveTerminal = true;
            recordList.Add(record);
            if (haveTerminal && haveTerminal) isFinished = true;
        }

        public void Build()
        {
            for (int i = 0; i < recordList.Count; i++)
            {
                switch (recordList[i].getRecordType())
                {
                    case 'H':
                        recordList[i] = recordList[i].ToAstmHRecord();
                        break;
                    case 'L':
                        recordList[i] = recordList[i].ToAstmLRecord();
                        break;
                    case 'P':
                        recordList[i] = recordList[i].ToAstmPRecord();
                        break;
                    case 'O':
                        recordList[i] = recordList[i].ToAstmORecord();
                        break;
                    case 'R':
                        recordList[i] = recordList[i].ToAstmRRecord();
                        break;
                    case 'C':
                        recordList[i] = recordList[i].ToAstmCRecord();
                        break;
                }
            }
        }

        public void UpdateToSQL()
        {
            string idketquatumaycls = "";            
            for (int i = 0; i < recordList.Count; i++)
            {
                if (recordList[i] is AstmPRecord)
                {
                    for (int j = i+1; j < recordList.Count; j++)
                    {
                        if (recordList[j] is AstmORecord)
                        {

                            idketquatumaycls = UpdateKetQuaTuMayCLS((AstmPRecord)recordList[i], (AstmORecord)recordList[j]);                            
                            List<AstmRecord> results =  new List<AstmRecord>();
                            for (int k = j+1; k < recordList.Count; k++)
                            {
                                if (recordList[k] is AstmRRecord
                                    || recordList[k] is AstmCRecord) results.Add(recordList[k]);
                                else
                                {
                                    j = k;
                                    i = k;
                                    break;
                                }
                            }
                            if (results.Count > 0 && idketquatumaycls!="") UpdateKetQuaTuMayCLS_CT(idketquatumaycls, results);
                        }
                    }
                }               
            }
        }

        private string UpdateKetQuaTuMayCLS(AstmPRecord patient, AstmORecord order)
        {
            try
            {
                string sql = @"set @idketquatumaycls = (select idketquatumaycls from ketquatumaycls where maketqua=@maketqua AND  LOAIMAY ='E2010');
                               if(@idketquatumaycls is null)
                                    begin
                                        insert into ketquatumaycls(mabenhnhan,TenBenhNhan,ngaythuchien,maketqua,loaimay,stt,mota)
                                        values (@mabenhnhan,@TenBenhNhan,@ngaythuchien,@maketqua,@loaimay,@stt,@mota);
                                        set @idketquatumaycls = @@IDENTITY;
                                    end
                               else 
                                    begin
                                        update ketquatumaycls
                                        set mabenhnhan=@mabenhnhan,
                                            TenBenhNhan=@TenBenhNhan,
                                            ngaythuchien=@ngaythuchien,
                                            maketqua=@maketqua,
                                            loaimay=@loaimay,
                                            stt=@stt,
                                            mota=@mota
                                        where idketquatumaycls = @idketquatumaycls;
                                    end";

                ServerConnect.I.Conn.Open();
                SqlCommand cmd = new SqlCommand(sql, ServerConnect.I.Conn);
                if (order.SpecimenID.Length<10)
                {
                    string [] a=order.SpecimenID.Split('-');
                    if(a[1].Length==2)
                        a[1] = "0" + a[1].ToString();
                    if (a[1].Length ==1)
                        a[1] = "00" + a[1].ToString();
                    order.SpecimenID = a[0].ToString()+"-"+a[1].ToString();
                }
                SqlParameter param1 = new SqlParameter("@idketquatumaycls", SqlDbType.BigInt);
                param1.Direction = ParameterDirection.Output;
                cmd.Parameters.Add(param1);
                SqlParameter param2 = new SqlParameter("@mabenhnhan", SqlDbType.VarChar);
                param2.Direction = ParameterDirection.Input;
                param2.Value = patient.IDNo;
                cmd.Parameters.Add(param2);
                SqlParameter param3 = new SqlParameter("@TenBenhNhan", SqlDbType.NVarChar);
                param3.Direction = ParameterDirection.Input;
                param3.Value = patient.Name;
                cmd.Parameters.Add(param3);
                SqlParameter param4 = new SqlParameter("@ngaythuchien", SqlDbType.DateTime);
                param4.Direction = ParameterDirection.Input;
                param4.Value = getDateTime(order.RequestedOrderedDateTime);
                cmd.Parameters.Add(param4);
                SqlParameter param5 = new SqlParameter("@maketqua", SqlDbType.VarChar);
                param5.Direction = ParameterDirection.Input;
                param5.Value = order.SpecimenID;
                cmd.Parameters.Add(param5);
                SqlParameter param6 = new SqlParameter("@loaimay", SqlDbType.VarChar);
                param6.Direction = ParameterDirection.Input;
                param6.Value = "E2010";
                cmd.Parameters.Add(param6);
                SqlParameter param7 = new SqlParameter("@stt", SqlDbType.VarChar);
                param7.Direction = ParameterDirection.Input;
                param7.Value = order.CollectorID;
                cmd.Parameters.Add(param7);
                SqlParameter param8 = new SqlParameter("@mota", SqlDbType.NVarChar);
                param8.Direction = ParameterDirection.Input;
                param8.Value = order.SpecimenDescriptor;
                cmd.Parameters.Add(param8);
                if (cmd.ExecuteNonQuery() >= 0)
                {
                    return cmd.Parameters["@idketquatumaycls"].Value.ToString();
                }
                else return "";
            }
            catch (Exception ex)
            {
                return "";
            }
            finally
            {
                if (ServerConnect.I.Conn.State != ConnectionState.Closed)
                    ServerConnect.I.Conn.Close();
            }
        }

        private void UpdateKetQuaTuMayCLS_CT(string idketquatumaycls,List<AstmRecord> results)
        {
            try
            {
                string sql="";
                if (idketquatumaycls != "" && results.Count > 0)
                {
                    for (int i = 0; i < results.Count; i++)
                    {
                        if (results[i] is AstmRRecord)
                        {
                            string paramName = getParamName(((AstmRRecord)results[i]).UniversalTestID);
                            string value = getValue(((AstmRRecord)results[i]).Values);
                            if (paramName=="A-HBS" && value==">1000"){value = "1000";}
                            if (paramName == "HBSAC-QN" && value == ">130"){value = "130";}
                            if (paramName == "FT4" && value == ">7.7"){value = "7.7";}
                            if (paramName == "FERR" && value == ">2000") { value = "2000"; }
                            if (paramName == "AFP" && value == ">1210") { value = "1210"; }
                            if (paramName == "A-HBS" && value == "<2.00") { value = "2.00"; }
                            if (paramName == "HBsAg-QN" && value == "<0.050") { value = "0.050"; }
                            sql += System.Environment.NewLine
                                + "delete from ketquatumaycls_ct where idketquatumaycls=" + idketquatumaycls + " and tenthongso='" + paramName + "';"
                                + System.Environment.NewLine
                                + @"insert into ketquatumaycls_ct(idketquatumaycls,tenthongso,giatri,referenceranges,units)
                                   values(" + idketquatumaycls + ",'" + paramName + "','" + value + "','" + ((AstmRRecord)results[i]).ReferenceRanges.Replace("^", " - ") + "','" + ((AstmRRecord)results[i]).Units + "');";
                            if (((i + 1) < results.Count) && results[i + 1] is AstmCRecord)
                            {
                                sql += System.Environment.NewLine
                                    + @"update ketquatumaycls_ct set description='"+((AstmCRecord)results[++i]).CommentText.Replace("^"," = ")+@"'
                                        where idketquatumaycls=" + idketquatumaycls + " and tenthongso='" + paramName + "';";                                
                            }
                        }
                    }
                    ServerConnect.I.BeginTran();
                    if (ServerConnect.I.ExecSQL(sql))
                    {
                        ServerConnect.I.Commit();
                    }
                    else ServerConnect.I.RollBack();
                }
            }
            catch(Exception ex)
            {

            }
        }

        public DateTime getDateTime(string value)
        {
            if (value == null || value == "") return DateTime.Now;
            int year,month,day,hour,minute,second;
            year = int.Parse(value.Substring(0, 4));
            month = int.Parse(value.Substring(4,2));
            day = int.Parse(value.Substring(6,2));
            hour = int.Parse(value.Substring(8, 2));
            minute = int.Parse(value.Substring(10, 2));
            second = int.Parse(value.Substring(12, 2));
            DateTime date = new DateTime(year,month,day,hour,minute,second);
            return date;
            
        }

        public string getValue(string value)
        {
            if (value == null || value == "") return "0";
            string[] list = value.Split('^');
            if (list.Length > 1)
            {
                if (list[0] == "-1") return "-" + list[1].Trim('>', '<', '=');
                else return list[1].Trim('>', '<', '=');
            }
            else return list[0];
        }

        public string getParamName(string value)
        {
            if (value == null || value == "") return "XXX";
            string[] list = value.Split('^');
            if (list[3] != "")
                if (Char.IsDigit(list[3], 0)) return TestParams.getTestCode(int.Parse(list[3]));
                else return list[3];
            else return "XXX";
        }
    }

    class AstmRecord
    {        
        public string data="";
        public bool isFinished = false;
        public int recordType = 0;

        public AstmRecord()
        {
        }
        public bool AddFrame(AstmFrame frame)
        {
            string fdata = frame.getData();
            this.data += fdata.Substring(1);
            if (frame.frameType == AstmFrame.END_FRAME) isFinished = true;
            return true;
        }

        public char getRecordType()
        {
            return this.data.ToCharArray()[0];
        }

        public AstmHRecord ToAstmHRecord()
        {
            AstmHRecord h = new AstmHRecord();
            h.data = this.data;
            h.isFinished = this.isFinished;
            h.recordType = this.recordType;
            h.Build();
            return h;
        }

        public AstmLRecord ToAstmLRecord()
        {
            AstmLRecord l = new AstmLRecord();
            l.data = this.data;
            l.isFinished = this.isFinished;
            l.recordType = this.recordType;
            l.Build();
            return l;
        }

        public AstmPRecord ToAstmPRecord()
        {
            AstmPRecord p = new AstmPRecord();
            p.data = this.data;
            p.isFinished = this.isFinished;
            p.recordType = this.recordType;
            p.Build();
            return p;
        }

        public AstmORecord ToAstmORecord()
        {
            AstmORecord o = new AstmORecord();
            o.data = this.data;
            o.isFinished = this.isFinished;
            o.recordType = this.recordType;
            o.Build();
            return o;
        }

        public AstmRRecord ToAstmRRecord()
        {
            AstmRRecord r = new AstmRRecord();
            r.data = this.data;
            r.isFinished = this.isFinished;
            r.recordType = this.recordType;
            r.Build();
            return r;
        }

        public AstmCRecord ToAstmCRecord()
        {
            AstmCRecord c = new AstmCRecord();
            c.data = this.data;
            c.isFinished = this.isFinished;
            c.recordType = this.recordType;
            c.Build();
            return c;
        }
    }

    class AstmHRecord: AstmRecord
    {
        public string RecordTypeID;
        public string DelimiterDefinition;
        public string MessageControlID;
        public string AccessPassword;
        public string SenderName;
        public string SenderAddress;
        public string ReservedField;
        public string SenderTelephoneNumber;
        public string Characteristics;
        public string ReceiverID;
        public string Comment;
        public string ProcessingID;
        public string VersionNo;
        public string DateTime;

        public void Build()
        {
            string[] paramArr; // 14 fields
            paramArr = this.data.Split('|');
            RecordTypeID = paramArr[0];
            DelimiterDefinition = paramArr[1];
            MessageControlID = paramArr[2];
            AccessPassword = paramArr[3];
            SenderName = paramArr[4];
            SenderAddress = paramArr[5];
            ReservedField = paramArr[6];
            SenderTelephoneNumber = paramArr[7];
            Characteristics = paramArr[8];
            ReceiverID = paramArr[9];
            Comment = paramArr[10];
            ProcessingID = paramArr[11];
            VersionNo = paramArr[12];
            DateTime = paramArr[13];
        }
    }

    class AstmLRecord: AstmRecord
    {
        public string RecordTypeID;
        public string SequenceNumber;
        public string TerminationCode;

        public AstmLRecord()
        {
        }

        public void Build()
        {
            string[] paramArr; // 2 or 3 fields
            paramArr = this.data.Split('|');
            RecordTypeID = paramArr[0];
            SequenceNumber = paramArr[1];
            if(paramArr.Length>2)
                TerminationCode = paramArr[2];
        }
    }

    class AstmPRecord: AstmRecord
    {
        public string RecordTypeID;
        public string SequenceNumber;
        public string PracticeID;
        public string LaboratoryID;
        public string IDNo;
        public string Name;
        public string MothersMaidenName;
        public string DateOfBirth;
        public string Sex;
        public string Race_ethnicOrigin;
        public string Address;
        public string ReservedField;
        public string TelephoneNumber;
        public string AttendingPhysicianID;
        public string SpecialField1;
        public string SpecialField2;
        public string Height;
        public string Weight;
        public string Diagnosis;
        public string Medications;
        public string Diet;
        public string PracticeFieldNo1;
        public string PracticeFieldNo2;
        public string AdmissionAndDischagerDates;
        public string AdmissionStatus;
        public string Location;
        public string NatureOfAlternativeDiagnosticCode;
        public string AlternativeDiagnosticCode;
        public string PatientReligion;
        public string MaritalStatus;
        public string IsolationStatus;
        public string Language;
        public string HospitalService;
        public string HospitalInstitution;
        public string DosageCategory;

        public AstmPRecord()
        {
        }

        public void Build()
        {
            string[] paramArr; // 35 fields
            paramArr = this.data.Split('|');
            RecordTypeID=paramArr[0];
            SequenceNumber = paramArr[1];
            PracticeID = paramArr[2];
            LaboratoryID = paramArr[3];
            IDNo = paramArr[4];
            Name = paramArr[5];
            MothersMaidenName = paramArr[6];
            DateOfBirth = paramArr[7];
            Sex = paramArr[8];
            Race_ethnicOrigin = paramArr[9];
            Address = paramArr[10];
            ReservedField = paramArr[11];
            TelephoneNumber = paramArr[12];
            AttendingPhysicianID = paramArr[13];
            SpecialField1 = paramArr[14];
            SpecialField2 = paramArr[15];
            Height = paramArr[16];
            Weight = paramArr[17];
            Diagnosis = paramArr[18];
            Medications = paramArr[19];
            Diet = paramArr[20];
            PracticeFieldNo1 = paramArr[21];
            PracticeFieldNo2 = paramArr[22];
            AdmissionAndDischagerDates = paramArr[23];
            AdmissionStatus = paramArr[24];
            Location = paramArr[25];
            NatureOfAlternativeDiagnosticCode = paramArr[26];
            AlternativeDiagnosticCode = paramArr[27];
            PatientReligion = paramArr[28];
            MaritalStatus = paramArr[29];
            IsolationStatus = paramArr[30];
            Language = paramArr[31];
            HospitalService = paramArr[32];
            HospitalInstitution = paramArr[33];
            DosageCategory = paramArr[34];
        }
    }

    class AstmORecord : AstmRecord
    {
        public string RecordTypeID;
        public string SequenceNumber;
        public string SpecimenID;
        public string InstrumentSpecimenId;
        public string UniversalTestID;
        public string Priority;
        public string RequestedOrderedDateTime;
        public string SpecimenCollectionDateTime;
        public string CollectionEndTime;
        public string CollectionVolume;
        public string CollectorID;
        public string ActionCode;
        public string DangerCode;
        public string RelevantClinical;
        public string DateTimeSpecimenReceived;
        public string SpecimenDescriptor;
        public string OrderingPhysician;
        public string PhysiciantelephoneNumber;
        public string UserFieldNo1;
        public string UserFieldNo2;
        public string LaboratoryFieldNo1;
        public string LaboratoryFieldNo2;
        public string DateTimeResultsReported;
        public string Instr;
        public string InstrumentSectionID;
        public string ReportTypes;
        public string ReservedField;
        public string LocationOrWard;
        public string NosocomialInfectionFlag;
        public string SpecimenService;
        public string SpecimenInstitution;

        public AstmORecord()
        {
        }

        public void Build()
        {
            string[] paramArr; // 31 fields
            paramArr = this.data.Split('|');

            RecordTypeID = paramArr[0];
            SequenceNumber = paramArr[1];
            string[] temp = paramArr[2].Split('-');
            if (temp.Length > 1)
                SpecimenID = paramArr[2];
            else SpecimenID = DateTime.Today.ToString("yyMMdd") + "-" + temp[0];
            InstrumentSpecimenId = paramArr[3];
            UniversalTestID = paramArr[4];
            Priority = paramArr[5];
            RequestedOrderedDateTime = paramArr[6];
            SpecimenCollectionDateTime = paramArr[7];
            CollectionEndTime = paramArr[8];
            CollectionVolume = paramArr[9];
            CollectorID = paramArr[10];
            ActionCode = paramArr[11];
            DangerCode = paramArr[12];
            RelevantClinical = paramArr[13];
            DateTimeSpecimenReceived = paramArr[14];
            SpecimenDescriptor = paramArr[15];
            OrderingPhysician = paramArr[16];
            PhysiciantelephoneNumber = paramArr[17];
            UserFieldNo1 = paramArr[18];
            UserFieldNo2 = paramArr[19];
            LaboratoryFieldNo1 = paramArr[20];
            LaboratoryFieldNo2 = paramArr[21];
            DateTimeResultsReported = paramArr[22];
            Instr = paramArr[23];
            InstrumentSectionID = paramArr[24];
            ReportTypes = paramArr[25];
            ReservedField = paramArr[26];
            LocationOrWard = paramArr[27];
            NosocomialInfectionFlag = paramArr[28];
            SpecimenService = paramArr[29];
            SpecimenInstitution = paramArr[30];
        }
    }

    class AstmRRecord : AstmRecord
    {
        public string RecordTypeID;
        public string SequenceNumber;
        public string UniversalTestID;
        public string Values;
        public string Units;
        public string ReferenceRanges;
        public string ResultAbnormalFlag;
        public string NatureOfAbnormalityTesting;
        public string ResultStatus;
        public string DateOfChange;
        public string OperatorID;
        public string DateTimeTestStarted;
        public string DateTimeTestCompleted;
        public string InstrumentID;

        public AstmRRecord()
        {
        }

        public void Build()
        {
            string[] paramArr; // 14 fields
            paramArr = this.data.Split('|');
            RecordTypeID = paramArr[0];
            SequenceNumber = paramArr[1];
            UniversalTestID = paramArr[2];
            Values = paramArr[3];
            Units = paramArr[4];
            ReferenceRanges = paramArr[5];
            ResultAbnormalFlag = paramArr[6];
            NatureOfAbnormalityTesting = paramArr[7];
            ResultStatus = paramArr[8];
            DateOfChange = paramArr[9];
            OperatorID = paramArr[10];
            DateTimeTestStarted = paramArr[11];
            DateTimeTestCompleted = paramArr[12];
            InstrumentID = paramArr[13];
        }
    }

    class AstmCRecord : AstmRecord
    {
        public string RecordTypeID;
        public string SequenceNumber;
        public string CommentSource;
        public string CommentText;
        public string CommentType;

        public AstmCRecord()
        {
        }

        public void Build()
        {
            string[] paramArr; // 5 fields
            paramArr = this.data.Split('|');
            RecordTypeID = paramArr[0];
            SequenceNumber = paramArr[1];
            CommentSource = paramArr[2];
            CommentText = paramArr[3];
            CommentType = paramArr[4];
        }
    }

    class AstmFrame
    {
        public const int INTERMEDIATE_FRAME = 1;
        public const int END_FRAME = 2;
        string data="";
        public bool isValidFrame=false;
        public int frameType = 0;
        public static int nextFrameIndex=1;

        public AstmFrame(string data)
        {
            this.data = data;
            if (this.data.StartsWith(ASTM.STX.ToString()))
            {
                if (this.data.Contains(ASTM.ETB.ToString()))
                {
                    this.frameType = INTERMEDIATE_FRAME;
                    if (checkSum())
                    {
                        char findex = this.getData().ToCharArray()[0];
                        if (Char.IsDigit(findex))
                        {
                            if (nextFrameIndex == int.Parse(findex.ToString()))
                            {
                                this.isValidFrame = true;
                                nextFrameIndex = (int.Parse(findex.ToString())+1)%8;
                            }
                            else this.isValidFrame = false;
                        }
                        else this.isValidFrame = false;
                    }
                    else this.isValidFrame = false;
                }
                else if (this.data.Contains(ASTM.ETX.ToString()))
                {
                    this.frameType = END_FRAME;
                    if (checkSum())
                    {
                        char findex = this.getData().ToCharArray()[0];
                        if (Char.IsDigit(findex))
                        {
                            if (nextFrameIndex == int.Parse(findex.ToString()))
                            {
                                this.isValidFrame = true;
                                nextFrameIndex = (int.Parse(findex.ToString()) + 1) % 8;
                            }
                            else this.isValidFrame = false;
                        }
                        else this.isValidFrame = false;
                    }
                    else this.isValidFrame = false;
                }
                else this.isValidFrame = false;
            }           
        }

        private bool checkSum()
        {
            return true;
        }

        public string getData()
        {
            if (this.frameType == INTERMEDIATE_FRAME)
                return this.data.Substring(1, this.data.IndexOf(ASTM.ETB)-1);
            else if (this.frameType == END_FRAME)
                return this.data.Substring(1, this.data.IndexOf(ASTM.ETX)-1);
            else return "";
        }
    }
}