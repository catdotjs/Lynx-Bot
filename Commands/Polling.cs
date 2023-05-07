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
using ScottPlot.Palettes;

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
            bool CanChangeVotes = (bool)Options["changevote"];

            // I love guard clauses fr fr
            if(OptionNames.Length<OptionDescription.Length) {
                await Context.RespondAsync("You can't have a description to a unnamed option", ephemeral: true);
                return;
            }

            // Embeds
            EmbedBuilder embed = new EmbedBuilder();
            embed.Title=$"{Options["question"]}";
            embed.Color=ImageProcessing.RandomColour();

            // making sure its all positive integers 
            double timeoffset = Math.Abs(Math.Ceiling((double)Options["time"]));
            DateTime Time = DateTime.UtcNow.AddMinutes(timeoffset);
            long timestamp = ((DateTimeOffset)Time).ToUnixTimeSeconds();
            embed.Description=$"Time left - <t:{timestamp}:R>";
            embed.Footer=new EmbedFooterBuilder() {
                Text=$"You're only allowed 1 answer. {(CanChangeVotes ? "You are allowed to change your answer" : "You cannot change your answer")}."
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
                embed.AddField($"• {name} - 0 votes", $" — \"{descript}\"");
            }

            Components.AddRow(Buttons);

            ///
            /// DATABASE PART
            ///
            JObject OptionJson = new JObject();
            JArray OptionsArray = new JArray();
            for(int i = 0;i<OptionNames.Length;i++) {
                JObject InnerArr = new JObject() {
                    { "name", OptionNames[i]},
                    { "voters", new JArray() }, // VotersIds will be here
                };
                OptionsArray.Add(InnerArr);
            }
            OptionJson.Add("options", OptionsArray);
            OptionJson.Add("question", (string)Options["question"]);
            OptionJson.Add("canchangevote", CanChangeVotes);

            try {
                IUserMessage message = await (Channel as IMessageChannel).SendMessageAsync(embed: embed.Build(), components: Components.Build());
                string messageid = $"{message.Channel.Id}/{message.Id}";
                using(var command = Program.Database.CreateCommand("INSERT INTO polling (message_id,vote_data,time_left) values (@message_id,@vote_data,@time_left)")) {
                    command.Parameters.AddWithValue("message_id", NpgsqlDbType.Text, messageid);
                    command.Parameters.AddWithValue("vote_data", NpgsqlDbType.Json, OptionJson.ToString());
                    command.Parameters.AddWithValue("time_left", NpgsqlDbType.Bigint, timestamp);
                    
                    await command.ExecuteNonQueryAsync();
                }
                await Context.RespondAsync("Sent the poll!", ephemeral: true);
            } catch(Exception ex) { 
                await LoggingAndErrors.LogException(ex);
                await Context.RespondAsync($"Error!(could be due to missing permission to specified channel): {ex.Message}",ephemeral:true);
            }
        }

        public static async Task RecieveVote(SocketMessageComponent Context) {
            string ReturnMessage = "Your vote has been registered!";
            string UserId = ""+Context.User.Id;
            string MessageId = $"{Context.Channel.Id}/{Context.Message.Id}";
            SocketUserMessage Message = Context.Message;
            try {
                JObject Poll = new JObject();
                long Time = 0;
                using(var command = Program.Database.CreateCommand("SELECT vote_data,time_left FROM polling WHERE message_id=@id")) {
                    command.Parameters.AddWithValue("id", NpgsqlDbType.Text, MessageId);
                    using(var reader = await command.ExecuteReaderAsync()) {
                        // This will return a JSON, Time, and People who voted
                        while(await reader.ReadAsync()) {
                            Poll=JObject.Parse(""+reader["vote_data"]);
                            Time=(long)reader["time_left"];
                        }
                    }
                }
                bool VotedBefore = HasVoted((JArray)Poll["options"],UserId)!=-1;
                // Guard clause time
                // C# makes me mad sometimes, you can't convert but by concat you can?!?!
                if(Time<DateTimeOffset.Now.ToUnixTimeSeconds()) {
                    ReturnMessage="Time to vote on this poll is over.";
                } else if(!(bool)Poll["canchangevote"] && VotedBefore) {
                    ReturnMessage="You've already voted on this poll! This vote does not allow vote changing.";
                } else {
                    // Update the embed
                    int index = Convert.ToInt32(Regex.Replace(Context.Data.CustomId, @"[\D]", "")); // Get field
                    EmbedBuilder embed = Message.Embeds.First().ToEmbedBuilder(); // Get embed
                    Poll["options"]=CalculatePollState((JArray)Poll["options"], UserId, index);

                    foreach(var option in Poll["options"].Select((value,i)=> new {value,i})) {
                        int VoteCount = ((JArray)option.value["voters"]).Count;
                        embed.Fields[option.i].Name=$"• {option.value["name"]} - {VoteCount} vote{(VoteCount!=1 ? "s" : "")}";
                    }

                    // Database update time yippie :|
                    using(var command = Program.Database.CreateCommand("UPDATE polling SET vote_data=@options WHERE message_id=@id")) {
                        command.Parameters.AddWithValue("options", NpgsqlDbType.Json, Poll.ToString());
                        command.Parameters.AddWithValue("id", MessageId);
                        await command.ExecuteNonQueryAsync();
                    }

                    // Send response
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
                string[] BreakdownId = messageid.Split("/");
                ulong ChannelId = Convert.ToUInt64(BreakdownId[0]);
                ulong? MessageId = Convert.ToUInt64(BreakdownId[1]);
                IMessageChannel Channel = Program.Client.GetChannel(ChannelId) as IMessageChannel;

                if(await Channel.GetMessageAsync((ulong)MessageId)!=null) { 
                    List<string> Labels = new List<string>(); 
                    List<double> Data = new List<double>();
                    // options:[name,value]
                    foreach(JObject option in (JArray)data["options"]) {
                        int count = ((JArray)option["voters"]).Count;
                        if(count>0) {
                            Labels.Add((string)option["name"]); // Label
                            Data.Add(count);
                        }
                    }
                    // Image processing
                    DoughnutPieData PieData = new DoughnutPieData(Data, Labels, messageid, "vote");
                    string ImageLocation = ImageProcessing.DoughnutPieChart(PieData);
                    
                    EmbedBuilder embed = new EmbedBuilder() {
                        Color=ImageProcessing.RandomColour(),
                        ImageUrl=$"attachment://{PieData.FileInfo.Name}",
                        Title=$"{data["question"]}'s results",
                        Description="We have switched to using [ScottPlot](https://scottplot.net/) for our charts. Thank you for this amazing library 💜",
                    };
                    MessageReference reference = new MessageReference(MessageId);
                    await Channel.SendFileAsync(ImageLocation, embed: embed.Build(), messageReference:reference);
                    
                    // Remove it from file system
                    FileManagement.Remove(ImageLocation);
                }
            } catch(Exception ex) { await LoggingAndErrors.LogException(ex); };
        }

        public static async Task RoutinePollCheck() {
            // Get ones that should've sent their results
            using(var commands = Program.Database.CreateCommand("SELECT message_id,vote_data FROM polling WHERE time_left<@time")) {
                commands.Parameters.AddWithValue("time", DateTimeOffset.Now.ToUnixTimeSeconds());
                using(var reader = await commands.ExecuteReaderAsync()) {
                    while(await reader.ReadAsync()) {
                        await SendResult((string)reader["message_id"], JObject.Parse((string)reader["vote_data"]));
                    }
                }
            }

            // Drop them from the table
            using(var commands = Program.Database.CreateCommand("DELETE FROM polling WHERE time_left<@time")) {
                commands.Parameters.AddWithValue("time", DateTimeOffset.Now.ToUnixTimeSeconds());
                await commands.ExecuteNonQueryAsync();
            }
        }
    
        private static JArray CalculatePollState(JArray CurrentState, string UserID, int index) {
            // First I check if the user has already voted
            int VoteIndex = HasVoted(CurrentState,UserID);
            // if index and has voted is same just don't bother
            if(VoteIndex==index) {
                return CurrentState;
            }

            // Remove if voted already
            if(VoteIndex!=-1) {
                // Worst line of code in this entire codebase
                JArray OldInd = (JArray)CurrentState[VoteIndex]["voters"];
                CurrentState[VoteIndex]["voters"]=OldInd.RemoveMember(UserID);
            }

            // This will happen regardless
            JObject Ind = CurrentState[index] as JObject;
            ((JArray)Ind["voters"]).Add(UserID);
            
            return CurrentState;
        }
    
        private static int HasVoted(JArray CurrentState, string UserID) {
            foreach(var option in CurrentState.Select((value, i) => new { value, i })) {
                if(option.value["voters"].Any((voter) => (string)voter==UserID)) { // this would be voters list
                    return option.i;
                }
            }
            return -1;
        }

    }
}