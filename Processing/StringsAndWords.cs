using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
#pragma warning disable CS8600,CS8602

namespace Lynx_Bot.Processing {
    static class StringsAndWords {
        public static string CapitalizeFirstLetter(string word) => char.ToUpper(word[0])+word.Substring(1);
        public static JArray RemoveMember(this JArray array,string token) {
            var NewArray = array.Where(a => (string)a!=token);
            return new JArray(NewArray);
        }
    }
}
