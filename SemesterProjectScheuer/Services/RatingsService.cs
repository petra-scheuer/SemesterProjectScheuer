using System.Runtime.InteropServices.Swift;
using Newtonsoft.Json;
using SemesterProjectScheuer.IRepositories;
using SemesterProjectScheuer.Models;

namespace SemesterProjectScheuer.Services;

public class RatingsService
{
    private readonly IRatingsRepository _ratingsRepository;

    public RatingsService(IRatingsRepository ratingsRepository)
    {
        _ratingsRepository = ratingsRepository;
    }
    public bool RegisterRating(HttpRequest request, CurrentActiveUser currentActiveUser)
    {
        string jsonBody = request.Body;

        var registerRatingDto = JsonConvert.DeserializeObject<RegisterRating>(jsonBody);
        if (registerRatingDto == null)
        {
            throw new Exception("Deserialisierung fehlgeschlagen");
        }

        registerRatingDto.UserId = currentActiveUser.userId;

        bool registrationSuccessful = _ratingsRepository.CreateRatings(registerRatingDto);
        return registrationSuccessful;
    }
    

    public RatingObject ChangeRating(HttpRequest request, CurrentActiveUser currentActiveUser)
    {
        if (currentActiveUser == null)
            throw new UnauthorizedAccessException("Unauthorized");

        string jsonBody = request.Body;

        var changeRatingDto = JsonConvert.DeserializeObject<ChangeRating>(jsonBody);
        if (changeRatingDto == null)
            throw new Exception("Deserialisierung fehlgeschlagen");

        RatingObject oldRatingObject = _ratingsRepository.GetRating(changeRatingDto.RatingId);
        
        if (oldRatingObject.UserId != currentActiveUser.userId)
            return null;
        
        changeRatingDto.UserId = currentActiveUser.userId;

        RatingObject updatedRating = _ratingsRepository.ChangeRating(changeRatingDto);
        return updatedRating;
    }

    public bool DeleteRating(HttpRequest request, CurrentActiveUser currentActiveUser)
    {
        string jsonBody = request.Body;

        var deleteRatingDto = JsonConvert.DeserializeObject<ChooseRating>(jsonBody);
        if (deleteRatingDto == null)
            throw new Exception("Deserialisierung fehlgeschlagen");

        RatingObject oldRatingObject = _ratingsRepository.GetRating(deleteRatingDto.RatingId);
        
        if (oldRatingObject.UserId != currentActiveUser.userId)
            return false;
        
        deleteRatingDto.UserId = currentActiveUser.userId;

        bool deleted = _ratingsRepository.DeleteRating(deleteRatingDto);
        return deleted;
    }

    public bool LikeRateing(HttpRequest request, CurrentActiveUser currentActiveUser)
    {
        string jsonBody = request.Body;

        var likeRatingDto = JsonConvert.DeserializeObject<ChooseRating>(jsonBody);
        if (likeRatingDto == null)
            throw new Exception("Deserialisierung fehlgeschlagen");
        
        likeRatingDto.UserId = currentActiveUser.userId;
        
        bool liked = _ratingsRepository.LikeRating(likeRatingDto);
        return liked;
        
    }
}