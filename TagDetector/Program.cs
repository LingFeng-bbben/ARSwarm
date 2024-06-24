using Newtonsoft.Json;
using WebSocketSharp.Server;

namespace TagDetector
{
    internal static class Program
    {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            // To customize application configuration such as set high DPI settings or default font,
            // see https://aka.ms/applicationconfiguration.
            ApplicationConfiguration.Initialize();
            webSocket = new WebSocketServer("ws://0.0.0.0:8080/");
            webSocket.AddWebSocketService<TagService>("/tag");
            webSocket.Start();
            Application.Run(new Form1());
            webSocket.Stop();
        }
        public static void broadcast()
        {
            webSocket.WebSocketServices.Broadcast(JsonConvert.SerializeObject(Program.Tagpos));
        }

        static WebSocketServer webSocket;
        public static float[,] Tagpos = new float[30,6];
    }
}