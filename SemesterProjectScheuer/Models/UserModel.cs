namespace SemesterProjectScheuer.Models;

public class UserModel
{
    public string username { get; set; }
    

}
public class RegisterUser
{    
    public string username { get; set; }
    public string password { get; set; }
}

public class LoginUser
{    
    public string username { get; set; }
    public string password { get; set; }
} 

public class CurrentActiveUser
{
    public int userId;
    public string username;
    public string token;
    public DateTime createdAt;
} 