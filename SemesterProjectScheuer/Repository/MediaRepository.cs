using Npgsql;
using SemesterProjectScheuer.IRepositories;
using SemesterProjectScheuer.Models;

namespace SemesterProjectScheuer.Repository;

public class MediaRepository: IMediaRepository
{
    public bool CreateMedia(RegisterMedia newMedia)
    {
        const string sql = @"
            INSERT INTO media (created_at, title, description, media_type, release_year, genres, age_restriction)
            VALUES (@c, @t, @d, @x, @y, @g, @a)";

        try
        {
            using var conn = DatabaseManager.GetConnection();

            using var cmd = new NpgsqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("c", DateTime.Now);               
            cmd.Parameters.AddWithValue("t", newMedia.Title);
            cmd.Parameters.AddWithValue("d", newMedia.Description);  
            cmd.Parameters.AddWithValue("x", newMedia.MediaType);
            cmd.Parameters.AddWithValue("y", newMedia.ReleaseYear);
            cmd.Parameters.AddWithValue("g", newMedia.Genres);
            cmd.Parameters.AddWithValue("a", newMedia.AgeRestriction);
            
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