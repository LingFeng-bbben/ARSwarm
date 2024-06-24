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
            if (Program.sessionToDevice.Any(o=>o.Key== this.ID))
            {
                var deviceinfo = Program.sessionToDevice[this.ID];
                Console.WriteLine("ID: " + deviceinfo.givenTag + ": " + e.Data);
            }
            
            List<bool> bits = new List<bool>();
            if (e.Data.StartsWith("IDREQ"))
            {
                
                var bitsCount = 4 * 4;
                var macid = e.Data.Split(' ')[1];
                var tagid = idCount;
                if(Program.sessionToDevice.Any(o=>o.Value.macAddress == macid)){
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
            }

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