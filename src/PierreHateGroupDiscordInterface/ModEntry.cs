using System.Net.WebSockets;
using System.Text;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;

namespace PierreHateGroupStardewInterface;

internal class ModEntry : Mod
{
    public override async void Entry(IModHelper helper)
    {
        Monitor.Log($"Starting Stardew!, Sending startup", LogLevel.Info);
        await MessageHandler.SendStartup();
        Monitor.Log($"Sent startup", LogLevel.Info);
    }
}
