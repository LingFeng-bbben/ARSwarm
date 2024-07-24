using CentralServer;
using Newtonsoft.Json;
using System.Collections.Generic;
using WebSocketSharp;
using WebSocketSharp.Server;

namespace ConsoleApp1
{
    public class BoardService : WebSocketBehavior
    {
        static int idCount = 1;

        protected override void OnMessage(MessageEventArgs e)
        {
            List<bool> bits = new List<bool>();
            if (e.Data.StartsWith("PINGS"))
            {
                Send("PINGR");
                return;
            }
            if (e.Data.StartsWith("IDREQ"))
            {

                var bitsCount = 4 * 4;
                var macid = e.Data.Split(' ')[1];
                var tagid = idCount;
                if (Program.sessionToDevice.Any(o => o.Value.macAddress == macid))
                {
                    var oldSession = Program.sessionToDevice.First(o => o.Value.macAddress == macid).Key;
                    if (Sessions.TryGetSession(oldSession, out var session))
                    {
                        session.Context.WebSocket.Close();
                    }
                    tagid = Program.sessionToDevice[oldSession].givenTag;
                    Program.sessionToDevice.Remove(oldSession);
                }
                else
                {
                    idCount++;
                    tagid = idCount;
                }
                Program.sessionToDevice.Add(this.ID, new CentralServer.DeviceInfo(tagid, macid));
                foreach (var byt in Program.aprilDict[tagid])
                {
                    var start = bitsCount - bits.Count;
                    for (var i = Math.Min(7, start - 1); i >= 0; i--)
                    {
                        bits.Add(((byt >> i) & 1) > 0);
                    }
                }
                string response = "APTAG ";
                foreach (var bit in bits)
                {
                    response += bit ? "1" : "0";
                }
                Send(response);
                Send("TAGNM "+ tagid);
                return;
            }

            //TODO: Make this more efficient
            if (!Program.sessionToDevice.Any(o=>o.Key== this.ID))
            {
                return;
            }
            var deviceinfo = Program.sessionToDevice[this.ID];
            Console.WriteLine("ID: " + deviceinfo.givenTag + ": " + e.Data);

            //still need the buffer here because esp32 may not tolorate high rate of data flow
            //(Takes cpu cycle)
            if (e.Data.StartsWith("MTREQ"))
            {
                if (Program.VirtualSensors.Any(o => o.givenTag == deviceinfo.givenTag))
                {
                    Send("MTSEN " + Program.VirtualSensors.Find(o => o.givenTag == deviceinfo.givenTag).message);
                }
                return;
            }

            //because the pc spec is higher so
            //no buffer, direct send
            if (Program.visualServiceCurrent == null) return;
            var deviceSensor = new DeviceSensor(deviceinfo.givenTag, e.Data);
            var sendstr = JsonConvert.SerializeObject(deviceSensor);
            Program.visualServiceCurrent.SendData(sendstr);

            base.OnMessage(e);
        }
        protected override void OnOpen()
        {
            base.OnOpen();
            Console.WriteLine("Someone Connected!");
        }
        protected override void OnClose(CloseEventArgs e)
        {
            Console.WriteLine("Close: " + this.ID);
            base.OnClose(e);
        }
        protected override void OnError(WebSocketSharp.ErrorEventArgs e)
        {
            Console.WriteLine("Error: " + this.ID);
            base.OnError(e);
        }
    }
}