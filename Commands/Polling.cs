using Discord;
using Discord.Rest;
using Discord.WebSocket;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using NpgsqlTypes;
using System.Net.Http.Json;
using System.Reactive;
using Lynx_Bot.Processing;

// Fuck these fr
#pragma warning disable CS8600, CS8602, CS8604, CS0168

namespace Lynx_Bot.Commands {
    static class Polling {
        public static async Task SendPoll(SocketSlashCommand Context) {
            ///
            /// MESSAGE PART
            ///
            // Get those options to more readible way
            Dictionary<string, object> Options = CommandManager.OptionsAsDictionary(Context.Data.Options);
            // Set the channel to send
            SocketTextChannel Channel = Context.Channel as SocketTextChannel;

            try {
                Channel=(SocketTextChannel)Options["channel"];
            } catch { };

            // Get choices
            string[] OptionNames = Options.Where(a => a.Key.Contains("option")&&!a.Key.Contains("description"))
                .Select(x => ((string)x.Value))
                .ToArray(); // We arrayin bb wooo

            string[] OptionDescription = Options.Where(a => a.Key.Contains("description"))
                .Select(x => (string)x.Value) // Just get option description
                .ToArray(); // We arrayin bb wooo


            // I love guard clauses fr fr
            if(OptionNames.Length<OptionDescription.Length) {
                await Context.RespondAsync("You can't have a description to a unnamed option", ephemeral: true);
                return;
            }

            // Embeds
            EmbedBuilder embed = new EmbedBuilder();
            embed.Title=$"\"{Options["question"]}\"";
            embed.Color=ImageProcessing.RandomColour();

            // making sure its all positive integers 
            double timeoffset = Math.Abs(Math.Ceiling((double)Options["time"]));
            DateTime Time = DateTime.UtcNow.AddMinutes(timeoffset);
            long timestamp = ((DateTimeOffset)Time).ToUnixTimeSeconds();
            embed.Description=$"Time left - <t:{timestamp}:R>";
            embed.Footer=new EmbedFooterBuilder() {
                Text="You're only allowed 1 answer, you cannot change your answer."
            };


            // Components
            ComponentBuilder Components = new ComponentBuilder();
            ActionRowBuilder Buttons = new ActionRowBuilder();

            // Reason not to zip those two arrays is, description array can be shorter
            for(int i = 0;i<OptionNames.Length;i++) {
                string descript = "No description was given";
                string name = OptionNames[i];
                try {
                    descript=OptionDescription[i];
                } catch { };

                ButtonBuilder button = new ButtonBuilder(label: name, customId: "PollingButton"+i, style: ButtonStyle.Secondary);
                Buttons.AddComponent(button.Build());
                embed.AddField($"• {name} - 0 Vote(s)", $" — \"{descript}\"");
            }

            Components.AddRow(Buttons);

            ///
            /// DATABASE PART
            ///
            JObject OptionJson = new JObject();
            JArray OptionsArray = new JArray();
            for(int i = 0;i<OptionNames.Length;i++) {
                JArray arr = new JArray() {
                    OptionNames[i],
                    0
                };
                OptionsArray.Add(arr);
            }
            OptionJson.Add("options", OptionsArray);
            OptionJson.Add("question", (string)Options["question"]);

            IUserMessage message = await (Channel as IMessageChannel).SendMessageAsync(embed: embed.Build(), components: Components.Build());
            string messageid = $"{message.Channel.Id}/{message.Id}";
            try {
                using(var command = Program.Database.CreateCommand("INSERT INTO polling (message_id,options_and_votes,time_left,voted) values (@message_id,@options_and_votes,@time_left,@voted)")) {
                    command.Parameters.AddWithValue("message_id", NpgsqlDbType.Text, messageid);
                    command.Parameters.AddWithValue("options_and_votes", NpgsqlDbType.Json, OptionJson.ToString());
                    command.Parameters.AddWithValue("time_left", NpgsqlDbType.Bigint, timestamp);
                    command.Parameters.AddWithValue("voted", new string[] { "0" });
                    await command.ExecuteNonQueryAsync();
                }
            } catch(Exception ex) { await LoggingAndErrors.LogException(ex); }
            await Context.RespondAsync("Sent the poll!", ephemeral: true);
        }

        public static async Task RecieveVote(SocketMessageComponent Context) {
            string ReturnMessage = "Your vote has been registered!";
            string UserId = ""+Context.User.Id;
            string MessageId = $"{Context.Channel.Id}/{Context.Message.Id}";
            SocketUserMessage Message = Context.Message;
            try {
                JObject poll = new JObject();
                long time = 0;
                List<string> voted = new List<string>();
                using(var command = Program.Database.CreateCommand("SELECT options_and_votes,time_left,voted FROM polling WHERE message_id=@id")) {
                    command.Parameters.AddWithValue("id", NpgsqlDbType.Text, MessageId);
                    using(var reader = await command.ExecuteReaderAsync()) {
                        // This will return a JSON, Time, and People who voted
                        while(await reader.ReadAsync()) {
                            poll=JObject.Parse(""+reader["options_and_votes"]);
                            time=(long)reader["time_left"];
                            voted=((string[])reader["voted"]).ToList();
                        }
                    }
                }

                // Guard clause time
                // C# makes me mad sometimes, you can't convert but by concat you can?!?!
                if(time<DateTimeOffset.Now.ToUnixTimeSeconds()) {
                    ReturnMessage="Time to vote on this poll is over.";
                } else if(voted.Any(x => x==UserId)) {
                    ReturnMessage="You've already voted on this poll!";
                } else {
                    // Update the embed
                    int index = Convert.ToInt32(Regex.Replace(Context.Data.CustomId, @"[\D]", "")); // Get field
                    EmbedBuilder embed = Message.Embeds.First().ToEmbedBuilder(); // Get embed
                    JArray OptionsArray = (JArray)poll["options"];

                    // Increment vote(there should be a better way to do this)
                    OptionsArray[index][1]=(int)OptionsArray[index][1]+1;
                    voted.Add(UserId);

                    poll["options"]=OptionsArray;

                    // Database update time yippie :|
                    using(var command = Program.Database.CreateCommand("UPDATE polling SET options_and_votes=@options, voted=@voted WHERE message_id=@id")) {
                        command.Parameters.AddWithValue("options", NpgsqlDbType.Json, poll.ToString());
                        command.Parameters.AddWithValue("id", MessageId);
                        command.Parameters.AddWithValue("voted", voted.ToArray());
                        await command.ExecuteNonQueryAsync();
                    }

                    // Send response
                    embed.Fields[index].Name=$"• {OptionsArray[index][0]} - {OptionsArray[index][1]} Vote(s)";
                    await Message.ModifyAsync(msg => msg.Embed=embed.Build());
                }
            } catch(Exception ex) {
                await LoggingAndErrors.LogException(ex);
                ReturnMessage="There was a problem with registering your vote.";
            }

            await Context.RespondAsync(ReturnMessage, ephemeral: true);
        }

        public static async Task SendResult(string messageid, JObject data) {
            // messageid = "channel.id/message.id"
            try {
                ulong ChannelId = Convert.ToUInt64(messageid.Split("/")[0]);
                IMessageChannel Channel = Program.Client.GetChannel(ChannelId) as IMessageChannel;

                string Labels = ""; //"a|b|c"
                string Data = "t:";
                int Total = 0;
                // options:[name,value]
                foreach(JArray option in (JArray)data["options"]) {
                    Labels+=$"{option[0]}({option[1]})|";
                    Data+=$"{option[1]},";
                    Total+=(int)option[1];
                }
                EmbedBuilder embed = new EmbedBuilder() {
                    Color=ImageProcessing.RandomColour(),
                    ImageUrl=ImageProcessing.DoughnutPieChart(Data, Labels, Total+" Vote(s)"),
                    Title=$"\"{data["question"]}\" Results",
                    Footer=new EmbedFooterBuilder() {
                        Text="This awesome graph is made with image-charts.com! Thanks for being awesome💖"
                    }
                };

                await Channel.SendMessageAsync(embed: embed.Build());
            } catch(Exception ex) { await LoggingAndErrors.LogException(ex); };
        }

        public static async Task RoutinePollCheck() {
            // Get ones that should've sent their results
            using(var commands = Program.Database.CreateCommand("SELECT message_id,options_and_votes FROM polling WHERE time_left<@time")) {
                commands.Parameters.AddWithValue("time", DateTimeOffset.Now.ToUnixTimeSeconds());
                using(var reader = await commands.ExecuteReaderAsync()) {
                    while(await reader.ReadAsync()) {
                        await SendResult((string)reader["message_id"], JObject.Parse((string)reader["options_and_votes"]));
                    }
                }
            }

            // Drop them from the table
            using(var commands = Program.Database.CreateCommand("DELETE FROM polling WHERE time_left<@time")) {
                commands.Parameters.AddWithValue("time", DateTimeOffset.Now.ToUnixTimeSeconds());
                await commands.ExecuteNonQueryAsync();
            }
        }
    }
}