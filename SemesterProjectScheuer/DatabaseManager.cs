using Npgsql;

namespace SemesterProjectScheuer;

public class DatabaseManager
{
    private static string _connectionString =
        "Host=localhost;Port=5435;Database=semesterproject;Username=postgres;Password=postgres";
    
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
        const string createMediasTable = @"
        CREATE TABLE IF NOT EXISTS media (
            id INT GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
            created_at Timestamp without time zone NOT NULL,
            title VARCHAR(50),
            description VARCHAR(255) NOT NULL,
            media_type VARCHAR(50) NOT NULL,
            release_year INT NOT NULL,
            genres VARCHAR(50) NOT NULL,
            age_restriction INT NOT NULL
      
        )
   

    ";
        const string createRatingsTable = @"
        CREATE TABLE IF NOT EXISTS rating (
            id INT GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
            created_at Timestamp without time zone NOT NULL,
            media_id INT NOT NULL,
            user_id INT NOT NULL,
            stars INT NOT NULL,
            comment VARCHAR(255) NOT NULL,
            is_confirmed BOOLEAN NOT NULL,
            likes INT NOT NULL DEFAULT 0
            
       
            
        )    ";
        
        
        NpgsqlConnection conn = GetConnection();
        
        using var userCmd = new NpgsqlCommand(createUsersTable, conn);
        userCmd.ExecuteNonQuery();
        
        using var mediacmd = new NpgsqlCommand(createMediasTable, conn);
        mediacmd.ExecuteNonQuery();
        using var ratingscmd = new NpgsqlCommand(createRatingsTable, conn);
        ratingscmd.ExecuteNonQuery();
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
        DROP TABLE media;
        DROP TABLE rating;
    ";
        
        using var cmd = new NpgsqlCommand(dropUsersTable, conn);
        cmd.ExecuteNonQuery();
        conn.Close();


    }
    
}

