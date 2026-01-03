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

    public MediaElement GetMediaById(int mediaId)
    {
        const string sql = "SELECT * FROM media WHERE id = @id";

        using var conn = DatabaseManager.GetConnection();

        using var cmd = new NpgsqlCommand(sql, conn);
        cmd.Parameters.AddWithValue("@id", mediaId);

        using var reader = cmd.ExecuteReader();

        if (!reader.Read())
            return null;

        return new MediaElement
        {
            MediaId = reader.GetInt32(0),
            Title = reader.GetString(2),
            Description = reader.GetString(3),
            MediaType = reader.GetString(4),
            ReleaseYear = reader.GetInt32(5),
            Genres = reader.GetString(6),
            AgeRestriction = reader.GetInt32(7)
        };
    }
    public List<MediaElement> GetAllMedias()
    {
        const string sql = "SELECT * FROM media ORDER BY created_at";

        using var conn = DatabaseManager.GetConnection();
        using var cmd = new NpgsqlCommand(sql, conn);
        using var reader = cmd.ExecuteReader();

        var medias = new List<MediaElement>();

        while (reader.Read())
        {
            medias.Add(new MediaElement
            {
                MediaId = reader.GetInt32(0),
                Title = reader.GetString(2),
                Description = reader.GetString(3),
                MediaType = reader.GetString(4),
                ReleaseYear = reader.GetInt32(5),
                Genres = reader.GetString(6),
                AgeRestriction = reader.GetInt32(7)
            });
        }

        return medias;
    }
}