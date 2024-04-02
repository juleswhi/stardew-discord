using Discord;
using Discord.Net;
using Discord.WebSocket;
using Newtonsoft.Json;
using PierreHateGroupDiscordBot.DAL;
using PierreHateGroupDiscordBot.WebSocketHandler;
using StardewDiscordPacketer;

public class Program
{
    private static DiscordSocketClient? _client;
    private static List<SocketGuild>? _guilds;
    public static async Task Main()
    {
        DotNetEnv.Env.Load();
        string token = DotNetEnv.Env.GetString("CONN_STRING");

        _client = new();

        await _client.LoginAsync(TokenType.Bot, token);

        _client!.Ready += _client_Ready;
        _client!.SlashCommandExecuted += _client_SlashCommandExecuted;

        _client.Log += Log;

        await _client.StartAsync();

        var server = new StardewSocketHandler(new string[] { "http://localhost:8080/" });


        server.MessageReceived += async (s, message) =>
        {
            Console.WriteLine($"Received message: [{message?.Data}] of type: {message?.Type}, {message?.dBUpdateType}");
            switch (message?.Type)
            {
                case StardewDiscordPacketer.PacketType.NOTIFY:
                    // _notifyChannel?.SendMessageAsync($"Update from Stardew: {message?.Data}");
                    Console.WriteLine($"--- STARDEW: Notify: {message?.Data} ---");
                    break;
                case PacketType.STARTUP:
                    break;
                case PacketType.DB_UPDATE:
                    Console.WriteLine($"------- DB UPDATE: {message?.dBUpdateType} ------");
                    switch (message?.dBUpdateType)
                    {
                        case DBUpdateType.DAY:
                            Console.WriteLine($"Updating the day");
                            await StardewDB.UpdateWorld(int.Parse(message?.Data!), (ulong)message?.WorldID!, (DBUpdateType)message?.dBUpdateType!);
                            break;
                        case DBUpdateType.GOLD:
                            Console.WriteLine($"Updating the gold");
                            await StardewDB.UpdateWorld(int.Parse(message?.Data!), (ulong)message?.WorldID!, (DBUpdateType)message?.dBUpdateType!);
                            break;
                        default:
                            break;
                    }
                    break;
                case PacketType.WORLD_INITIALISE:
                    DbWorld world = JsonConvert.DeserializeObject<DbWorld>(message?.Data!)!;
                    if(!await StardewDB.CheckWorldExists(world.WorldID)) { 
                        Console.WriteLine($"Stardew world does not exist");
                        await StardewDB.CreateWorld(world);
                    }
                    else {
                        Console.WriteLine($"Stardew world already exists");
                    }

                    // check if database already contains id
                    // If not then add it
                    break;

                default:
                    break;
            }
            // _notifyChannel?.SendMessageAsync($"Stardew Update: -- {message?.Data} --");
        };

        Console.WriteLine($"Starting server");
        await server.StartAsync();
        await Task.Delay(-1);
    }

    private static async Task _client_SlashCommandExecuted(SocketSlashCommand command)
    {
        switch (command.Data.Name)
        {
            case "hello":
                await HandleHello(command);
                break;
            case "set-notify-channel":
                await HandleNotifyChannel(command);
                break;
            case "money":
                await HandleMoneyRequest(command);
                break;
            case "day":
                await HandleDay(command);
                break;
            case "watchworld":
                await HandleWatchWorld(command);
                break;

            case "stats":
                break;
        }
    }

    private static async Task HandleStats(SocketSlashCommand command) {
        await Task.CompletedTask;
    }

    private static async Task HandleWatchWorld(SocketSlashCommand command) {
        if(command.GuildId is null) {
            return;
        }

        DbDiscordServer? server = await StardewDB.GetServer((ulong)command.GuildId);

        if(server is null) {
            await ReadGuilds();
            await command.RespondAsync($"Your guild is not registered with the bot. Please wait while registering", ephemeral: true);
            return;
        }

        var worldname = (string)command.Data.Options.First().Value;

        Console.WriteLine($"Found world: {worldname}");

        var world = await StardewDB.GetWorld(worldname);

        if(world is null) {
            await command.RespondAsync($"Specified world not found :( [Please ensure proper capitalsation]", ephemeral: true);
            return;
        }

        await StardewDB.UpdateDiscordServerWorldID(server.ServerID, world.WorldID);
        await command.RespondAsync($"World reference updated", ephemeral: true);
    }

    private static async Task HandleDay(SocketSlashCommand command) {
        if(command.GuildId is null) {
            return;
        }
        DbDiscordServer? server = await StardewDB.GetServer((ulong)command.GuildId);
        if(server is null || server.WorldID is 0) {
            return;
        }

        DbWorld? world = await StardewDB.GetWorld(server.WorldID);

        if(world is null) {
            return;
        }

        await command.RespondAsync($"{world.Day}", ephemeral: true);
    }

    private static async Task HandleNotifyChannel(SocketSlashCommand command)
    {
        var id = command.GuildId;
        if(id is null) {
            return;
        }

        DbDiscordServer? server = await StardewDB.GetServer((ulong)id);

        if(server is null) {
            return;
        }

        server.NotifyChannel = (ulong)command.ChannelId!;

        await StardewDB.UpdateDiscordServerNotifyChannel(server.ServerID, server.NotifyChannel);
        await command.RespondAsync($"Updated channel to: {command.Channel.Name}", ephemeral: true);
    }

    private static async Task HandleMoneyRequest(SocketSlashCommand command) {

        if(command.GuildId is null) {
            await command.RespondAsync($"Could not find the guild :(", ephemeral: true);
            return;
        }

        DbDiscordServer? server = await StardewDB.GetServer((ulong)command.GuildId);

        if(server is null || server.WorldID is 0) {
            await command.RespondAsync($"No WorldID specified :(", ephemeral: true);
            return;
        }

        var guildUser = command.User;

        DbWorld? world = await StardewDB.GetWorld(server.WorldID);

        if(world is null) {
            await command.RespondAsync($"No world is connected. Please use \"/watchworld {{farm name}}\" to connect to a world");
            return;
        }

        var embed = new EmbedBuilder()
            .WithAuthor(guildUser.ToString(), guildUser.GetAvatarUrl() ?? guildUser.GetDefaultAvatarUrl())
            .WithTitle($"{world?.Name}'s Gold")
            .WithDescription($"GOLD: {world?.Gold}")
            .WithColor(Color.Green)
            .WithFooter($"Day: {world?.Day}");
    
        await command.RespondAsync(embed: embed.Build());
    }

    private static async Task HandleHello(SocketSlashCommand command)
    {
        await command.RespondAsync("Pierre really sucks");
    }

    private static async Task _client_Ready()
    {
        await ReadGuilds();

        List<SlashCommandBuilder> commands = new() {
            new SlashCommandBuilder()
                .WithName("hello")
                .WithDescription("Say hello to the pierre hate group association;"),

            new SlashCommandBuilder()
                .WithName("set-notify-channel")
                .WithDescription("Set the current channel to be the notify channel"),

            new SlashCommandBuilder()
                .WithName("money")
                .WithDescription("Gets the current money of the watched world"),

            new SlashCommandBuilder()
                .WithName("day")
                .WithDescription("Gets the current day of the watched world"),

            new SlashCommandBuilder()
                .WithName("watchworld")
                .WithDescription("Watched a specified world. Applies to entire server")
                .AddOption("world", ApplicationCommandOptionType.String, "The name of the stardew world", isRequired: true)
        };


        foreach (var cmd in commands)
        {
            try
            {
                _guilds?.ForEach(async x => {
                    await x.CreateApplicationCommandAsync(cmd.Build())!;
                });
            }
            catch (HttpException ex)
            {
                var json = JsonConvert.SerializeObject(ex.Errors, Formatting.Indented);
                Console.WriteLine(json);
            }

        }
        Console.WriteLine($"Created Commands");
    }

    private static async Task ReadGuilds() {
        _guilds = new();

        _client?.Guilds.ToList().ForEach(async x => {
            var exists = await StardewDB.CheckServerExists(x.Id);
            if(!exists) {
                await StardewDB.CreateDiscordServer(new DbDiscordServer {
                    ServerID = x.Id,
                    ServerName = x.Name,
                    MemberCount = x.MemberCount,
                    NotifyChannel = x.DefaultChannel.Id
                });
            }
            _guilds?.Add(x);
        });

        await Task.CompletedTask;
    }

    public static Task Log(LogMessage msg)
    {
        Console.WriteLine(msg.ToString());
        return Task.CompletedTask;
    }
}
