namespace SemesterProjectScheuer;

public class Router
{
    public static HttpResponse Route(HttpRequest request)
    {


        if (request.Path.StartsWith("/hello"))
        {
            return new HttpResponse
            {
                StatusCode = 200,
                ContentType = "text/plain",
                Body = "Route gefunden"
            };
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