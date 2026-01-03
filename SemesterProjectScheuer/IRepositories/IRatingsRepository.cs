using SemesterProjectScheuer.Models;

namespace SemesterProjectScheuer.IRepositories;

public interface IRatingsRepository
{
    bool CreateRatings(RegisterRating newRating);
    
    RatingObject ChangeRating(ChangeRating changeRatingDto);
    
    RatingObject GetRating(int ratingId);

}