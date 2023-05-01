using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lynx_Bot.Processing {
    static class StringsAndWords {
        public static string CapitalizeFirstLetter(string word) => char.ToUpper(word[0])+word.Substring(1);
  
    }
}
