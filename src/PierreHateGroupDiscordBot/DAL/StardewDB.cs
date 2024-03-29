using System.Configuration;
using Microsoft.Extensions.Configuration;

namespace PierreHateGroupDiscordBot.DAL;

internal static class StardewDB
{
    private const string s_connectionString = "StardewDB";
    static StardewDB()
    {
    }

    public static List<DbUser> GetUsers()
    {
        return default;
    }

    public static void SaveUser(DbUser user)
    {

    }
}
