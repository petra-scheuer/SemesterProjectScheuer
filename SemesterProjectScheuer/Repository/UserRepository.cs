using System.Security.Cryptography;
using System.Text;
using Npgsql;
using SemesterProjectScheuer.IRepositories;
using SemesterProjectScheuer.Models;

namespace SemesterProjectScheuer.Repository;

public class UserRepository: IUserRepository
{
    public bool CreateUser(RegisterUser newUser)
    {
        const string sql = @"
            INSERT INTO myusers (created_at, username, password, token)
            VALUES (@c, @u, @p, '')";

        try
        {
            using var conn = DatabaseManager.GetConnection();

            using var cmd = new NpgsqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("c", DateTime.Now);               
            cmd.Parameters.AddWithValue("u", newUser.username);
            cmd.Parameters.AddWithValue("p", MyHash(newUser.password));  

            cmd.ExecuteNonQuery();
            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            return false;
        }
    }
    private static string MyHash(string text)
    {
        using var sha = SHA256.Create();
        var bytes = sha.ComputeHash(Encoding.UTF8.GetBytes(text));
        return Convert.ToBase64String(bytes);
    }
    
    public static UserModel? AuthenticateUser(string username, string password)
    {
        using var conn = DatabaseManager.GetConnection();
        using var cmd = new NpgsqlCommand("SELECT username, password FROM myusers WHERE username = @u", conn);
        cmd.Parameters.AddWithValue("@u", username);

        using var reader = cmd.ExecuteReader(); // kein await!
        if (reader.Read())
        {
            string hash = reader.GetString(1);
            if (VerifyPassword(password, hash))
            {
                return new UserModel
                {
                    username = reader.GetString(0)
                };
            }
        }

        return null;
    }
    public static UserModel GetUserByToken(string token)
    {
        using var conn = DatabaseManager.GetConnection();
        using var cmd = new NpgsqlCommand("SELECT username FROM myusers WHERE token = @t", conn);
        cmd.Parameters.AddWithValue("@t", token);
        using var reader = cmd.ExecuteReader();

        if ( reader.Read())
        {
            return new UserModel
            {
                username = reader.GetString(0)
            };
        }
        return null;
    }
    public static bool SaveToken(string username, string token)
    {
        using var conn = DatabaseManager.GetConnection();
        using var cmd = new NpgsqlCommand("UPDATE myusers SET token = @t WHERE username = @un", conn);
        cmd.Parameters.AddWithValue("@t", token);
        cmd.Parameters.AddWithValue("@un", username); 
        cmd.ExecuteNonQuery();
        return true;
    }
    private static bool VerifyPassword(string password, string storedHash)
    {
        // wenn du SHA256 nutzt
        using var sha = SHA256.Create();
        var bytes = sha.ComputeHash(Encoding.UTF8.GetBytes(password));
        string hashed = Convert.ToBase64String(bytes);
        return hashed == storedHash;
    }

    
}