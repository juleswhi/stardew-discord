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
        await MessageHandler.SendStartupAsync();
        Monitor.Log($"Sent startup", LogLevel.Info);

        new GoldHunter(helper, Monitor);

    }

    private async void OnLoadStageChanged(object sender, LoadStageChangedEventArgs e)
    {
        if(e.NewStage == StardewModdingAPI.Enums.LoadStage.Ready && 
            Context.IsWorldReady)
        {
            await MessageHandler.SendMessageAsync($"Stardew World Loaded!", StardewDiscordPacketer.PacketType.NOTIFY);
        }
    }
}
