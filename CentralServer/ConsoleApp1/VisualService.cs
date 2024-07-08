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
        protected override async void OnOpen()
        {
            base.OnOpen();
            while (this.State == WebSocketSharp.WebSocketState.Open) {
                var sessions = Program.sessionToDevice.Select(o=>o.Value).ToList().OrderBy(o=>o.givenTag);
                var str = JsonConvert.SerializeObject(sessions);
                this.Send(str);
                await Task.Delay(100);
            }
        }

        protected override void OnMessage(MessageEventArgs e)
        {
            Program.VirtualSensors = JsonConvert.DeserializeObject<List<DeviceSensor>>(e.Data);
            base.OnMessage(e);
        }
    }
}
