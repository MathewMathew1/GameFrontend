using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Windows;
using BoardGameFrontend.Models;
using BoardGameFrontend.Windows;
using System.IO;
using BoardGameFrontend.Helpers;
using System.Windows.Input;

namespace BoardGameFrontend
{
    public partial class MainWindow : FullScreenWindow
    {
        private readonly HttpClient _httpClient = new HttpClient();
        private StringStorage _stringStorage = new StringStorage();
        private readonly string LOGIN_STRING = "login";

        public MainWindow()
        {
            InitializeComponent();
        }

        private void InputTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                Login_Click(sender, e);

                e.Handled = true;
            }
        }

        private async void Login_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if(_stringStorage.ContainsString(LOGIN_STRING)) return;
                _stringStorage.AddString(LOGIN_STRING);
                var username = UsernameBox.Text;
                var password = PasswordBox.Password;

                if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
                {
                    ErrorMessage.Text = "Please enter both username and password.";
                    ErrorMessage.Visibility = Visibility.Visible;
                    _stringStorage.RemoveString(LOGIN_STRING);
                    return;
                }

                var content = new StringContent(JsonSerializer.Serialize(new { Username = username, Password = password }), Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync($"{UrlHelper.GetBaseUrl()}auth/login", content);
                _stringStorage.RemoveString(LOGIN_STRING);
                if (response.IsSuccessStatusCode)
                {
                    var responseData = await response.Content.ReadAsStringAsync();
                    var a = JsonSerializer.Deserialize<UserDataFromLogin>(responseData)!;

                    SaveUserData(a.Token, a.User.Username, a.User.Id);

                    // Navigate to the main game window
                    var mainViewWindow = new MainViewWindow();
                    mainViewWindow.Show();
                    Close();
                }
                else
                {
                    ErrorMessage.Text = "Invalid username or password.";
                    ErrorMessage.Visibility = Visibility.Visible;
                }
            }catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
                MessageBox.Show($"An error occurred: {ex.Message}");
            }


        }

        public void SaveUserData(string token, string username, Guid id)
        {
            var userData = new UserData { Token = token, Username = username, Id = id };
            var json = JsonSerializer.Serialize(userData);
            File.WriteAllText("user_data.json", json);
        }

        private void OpenSignUp_Click(object sender, RoutedEventArgs e)
        {
            var signUpWindow = new SignUpWindow();
            signUpWindow.Show();
            Close();
        }
    }
}