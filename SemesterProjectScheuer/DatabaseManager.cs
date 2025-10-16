using Npgsql;

namespace SemesterProjectScheuer;

public class DatabaseManager
{
    private static string _connectionString =
        "Host=localhost;Port=5433;Database=semesterproject;Username=postgres;Password=postgres";
    
    public static bool TestConnection()
    {
        try
        {
            using var conn = new NpgsqlConnection(_connectionString);
            conn.Open();
            using var cmd = new NpgsqlCommand("SELECT 1", conn);
            cmd.ExecuteNonQuery();
            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[DatabaseManager] Connection failed: {ex.Message}");
            return false;
        }
    }

    public static void SetupTables()
    {

        const string createUsersTable = @"
        CREATE TABLE IF NOT EXISTS myusers (
            username VARCHAR(50) PRIMARY KEY,
            password VARCHAR(255) NOT NULL,
            token VARCHAR(255),
            created_at Timestamp without time zone NOT NULL
                                      
        );
    ";
        NpgsqlConnection conn = GetConnection();
        
        using var cmd = new NpgsqlCommand(createUsersTable, conn);
        cmd.ExecuteNonQuery();
        conn.Close();
        
    }

    public static NpgsqlConnection GetConnection()
    {
        var conn = new NpgsqlConnection(_connectionString);
        conn.Open();
        return conn;
    }

    public static void ResetTables()
    {
        NpgsqlConnection conn = GetConnection();
        const string dropUsersTable = @"
        Drop TABLE myusers;
    ";
        
        using var cmd = new NpgsqlCommand(dropUsersTable, conn);
        cmd.ExecuteNonQuery();
        conn.Close();


    }
    
}

