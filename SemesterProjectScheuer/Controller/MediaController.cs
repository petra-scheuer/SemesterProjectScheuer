using Newtonsoft.Json;
using SemesterProjectScheuer.Models;

namespace SemesterProjectScheuer.Controller;
using SemesterProjectScheuer.Repository;
using SemesterProjectScheuer.Services;

public class MediaController
{
    private MediaService _mediaService = new MediaService(new MediaRepository());
    public HttpResponse Handle(HttpRequest request)
    {
        string path = request.Path;
        if (path == "/")
            return new HttpResponse() { StatusCode = 404 };
        if (path == "/media/register" && request.Method == "POST")
        { 
            bool success = _mediaService.RegisterMedia(request);
            if (success)
            {
                return new HttpResponse
                {
                    StatusCode = 200,
                    ContentType = "text/plain",
                    Body = "Media registered successfully!"
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
                
        if (request.Method == "GET" && path.StartsWith("/media/all"))
        {
            try
            {
                List<MediaElement> medias = _mediaService.GetAllMedia();

                if (medias == null || medias.Count == 0)
                {
                    return new HttpResponse
                    {
                        StatusCode = 404,
                        ContentType = "text/plain",
                        Body = "No medias found!"
                    };
                }

                return new HttpResponse
                {
                    StatusCode = 200,
                    ContentType = "application/json",
                    Body = JsonConvert.SerializeObject(medias)
                };
            }
            catch (Exception)
            {
                return new HttpResponse
                {
                    StatusCode = 500,
                    ContentType = "text/plain",
                    Body = "Internal server error"
                };
            }
        }
        
        if (request.Method == "GET" && path.StartsWith("/media/"))
        {
            MediaElement media = _mediaService.GetMediaById(request);

            if (media == null)
            {
                return new HttpResponse
                {
                    StatusCode = 404,
                    ContentType = "text/plain",
                    Body = "Media not found!"
                };
            }

            return new HttpResponse
            {
                StatusCode = 200,
                ContentType = "application/json",
                Body = JsonConvert.SerializeObject(media)
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
        if (path == "/media/{media_id}" && request.Method == "PUT")
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