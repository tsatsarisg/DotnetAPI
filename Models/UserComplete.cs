namespace DotnetAPI.Models;

public partial class UserComplete
{
    public int UserId { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Email { get; set; }
    public string Gender { get; set; }
    public bool Active { get; set; }
    public string JobTitle { get; set; }
    public string Department { get; set; }
    public string Salary { get; set; }
    public string AvgSalary { get; set; }
    public UserComplete()
    {
        FirstName ??= string.Empty;
        LastName ??= string.Empty;
        Gender ??= string.Empty;
        Email ??= string.Empty;
        JobTitle ??= string.Empty;
        Department ??= string.Empty;
        Salary ??= string.Empty;
        AvgSalary ??= string.Empty;
    }
}