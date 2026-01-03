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

public class MediaElement
{
    public int MediaId { get; set; }
    public DateTime created_at { get; set; }
    public string? Title { get; set; }
    public string? Description { get; set; }
    public string? MediaType { get; set; }   // e.g. "movie", "series"
    public int? ReleaseYear { get; set; }
    public string Genres { get; set; }
    public int? AgeRestriction { get; set; }
    
}

public class ChangeMedia
{
    public int MediaId { get; set; }
    public string? Title { get; set; }
    public string? Description { get; set; }
    public string? MediaType { get; set; }   // e.g. "movie", "series"
    public int? ReleaseYear { get; set; }
    public  List<string>? Genres { get; set; }
    public int? AgeRestriction { get; set; }
    
}

public class GetMediaRequest
{
    public int MediaId { get; set; }
}