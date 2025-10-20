using SemesterProjectScheuer.Services;

namespace SemesterProjectScheuer.Controller;

public class UsersController
{
    private UserService _userService = new UserService();
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
            Console.WriteLine(token);
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

    
}