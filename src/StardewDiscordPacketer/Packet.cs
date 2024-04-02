using Newtonsoft.Json;

namespace StardewDiscordPacketer;

public enum PacketType
{
    NOTIFY,
    STARTUP,
    DB_UPDATE,
    WORLD_INITIALISE
}

public enum DBUpdateType {
    DAY,
    WORLD,
    GOLD,
    NONE
}

public class Packet
{
    public ulong WorldID { get; set; }
    public PacketType Type { get; set; }
    public string Data { get; set; } = string.Empty;
    public DBUpdateType? dBUpdateType { get; set; } = null;

    public Packet(string data, PacketType type, DBUpdateType updateType, ulong worldID = 0)
    {
        Data = data;
        Type = type;
        dBUpdateType = updateType;
        WorldID = worldID;
    }

    public string ToJson()
    {
        return JsonConvert.SerializeObject(this);
    }

    public static Packet? FromJson(string json)
    {
        return JsonConvert.DeserializeObject<Packet>(json);
    }
}

