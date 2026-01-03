using SemesterProjectScheuer.Models;
using SemesterProjectScheuer.Repository;
using SemesterProjectScheuer.Services;

namespace SemesterProjectScheuer.Controller;

public class UsersController
{
    private UserService _userService = new UserService(new UserRepository());
    public HttpResponse Handle(HttpRequest request)
    {
        string path = request.Path;
        if (path == "/")
            return new HttpResponse() { StatusCode = 404 };
        if (path == "/users/register" && request.Method == "POST")
        {
            bool success = _userService.RegisterUser(request);
            if (success)
            {
                return new HttpResponse
                {
                    StatusCode = 200,
                    ContentType = "text/plain",
                    Body = "User registered successfully!"
                };
            }
            else
            {
                return new HttpResponse
                {
                    StatusCode = 400,
                    ContentType = "text/plain",
                    Body = "Some error occured!"
                };
            }
        }


        if (path == "/users/login" && request.Method == "POST")
        {
            var token = _userService.LoginUser(request);
            
            return new HttpResponse
                {
                    StatusCode = 200,
                    ContentType = "text/plain",
                    Body = token
                };
        }
        {
                return new HttpResponse
                {
                    StatusCode = 400,
                    ContentType = "text/plain",
                    Body = "Some error occured!"
                };
        }
        
    }
    public CurrentActiveUser? getCurrentActiveUser(HttpRequest request)
    {
        // 1) Authorization Header muss da sein
        if (string.IsNullOrWhiteSpace(request.Authorization))
            return null;

        // 2) Muss "Bearer <token>" sein
        const string prefix = "Bearer ";
        if (!request.Authorization.StartsWith(prefix, StringComparison.OrdinalIgnoreCase))
            return null;

        // 3) Token extrahieren
        string token = request.Authorization.Substring(prefix.Length).Trim();
        if (string.IsNullOrWhiteSpace(token))
            return null;

        // 4) Token validieren (muss bei ungültig/abgelaufen -> null liefern)
        CurrentActiveUser user = _userService.ValidateTokenAsync(token);

        return user; // user kann null sein -> passt
    }
  
}