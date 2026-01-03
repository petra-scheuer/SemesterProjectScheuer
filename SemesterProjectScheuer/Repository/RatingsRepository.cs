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

    public RatingObject ChangeRating(ChangeRating changeRatingDto)
    {
        if (changeRatingDto.Stars < 1 || changeRatingDto.Stars > 5)
            throw new Exception("Stars must be between 1 and 5.");

        const string sql = @"
        UPDATE rating
        SET
            stars = @s,
            comment = @c,
            is_confirmed = @ic
        WHERE id = @id
          AND user_id = @uid
        RETURNING id, media_id, user_id, stars, comment, is_confirmed, created_at;
    ";

        using var conn = DatabaseManager.GetConnection();
        using var cmd = new NpgsqlCommand(sql, conn);

        var comment = string.IsNullOrWhiteSpace(changeRatingDto.Comment)
            ? "Nicht kommentiert"
            : changeRatingDto.Comment;

        cmd.Parameters.AddWithValue("id", changeRatingDto.RatingId);
        cmd.Parameters.AddWithValue("uid", changeRatingDto.UserId);               // schützt davor, fremde Ratings zu ändern
        cmd.Parameters.AddWithValue("s", changeRatingDto.Stars);
        cmd.Parameters.AddWithValue("c", comment);
        cmd.Parameters.AddWithValue("ic", changeRatingDto.IsCommentConfirmed);

        using var reader = cmd.ExecuteReader();

        if (!reader.Read())
            throw new Exception($"Rating with id {changeRatingDto.RatingId} not found (or not owned by this user).");

        return new RatingObject
        {
            RatingId = reader.GetInt32(reader.GetOrdinal("id")),
            MediaId = reader.GetInt32(reader.GetOrdinal("media_id")),
            UserId = reader.GetInt32(reader.GetOrdinal("user_id")),
            Stars = reader.GetInt32(reader.GetOrdinal("stars")),
            Comment = reader.IsDBNull(reader.GetOrdinal("comment"))
                ? null
                : reader.GetString(reader.GetOrdinal("comment")),
            IsCommentConfirmed = reader.GetBoolean(reader.GetOrdinal("is_confirmed")),
            CreatedAt = reader.GetDateTime(reader.GetOrdinal("created_at")),
        };
    }

    public RatingObject GetRating(int ratingId)
    {
        
        const string sql = @"Select * from rating where id = @id";
        using var conn = DatabaseManager.GetConnection();
        using var cmd = new NpgsqlCommand(sql, conn);
        cmd.Parameters.AddWithValue("id", ratingId);
        
        using var reader = cmd.ExecuteReader();

        if (!reader.Read())
            throw new Exception($"Rating not found (or not owned by this user).");

        return new RatingObject
        {
            RatingId = reader.GetInt32(reader.GetOrdinal("id")),
            MediaId = reader.GetInt32(reader.GetOrdinal("media_id")),
            UserId = reader.GetInt32(reader.GetOrdinal("user_id")),
            Stars = reader.GetInt32(reader.GetOrdinal("stars")),
            Comment = reader.IsDBNull(reader.GetOrdinal("comment"))
                ? null
                : reader.GetString(reader.GetOrdinal("comment")),
            IsCommentConfirmed = reader.GetBoolean(reader.GetOrdinal("is_confirmed")),
            CreatedAt = reader.GetDateTime(reader.GetOrdinal("created_at")),
        };
        
        
        
        
    }


}