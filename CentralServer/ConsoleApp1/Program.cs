using CentralServer;
using System;
using WebSocketSharp.Server;

namespace ConsoleApp1
{
    public class Program
    {
        public static byte[][] aprilDict = new byte[0][];
        public static Dictionary<string,DeviceInfo> sessionToDevice = new Dictionary<string,DeviceInfo>();
        public static string Command = "0,0";
        public static void Main(string[] args)
        {
            var json = File.ReadAllText("dict.json");
            aprilDict = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, byte[][]>>(json)["april_16h5"];
            
            var wssv = new WebSocketServer("ws://0.0.0.0");
            wssv.AddWebSocketService<BoardService>("/ws");
            wssv.AddWebSocketService<VisualService>("/vs");
            
            wssv.Start();

            while (true) {
                var key = Console.ReadKey(true);
                Console.WriteLine(key.KeyChar);
                if(key.KeyChar == 'w')
                {
                    Command = "10,10";
                }
                if (key.KeyChar == 'a')
                {
                    Command = "-10,10";
                }
                if (key.KeyChar == 's')
                {
                    Command = "0,0";
                }
                if (key.KeyChar == 'd')
                {
                    Command = "10,-10";
                }
            }
            

            wssv.Stop();
        }
    }
}