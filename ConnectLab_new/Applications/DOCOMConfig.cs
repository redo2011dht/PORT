using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Reflection;

namespace ConnectLab
{
    [Serializable]
    public abstract class DOCOMConfig
    {

        protected string portName = "COM1";
        protected string baudRate = "9600";
        protected string dataBits = "8";
        protected string parityType = "None";
        protected string stopBit = "1";
        protected string FileName = "";
        protected string maMayCLS = "";
        protected string handshake = "XOnXOff";

        public string PortName
        {
            get { return portName; }
            set { portName = value; }
        }
        public DOCOMConfig()
        {

        }
        public string StopBit
        {
            get { return stopBit; }
            set { stopBit = value; }
        }
        public string ParityType
        {
            get { return parityType; }
            set { parityType = value; }
        }
        public string DataBits
        {
            get { return dataBits; }
            set { dataBits = value; }
        }
        public string BaudRate
        {
            get { return baudRate; }
            set { baudRate = value; }
        }
        public string MaMayCLS
        {
            get { return maMayCLS; }
            set { maMayCLS = value; }
        }
        public string Handshake
        {
            get { return handshake; }
            set { handshake = value; }
        }
        public virtual bool Write()
        {
            return AppUntil.Write(this, AppParams.ConfigFolder + "\\" + this.FileName);
        }

        public virtual object Read()
        {
            return AppUntil.Read(this, AppParams.ConfigFolder + "\\" + this.FileName);
        }




       
    }
}
