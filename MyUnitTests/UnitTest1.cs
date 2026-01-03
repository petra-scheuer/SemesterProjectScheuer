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
}