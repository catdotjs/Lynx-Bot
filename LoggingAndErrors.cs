using Discord;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lynx_Bot {
    static class LoggingAndErrors {
        
        private static Dictionary<string, string> ConsoleCommands = new Dictionary<string, string>() {
            { "","Things that start with '--' are commands, if it doesn't have it then it is a tag"},
            { "Global","This tag tells bot to apply changes globally rather than just test server(specified in config.js)"},
            { "--AddCommands","Adds slash commands to bot's either test server or global. Checks if 'global' tag is said"},
            { "--RemoveCommands","Removes all slash commands to bot's either test server or global. Checks if 'global' tag is said"},
        };
        public static Task Log(LogMessage message) {
            ConsoleColor severityColor = message.Severity switch {
                LogSeverity.Warning => ConsoleColor.Yellow,
                LogSeverity.Error => ConsoleColor.Red,
                LogSeverity.Critical => ConsoleColor.DarkRed,
                LogSeverity.Verbose => ConsoleColor.Green,
                LogSeverity.Debug => ConsoleColor.Magenta,
                _ => ConsoleColor.White
            };
            Console.ForegroundColor = severityColor;
            Console.WriteLine($"[{DateTime.Now}/{message.Severity}] {message.Source} | {message.Message}");
            Console.ForegroundColor=ConsoleColor.White;
            return Task.CompletedTask;
        }
        public static Task LogException(Exception ex,LogSeverity severity = LogSeverity.Error) {
            return Log(new LogMessage(severity, ex.Source, ex.Message));
        }

        public static Task CommandHelp() {
            foreach(KeyValuePair<string, string> commands in ConsoleCommands) {
                Console.WriteLine((commands.Key!=""?$"   {commands.Key} - ":"")+$"{commands.Value}");
            }
            return Task.CompletedTask;
        }
    }
}
