using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CentralServer
{
    public class DeviceInfo
    {
        public int givenTag = -1;
        public string macAddress = string.Empty;
        public DeviceInfo(int givenTag, string macAddress)
        {
            this.givenTag = givenTag;
            this.macAddress = macAddress;
        }
    }
}
