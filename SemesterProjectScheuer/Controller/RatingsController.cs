namespace SemesterProjectScheuer.Controller;

public class RatingsController
{
    public HttpResponse Handle(HttpRequest request)
    {
        string path = request.Path;
        if (path == "/")
            return new HttpResponse() { StatusCode = 404 };
        if (path == "/ratings/media/{id}/rate" && request.Method == "POST")
        { 
            return new HttpResponse
            {
                StatusCode = 404,
                ContentType = "text/plain",
                Body = "Not yet implemented."
            };
        }
        if (path == "/ratings/{id}/like" && request.Method == "POST")
        { 
            return new HttpResponse
            {
                StatusCode = 404,
                ContentType = "text/plain",
                Body = "Not yet implemented."
            };
        }
        
        if (path == "/media/{media_id}" && request.Method == "GET")
        {
            return new HttpResponse
            {
                StatusCode = 404,
                ContentType = "text/plain",
                Body = "Not yet implemented."
            };
        }
        if (path == "/media/{media_id}" && request.Method == "DELETE")
        {
            return new HttpResponse
            {
                StatusCode = 404,
                ContentType = "text/plain",
                Body = "Not yet implemented."
            };
        }
        if (path == "/ratings/{id}" && request.Method == "PUT")
        {
            return new HttpResponse
            {
                StatusCode = 404,
                ContentType = "text/plain",
                Body = "Not yet implemented."
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