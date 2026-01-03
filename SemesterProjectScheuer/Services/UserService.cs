using SemesterProjectScheuer.Models;
using SemesterProjectScheuer.Repository;
using Newtonsoft.Json;
using SemesterProjectScheuer.IRepositories;

namespace SemesterProjectScheuer.Services;

public class UserService
{
    private readonly IUserRepository _userRepository;

    public UserService(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }


    public bool RegisterUser(HttpRequest request)
    {
        string jsonBody = request.Body;

        var registerUserDto = JsonConvert.DeserializeObject<RegisterUser>(jsonBody);
        if(registerUserDto == null)
        {
            throw new Exception("Deserialisierung fehlgeschlagen");
        }
        bool registrationSuccessful = _userRepository.CreateUser(registerUserDto);
        return registrationSuccessful;
        
    }
    
    public string LoginUser(HttpRequest request)
    {
        string jsonBody = request.Body;
        if (jsonBody == null)
        {
            throw new Exception("Body Leer");

        }
        LoginUser loginUserDto = JsonConvert.DeserializeObject<LoginUser>(jsonBody);
        if (loginUserDto == null)
        {
            throw new Exception("Deserialisierung fehlgeschlagen");
        }
        UserModel user = _userRepository.AuthenticateUser(loginUserDto.username, loginUserDto.password);
        if (user == null)
        {
            throw new Exception("Authentifizierung fehlgeschlagen");

        }

        // Token generieren: "username-mrpToken" oder GUID-basierend
        string token = $"{user.username}-mrpToken-{Guid.NewGuid()}";
        bool success = _userRepository.SaveToken(user.username, token);
        if (success is true)
        {
            return token;
            
        }
        else
        {
            throw new Exception("Token nicht gespreichert");
        }
    }
    public CurrentActiveUser ValidateTokenAsync(string token)
    {
        if (string.IsNullOrEmpty(token)) return null;
        return _userRepository.GetUserByToken(token);
    }
    
}