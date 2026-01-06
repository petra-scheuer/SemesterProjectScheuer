using Moq;
using Newtonsoft.Json;
using SemesterProjectScheuer;
using SemesterProjectScheuer.IRepositories;
using SemesterProjectScheuer.Models;
using SemesterProjectScheuer.Services;

namespace MyUnitTests;

[TestFixture]
public class MediaServiceTests
{
    [Test]
    public void RegisterMedia_ValidJson_CallsCreateMediaAndReturnsTrue()
    {
        // Vorbereitung
        var request = new HttpRequest
        {
            Body = "{\"title\":\"Inception\",\"description\":\"Mind-bending\",\"mediaType\":\"movie\",\"releaseYear\":2010,\"genres\":[\"Sci-Fi\",\"Thriller\"],\"ageRestriction\":12}"
        };

        var repoMock = new Mock<IMediaRepository>();
        repoMock
            .Setup(r => r.CreateMedia(It.IsAny<RegisterMedia>()))
            .Returns(true);

        var service = new MediaService(repoMock.Object);

        // Ausführen
        var result = service.RegisterMedia(request);

        // Überprüfen
        Assert.That(result, Is.True);

        repoMock.Verify(r => r.CreateMedia(It.Is<RegisterMedia>(m =>
            m.Title == "Inception" &&
            m.Description == "Mind-bending" &&
            m.MediaType == "movie" &&
            m.ReleaseYear == 2010 &&
            m.Genres != null && m.Genres.Count == 2 &&
            m.Genres[0] == "Sci-Fi" &&
            m.Genres[1] == "Thriller" &&
            m.AgeRestriction == 12
        )), Times.Once);
    }

    [Test]
    public void RegisterMedia_MissingFields_RepositoryReturnsFalse_CallsCreateMediaAndReturnsFalse()
    {
        // "Invalid" im Sinn von: unvollständige Daten (z.B. nur Title)
        var request = new HttpRequest
        {
            Body = "{\"title\":\"OnlyTitle\"}"
        };

        var repoMock = new Mock<IMediaRepository>();
        repoMock
            .Setup(r => r.CreateMedia(It.IsAny<RegisterMedia>()))
            .Returns(false);

        var service = new MediaService(repoMock.Object);

        // Ausführen
        var result = service.RegisterMedia(request);

        // Überprüfen
        Assert.That(result, Is.False);
        repoMock.Verify(r => r.CreateMedia(It.IsAny<RegisterMedia>()), Times.Once);
    }
    
    
    [Test]
    public void RegisterMedia_BodyIsNullLiteral_ThrowsException_AndDoesNotCallRepo()
    {
        // JsonConvert.DeserializeObject<RegisterMedia>("null") => null
        var request = new HttpRequest
        {
            Body = "null"
        };

        var repoMock = new Mock<IMediaRepository>();
        var service = new MediaService(repoMock.Object);

        Assert.That(
            () => service.RegisterMedia(request),
            Throws.Exception.With.Message.EqualTo("Deserialisierung fehlgeschlagen")
        );

        repoMock.Verify(r => r.CreateMedia(It.IsAny<RegisterMedia>()), Times.Never);
    }

    [Test]
    public void RegisterMedia_InvalidJson_ThrowsJsonReaderException_AndDoesNotCallRepo()
    {
        // Ungültiges JSON -> JsonConvert wirft JsonReaderException
        var request = new HttpRequest
        {
            Body = "{this is not valid json"
        };

        var repoMock = new Mock<IMediaRepository>();
        var service = new MediaService(repoMock.Object);

        Assert.That(
            () => service.RegisterMedia(request),
            Throws.TypeOf<JsonReaderException>()
        );

        repoMock.Verify(r => r.CreateMedia(It.IsAny<RegisterMedia>()), Times.Never);
    }

    [Test]
    public void GetMediaById_ValidJson_CallsRepoWithId_AndReturnsMedia()
    {
        var request = new HttpRequest
        {
            Body = "{\"mediaId\": 42}"
        };

        var repoMock = new Mock<IMediaRepository>();
        repoMock
            .Setup(r => r.GetMediaById(42))
            .Returns(new MediaElement
            {
                MediaId = 42,
                Title = "Inception",
                Description = "Mind-bending",
                MediaType = "movie",
                ReleaseYear = 2010,
                Genres = "Sci-Fi,Thriller",
                AgeRestriction = 12
            });
        
        repoMock.Setup(r => r.GetRatingsOfMedia(42))
            .Returns(new List<RatingObject>());

        var service = new MediaService(repoMock.Object);

        var result = service.GetMediaById(request);
        repoMock.Setup(r => r.GetRatingsOfMedia(It.IsAny<int>()))
            .Returns(new List<RatingObject>());


        Assert.That(result, Is.Not.Null);
        Assert.That(result.MediaId, Is.EqualTo(42));
        Assert.That(result.Title, Is.EqualTo("Inception"));

        repoMock.Verify(r => r.GetMediaById(42), Times.Once);
    }

    [Test]
    public void GetMediaById_BodyIsNullLiteral_ThrowsException_AndDoesNotCallRepo()
    {
        // DeserializeObject<GetMediaRequest>("null") => null
        var request = new HttpRequest
        {
            Body = "null"
        };

        var repoMock = new Mock<IMediaRepository>();
        var service = new MediaService(repoMock.Object);

        Assert.That(
            () => service.GetMediaById(request),
            Throws.Exception.With.Message.EqualTo("Deserialisierung fehlgeschlagen")
        );

        repoMock.Verify(r => r.GetMediaById(It.IsAny<int>()), Times.Never);
    }

    [Test]
    public void GetAllMedia_CallsRepoOnce_AndReturnsList()
    {
        var expected = new List<MediaElement>
        {
            new MediaElement { MediaId = 1, Title = "A", Genres = "X" },
            new MediaElement { MediaId = 2, Title = "B", Genres = "Y" }
        };

        var repoMock = new Mock<IMediaRepository>();
        repoMock
            .Setup(r => r.GetAllMedias())
            .Returns(expected);

        var service = new MediaService(repoMock.Object);

        var result = service.GetAllMedia();

        Assert.That(result, Is.SameAs(expected));
        Assert.That(result.Count, Is.EqualTo(2));

        repoMock.Verify(r => r.GetAllMedias(), Times.Once);
    }

    [Test]
    public void GetAllMedia_RepoReturnsEmptyList_ReturnsEmptyList()
    {
        var repoMock = new Mock<IMediaRepository>();
        repoMock
            .Setup(r => r.GetAllMedias())
            .Returns(new List<MediaElement>());

        var service = new MediaService(repoMock.Object);

        var result = service.GetAllMedia();

        Assert.That(result, Is.Not.Null);
        Assert.That(result, Is.Empty);
        repoMock.Verify(r => r.GetAllMedias(), Times.Once);
    }
    
     [Test]
    public void FilterMedia_NoFilters_ReturnsAllMedia_AndCallsRepoOnce()
    {
        var data = new List<MediaElement>
        {
            new MediaElement { MediaId = 1, Title = "Inception", AgeRestriction = 12, ReleaseYear = 2010 },
            new MediaElement { MediaId = 2, Title = "Interstellar", AgeRestriction = 12, ReleaseYear = 2014 },
            new MediaElement { MediaId = 3, Title = "The Dark Knight", AgeRestriction = 16, ReleaseYear = 2008 },
            new MediaElement { MediaId = 4, Title = null, AgeRestriction = 0, ReleaseYear = 1999 }
        };

        var repoMock = new Mock<IMediaRepository>();
        repoMock.Setup(r => r.GetAllMedias()).Returns(data);
        repoMock.Setup(r => r.GetRatingsOfMedia(It.IsAny<int>()))
            .Returns(new List<RatingObject>());


        var service = new MediaService(repoMock.Object);

        var result = service.FilterMedia(null, null, null);

        Assert.That(result, Is.Not.Null);
        Assert.That(result, Has.Count.EqualTo(4));
        Assert.That(result, Is.SameAs(data)); // gleiche List-Instanz, weil ohne Filter unverändert

        repoMock.Verify(r => r.GetAllMedias(), Times.Once);
        
    }

    
    [Test]
    public void FilterMedia_FilterByAgeRestriction_ReturnsMatching()
{
    var data = new List<MediaElement>
    {
        new MediaElement { MediaId = 1, Title = "Inception", AgeRestriction = 12, ReleaseYear = 2010 },
        new MediaElement { MediaId = 2, Title = "Interstellar", AgeRestriction = 12, ReleaseYear = 2014 },
        new MediaElement { MediaId = 3, Title = "The Dark Knight", AgeRestriction = 16, ReleaseYear = 2008 },
    };

    var repoMock = new Mock<IMediaRepository>();
    repoMock.Setup(r => r.GetAllMedias()).Returns(data);
    repoMock.Setup(r => r.GetRatingsOfMedia(It.IsAny<int>()))
            .Returns(new List<RatingObject>());

    var service = new MediaService(repoMock.Object);

    var result = service.FilterMedia(null, 12, null);

    Assert.That(result.Select(m => m.MediaId), Is.EquivalentTo(new[] { 1, 2 }));
    repoMock.Verify(r => r.GetAllMedias(), Times.Once);
}

    [Test]
    public void FilterMedia_MultipleFilters_ReturnsIntersection()
    {
        var data = new List<MediaElement>
        {
            new MediaElement { MediaId = 1, Title = "Inception", AgeRestriction = 12, ReleaseYear = 2010 },
            new MediaElement { MediaId = 2, Title = "Inception 2", AgeRestriction = 16, ReleaseYear = 2010 },
            new MediaElement { MediaId = 3, Title = "Inception", AgeRestriction = 12, ReleaseYear = 2014 },
            new MediaElement { MediaId = 4, Title = "Other", AgeRestriction = 12, ReleaseYear = 2010 },
        };

        var repoMock = new Mock<IMediaRepository>();
        repoMock.Setup(r => r.GetAllMedias()).Returns(data);
        repoMock.Setup(r => r.GetRatingsOfMedia(It.IsAny<int>()))
            .Returns(new List<RatingObject>());


        var service = new MediaService(repoMock.Object);

        var result = service.FilterMedia("Inception", 12, 2010);

        Assert.That(result.Select(m => m.MediaId), Is.EquivalentTo(new[] { 1 }));

        repoMock.Verify(r => r.GetAllMedias(), Times.Once);
        
    }

    [Test]
    public void FilterMedia_NoMatches_ReturnsEmptyList()
    {
        var data = new List<MediaElement>
        {
            new MediaElement { MediaId = 1, Title = "Inception", AgeRestriction = 12, ReleaseYear = 2010 },
            new MediaElement { MediaId = 2, Title = "Interstellar", AgeRestriction = 12, ReleaseYear = 2014 },
        };

        var repoMock = new Mock<IMediaRepository>();
        repoMock.Setup(r => r.GetAllMedias()).Returns(data);
        repoMock.Setup(r => r.GetRatingsOfMedia(It.IsAny<int>()))
            .Returns(new List<RatingObject>());


        var service = new MediaService(repoMock.Object);

        var result = service.FilterMedia("DoesNotExist", 99, 1900);

        Assert.That(result, Is.Not.Null);
        Assert.That(result, Is.Empty);

        repoMock.Verify(r => r.GetAllMedias(), Times.Once);
    }

    [Test]
    public void FilterMedia_TitleIsEmptyString_FiltersToNonNullTitles_BecauseContainsEmptyStringIsTrue()
    {
        // Dieser Test zeigt das aktuelle Verhalten deiner Implementierung (evtl. unerwünscht, aber dokumentiert)
        var data = new List<MediaElement>
        {
            new MediaElement { MediaId = 1, Title = "A", AgeRestriction = 0, ReleaseYear = 2000 },
            new MediaElement { MediaId = 2, Title = null, AgeRestriction = 0, ReleaseYear = 2000 },
            new MediaElement { MediaId = 3, Title = "B", AgeRestriction = 0, ReleaseYear = 2000 },
        };

        var repoMock = new Mock<IMediaRepository>();
        repoMock.Setup(r => r.GetAllMedias()).Returns(data);
        repoMock.Setup(r => r.GetRatingsOfMedia(It.IsAny<int>()))
            .Returns(new List<RatingObject>());


        var service = new MediaService(repoMock.Object);

        var result = service.FilterMedia("", null, null);

        // "" ist in jedem non-null string enthalten => alle non-null Titles bleiben übrig
        Assert.That(result.Select(m => m.MediaId), Is.EquivalentTo(new[] { 1, 3 }));

        repoMock.Verify(r => r.GetAllMedias(), Times.Once);
    }
}