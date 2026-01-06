using NUnit.Framework;
using Moq;
using SemesterProjectScheuer;
using SemesterProjectScheuer.Models;
using SemesterProjectScheuer.IRepositories;
using SemesterProjectScheuer.Services;

[TestFixture]
public class UserServiceTests
{
    [Test]
    public void RegisterUser_ValidJson_CallsCreateUserAndReturnsTrue()
    {
        // Vorbereitung
        var request = new HttpRequest
        {
            Body = "{\"username\":\"petra\",\"password\":\"1234\"}"
        };

        var repoMock = new Mock<IUserRepository>();
        repoMock
            .Setup(r => r.CreateUser(It.IsAny<RegisterUser>()))
            .Returns(true);

        var service = new UserService(repoMock.Object); // Mock übergeben 

        // Ausführen
        var result = service.RegisterUser(request); 

        // Überprüfen
        Assert.That(result, Is.True);
        repoMock.Verify(r => r.CreateUser(It.IsAny<RegisterUser>()), Times.Once);
    }
    [Test]
    public void RegisterUser_InvalidJson_CallsCreateUserAndReturnsFalse()
    {
        // Vorbereitung
        var request = new HttpRequest
        {
            Body = "{\"username\":\"petra\"}"
        };

        var repoMock = new Mock<IUserRepository>();
        repoMock
            .Setup(r => r.CreateUser(It.IsAny<RegisterUser>()))
            .Returns(false);

        var service = new UserService(repoMock.Object); // Mock übergeben

        // Ausführen
        var result = service.RegisterUser(request); 

        // Überprüfen
        Assert.That(result, Is.False);
        repoMock.Verify(r => r.CreateUser(It.IsAny<RegisterUser>()), Times.Once);
        
    }
    [Test]
    public void LoginUser_ValidCredentials_ReturnsToken_AndSavesIt()
    {
        // Arrange
        var request = new HttpRequest
        {
            Body = @"{
            ""username"": ""petra"",
            ""password"": ""1234""
        }"
        };

        var user = new UserModel
        {
            username = "petra"
        };

        var repoMock = new Mock<IUserRepository>();

        repoMock
            .Setup(r => r.AuthenticateUser("petra", "1234"))
            .Returns(user);

        repoMock
            .Setup(r => r.SaveToken("petra", It.IsAny<string>()))
            .Returns(true);

        var service = new UserService(repoMock.Object);

        // Act
        var token = service.LoginUser(request);

        // Assert
        Assert.That(token, Does.Contain("petra-mrpToken"));

        repoMock.Verify(r => r.AuthenticateUser("petra", "1234"), Times.Once);
        repoMock.Verify(r => r.SaveToken("petra", It.IsAny<string>()), Times.Once);
    }
    [Test]
    public void LoginUser_InvalidCredentials_ThrowsException()
    {
        var request = new HttpRequest
        {
            Body = @"{
            ""username"": ""petra"",
            ""password"": ""wrong""
        }"
        };

        var repoMock = new Mock<IUserRepository>();

        repoMock
            .Setup(r => r.AuthenticateUser("petra", "wrong"))
            .Returns((UserModel)null);

        var service = new UserService(repoMock.Object);

        Assert.That(
            () => service.LoginUser(request),
            Throws.Exception.With.Message.EqualTo("Authentifizierung fehlgeschlagen")
        );

        repoMock.Verify(r => r.SaveToken(It.IsAny<string>(), It.IsAny<string>()), Times.Never);
    }
    [Test]
    public void ValidateTokenAsync_ValidToken_ReturnsCurrentActiveUser()
    {
        var token = "petra-mrpToken-123";

        var expectedUser = new CurrentActiveUser
        {
            userId = 1,
            username = "petra"
        };

        var repoMock = new Mock<IUserRepository>();

        repoMock
            .Setup(r => r.GetUserByToken(token))
            .Returns(expectedUser);

        var service = new UserService(repoMock.Object);

        var result = service.ValidateTokenAsync(token);

        Assert.That(result, Is.Not.Null);
        Assert.That(result.username, Is.EqualTo("petra"));

        repoMock.Verify(r => r.GetUserByToken(token), Times.Once);
    }
    [Test]
    public void ValidateTokenAsync_InvalidToken_ReturnsNull()
    {
        var token = "invalid-token";

        var repoMock = new Mock<IUserRepository>();

        repoMock
            .Setup(r => r.GetUserByToken(token))
            .Returns((CurrentActiveUser)null);

        var service = new UserService(repoMock.Object);

        var result = service.ValidateTokenAsync(token);

        Assert.That(result, Is.Null);
    }
}