using Newtonsoft.Json;
using PierreHateGroupDiscordBot.DAL;
using StardewDiscordPacketer;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Menus;

namespace PierreHateGroupStardewInterface;

internal class ModEntry : Mod
{
    public static bool s_connectionStatus = false;
    public override async void Entry(IModHelper helper)
    {
        bool connected = await MessageHandler.CheckHealth();

        if(connected) {
            s_connectionStatus = true;
            Monitor.Log($"Connected to discord!", LogLevel.Info);
        }
        else {
            Monitor.Log($"Could not connect to discord. Please ensure the discord bot is up and the correct IP is supplied",
            LogLevel.Warn);
        }

        helper.Events.GameLoop.SaveLoaded += async (s, e) =>
        {
            new GoldHunter(helper, Monitor);

            DbWorld world = new DbWorld()
            {
                WorldID = Game1.uniqueIDForThisGame,
                Gold = Game1.player.Money,
                Day = Game1.Date.TotalDays,
                Name = Game1.GetSaveGameName()
            };

            Packet packet = new Packet(JsonConvert.SerializeObject(world), PacketType.WORLD_INITIALISE, DBUpdateType.WORLD, Game1.uniqueIDForThisGame);
            await MessageHandler.SendMessageAsync(packet);

            helper.Events.GameLoop.DayStarted += async (s, e) =>
            {
                Packet dayUpdatePacket = new Packet(
                    Game1.Date.TotalDays.ToString(),
                    PacketType.DB_UPDATE,
                    DBUpdateType.DAY,
                    Game1.uniqueIDForThisGame
                    );

                await MessageHandler.SendMessageAsync(dayUpdatePacket);
            };
        };


        helper.Events.GameLoop.UpdateTicked += async (s, e) =>
        {
            if (!e.IsMultipleOf(120))
            {
                return;
            }

            var result = await MessageHandler.CheckHealth();

            if (result == s_connectionStatus)
            {
                return;
            }

            if (result == true)
            {
                Game1.addHUDMessage(new HUDMessage("Connected to Discord!", 1));
            }
            else
            {
                Game1.addHUDMessage(new HUDMessage("You have been disconnected from Discord :(", 3));
            }

            s_connectionStatus = result;
        };

        helper.Events.Input.ButtonPressed += (s, e) => {
            if(e.Button == SButton.F2) {
                string message = $"Connection Status: ^{(s_connectionStatus ? "Connected!" : "Not Connected :(")}";
                Game1.activeClickableMenu = new DialogueBox(message);
            }
        };


    }
}
