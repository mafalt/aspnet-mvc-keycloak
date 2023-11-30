namespace MVCClient.Models;

public class UserInformationViewModel
{
    public Dictionary<string, string> Claims { get; set; } = new();
    public required string UserName { get; set; }
}
