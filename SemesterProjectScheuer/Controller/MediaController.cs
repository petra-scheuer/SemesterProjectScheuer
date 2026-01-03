using System.Net;
using Newtonsoft.Json;
using SemesterProjectScheuer.Models;
using SemesterProjectScheuer.Repository;
using SemesterProjectScheuer.Services;

namespace SemesterProjectScheuer.Controller;

public class MediaController
{
    private MediaService _mediaService = new MediaService(new MediaRepository());
    
    
    public HttpResponse Handle(HttpRequest request)
    {
        string path = request.Path;
        if (path == "/")
            return new HttpResponse() { StatusCode = 404 };
        if (request.Method == "GET" && request.Path.StartsWith("/media/filter"))
        {
            var query = ParseQuery(request.Path);

            string? title = query.ContainsKey("title") ? query["title"] : null;

            int? ageRestriction = null;
            if (query.ContainsKey("ageRestriction"))
                ageRestriction = int.Parse(query["ageRestriction"]);

            int? releaseYear = null;
            if (query.ContainsKey("releaseYear"))
                releaseYear = int.Parse(query["releaseYear"]);

            var result = _mediaService.FilterMedia(title, ageRestriction, releaseYear);

            return new HttpResponse
            {
                StatusCode = 200,
                ContentType = "application/json",
                Body = JsonConvert.SerializeObject(result)
            };
        }
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

                if (medias.Count == 0)
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

            return new HttpResponse
            {
                StatusCode = 200,
                ContentType = "application/json",
                Body = JsonConvert.SerializeObject(media)
            };
        }
        if (request.Method == "DELETE" && path.StartsWith("/media/"))
        {
            bool deletion = _mediaService.DeleteMediaById(request);

            if (deletion == false)
            {
                return new HttpResponse
                {
                    StatusCode = 404,
                    ContentType = "text/plain",
                    Body = "Problem with deleting!"
                };
            }

            return new HttpResponse
            {
                StatusCode = 200,
                ContentType = "text/plain",
                Body = "Media deleted successfully!"
            };
        }
       
        if (request.Method == "PUT" && path.StartsWith("/media/"))
        {
            MediaElement media = _mediaService.ChangeMedia(request);

            return new HttpResponse
            {
                StatusCode = 200,
                ContentType = "application/json",
                Body = JsonConvert.SerializeObject(media)
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
    private static Dictionary<string, string> ParseQuery(string path)
    {
        Dictionary<string, string> queryParams = new Dictionary<string, string>();

        // Gibt es überhaupt ein ? = Filter
        int questionMarkIndex = path.IndexOf('?');
        if (questionMarkIndex == -1)
            return queryParams;

        // Alles nach dem ?
        string queryString = path.Substring(questionMarkIndex + 1);

        // title=ring&ageRestriction=12
        string[] pairs = queryString.Split('&');

        foreach (string pair in pairs)
        {
            // key=value
            string[] parts = pair.Split('=');

            if (parts.Length == 2)
            {
                string key = WebUtility.UrlDecode(parts[0]);
                string value = WebUtility.UrlDecode(parts[1]);

                queryParams[key] = value;
            }
        }
        return queryParams;
    }
}