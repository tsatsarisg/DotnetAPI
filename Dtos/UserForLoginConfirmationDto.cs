namespace DotnetAPI.DTOS;

public class UserForLoginConfirmationDto
{
    public byte[] PasswordHash {get; set;}
    public byte[] PasswordSalt {get; set;}
    UserForLoginConfirmationDto()
    {
        PasswordHash ??= new byte[0];
        PasswordSalt ??= new byte[0];
    }
}
