using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using Lynx_Bot.Processing;
using Lynx_Bot.Processing.UnitConversion;
using UnitType = Lynx_Bot.Processing.UnitConversion.ConversionUnit.Type;

namespace Lynx_Bot.Commands {
    static class Conversion {
        public static async Task ConvertUnit(SocketSlashCommand Context) {
            string UnitName = Context.Data.Options.First().Name;
            Dictionary<string, object> Data = CommandManager.OptionsAsDictionary(Context.Data.Options.First().Options);
            EmbedBuilder embed = new();
            double value = (double)Data["value"];
            UnitType type= (UnitType)(int)(long)Data["unit"]; // Fuck off Discord and C#
            try { 
                switch(UnitName) {
                    case "angles":
                        embed=ConvertAngleEmbed(value, type);
                    break;

                    case "temperature":
                        embed=ConvertTemperatureEmbed(value,type);
                    break;

                    case "mass":
                        embed=ConvertMassEmbed(value,type);
                    break;

                    case "length":
                        embed=ConvertLengthEmbed(value, type);
                    break;

                    default:
                        throw new ArgumentException($"There is no converstion for {UnitName}.");
                }
                
                embed.Title=$"{Math.Round(value, 4)}{ConversionUnit.Symbols[type]} is...";
                await Context.RespondAsync(embed:embed.Build());
            } catch(Exception ex) { 
                await LoggingAndErrors.LogException(ex); 
                await Context.RespondAsync(ex.Message);
            }
        }

        private static EmbedBuilder ConvertAngleEmbed(double value, UnitType type) {
            Angle angle = new Angle(type,value);
            return EmbedBuilder(angle.Data);
        }
        private static EmbedBuilder ConvertTemperatureEmbed(double value, UnitType type) {
            Temperature temp = new Temperature(type, value);
            return EmbedBuilder(temp.Data);
        }
        private static EmbedBuilder ConvertMassEmbed(double value, UnitType type) {
            Mass mass = new Mass(type, value);
            return EmbedBuilder(mass.Data);
        }
        private static EmbedBuilder ConvertLengthEmbed(double value, UnitType type) {
            Length length = new Length(type, value);
            return EmbedBuilder(length.Data);
        }
        private static EmbedBuilder EmbedBuilder(List<(UnitType type, double value)> Data) {
            EmbedBuilder embed = new EmbedBuilder();
            embed.Color=ImageProcessing.RandomColour();

            foreach(var key in Data) {
                // Titles may have _'s they look ugly so I remove them :pog:
                embed.AddField($"__**In {(key.type+"").Replace("_", " ")}**__", $"{Math.Round(key.value, 4)}{ConversionUnit.Symbols[key.type]}",inline:true);
            }
            return embed;
        }
    }
}