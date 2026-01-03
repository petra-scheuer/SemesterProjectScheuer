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

    public bool DeleteMediaById(int mediaId)
    {
        const string sql = "DELETE FROM media WHERE id = @id";
        using var conn = DatabaseManager.GetConnection();
        using var cmd = new NpgsqlCommand(sql, conn);
        cmd.Parameters.AddWithValue("@id", mediaId);
        return cmd.ExecuteNonQuery() == 1;
        
    }
    public MediaElement ChangeMedia(ChangeMedia updatedMedia)
    {
        const string sql = @"
        UPDATE media
        SET
            title = @t,
            description = @d,
            media_type = @x,
            release_year = @y,
            genres = @g,
            age_restriction = @a
        WHERE id = @id
        RETURNING id, created_at, title, description, media_type, release_year, genres, age_restriction;
    ";

        using var conn = DatabaseManager.GetConnection();
        using var cmd = new NpgsqlCommand(sql, conn);

        cmd.Parameters.AddWithValue("id", updatedMedia.MediaId);
        cmd.Parameters.AddWithValue("t", updatedMedia.Title);
        cmd.Parameters.AddWithValue("d", updatedMedia.Description);
        cmd.Parameters.AddWithValue("x", updatedMedia.MediaType);
        cmd.Parameters.AddWithValue("y", updatedMedia.ReleaseYear);
        cmd.Parameters.AddWithValue("g", updatedMedia.Genres);
        cmd.Parameters.AddWithValue("a", updatedMedia.AgeRestriction);

        using var reader = cmd.ExecuteReader();

        if (!reader.Read())
            throw new Exception($"Media with id {updatedMedia.MediaId} not found.");

        return new MediaElement
        {
            MediaId = reader.GetInt32(reader.GetOrdinal("id")),
            CreatedAt = reader.GetDateTime(reader.GetOrdinal("created_at")),
            Title = reader.GetString(reader.GetOrdinal("title")),
            Description = reader.GetString(reader.GetOrdinal("description")),
            MediaType = reader.GetString(reader.GetOrdinal("media_type")),
            ReleaseYear = reader.GetInt32(reader.GetOrdinal("release_year")),
            Genres = reader.GetString(reader.GetOrdinal("genres")),
            AgeRestriction = reader.GetInt32(reader.GetOrdinal("age_restriction")),
        };
    }

    public List<RatingObject> GetRatingsOfMedia(int mediaId)
    {
        const string sql = @"
        SELECT 
            id as rating_id,
            media_id,
            user_id,
            stars,
            comment,
            is_confirmed as is_comment_confirmed,
            created_at
        FROM rating
        WHERE media_id = @id
        ORDER BY created_at DESC
    ";

        using var conn = DatabaseManager.GetConnection();
        using var cmd = new NpgsqlCommand(sql, conn);
        cmd.Parameters.AddWithValue("@id", mediaId);

        using var reader = cmd.ExecuteReader();

        var ratings = new List<RatingObject>();

        while (reader.Read())
        {
            ratings.Add(new RatingObject
            {
                RatingId = reader.GetInt32(reader.GetOrdinal("rating_id")),
                MediaId = reader.GetInt32(reader.GetOrdinal("media_id")),
                UserId = reader.GetInt32(reader.GetOrdinal("user_id")),
                Stars = reader.GetInt32(reader.GetOrdinal("stars")),
                Comment = reader.IsDBNull(reader.GetOrdinal("comment"))
                    ? null
                    : reader.GetString(reader.GetOrdinal("comment")),
                IsCommentConfirmed = reader.GetBoolean(reader.GetOrdinal("is_comment_confirmed")),
                CreatedAt = reader.GetDateTime(reader.GetOrdinal("created_at"))
            });
        }

        return ratings;
    }

}