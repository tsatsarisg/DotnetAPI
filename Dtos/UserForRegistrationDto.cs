namespace DotnetAPI.DTOS;

public class UserForRegistrationDto
{
    string Email { get; set; }
    string Password { get; set; }
    string PasswordConfirm { get; set; }
    
    public UserForRegistrationDto()
    {
        PasswordConfirm ??= string.Empty;
        Password ??= string.Empty;
        Email ??= string.Empty;
    }
}