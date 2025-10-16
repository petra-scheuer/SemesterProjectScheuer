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
            cmd.Parameters.AddWithValue("u", newUser.Username);
            cmd.Parameters.AddWithValue("p", MyHash(newUser.Password));  

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
}