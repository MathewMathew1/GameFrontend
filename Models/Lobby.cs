using System.Text.Json.Serialization;
using System;
using System.Collections.Generic;

namespace BoardGameFrontend.Models
{
    public class LobbyResponse
    {
        public required string lobbyId { get; set; }
    }

    public class LobbyViewModel
    {
        [JsonPropertyName("id")]
        public required string Id { get; set; }
        [JsonPropertyName("hostId")]
        public Guid HostId { get; set; }
        [JsonPropertyName("lobbyName")]
        public required string LobbyName { get; set; }
        [JsonPropertyName("players")]
        public required List<PlayerViewModel> Players { get; set; } = new List<PlayerViewModel>();

    }

     public class LobbyViewModelWithHostName
    {
        public required string Id { get; set; }
        public required Guid HostId { get; set; }
        public required string LobbyName { get; set; }
        public required string HostName { get; set; }
        public required List<PlayerViewModel> Players { get; set; } = new List<PlayerViewModel>();

    }

    public class LobbyDetailedViewModel
    {
        [JsonPropertyName("id")]
        public required string Id { get; set; }
        [JsonPropertyName("hostId")]
        public Guid HostId { get; set; }
        [JsonPropertyName("lobbyName")]
        public required string LobbyName { get; set; }
        [JsonPropertyName("players")]
        public required List<PlayerViewModel> Players { get; set; } = new List<PlayerViewModel>();
    }

    public class LobbyManagerInfo
    {
        [JsonPropertyName("lobby")]
        public required LobbyDetailedViewModel Lobby { get; set; }
        [JsonPropertyName("startGameModel")]
        public required StartGameModel StartGameModel { get; set; }
    }

    

}