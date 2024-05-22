using System;
using WebSocketSharp;
using WebSocketSharp.Server;

namespace ConsoleApp1
{
    public class TagGiver : WebSocketBehavior
    {
        static int id = -1;
        protected override void OnMessage(MessageEventArgs e)
        {
            Console.WriteLine(e.Data);
            List<bool> bits = new List<bool>();
            if (e.Data.StartsWith("IDREQ"))
            {
                id++;
                var bitsCount = 4 * 4;
                var guid = e.Data.Split(' ')[1];
                foreach (var byt in Program.aprilDict[id])
                {
                    var start = bitsCount - bits.Count;
                    for (var i = Math.Min(7, start - 1); i >= 0; i--)
                    {
                        bits.Add(((byt >> i) & 1)>0);
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
    }
    
    public class Program
    {
        public static byte[][] aprilDict = new byte[0][];
        public static void Main(string[] args)
        {
            var json = File.ReadAllText("dict.json");
            aprilDict = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, byte[][]>>(json)["april_16h5"];
            var wssv = new WebSocketServer("ws://0.0.0.0");

            wssv.AddWebSocketService<TagGiver>("/ws");
            wssv.Start();
            Console.ReadKey(true);
            wssv.Stop();
        }
    }
}