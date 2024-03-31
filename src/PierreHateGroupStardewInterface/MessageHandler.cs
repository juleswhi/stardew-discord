using System.Text;
using StardewDiscordPacketer;

namespace PierreHateGroupStardewInterface;

internal static class MessageHandler
{
    private static HttpClient s_client;
    static MessageHandler()
    {
        s_client = new();
    }

    public static async Task SendMessageAsync(string message, PacketType type, string serverUri = "http://localhost:8080")
    {
        var json = new Packet(message, type).ToJson();
        var content = new StringContent(json, Encoding.UTF8);
        _ = s_client.PostAsync(serverUri, content);
        await Task.Delay(1);
    }

    public static async Task SendMessageAsync(Packet packet, string serverUri = "http://localhost:8080") {
        var json = packet.ToJson();
        var content = new StringContent(json, Encoding.UTF8);
        _ = s_client.PostAsync(serverUri, content);
        await Task.Delay(1);
    }

    public static async Task SendDbUpdateAsync(string data, PacketType packetType, DBUpdateType dBUpdateType) {
        var packet = new Packet(data, packetType, dBUpdateType);
        await SendMessageAsync(packet);
    }

    public static async Task SendStartupAsync()
    {
        await SendMessageAsync("Stardew Started!", PacketType.STARTUP);
    }
}
