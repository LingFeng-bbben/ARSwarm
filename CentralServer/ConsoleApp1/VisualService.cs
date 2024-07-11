using ConsoleApp1;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebSocketSharp;
using WebSocketSharp.Server;

namespace CentralServer
{
    public class VisualService : WebSocketBehavior
    {
        public void SendData(string data)
        {
            Send(data);
        }
        protected override void OnOpen()
        {
            Program.visualServiceCurrent = this;
            base.OnOpen();
        }

        protected override void OnMessage(MessageEventArgs e)
        {
            Program.VirtualSensors = JsonConvert.DeserializeObject<List<DeviceSensor>>(e.Data);
            base.OnMessage(e);
        }
    }
}
