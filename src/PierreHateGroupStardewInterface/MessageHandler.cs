using System.Text;

namespace PierreHateGroupStardewInterface;

internal static class MessageHandler
{
    private static HttpClient s_client;
    static MessageHandler()
    {
        s_client = new();
    }
    public static async Task SendMessageAsync(string message, StardewDiscordPacketer.PacketType type, string serverUri = "http://localhost:8080")
    {
        var json = new StardewDiscordPacketer.Packet(message, type).ToJson();
        var content = new StringContent(json, Encoding.UTF8);
        _ = s_client.PostAsync(serverUri, content);
        await Task.Delay(1);
    }

    public static async Task SendStartup()
    {
        await SendMessageAsync("Stardew Started!", StardewDiscordPacketer.PacketType.STARTUP);
    }
}
