using System;
using nanoFramework.M5Core2;
using nanoFramework.M5Stack;
using nanoFramework.UI;
using Console = nanoFramework.M5Stack.Console;
using System.Threading;
using nanoFramework.Networking;
using System.Net.WebSockets;
using System.Net.WebSockets.WebSocketFrame;
using System.Text;
using System.Drawing;
using nanoFramework.Presentation.Media;
using System.Diagnostics;
using System.Device.Spi;
using nanoFramework.Runtime.Native;

namespace NFApp1
{
    public class Program
    {
        public static ClientWebSocket websocketClient;
        public static string guid = "";
        public static void Main()
        {
            M5Core2.InitializeScreen();
            M5Core2.TouchEvent += TouchEventCallback;
            Console.Clear();
            const string Ssid = "PinkyHair";
            const string Password = "niconiconi";
            print("QAQ BOOTUP CHECK");
            print("");
            // Give 60 seconds to the wifi join to happen
            CancellationTokenSource cs = new(60000);
            var success = WifiNetworkHelper.ScanAndConnectDhcp(Ssid, Password, token: cs.Token, requiresDateTime: true);
            if (!success)
            {
                print("  WIFI FAILED");
                print("");
                print("REBOOT IN 1SEC");
                Thread.Sleep(1000);
                Power.RebootDevice();
                //Red Light indicates no Wifi connection
                throw new Exception("Couldn't connect to the Wifi network");
            }
            print("  WIFI   CONNECTED");
            //setup WebSocketClient
            websocketClient = new ClientWebSocket(new ClientWebSocketOptions()
            {
                //Change the heart beat to a 30 second interval
                KeepAliveInterval = TimeSpan.FromSeconds(30)
            });

            //Handler for receiving websocket messages. 
            websocketClient.MessageReceived += WebsocketClient_MessageReceived;

            //Connect the client to the websocket server with custom headers
            websocketClient.Connect("ws://192.168.137.1/ws");
            guid = Guid.NewGuid().ToString();
            if (websocketClient.State == WebSocketState.Open)
            {
                print("  WEBSOC CONNECTED");
                websocketClient.SendString("IDREQ " + guid);
                //Send a message very 5 seconds
                while (websocketClient.State == WebSocketState.Open)
                {
                    Thread.Sleep(300);
                    //Add something to report here
                    websocketClient.SendString("BEAT " + DateTime.UtcNow.Ticks);
                }
            }
            print("  WEBSOC FAILED");
            print("");
            print("REBOOT IN 1SEC");
            Thread.Sleep(1000);
            Power.RebootDevice();
        }
        private static void WebsocketClient_MessageReceived(object sender, MessageReceivedEventArgs e)
        {
            var client = (ClientWebSocket)sender;
            //If message is of type Text, echo message back to client
            if (e.Frame.MessageType == WebSocketMessageType.Text)
            {
                //check if message is not fragmented
                if (!e.Frame.IsFragmented)
                {
                    string message = Encoding.UTF8.GetString(e.Frame.Buffer, 0, e.Frame.MessageLength);
                    print(message);
                    ProcessMessage(message);
                    //client.SendString(message);
                }
                //close connection because fragmented messages are not allowed
                else
                {
                    client.Close(WebSocketCloseStatus.PolicyViolation, "Fragmented messages are not allowed"!);
                }
            }
        }

        private static void ProcessMessage(string message)
        {
            if (message.StartsWith("APTAG"))
            {
                var tagbin = message.Split(' ')[1];
                Console.Clear();
                for (ushort i = 0; i < 4; i++)
                {
                    for (ushort j = 0; j < 4; j++)
                    {
                        var thisbit = tagbin[i+j*4] == '1' ? true : false;
                        if (thisbit)
                        {
                            drawSquare((ushort)(i * 60+40), (ushort)(j * 60), 60, 60);
                        }
                    }

                }
            }
        }

        private static void TouchEventCallback(object sender, TouchEventArgs e)
        {
            if (websocketClient != null)
            {
                websocketClient.SendString("x " + e.X + "y " + e.Y);
            }
        }

        static void drawSquare(ushort x, ushort y, ushort width, ushort height)
        {
            int colorcount = width* height;
            ushort[] colors = new ushort[colorcount];
            for(int i=0;i<colors.Length;i++)
            {
                colors[i] = ushort.MaxValue;
            }
            Screen.Write(x, y, width, height,colors);
        }
        public static ushort ycol = 0;
        static void print(string message)
        {
            IFont font = new Retro8x16();
            if (ycol >= 240) {
                Console.Clear();
                ycol = 0; 
            }
            for (ushort i = 0; i < message.Length; i++)
            {
                var cha = message[i];
                var img = font[cha];
                var bitmap = new ushort[font.Height * font.Width];
                for (ushort j = 0; j < img.Length; j++)
                {
                    for(ushort k = 0; k < font.Width; k++)
                    {
                        var bit = (img[j] & ((byte)(0x01 << k)));
                        bitmap[j * font.Width + k] = bit > 0 ? ushort.MaxValue : ushort.MinValue;
                    }
                }
                Screen.Write((ushort)(i * font.Width), ycol, font.Width, font.Height, bitmap);
            }
            ycol += 16;
            
        }
    }
}
