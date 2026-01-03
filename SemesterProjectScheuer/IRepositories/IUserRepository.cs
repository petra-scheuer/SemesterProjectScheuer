using SemesterProjectScheuer.Models;

namespace SemesterProjectScheuer.IRepositories;


public interface IUserRepository
{
    bool CreateUser(RegisterUser newUser);
    CurrentActiveUser GetUserByToken(string token);

    string MyHash(string text);

    UserModel? AuthenticateUser(string username, string password);
    bool SaveToken(string username, string token);
    
}