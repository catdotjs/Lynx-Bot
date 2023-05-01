using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// Please fuck off
#pragma warning disable CS8600, CS8602, CS8604

namespace Lynx_Bot.APIs {
    static class APINinja {
        public static HttpClient API = new HttpClient() {
            BaseAddress=new Uri("https://api.api-ninjas.com/"),
        };
        public static void OnStart() {
            API.DefaultRequestHeaders.Add("X-Api-Key", (string)Program.Config["APINinja"]["APIKey"]);
        }
    }
}
