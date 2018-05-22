using System;
using System.Collections.Generic;
using System.Text;

namespace ConnectLab
{
    
    public interface IDevice
    {
        bool Connect();
        bool IsConnectting
        {
            get;
        }
        bool DeviceName
        {
            get;
        }
        bool DeviceID
        {
            get;
        }
        bool IsUsing
        {
            get;
            set;
        }
        
    }
}
