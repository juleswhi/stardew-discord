
namespace PierreHateGroupDiscordBot.DAL;

public class DbDiscordServer : DbObject
{
    public ulong ServerID { get; set; }
    public string? ServerName { get; set; }
    public int MemberCount { get; set; }
    public ulong WorldID { get; set; } = 0;
    public ulong NotifyChannel { get; set; }
}
