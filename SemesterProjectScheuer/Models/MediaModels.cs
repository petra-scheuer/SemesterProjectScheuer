namespace SemesterProjectScheuer.Models;

public class MediaModels
{
    
}

public class RegisterMedia
{
    public string? Title { get; set; }
    public string? Description { get; set; }
    public string? MediaType { get; set; }   // e.g. "movie", "series"
    public int? ReleaseYear { get; set; }
    public List<string>? Genres { get; set; }
    public int? AgeRestriction { get; set; }
}