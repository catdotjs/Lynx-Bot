using Discord;
using Discord.Net;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lynx_Bot {
    class CommandLoader {
        private static SlashCommandBuilder[] Commands = new SlashCommandBuilder[]{
            new SlashCommandBuilder() {
                Name="ping",
                Description="a command to check if bot is alive"
            },
            new SlashCommandBuilder() {
                Name="send-poll",
                Description="Sends a question to poll. A and B are required. Max 5 options(a,b,c,d, and e)",
                Options=new List<SlashCommandOptionBuilder>{
                    // Question
                    new SlashCommandOptionBuilder{
                        Name="question",
                        Description="Question to ask",
                        Type=ApplicationCommandOptionType.String,
                        IsRequired=true,
                    },

                    // Time Limit
                    new SlashCommandOptionBuilder{
                        Name="time",
                        Description="Time limit(in minutes)",
                        Type=ApplicationCommandOptionType.Number,
                        IsRequired=true,
                    },

                    // Channel to send(if empty, just send it to current channel)
                    new SlashCommandOptionBuilder{
                        Name="channel",
                        Description="Which channel to send",
                        Type=ApplicationCommandOptionType.Channel,
                        IsRequired=false,
                    },


                    // Option + Explaining
                    // A
                    new SlashCommandOptionBuilder{
                        Name="option-a",
                        Description="First option",
                        Type=ApplicationCommandOptionType.String,
                        IsRequired=true,
                    },
                    new SlashCommandOptionBuilder{
                        Name="option-a-description",
                        Description="First option's description",
                        Type=ApplicationCommandOptionType.String,
                        IsRequired=false,
                    },

                    // B
                    new SlashCommandOptionBuilder{
                        Name="option-b",
                        Description="Second option",
                        Type=ApplicationCommandOptionType.String,
                        IsRequired=true,
                    },
                    new SlashCommandOptionBuilder{
                        Name="option-b-description",
                        Description="Second option's description",
                        Type=ApplicationCommandOptionType.String,
                        IsRequired=false,
                    },

                    // C
                    new SlashCommandOptionBuilder{
                        Name="option-c",
                        Description="Third option",
                        Type=ApplicationCommandOptionType.String,
                        IsRequired=false,
                    },
                    new SlashCommandOptionBuilder{
                        Name="option-c-description",
                        Description="Third option's description",
                        Type=ApplicationCommandOptionType.String,
                        IsRequired=false,
                    },

                    // D
                    new SlashCommandOptionBuilder{
                        Name="option-d",
                        Description="Fouth option",
                        Type=ApplicationCommandOptionType.String,
                        IsRequired=false,
                    },
                    new SlashCommandOptionBuilder{
                        Name="option-d-description",
                        Description="Fouth option's description",
                        Type=ApplicationCommandOptionType.String,
                        IsRequired=false,
                    },

                    // E
                    new SlashCommandOptionBuilder{
                        Name="option-e",
                        Description="Fifth option",
                        Type=ApplicationCommandOptionType.String,
                        IsRequired=false,
                    },
                    new SlashCommandOptionBuilder{
                        Name="option-e-description",
                        Description="Fifth option's description",
                        Type=ApplicationCommandOptionType.String,
                        IsRequired=false,
                    },
                },
            }
        };

        public static async Task CommandOverwrite(DiscordSocketClient client,bool isGlobal=false,ulong guildId=0,bool IsRemove=false) {
            try {
                // In case user wants to purge commands
                SlashCommandProperties[] BuildCommands = IsRemove?new SlashCommandProperties[0]: Commands.Select(a => a.Build()).ToArray();
                
                if(isGlobal) {
                    await client.BulkOverwriteGlobalApplicationCommandsAsync(BuildCommands);
                } else {
                    // Helps with debugging :>
                    BuildCommands.AsParallel().ForAll(a => a.Name+="-dev");
                    await client.GetGuild(guildId).BulkOverwriteApplicationCommandAsync(BuildCommands);
                }
                await LoggingAndErrors.Log(new LogMessage(message:$"Successfully updated slash commands {(isGlobal?"globally":"to test server")}",source: "CommandLoader", severity:LogSeverity.Debug));
            } catch(Exception ex) {
                await LoggingAndErrors.LogException(ex);
            }
        }
    }
}
