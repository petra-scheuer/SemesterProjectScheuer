using SemesterProjectScheuer.Controller;

namespace SemesterProjectScheuer;

public class Router
{
    public static HttpResponse Route(HttpRequest request)
    {
        var usersController   = new UsersController();
        
        if (request.Path.StartsWith("/users"))
        {
            return usersController.Handle(request);
        }
        
        else
        {
            return new HttpResponse
            {
                StatusCode = 404,
                ContentType = "text/plain",
                Body = "Nicht gefunden"
            };
        }
    }
    
}