using System;
using System.Text.Json.Serialization;

namespace BoardGameFrontend.Models
{
    public class UserData
    {
        public required string Token { get; set; }
        public required string Username { get; set; }
        public required Guid Id { get; set; }
    }

    public class User
    {
        [JsonPropertyName("username")]
        public required string Username { get; set; }
        [JsonPropertyName("id")]
        public required Guid Id { get; set; }
    }

    

    public class UserDataFromLogin
    {
        [JsonPropertyName("token")]
        public required string Token { get; set; }

        [JsonPropertyName("user")]
        public required User User { get; set; }
    }
}

