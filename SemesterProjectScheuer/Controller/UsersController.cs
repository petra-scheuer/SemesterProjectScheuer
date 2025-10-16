using SemesterProjectScheuer.Services;

namespace SemesterProjectScheuer.Controller;

public class UsersController
{
    private UserService _UserService = new UserService();
    public HttpResponse Handle(HttpRequest request)
    {
        if (request.Method == "POST")
        {
            bool success = _UserService.RegisterUser(request);
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
        else
        {
            return new HttpResponse
            {
                StatusCode = 404,
                ContentType = "text/plain",
                Body = "Not found!"
            };
        }
    }

    
}