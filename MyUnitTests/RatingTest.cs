using Moq;
using Newtonsoft.Json;
using SemesterProjectScheuer;
using SemesterProjectScheuer.IRepositories;
using SemesterProjectScheuer.Models;
using SemesterProjectScheuer.Services;

namespace MyUnitTests;

[TestFixture]
public class RatingTest
{
    [Test]
    public void RegisterRating_ValidJson_SetsUserIdFromCurrentUser_CallsCreateRatings_AndReturnsTrue()
    {
        // Vorbereitung
        var request = new HttpRequest
        {
            Body = "{\"mediaId\": 1, \"stars\": 5, \"comment\": \"Top!\"}"
        };

        var currentUser = new CurrentActiveUser { userId = 123 };

        var repoMock = new Mock<IRatingsRepository>();
        repoMock
            .Setup(r => r.CreateRatings(It.IsAny<RegisterRating>()))
            .Returns(true);

        var service = new RatingsService(repoMock.Object);

        // Ausführen
        var result = service.RegisterRating(request, currentUser);

        // Überprüfen
        Assert.That(result, Is.True);

        repoMock.Verify(r => r.CreateRatings(It.Is<RegisterRating>(dto =>
            dto.MediaId == 1 &&
            dto.Stars == 5 &&
            dto.Comment == "Top!" &&
            dto.UserId == 123   // <-- wichtig: kommt aus currentActiveUser
        )), Times.Once);
    }

    [Test]
    public void RegisterRating_BodyIsNullLiteral_ThrowsException_AndDoesNotCallRepo()
    {
        var request = new HttpRequest { Body = "null" };
        var currentUser = new CurrentActiveUser { userId = 123 };

        var repoMock = new Mock<IRatingsRepository>();
        var service = new RatingsService(repoMock.Object);

        Assert.That(
            () => service.RegisterRating(request, currentUser),
            Throws.Exception.With.Message.EqualTo("Deserialisierung fehlgeschlagen")
        );

        repoMock.Verify(r => r.CreateRatings(It.IsAny<RegisterRating>()), Times.Never);
    }

    [Test]
    public void RegisterRating_InvalidJson_ThrowsJsonReaderException_AndDoesNotCallRepo()
    {
        var request = new HttpRequest { Body = "{this is not valid json" };
        var currentUser = new CurrentActiveUser { userId = 123 };

        var repoMock = new Mock<IRatingsRepository>();
        var service = new RatingsService(repoMock.Object);

        Assert.That(
            () => service.RegisterRating(request, currentUser),
            Throws.TypeOf<JsonReaderException>()
        );

        repoMock.Verify(r => r.CreateRatings(It.IsAny<RegisterRating>()), Times.Never);
    }

    [Test]
    public void ChangeRating_CurrentActiveUserIsNull_ThrowsUnauthorizedAccessException()
    {
        var request = new HttpRequest
        {
            Body = "{\"ratingId\": 1, \"mediaId\": 1, \"stars\": 3, \"comment\": \"Update\"}"
        };

        var repoMock = new Mock<IRatingsRepository>();
        var service = new RatingsService(repoMock.Object);

        Assert.That(
            () => service.ChangeRating(request, null),
            Throws.TypeOf<UnauthorizedAccessException>()
                 .With.Message.EqualTo("Unauthorized")
        );

        repoMock.Verify(r => r.GetRating(It.IsAny<int>()), Times.Never);
        repoMock.Verify(r => r.ChangeRating(It.IsAny<ChangeRating>()), Times.Never);
    }

    [Test]
    public void ChangeRating_BodyIsNullLiteral_ThrowsException_AndDoesNotCallRepo()
    {
        var request = new HttpRequest { Body = "null" };
        var currentUser = new CurrentActiveUser { userId = 123 };

        var repoMock = new Mock<IRatingsRepository>();
        var service = new RatingsService(repoMock.Object);

        Assert.That(
            () => service.ChangeRating(request, currentUser),
            Throws.Exception.With.Message.EqualTo("Deserialisierung fehlgeschlagen")
        );

        repoMock.Verify(r => r.GetRating(It.IsAny<int>()), Times.Never);
        repoMock.Verify(r => r.ChangeRating(It.IsAny<ChangeRating>()), Times.Never);
    }

    [Test]
    public void ChangeRating_OwnerMatches_CallsGetRatingAndChangeRating_ReturnsUpdatedRating()
    {
        var request = new HttpRequest
        {
            Body = "{\"ratingId\": 10, \"mediaId\": 1, \"stars\": 4, \"comment\": \"Besser als gedacht\", \"isCommentConfirmed\": true}"
        };
        var currentUser = new CurrentActiveUser { userId = 123 };

        var oldRating = new RatingObject
        {
            RatingId = 10,
            MediaId = 1,
            UserId = 123,
            Stars = 5,
            Comment = "Alt",
            IsCommentConfirmed = false
        };

        var updatedRating = new RatingObject
        {
            RatingId = 10,
            MediaId = 1,
            UserId = 123,
            Stars = 4,
            Comment = "Besser als gedacht",
            IsCommentConfirmed = true
        };

        var repoMock = new Mock<IRatingsRepository>();
        repoMock.Setup(r => r.GetRating(10)).Returns(oldRating);
        repoMock
            .Setup(r => r.ChangeRating(It.IsAny<ChangeRating>()))
            .Returns(updatedRating);

        var service = new RatingsService(repoMock.Object);

        var result = service.ChangeRating(request, currentUser);

        Assert.That(result, Is.Not.Null);
        Assert.That(result, Is.SameAs(updatedRating));
        Assert.That(result.Stars, Is.EqualTo(4));
        Assert.That(result.Comment, Is.EqualTo("Besser als gedacht"));

        repoMock.Verify(r => r.GetRating(10), Times.Once);

        repoMock.Verify(r => r.ChangeRating(It.Is<ChangeRating>(dto =>
            dto.RatingId == 10 &&
            dto.MediaId == 1 &&
            dto.Stars == 4 &&
            dto.Comment == "Besser als gedacht" &&
            dto.IsCommentConfirmed == true &&
            dto.UserId == 123 // <-- wird im Service gesetzt
        )), Times.Once);
    }

    [Test]
    public void ChangeRating_OwnerMismatch_ReturnsNull_AndDoesNotCallChangeRating()
    {
        var request = new HttpRequest
        {
            Body = "{\"ratingId\": 10, \"mediaId\": 1, \"stars\": 1, \"comment\": \"Hack\"}"
        };
        var currentUser = new CurrentActiveUser { userId = 123 };

        var oldRating = new RatingObject
        {
            RatingId = 10,
            MediaId = 1,
            UserId = 999 // anderer User
        };

        var repoMock = new Mock<IRatingsRepository>();
        repoMock.Setup(r => r.GetRating(10)).Returns(oldRating);

        var service = new RatingsService(repoMock.Object);

        var result = service.ChangeRating(request, currentUser);

        Assert.That(result, Is.Null);

        repoMock.Verify(r => r.GetRating(10), Times.Once);
        repoMock.Verify(r => r.ChangeRating(It.IsAny<ChangeRating>()), Times.Never);
    }
    
    [Test]
public void DeleteRating_OwnerMatches_CallsGetRatingAndDeleteRating_ReturnsTrue()
{
    // Vorbereitung
    var request = new HttpRequest
    {
        Body = "{\"ratingId\": 10}"
    };
    var currentUser = new CurrentActiveUser { userId = 123 };

    var oldRating = new RatingObject
    {
        RatingId = 10,
        MediaId = 1,
        UserId = 123
    };

    var repoMock = new Mock<IRatingsRepository>();
    repoMock.Setup(r => r.GetRating(10)).Returns(oldRating);
    repoMock.Setup(r => r.DeleteRating(It.IsAny<ChooseRating>())).Returns(true);

    var service = new RatingsService(repoMock.Object);

    // Ausführen
    var result = service.DeleteRating(request, currentUser);

    // Überprüfen
    Assert.That(result, Is.True);

    repoMock.Verify(r => r.GetRating(10), Times.Once);

    repoMock.Verify(r => r.DeleteRating(It.Is<ChooseRating>(dto =>
        dto.RatingId == 10 &&
        dto.UserId == 123   // <-- wird im Service gesetzt
    )), Times.Once);
}

[Test]
public void DeleteRating_OwnerMismatch_ReturnsFalse_AndDoesNotCallDeleteRating()
{
    // Vorbereitung
    var request = new HttpRequest
    {
        Body = "{\"ratingId\": 10}"
    };
    var currentUser = new CurrentActiveUser { userId = 123 };

    var oldRating = new RatingObject
    {
        RatingId = 10,
        MediaId = 1,
        UserId = 999 // anderer User
    };

    var repoMock = new Mock<IRatingsRepository>();
    repoMock.Setup(r => r.GetRating(10)).Returns(oldRating);

    var service = new RatingsService(repoMock.Object);

    // Ausführen
    var result = service.DeleteRating(request, currentUser);

    // Überprüfen
    Assert.That(result, Is.False);

    repoMock.Verify(r => r.GetRating(10), Times.Once);
    repoMock.Verify(r => r.DeleteRating(It.IsAny<ChooseRating>()), Times.Never);
}

[Test]
public void DeleteRating_CurrentActiveUserIsNull_ThrowsUnauthorizedAccessException()
{
    // Vorbereitung
    var request = new HttpRequest
    {
        Body = "{\"ratingId\": 10}"
    };

    var repoMock = new Mock<IRatingsRepository>();
    var service = new RatingsService(repoMock.Object);

    // Ausführen + Überprüfen
    Assert.That(
        () => service.DeleteRating(request, null),
        Throws.TypeOf<UnauthorizedAccessException>()
            .With.Message.EqualTo("Unauthorized")
    );

    repoMock.Verify(r => r.GetRating(It.IsAny<int>()), Times.Never);
    repoMock.Verify(r => r.DeleteRating(It.IsAny<ChooseRating>()), Times.Never);
}
}