using Newtonsoft.Json;

namespace StardewDiscordPacketer;

public enum PacketType
{
    NOTIFY,
    STARTUP,
    DB_UPDATE
}

public enum DBUpdateType {
    GOLD
}

public class Packet
{
    public PacketType Type { get; set; }
    public string Data { get; set; } = string.Empty;
    public DBUpdateType? dBUpdateType { get; set; } = null;

    public Packet(string data, PacketType type)
    {
        Data = data;
        Type = type;
    }

    public Packet(string data, PacketType type, DBUpdateType updateType)
    {
        Data = data;
        Type = type;
        dBUpdateType = updateType;
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

