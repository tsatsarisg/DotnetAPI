namespace DotnetAPI.DTOS;

public class UserForRegistrationDto
{
    public string Email { get; set; }
    public string Password { get; set; }
    public string PasswordConfirm { get; set; }
    
    public string FirstName {get; set;}
    public string LastName {get; set;}
    public string Gender {get; set;}
    
    public UserForRegistrationDto()
    {
        PasswordConfirm ??= string.Empty;
        Password ??= string.Empty;
        Email ??= string.Empty;
        FirstName ??= string.Empty;
        LastName ??= string.Empty;
        Gender ??= string.Empty;
    }
}