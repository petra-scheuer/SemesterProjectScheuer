using Npgsql;
using SemesterProjectScheuer.IRepositories;
using SemesterProjectScheuer.Models;

namespace SemesterProjectScheuer.Repository;

public class RatingsRepository: IRatingsRepository
{
    public bool CreateRatings(RegisterRating newRating)
    {
        const string sql = @"
            INSERT INTO rating (created_at, media_id, user_id, stars, comment, is_confirmed )
            VALUES (@c, @t, @d, @x, @y, @g)";

        try
        {
            using var conn = DatabaseManager.GetConnection();

            using var cmd = new NpgsqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("c", DateTime.Now);               
            cmd.Parameters.AddWithValue("t", newRating.MediaId);
            cmd.Parameters.AddWithValue("d", newRating.UserId);  
            cmd.Parameters.AddWithValue("x", newRating.Stars);
            var comment = string.IsNullOrWhiteSpace(newRating.Comment)
                ? "Nicht kommentiert"
                : newRating.Comment;
            cmd.Parameters.AddWithValue("y", comment);
            cmd.Parameters.AddWithValue("g", false);
            cmd.ExecuteNonQuery();
            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            return false;
        }
        
    }
}