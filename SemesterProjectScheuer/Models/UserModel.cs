namespace SemesterProjectScheuer.Models;

public class UserModel
{
    public string username { get; set; }
    

}
public class RegisterUser
{    
    public string username { get; }
    public string password { get; }
}

public class LoginUser
{    
    public string username { get; }
    public string password { get; }
} 