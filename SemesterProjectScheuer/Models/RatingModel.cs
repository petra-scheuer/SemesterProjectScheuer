namespace SemesterProjectScheuer.Models;

public class RatingModel
{
    public int RatingId { get; set; }
    public int MediaId { get; set; }
    public int UserId { get; set; }
    public int Stars { get; set; }          // 1–5
    public string? Comment { get; set; }
    public bool IsCommentConfirmed { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class RegisterRating
{
    public int MediaId { get; set; }
    public int UserId { get; set; }
    public int Stars { get; set; }          // 1–5
    public string? Comment { get; set; }
}