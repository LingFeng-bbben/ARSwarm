using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebSocketSharp;
using WebSocketSharp.Server;
using Newtonsoft.Json;

namespace TagDetector
{
    public class TagService : WebSocketBehavior
    {
        protected override void OnMessage(MessageEventArgs e)
        {
            if (e.Data != null) {
                var str = JsonConvert.SerializeObject(Program.Tagpos);
                Send(str);
            }
        }
    }
}
