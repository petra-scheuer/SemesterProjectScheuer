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
    public bool RegisterRating(HttpRequest request)
    {
        string jsonBody = request.Body;

        var registerRatingDto = JsonConvert.DeserializeObject<RegisterRating>(jsonBody);
        if (registerRatingDto == null)
        {
            throw new Exception("Deserialisierung fehlgeschlagen");
        }

        bool registrationSuccessful = _ratingsRepository.CreateRatings(registerRatingDto);
        return registrationSuccessful;

    }
}