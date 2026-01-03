using Moq;
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
    
}