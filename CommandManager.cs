using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Timers;
using Timer = System.Timers.Timer;
using Discord;
using Lynx_Bot.Commands;
using System.Text.RegularExpressions;

namespace Lynx_Bot {
    class CommandManager {
        public static Dictionary<string, Func<SocketSlashCommand,Task>> CommandDict = new Dictionary<string, Func<SocketSlashCommand, Task>>() {
            { "ping", async(Context)=>{ await Context.RespondAsync("pong"); }}, // This command is soley to test functionality of bot. DO NOT WRITE CODE LIKE THIS. EVER.
            
            // Polling
            { "send-poll", Polling.SendPoll}, // Right way to do commands

            // Conversion
            { "convert-angle", Conversion.ConvertAngle},
            { "convert-temperature", Conversion.ConvertTemperature},
            { "convert-mass", Conversion.ConvertMass},
        };

        public static Dictionary<string, Func<SocketMessageComponent, Task>> ButtonDict = new Dictionary<string, Func<SocketMessageComponent, Task>>() {
            { "PollingButton", Polling.RecieveVote}
        };

        private static List<(Func<Task> Function,Timer Time)> RoutineList = new List<(Func<Task> Function, Timer Time)>();
        public static async Task SlashCommands(SocketSlashCommand Context) {
            // I found a better way to do this :)
            try { 
                await CommandDict[Context.CommandName.Replace("-dev", "")].Invoke(Context);
            } catch { 
                // Not in dictionary
                string ErrorMessage = $"{Context.CommandName} could not be found. May be legacy command.";
                await Context.RespondAsync(ErrorMessage, ephemeral: true);
                await LoggingAndErrors.Log(new LogMessage(LogSeverity.Error, "CommandManager", ErrorMessage));
            }
        }
        public static async Task Buttons(SocketMessageComponent Context) {
            string id = Regex.Replace(Context.Data.CustomId, @"[\d-]", "");

            try {
                await ButtonDict[id].Invoke(Context);
            } catch {
                // Not in dictionary
                string ErrorMessage = $"{id} could not be evaulated";
                await Context.RespondAsync(ErrorMessage, ephemeral: true);
                await LoggingAndErrors.Log(new LogMessage(LogSeverity.Error, "CommandManager", ErrorMessage));
            }

            // Nothing
            await Context.RespondAsync(id);
        }
        public static Dictionary<string, object> OptionsAsDictionary(IReadOnlyCollection<SocketSlashCommandDataOption> Options) {
            Dictionary<string, object> options = new Dictionary<string, object>();
            foreach(SocketSlashCommandDataOption Option in Options) {
                options.Add(Option.Name, Option.Value);
            }
            return options;
        }
        public static void AddRoutine(Func<Task> Function, float SecondInterval=1) {
            // Its easier to deal with seconds than ms
            Timer time = new Timer(SecondInterval*1000);
            time.Elapsed+=new ElapsedEventHandler((a,b)=>Function.Invoke());
            time.AutoReset=true;
            time.Start();
            RoutineList.Add((Function,time));
        }
    }
}
