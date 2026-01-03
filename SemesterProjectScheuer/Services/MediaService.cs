using Newtonsoft.Json;
using SemesterProjectScheuer.IRepositories;
using SemesterProjectScheuer.Models;
using Exception = System.Exception;

namespace SemesterProjectScheuer.Services;

public class MediaService
{
    private readonly IMediaRepository _mediaRepository;

    public MediaService(IMediaRepository mediaRepository)
    {
        _mediaRepository = mediaRepository;
    }

    public bool RegisterMedia(HttpRequest request)
    {
        string jsonBody = request.Body;

        
        var registerMediaDto = JsonConvert.DeserializeObject<RegisterMedia>(jsonBody);
        if (registerMediaDto == null)
        {
            throw new Exception("Deserialisierung fehlgeschlagen");
        }

        bool registrationSuccessful = _mediaRepository.CreateMedia(registerMediaDto);
        return registrationSuccessful;

    }

    public MediaElement GetMediaById(HttpRequest request)
    {
        var dto = JsonConvert.DeserializeObject<GetMediaRequest>(request.Body);

        if (dto == null)
        {
            throw new Exception("Deserialisierung fehlgeschlagen");
        }

        MediaElement media = _mediaRepository.GetMediaById(dto.MediaId);
        
        media.Ratings = _mediaRepository.GetRatingsOfMedia(dto.MediaId);
        
        media.AverageScore = media.Ratings.Any()
            ? media.Ratings.Average(r => r.Stars)
            : 0;

        if (media == null)
        {
            throw new Exception("Media nicht gefunden");
        }

        return media;
    }

    public List<MediaElement> GetAllMedia()
    {
        List<MediaElement> medias = _mediaRepository.GetAllMedias();
        return medias;
    }

    public bool DeleteMediaById(HttpRequest request)
    {
        var dto = JsonConvert.DeserializeObject<GetMediaRequest>(request.Body);

        if (dto == null)
        {
            throw new Exception("Deserialisierung fehlgeschlagen");
        }

        bool deletion = _mediaRepository.DeleteMediaById(dto.MediaId);


        return deletion;
    }

    public MediaElement ChangeMedia(HttpRequest request)
    {
        string jsonBody = request.Body;

        var changeMediaDto = JsonConvert.DeserializeObject<ChangeMedia>(jsonBody);
        if (changeMediaDto == null)
        {
            throw new Exception("Deserialisierung fehlgeschlagen");

        }

        MediaElement changeSuccessful = _mediaRepository.ChangeMedia(changeMediaDto);
        return changeSuccessful;

    }

    public List<MediaElement> FilterMedia(string? title, int? ageRestriction, int? releaseYear)
    {
        // 1. Alle Medien holen
        var media = _mediaRepository.GetAllMedias();
        foreach (var m in media)
        {
            m.Ratings = _mediaRepository.GetRatingsOfMedia(m.MediaId);

            m.AverageScore = m.Ratings.Any()
                ? m.Ratings.Average(r => r.Stars)
                : 0;
        }
        // 2. Nach Titel filtern
        if (title != null)
        {
            media =
                (from m in media
                    where m.Title != null && m.Title.Contains(title)
                    select m).ToList();
        }

        // 3. Nach Altersfreigabe filtern
        if (ageRestriction != null)
        {
            media =
                (from m in media
                    where m.AgeRestriction == ageRestriction
                    select m).ToList();
        }

        // 4. Nach Release-Jahr filtern
        if (releaseYear != null)
        {
            media =
                (from m in media
                    where m.ReleaseYear == releaseYear
                    select m).ToList();
        }

        // 5. Ergebnis zurückgeben
        return media;
    }
}