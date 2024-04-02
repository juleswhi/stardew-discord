using StardewModdingAPI;
using StardewValley;

namespace PierreHateGroupStardewInterface;

internal class GoldHunter
{
    private IModHelper? _helper;

    private int lastGold = 0;

    public GoldHunter(IModHelper helper, IMonitor monitor)
    {
        SetHelper(helper);
        _helper!.Events.GameLoop.UpdateTicked += (s, e) =>
        {
            if (!Context.IsWorldReady)
                return;

            int currentGold = Game1.player.Money;

            if(lastGold != currentGold) {
                // Db Call
                _ = MessageHandler.SendDbUpdateAsync(currentGold.ToString(), StardewDiscordPacketer.PacketType.DB_UPDATE, StardewDiscordPacketer.DBUpdateType.GOLD);
            }

            lastGold = currentGold;
        };
    }

    public void SetHelper(IModHelper helper) => _helper = helper;
       private void MonitorInventory()
    {

    }

    private void MonitorUpgrades()
    {

    }

    private void MonitorFarmExpansions()
    {

    }

    private void MonitorGiftGiving()
    {

    }

}
