using System.Data;
using System.Data.SQLite;
using StardewDiscordPacketer;

namespace PierreHateGroupDiscordBot.DAL;

internal static class StardewDB
{
    private const string s_connectionString = "StardewDB";
    private static SQLiteConnection _conn;
    static StardewDB()
    {

        _conn = new SQLiteConnection("Data Source=stardew.db");
    }

    public static async Task UpdateWorld<T>(T data, ulong worldId, DBUpdateType type)
    {
        _conn.Open();
        Console.WriteLine($"WorldID: {worldId}, data: {data}");

        using var command = new SQLiteCommand(_conn);

        command.CommandText = @"UPDATE Worlds SET Day = @dayValue WHERE WorldID = @WorldID";
        command.Parameters.AddWithValue("@dayValue", data);
        command.Parameters.AddWithValue("@WorldID", worldId);

        await command.ExecuteNonQueryAsync();
        Console.WriteLine($"Updated world");
        _conn.Close();
        return;
    }


    public static async Task CreateWorld(DbWorld world) {
        _conn.Open();

        string sql = @"
            insert into Worlds (Gold, WorldID, Day, Name)
            Values (@Gold, @WorldID, @Day, @Name)
        ";

        using var command = new SQLiteCommand(sql, _conn);

        command.Parameters.AddWithValue("@Gold", world.Gold);
        command.Parameters.AddWithValue("@WorldID", world.WorldID);
        command.Parameters.AddWithValue("@Day", world.Day);
        command.Parameters.AddWithValue("@Name", world.Name);

        await command.ExecuteNonQueryAsync();
        _conn.Close();
    }

    public static async Task CreateDiscordServer(DbDiscordServer server) {
        _conn.Open();
        
        string sql = @"
            INSERT INTO DiscordServers (ServerID, ServerName, MemberCount, NotifyChannel)
            VALUES (@ServerID, @ServerName, @MemberCount, @NotifyChannel)
        ";

        using var command = new SQLiteCommand(sql, _conn);
        command.Parameters.AddWithValue("@ServerID", server.ServerID); 
        command.Parameters.AddWithValue("@ServerName", server.ServerName); 
        command.Parameters.AddWithValue("@MemberCount", server.MemberCount); 
        command.Parameters.AddWithValue("@NotifyChannel", server.NotifyChannel); 

        await command.ExecuteNonQueryAsync();
        _conn.Close();
    }

    public static async Task UpdateDiscordServerNotifyChannel(ulong server, object data) {
        _conn.Open();
        string sql = @"
            update DiscordServers 
            set NotifyChannel = @data
            where ServerID = @ServerID
        ";

        using var command = new SQLiteCommand(sql, _conn);

        command.Parameters.AddWithValue("@data", data);
        command.Parameters.AddWithValue("@ServerID", server);

        await command.ExecuteNonQueryAsync();

        _conn.Close();
    }


    public static async Task UpdateDiscordServerWorldID(ulong server, object data) {
        _conn.Open();
        string sql = @"
            update DiscordServers 
            set WorldID = @data
            where ServerID = @ServerID
        ";

        using var command = new SQLiteCommand(sql, _conn);

        command.Parameters.AddWithValue("@data", data);
        command.Parameters.AddWithValue("@ServerID", server);

        await command.ExecuteNonQueryAsync();

        _conn.Close();
    }

    public static async Task<DbDiscordServer?> GetServer(ulong id) {
        _conn.Open();

        using var command = new SQLiteCommand(_conn);

        command.CommandText = @"select * from DiscordServers where ServerID = @ServerID";
        command.Parameters.AddWithValue("@ServerID", id);

        using var reader = await command.ExecuteReaderAsync();

        while (reader.Read())
        {
            var name = reader.GetString(reader.GetOrdinal("ServerName"));
            var member = reader.GetInt32(reader.GetOrdinal("MemberCount"));
            var notify = (ulong)reader.GetInt64(reader.GetOrdinal("NotifyChannel"));
            var serverid = (ulong)reader.GetInt64(reader.GetOrdinal("ServerID"));
            var world = reader.IsDBNull(reader.GetOrdinal("WorldID")) ? 0 : (ulong)reader.GetInt64(reader.GetOrdinal("WorldID"));

            var server = new DbDiscordServer {
                ServerName = name,
                MemberCount = member,
                NotifyChannel = notify,
                ServerID = serverid,
                WorldID = world
            };

            _conn.Close();
            return server;
        }

        _conn.Close();
        return null;
    }

    public static async Task<DbWorld?> GetWorld(ulong id)
    {
        _conn.Open();

        using var command = new SQLiteCommand(_conn);

        command.CommandText = @"select * from Worlds where WorldID = @ServerID";
        command.Parameters.AddWithValue("@ServerID", id);

        using var reader = await command.ExecuteReaderAsync();

        while (reader.Read())
        {
            var world = new DbWorld
            {
                Gold = reader.GetInt32(reader.GetOrdinal("Gold")),
                WorldID = (ulong)reader.GetInt64(reader.GetOrdinal("WorldID")),
                Day = reader.GetInt32(reader.GetOrdinal("Day")),
                Name = reader.GetString(reader.GetOrdinal("Name"))
            };
            _conn.Close();
            return world;
        }
        _conn.Close();

        return null;
    }

    public static async Task<DbWorld?> GetWorld(string name)
    {
        _conn.Open();

        using var command = new SQLiteCommand(_conn);

        command.CommandText = @"select * from Worlds where Name = @Name";
        command.Parameters.AddWithValue("@Name", name);

        using var reader = await command.ExecuteReaderAsync();

        while (reader.Read())
        {
            var world = new DbWorld
            {
                Gold = reader.GetInt32(reader.GetOrdinal("Gold")),
                WorldID = (ulong)reader.GetInt64(reader.GetOrdinal("WorldID")),
                Day = reader.GetInt32(reader.GetOrdinal("Day")),
                Name = reader.GetString(reader.GetOrdinal("Name"))
            };
            _conn.Close();
            return world;

        }

        _conn.Close();
        return null;
    }

    public static async Task<bool> CheckServerExists(ulong id) {
        _conn.Open();
        using var command = new SQLiteCommand(_conn);
        command.CommandText = @"SELECT exists(select 1 from DiscordServers where ServerID=@ServerID)";
        command.Parameters.AddWithValue("@ServerID", id);
        object? result = await command.ExecuteScalarAsync();
        _conn.Close();
        return (long?)result == 1;
    }

    public static async Task<bool> CheckWorldExists(ulong id)
    {
        _conn.Open();
        using var command = new SQLiteCommand(_conn);

        command.CommandText = @"select exists(select 1 from Worlds where WorldID=@WorldID)";
        command.Parameters.AddWithValue("@WorldID", id);
        object? result = await command.ExecuteScalarAsync();
        _conn.Close();
        return (long?)result == 1;
    }

    public static async Task<List<DbWorld>> GetWorlds() {

        _conn.Open();
        string sql = @"select * from Worlds";

        using var command = new SQLiteCommand(sql, _conn);

        using var reader = command.ExecuteReader();

        List<DbWorld> worlds = new();
        while(reader.Read()) {
            var world = new DbWorld {
                Gold = reader.GetInt32(reader.GetOrdinal("Gold")),
                WorldID = (ulong)reader.GetInt64(reader.GetOrdinal("WorldID")),
                Day = reader.GetInt32(reader.GetOrdinal("Day")),
                Name = reader.GetString(reader.GetOrdinal("Name"))
            };

            worlds.Add(world);
        }

        _conn.Close();

        await Task.CompletedTask;
        return worlds;
    }

  
}
