using SemesterProjectScheuer.Controller;
using SemesterProjectScheuer.Models;

namespace SemesterProjectScheuer;

public class Router
{
    public static HttpResponse Route(HttpRequest request)
    {
        var usersController   = new UsersController();
        var mediaController = new MediaController();
        var ratingsController = new RatingsController();
        
        if (request.Path.StartsWith("/users"))
        {
            return usersController.Handle(request);
        }
        
        CurrentActiveUser currentActiveUser = usersController.getCurrentActiveUser(request);
        Console.WriteLine(currentActiveUser);
        if (currentActiveUser == null)
        {
            return new HttpResponse
            {
                StatusCode = 401,
                ContentType = "text/plain",
                Body = "Unauthorized"
            };
        }
        
        if (request.Path.StartsWith("/media"))
        {
            return mediaController.Handle(request);
        }
        if (request.Path.StartsWith("/rate/"))
        {
            return ratingsController.Handle(request, currentActiveUser);
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