using SemesterProjectScheuer.Models;

namespace SemesterProjectScheuer.IRepositories;

public interface IRatingsRepository
{
    bool CreateRatings(RegisterRating newRating);

}