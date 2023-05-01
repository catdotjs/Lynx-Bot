using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Text;
using System.Text.Json.Nodes;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using Lynx_Bot.APIs;
using Lynx_Bot.Processing;
using Newtonsoft.Json.Linq;

// Please fuck off
#pragma warning disable CS8600, CS8602, CS8604, CS8603

namespace Lynx_Bot.Commands {
    static class Generate {
        public static async Task ChooseGeneration(SocketSlashCommand Context) {
            string UnitName = Context.Data.Options.First().Name;
            Dictionary<string, object> Data = CommandManager.OptionsAsDictionary(Context.Data.Options.First().Options);

            try { 
                switch(UnitName) {
                    case "password":
                        await Context.RespondAsync(await GeneratePassword((long)Data["length"]), ephemeral: true);
                    break;

                    case "quote":
                        await Context.RespondAsync(embed:await GenerateQuote());
                    break;

                    default:
                        throw new ArgumentException($"There is no generation function for {UnitName}.");
                }
            } catch(Exception ex) { 
                await LoggingAndErrors.LogException(ex); 
                await Context.RespondAsync(ex.Message);
            }
        }

        private static async Task<string> GeneratePassword(long length) {
            // Get that response
            HttpResponseMessage resp = await APINinja.API.GetAsync($"/v1/passwordgenerator?length={Math.Clamp(length,3,32)}");

            // Turn that to JSON
            JObject respJson = JObject.Parse(await resp.Content.ReadAsStringAsync());

            // Slay the results
            return $"Generated Password: {respJson["random_password"]}";
        }

        private static async Task<Embed> GenerateQuote() {
            // Get that response
            HttpResponseMessage resp = await APINinja.API.GetAsync($"/v1/quotes");

            // Turn that to JSON
            JObject respJson = (JObject)JArray.Parse(await resp.Content.ReadAsStringAsync())[0];
            EmbedBuilder embed = new EmbedBuilder();
            embed.Color=ImageProcessing.RandomColour();
            embed.Title=$"# \u201C {(""+respJson["quote"]).TrimEnd('.')} \u201E";
            embed.Description=$"  \u2014 **{respJson["author"]}**";

            // Slay the results
            return embed.Build();
        }
    }
}