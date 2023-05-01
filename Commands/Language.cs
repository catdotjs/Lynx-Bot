using Discord;
using Discord.WebSocket;
using Lynx_Bot.APIs;
using Lynx_Bot.Processing;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

#pragma warning disable CS8604, CS8602, CS8600, CS8619

namespace Lynx_Bot.Commands {
    class Language {
        public static async Task Define(SocketSlashCommand Context) {
            Dictionary<string, object> Data = CommandManager.OptionsAsDictionary(Context.Data.Options);

            // For some reason phonetics is only in API's "/everything"?!
            HttpResponseMessage resp = await RapidAPI.WordsAPI.GetAsync($"{Data["word"]}/definitions");
            HttpResponseMessage respPhonetics = await RapidAPI.WordsAPI.GetAsync($"{Data["word"]}");
            
            // Don't even bother with the rest
            if(!resp.IsSuccessStatusCode) {
                await Context.RespondAsync("Word was not found in the dictionary(API may be down)");
                return;
            }

            // Convert these bad boys to JSON
            JObject responseJson = JObject.Parse(await resp.Content.ReadAsStringAsync());
            JObject phoneticsJson = JObject.Parse(await respPhonetics.Content.ReadAsStringAsync());

            // Embeds
            EmbedBuilder embed = new EmbedBuilder();
            embed.Title=$"**{StringsAndWords.CapitalizeFirstLetter((string)responseJson["word"])}** \u2014 {phoneticsJson["pronunciation"]["all"]}"; ;
            embed.Color=ImageProcessing.RandomColour();

            // Get all of these to a dictionary
            Dictionary<string, string> definitions = new Dictionary<string, string>();

            foreach(JObject definition in (JArray)responseJson["definitions"]) {
                (string part, string meaning) word = ($"{definition["partOfSpeech"]}", $"🢂 {definition["definition"]}\n");

                // Does it exist bbg?
                if(definitions.Any(x=>x.Key==word.part)) {
                    // Horrible way to do it but fuck it :sob:
                    if((definitions[word.part].Length+word.meaning.Length)<1024) {
                        definitions[word.part]+=word.meaning;
                    }
                } else {
                    // Some part of the speech return god damn null. WHY?!?!='^^!'?
                    definitions.Add(word.part=="" ?"unknown":word.part, word.meaning);
                }
            }

            // Order them from least to most wordy
            bool inline = definitions.Count<4;
            foreach(KeyValuePair<string,string> ListOfDef in definitions.OrderByDescending(x => x.Value.Length*(inline?1:-1) )) {
                embed.AddField(ListOfDef.Key.ToUpperInvariant(),ListOfDef.Value, inline);
            }

            // Ship that bad boy off to discord
            await Context.RespondAsync(embed:embed.Build());
        }
    }
}
