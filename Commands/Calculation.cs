using Discord;
using Discord.WebSocket;
using Lynx_Bot.Processing;
using Lynx_Bot.Processing.Calculations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

#pragma warning disable CS8605

namespace Lynx_Bot.Commands {
    static class Calculation {
        public static async Task ChooseOperation(SocketSlashCommand Context) {
            string FunctionName = Context.Data.Options.First().Name;
            Dictionary<string, object> Data = CommandManager.OptionsAsDictionary(Context.Data.Options.First().Options);            
            EmbedBuilder embed = new EmbedBuilder();
            try {
                switch(FunctionName) {
                    case "evaluate":
                    // C# bs
                    if(Data.ContainsKey("wanthelp")) { 
                        if((bool)Data["wanthelp"]) { 
                            embed=CustomMath.HelpEmbed;
                            break;
                        }
                    }

                    string expression = (string)Data["expression"];
                    double result = CustomMath.Evaluate(expression);
                    embed.Title=$"{expression} = {result}";
                    break;

                    default:
                    throw new ArgumentException($"There is no calculation function for {FunctionName}.");
                }
                embed.Color=ImageProcessing.RandomColour();
                await Context.RespondAsync(embed:embed.Build());
            } catch(Exception ex) {
                await LoggingAndErrors.LogException(ex);
                await Context.RespondAsync(ex.Message,ephemeral:true);
            }
        }
    }
}
