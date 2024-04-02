using System.ComponentModel;
using System.Text;
using StardewDiscordPacketer;
using StardewValley;
using StardewValley.Network;

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
        var json = new Packet(message, type, DBUpdateType.NONE, 0).ToJson();
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
        var packet = new Packet(data, packetType, dBUpdateType, Game1.uniqueIDForThisGame);
        await SendMessageAsync(packet);
    }

    public static async Task SendStartupAsync(string serverUri = "http://localhost:8080")
    {
        Packet packet = new Packet("Starting Stardew..", PacketType.STARTUP, DBUpdateType.NONE);
        var json = packet.ToJson();
        var content = new StringContent(json, Encoding.UTF8);
        _ = s_client.PostAsync(serverUri, content);
        await Task.CompletedTask;
    }

    public static async Task<bool> CheckHealth() {
        try {
            HttpResponseMessage response = await s_client.GetAsync("http://localhost:8080/health");
            response.EnsureSuccessStatusCode();
            return true;
        }
        catch(Exception) {
            return false;
        }
    }
}
