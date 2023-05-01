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
            // Ping
            new SlashCommandBuilder() {
                Name="ping",
                Description="a command to check if bot is alive"
            },

            // Poll
            new SlashCommandBuilder() {
                Name="poll",
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
                DefaultMemberPermissions = GuildPermission.ManageMessages,
            },
            
            // Convert
            new SlashCommandBuilder() {
                Name="convert",
                Description="Converts units",
                Options= new List<SlashCommandOptionBuilder>{
                    // Convert Angle
                    new SlashCommandOptionBuilder {
                        Name="angles",
                        Description="Convert any angle to any other angle",
                        Type=ApplicationCommandOptionType.SubCommand,
                        Options=new List<SlashCommandOptionBuilder>{
                            // Value
                            new SlashCommandOptionBuilder{
                                Name="value",
                                Description="Value you would like to convert(just number, no symbols)",
                                Type=ApplicationCommandOptionType.Number,
                                IsRequired=true,
                            },

                            // Unit
                            new SlashCommandOptionBuilder{
                                Name="unit",
                                Description="Unit you would like to convert FROM(conversion will be done to all other possible values)",
                                Type=ApplicationCommandOptionType.Integer,
                                IsRequired=true,
                                Choices=new List<ApplicationCommandOptionChoiceProperties> {
                                    new ApplicationCommandOptionChoiceProperties{
                                        Name="Degree",
                                        Value=0,
                                    },
                                    new ApplicationCommandOptionChoiceProperties{
                                        Name="Radian",
                                        Value=1,
                                    },
                                    new ApplicationCommandOptionChoiceProperties{
                                        Name="Gradian",
                                        Value=2,
                                    },
                                }
                            }
                        }
                    },
                
                    // Convert Temperature
                    new SlashCommandOptionBuilder() {
                        Name="temperature",
                        Description="Converts any unit of temperature to any other unit of temperature",
                        Type=ApplicationCommandOptionType.SubCommand,
                        Options=new List<SlashCommandOptionBuilder>{
                            // Value
                            new SlashCommandOptionBuilder{
                                Name="value",
                                Description="Value you would like to convert(just number, no symbols)",
                                Type=ApplicationCommandOptionType.Number,
                                IsRequired=true,
                            },
                            
                            // Unit
                            new SlashCommandOptionBuilder{
                                Name="unit",
                                Description="Unit you would like to convert FROM(conversion will be done to all other possible values)",
                                Type=ApplicationCommandOptionType.Integer,
                                IsRequired=true,
                                Choices=new List<ApplicationCommandOptionChoiceProperties> {
                                    new ApplicationCommandOptionChoiceProperties{
                                        Name="Celsius",
                                        Value=3,
                                    },
                                    new ApplicationCommandOptionChoiceProperties{
                                        Name="Kelvin",
                                        Value=4,
                                    },
                                    new ApplicationCommandOptionChoiceProperties{
                                        Name="Fahrenheit",
                                        Value=5,
                                    },
                                }
                            }
                        },
                    },
                
                    // Convert Mass
                    new SlashCommandOptionBuilder() {
                        Name="mass",
                        Description="Converts any unit of mass to any other unit of mass",
                        Type=ApplicationCommandOptionType.SubCommand,
                        Options=new List<SlashCommandOptionBuilder>{
                            // Value
                            new SlashCommandOptionBuilder{
                                Name="value",
                                Description="Value you would like to convert(just number, no symbols)",
                                Type=ApplicationCommandOptionType.Number,
                                IsRequired=true,
                            },
                            
                            // Unit
                            new SlashCommandOptionBuilder{
                                Name="unit",
                                Description="Unit you would like to convert FROM(conversion will be done to all other possible values)",
                                Type=ApplicationCommandOptionType.Integer,
                                IsRequired=true,
                                Choices=new List<ApplicationCommandOptionChoiceProperties> {
                                    new ApplicationCommandOptionChoiceProperties{
                                        Name="Gram",
                                        Value=6,
                                    },
                                    new ApplicationCommandOptionChoiceProperties{
                                        Name="Kilogram",
                                        Value=7,
                                    },
                                    new ApplicationCommandOptionChoiceProperties{
                                        Name="Metric Tonne",
                                        Value=8,
                                    },

                                    new ApplicationCommandOptionChoiceProperties{
                                        Name="Ounce",
                                        Value=9,
                                    },
                                    new ApplicationCommandOptionChoiceProperties{
                                        Name="Pound",
                                        Value=10,
                                    },
                                    new ApplicationCommandOptionChoiceProperties{
                                        Name="US(Short) Ton",
                                        Value=11,
                                    },
                                    new ApplicationCommandOptionChoiceProperties{
                                        Name="UK(Long) Ton",
                                        Value=12,
                                    },
                                    new ApplicationCommandOptionChoiceProperties{
                                        Name="Dalton/Unified Atomic Mass",
                                        Value=21,
                                    },
                                }
                            }
                        },
                    },
                
                    // Convert Length
                    new SlashCommandOptionBuilder() {
                        Name="length",
                        Description="Converts any unit of length to any other unit of length",
                        Type=ApplicationCommandOptionType.SubCommand,
                        Options=new List<SlashCommandOptionBuilder>{
                            // Value
                            new SlashCommandOptionBuilder{
                                Name="value",
                                Description="Value you would like to convert(just number, no symbols)",
                                Type=ApplicationCommandOptionType.Number,
                                IsRequired=true,
                            },
                            
                            // Unit
                            new SlashCommandOptionBuilder{
                                Name="unit",
                                Description="Unit you would like to convert FROM(conversion will be done to all other possible values)",
                                Type=ApplicationCommandOptionType.Integer,
                                IsRequired=true,
                                Choices=new List<ApplicationCommandOptionChoiceProperties> {
                                    new ApplicationCommandOptionChoiceProperties{
                                        Name="Millimetre",
                                        Value=13,
                                    },
                                    new ApplicationCommandOptionChoiceProperties{
                                        Name="Centimetre",
                                        Value=14,
                                    },
                                    new ApplicationCommandOptionChoiceProperties{
                                        Name="Metre",
                                        Value=15,
                                    },
                                    new ApplicationCommandOptionChoiceProperties{
                                        Name="Kilometre",
                                        Value=16,
                                    },

                                    new ApplicationCommandOptionChoiceProperties{
                                        Name="Inch",
                                        Value=17,
                                    },
                                    new ApplicationCommandOptionChoiceProperties{
                                        Name="Feet",
                                        Value=18,
                                    },
                                    new ApplicationCommandOptionChoiceProperties{
                                        Name="Yard",
                                        Value=19,
                                    },
                                    new ApplicationCommandOptionChoiceProperties{
                                        Name="Mile",
                                        Value=20,
                                    },
                                }
                            }
                        },
                    }
                },
            },
            
            // Define
            new SlashCommandBuilder() {
                Name="define",
                Description="Check meaning of words(english only)",
                Options=new List<SlashCommandOptionBuilder> {
                    new SlashCommandOptionBuilder {
                        Name="word",
                        Description="Word you would like meaning of",
                        Type=ApplicationCommandOptionType.String,
                        IsRequired=true,
                    }
                }
            },
            
            // Generate
            new SlashCommandBuilder() {
                Name="generate",
                Description="Generates specified thing",
                Options=new List<SlashCommandOptionBuilder> {
                    // Password
                    new SlashCommandOptionBuilder {
                        Name="password",
                        Description="Generate password with given length",
                        Type=ApplicationCommandOptionType.SubCommand,
                        Options= new List<SlashCommandOptionBuilder> {
                            // Length
                            new SlashCommandOptionBuilder {
                                Name="length",
                                Description="Length of the generated password",
                                Type=ApplicationCommandOptionType.Integer,
                            }
                        }
                    },
                
                    // Quote
                    new SlashCommandOptionBuilder {
                        Name="quote",
                        Description="Pick a quote from somebody",
                        Type=ApplicationCommandOptionType.SubCommand
                    }
                }
            },
        
            // Calculate
            new SlashCommandBuilder() {
                Name="calculate",
                Description="Used for math, physics, statistics and chemistry calculations",
                Options=new List<SlashCommandOptionBuilder> {
                    // Evaluate
                    new SlashCommandOptionBuilder {
                        Name="evaluate",
                        Description="evaluates given expression",
                        Type=ApplicationCommandOptionType.SubCommand,
                        Options= new List<SlashCommandOptionBuilder> {
                            // Expression
                            new SlashCommandOptionBuilder {
                                Name="expression",
                                Description="Expression to evaluate(please put operators before parentheses, example:'2*(4)')",
                                Type=ApplicationCommandOptionType.String,
                            },

                            // WantHelp
                            new SlashCommandOptionBuilder {
                                Name="wanthelp",
                                Description="get the documentation how to use eval function",
                                Type=ApplicationCommandOptionType.Boolean,
                                
                            }
                        }
                    }
                }
            },
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
