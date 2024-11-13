namespace DotnetAPI.DTOS;

public class PostToAddDto
{
    public string PostTitle {get; set;}
    public string PostContent {get; set;}

    public PostToAddDto()
    {
        PostTitle ??= "";
        PostContent ??= "";
    }
}