using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lynx_Bot.Processing {
    static class FileManagement {
        public static void Remove(string path) {
            try { 
                File.Delete(path);
            }catch(Exception ex) { LoggingAndErrors.LogException(ex); }
        }
    }
}
