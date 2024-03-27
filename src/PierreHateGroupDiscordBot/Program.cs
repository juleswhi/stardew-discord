using Discord;
using Discord.Net;
using Discord.WebSocket;
using Newtonsoft.Json;
using PierreHateGroupDiscordBot.WebSocketHandler;

public class Program
{
    public static DiscordSocketClient? _client;
    public static SocketGuild? _guild;
    public static SocketTextChannel? _notifyChannel;
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

        server.MessageReceived += (s, message) =>
        {
            Console.WriteLine($"Received message: {message}");
            _notifyChannel?.SendMessageAsync($"Stardew Update: -- {message?.Data} --");
        };

        Console.WriteLine($"Starting server");
        await server.StartAsync();
        await Task.Delay(-1);
    }

    private static async Task _client_SlashCommandExecuted(SocketSlashCommand command)
    {
        switch(command.Data.Name)
        {
            case "hello":
                await HandleHello(command);
                break;
            case "set-notify-channel":
                await HandleNotifyChannel(command);
                break;

        }
    }

    private static async Task HandleNotifyChannel(SocketSlashCommand command)
    {
        _notifyChannel = _client?.Guilds
            .FirstOrDefault(x => x.Name.ToLower()
            .Contains("stardew"))?
            .GetTextChannel(command.Channel.Id);
        await command.RespondAsync("Updated channel", ephemeral: true);
    }

    private static async Task HandleHello(SocketSlashCommand command)
    {
        await command.RespondAsync("Pierre really sucks");
    }

    private static async Task _client_Ready()
    {
        _client?.Guilds.ToList().ForEach(x => Console.WriteLine($"{x.Name}"));
        _guild = _client?.Guilds.FirstOrDefault(x => x.Name.Contains("stardew"));

        if(_guild is null)
        {
            Console.WriteLine($"No Guild Found");
            return;
        }

        _notifyChannel = _guild?.DefaultChannel;

        List<SlashCommandBuilder> commands = new() {
            new SlashCommandBuilder()
                .WithName("hello")
                .WithDescription("Say hello to the pierre hate group association;"),

            new SlashCommandBuilder()
                .WithName("set-notify-channel")
                .WithDescription("Set the current channel to be the notify channel"),
        };


        foreach (var cmd in commands)
        {
            try
            {
                await _client?.Guilds
                    .FirstOrDefault(x => x.Name.ToLower()
                    .Contains("stardew"))?
                .CreateApplicationCommandAsync(cmd.Build())!;
            }
            catch (HttpException ex)
            {
                var json = JsonConvert.SerializeObject(ex.Errors, Formatting.Indented);
                Console.WriteLine(json);
            }

        }
        Console.WriteLine($"Created Commands");
    }

    public static Task Log(LogMessage msg)
    {
        Console.WriteLine(msg.ToString());
        return Task.CompletedTask;
    }
}