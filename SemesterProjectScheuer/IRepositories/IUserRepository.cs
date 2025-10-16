using SemesterProjectScheuer.Models;

namespace SemesterProjectScheuer.IRepositories;


public interface IUserRepository
{
    bool CreateUser(RegisterUser newUser);
    //bool AuthUser(string username, string password);
    //bool UpdateToken(string username, string token);
    //bool ChangeUsername(string oldUsername, string newUsername);
    //bool AuthByUsernameAndToken(string username, string token);

}