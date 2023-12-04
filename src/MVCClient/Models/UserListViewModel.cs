using Newtonsoft.Json;

namespace MVCClient.Models;

public class UserListViewModel
{
    [JsonProperty("id")] public Guid Id { get; set; }

    [JsonProperty("username")] public required string Username { get; set; }

    [JsonProperty("enabled")] public bool Enabled { get; set; }

    [JsonProperty("firstName")] public string? FirstName { get; set; }
    
    [JsonProperty("lastName")] public string? LastName { get; set; }

    [JsonProperty("email")] public string? Email { get; set; }
}