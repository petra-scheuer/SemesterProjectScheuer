using SemesterProjectScheuer.Repository;
using SemesterProjectScheuer.Services;

namespace SemesterProjectScheuer.Controller;

public class RatingsController
{
    private RatingsService _ratingsService = new RatingsService(new RatingsRepository());

    public HttpResponse Handle(HttpRequest request)
    {
        string path = request.Path;
        if (path == "/")
            return new HttpResponse() { StatusCode = 404 };
        if (path == "/rate/media/" && request.Method == "POST")
        { 
            bool success = _ratingsService.RegisterRating(request);

            if (success)
            {
                return new HttpResponse
                {
                    StatusCode = 200,
                    ContentType = "text/plain",
                    Body = "Rating successful."
                };
            }
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
        
        if (path == "/ratings/" && request.Method == "GET")
        {
            return new HttpResponse
            {
                StatusCode = 404,
                ContentType = "text/plain",
                Body = "Not yet implemented."
            };
        }
        if (path == "/ratings/" && request.Method == "DELETE")
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