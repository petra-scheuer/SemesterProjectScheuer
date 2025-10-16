using SemesterProjectScheuer.Models;
using SemesterProjectScheuer.Repository;
using Newtonsoft.Json;

namespace SemesterProjectScheuer.Services;

public class UserService
{
    private UserRepository _userRepository = new UserRepository();

    public bool RegisterUser(HttpRequest request)
    {
        string jsonBody = request.Body;

        var registerUserDTO = JsonConvert.DeserializeObject<RegisterUser>(jsonBody);
        if(registerUserDTO == null)
        {
            throw new Exception("Deserialisierung fehlgeschlagen");
        }
        bool registrationSuccessful = _userRepository.CreateUser(registerUserDTO);
        return registrationSuccessful;
        
    }
    
}