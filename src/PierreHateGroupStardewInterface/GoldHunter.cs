using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Menus;

namespace PierreHateGroupStardewInterface;

internal class GoldHunter
{
    private static IModHelper? _helper;

    private int lastGold = 0;


    public GoldHunter(IModHelper helper)
    {
        SetHelper(helper);
        _helper!.Events.Display.MenuChanged += MonitorShop;
        _helper.Events.GameLoop.UpdateTicked += async (s, e) =>
        {
            if (!Context.IsWorldReady)
                return;

            int currentGold = Game1.player.Money;
            if((lastGold - currentGold) >= 5000)
            {
                await MessageHandler.SendMessageAsync($"{lastGold - currentGold} Gold Has Just Been Spent!", StardewDiscordPacketer.PacketType.NOTIFY);
            }
        };
    }

    public void SetHelper(IModHelper helper) => _helper = helper;
   
    private void MonitorShop(object? sender, MenuChangedEventArgs e)
    {
        if(e.NewMenu is ShopMenu menu)
        {
            menu.onPurchase += (a, b, c) =>
            {
                // Update database here
                return true;
            };
        }
    }

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
