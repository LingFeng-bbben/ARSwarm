using CentralServer;
using System;
using WebSocketSharp.Server;

namespace ConsoleApp1
{
    public class Program
    {
        public static byte[][] aprilDict = new byte[0][];
        public static Dictionary<string,DeviceInfo> sessionToDevice = new Dictionary<string,DeviceInfo>();
        public static List<DeviceSensor> VirtualSensors = new List<DeviceSensor>();
        public static VisualService? visualServiceCurrent;
        public static void Main(string[] args)
        {
            var json = File.ReadAllText("dict.json");
            aprilDict = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, byte[][]>>(json)["april_16h5"];
            
            var wssv = new WebSocketServer("ws://0.0.0.0");
            wssv.AddWebSocketService<BoardService>("/ws");
            wssv.AddWebSocketService<VisualService>("/vs");
            
            wssv.Start();
            Console.ReadKey(true);
           
            wssv.Stop();
        }
    }
}