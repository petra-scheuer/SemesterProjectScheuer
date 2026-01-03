using SemesterProjectScheuer.Controller;

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
        if (request.Path.StartsWith("/media"))
        {
            return mediaController.Handle(request);
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