namespace PierreHateGroupDiscordBot.DAL;

public class DbWorld : DbObject
{
    public string? Name { get; set; }
    public int Gold { get; set; }
    public ulong WorldID { get; set; }
    public int Day { get; set; }
}
