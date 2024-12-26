using System.IO;
using System.Text.Json;
using BoardGameFrontend.Models;

namespace BoardGameFrontend.Helpers
{
    public static class UserHelpers
    {
        public static UserData? LoadUserData()
        {
            if (File.Exists("user_data.json"))
            {
                var json = File.ReadAllText("user_data.json");
                return JsonSerializer.Deserialize<UserData>(json);
            }
            return null;
        }
    }


}