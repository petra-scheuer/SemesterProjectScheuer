using Newtonsoft.Json;
using SemesterProjectScheuer.IRepositories;
using SemesterProjectScheuer.Models;

namespace SemesterProjectScheuer.Services;

public class MediaService
{
    private readonly IMediaRepository _mediaRepository;
    
    public MediaService(IMediaRepository mediaRepository)
    {
        _mediaRepository = mediaRepository;
    }

    public bool RegisterMedia(HttpRequest request)
    {
        string jsonBody = request.Body;

        var registerUserDto = JsonConvert.DeserializeObject<RegisterMedia>(jsonBody);
        if(registerUserDto == null)
        {
            throw new Exception("Deserialisierung fehlgeschlagen");
            return false;
        }
        bool registrationSuccessful = _mediaRepository.CreateMedia(registerUserDto);
        return registrationSuccessful;
        
    }

    
}
