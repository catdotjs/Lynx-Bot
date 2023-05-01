using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// Please fuck off
#pragma warning disable CS8600, CS8602, CS8604

namespace Lynx_Bot.APIs {
    static class RapidAPI {
        public static HttpClient WordsAPI = new HttpClient() {
            BaseAddress=new Uri("https://wordsapiv1.p.rapidapi.com/words/"),
        };
        public static void OnStart() {
            // Words
            WordsAPI.DefaultRequestHeaders.Add("X-RapidAPI-Key", (string)Program.Config["RapidAPI"]["WordsAPI"]);
            WordsAPI.DefaultRequestHeaders.Add("X-RapidAPI-Host", "wordsapiv1.p.rapidapi.com");
        }
    }

}
