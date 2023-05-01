using Discord;
using Discord.WebSocket;
using Newtonsoft.Json.Linq;
using Lynx_Bot;
using Npgsql;
using Lynx_Bot.Commands;
using Lynx_Bot.APIs;

// Not a fan of these warns
#pragma warning disable CS8600, CS8602, CS8604, CS0168, CS8618
class Program {
    // Damn intents
    public static DiscordSocketClient Client = new DiscordSocketClient();
    public static JObject Config = new JObject();
    public static Random rand = new Random();
    public static NpgsqlDataSource Database;
    private static bool IsGlobal = false;
    public static Task Main(string[] args) {
        string[] arg = args.Select(a => a.ToLower()).ToArray();
        if(arg.Contains("help")) {
            return LoggingAndErrors.CommandHelp();
        }

        IsGlobal = args.Contains("global");
        return new Program().MainAsync(arg);
    }
    public async Task MainAsync(string[] args){
        // Probably not the best idea to store BotToken with other tokens but eh
        using(StreamReader config = File.OpenText("config.json")) {
            Config = JObject.Parse(config.ReadToEnd());
        }

        // Database
        JObject dbConfig = (JObject)Config["SQL"];
        Database = NpgsqlDataSource.Create($"Host={dbConfig["Host"]};Username={dbConfig["Username"]};Password={dbConfig["Password"]};Database={dbConfig["Database"]}");
        try { 
            using(StreamReader SqlFile = File.OpenText("SQLfile.sql")) {
                using(var command = Database.CreateCommand(SqlFile.ReadToEnd())) {
                   await command.ExecuteNonQueryAsync();
                }
            }
            await LoggingAndErrors.Log(new LogMessage(LogSeverity.Verbose,"database","database has been updated"));
        }catch(Exception ex) { 
            await LoggingAndErrors.LogException(ex);
        }

        // APIs
        RapidAPI.OnStart();
        APINinja.OnStart();

        ///
        /// Discord Stuff
        /// 
        await Client.LoginAsync(TokenType.Bot,(string)Config["Discord"]["BotToken"]);
        await Client.StartAsync();

        Client.Log+=LoggingAndErrors.Log;

        // Routines are non-stop checks with time intervals
        CommandManager.AddRoutine(Polling.RoutinePollCheck,15);

        // Deals with if user wants to add slash commands
        if(args.Any(a=> a=="--addcommands"||a=="--removecommands")) { 
            Client.Ready += () => CommandLoader.CommandOverwrite(Client, IsGlobal, (ulong)Config["Discord"]["TestServer"], args.Any(a => a=="--removecommands"));
        }

        // Where the fun stuff happens
        Client.SlashCommandExecuted+=CommandManager.SlashCommands;
        Client.ButtonExecuted+=CommandManager.Buttons;

        await Task.Delay(-1);
    }
}