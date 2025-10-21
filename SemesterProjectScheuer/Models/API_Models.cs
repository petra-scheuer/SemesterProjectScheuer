namespace SemesterProjectScheuer.Models;

// Definiert für  PUT /users/{userId}/profile
public class UserProfileUpdate
{
    public string? Email { get; set; }
    public string? FavoriteGenre { get; set; }
}

// Definiert für  POST /media, PUT /media/{mediaId}
public class MediaInput
{
    public string? Title { get; set; }
    public string? Description { get; set; }
    public string? MediaType { get; set; }   // e.g. "movie", "series"
    public int? ReleaseYear { get; set; }
    public List<string>? Genres { get; set; }
    public int? AgeRestriction { get; set; }
}

// Definiert für POST /media/{mediaId}/rate, PUT /ratings/{ratingId}
public class RatingInput
{ 
    public int Stars { get; set; }           // zwischen 1 und 5 
    public string? Comment { get; set; }
}