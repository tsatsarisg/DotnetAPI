namespace DotnetAPI.DTOS;

public partial class UserDto
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Email { get; set; }
    public string Gender { get; set; }
    public bool Active { get; set; }

    public UserDto()
    {
        FirstName ??= string.Empty;
        LastName ??= string.Empty;
        Gender ??= string.Empty;
        Email ??= string.Empty;
    }
}