using Newtonsoft.Json;

namespace StardewDiscordPacketer;


public enum PacketType
{
    NOTIFY,
    STARTUP,
}

public class Packet
{
    public PacketType Type { get; set; }
    public string Data { get; set; } = string.Empty;

    public Packet(string data, PacketType type)
    {
        Data = data;
        Type = type;
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

