using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Windows;
using BoardGameFrontend.Windows;
using BoardGameFrontend.Helpers;
using System.Windows.Input;

namespace BoardGameFrontend
{
    public partial class SignUpWindow : FullScreenWindow
    {
        private readonly HttpClient _httpClient = new HttpClient();
        private readonly string SIGN_UP_STRING = "string";
        private StringStorage _stringStorage = new StringStorage();

        public SignUpWindow()
        {
            InitializeComponent();
        }

        private void InputTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                SignUp_Click(sender, e);

                e.Handled = true;
            }
        }

        private async void SignUp_Click(object sender, RoutedEventArgs e)
        {
            if(_stringStorage.ContainsString(SIGN_UP_STRING)) return;
            _stringStorage.AddString(SIGN_UP_STRING);

            var username = UsernameBox.Text;
            var password = PasswordBox.Password;
            var confirmPassword = ConfirmPasswordBox.Password;

            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password) || string.IsNullOrEmpty(confirmPassword))
            {
                ErrorMessage.Text = "All fields are required.";
                ErrorMessage.Visibility = Visibility.Visible;
                _stringStorage.RemoveString(SIGN_UP_STRING);
                return;
            }

            if (password != confirmPassword)
            {
                ErrorMessage.Text = "Passwords do not match.";
                ErrorMessage.Visibility = Visibility.Visible;
                _stringStorage.RemoveString(SIGN_UP_STRING);
                return;
            }

            var content = new StringContent(JsonSerializer.Serialize(new { Username = username, Password = password }), Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync($"{UrlHelper.GetBaseUrl()}auth/signup", content);
            _stringStorage.RemoveString(SIGN_UP_STRING);
            if (response.IsSuccessStatusCode)
            {
                MessageBox.Show("Sign-up successful! Please log in.");
                var loginWindow = new MainWindow();
                loginWindow.Show();
                Close();
            }
            else
            {
                // Read the response content
                var errorResponse = await response.Content.ReadAsStringAsync();

                // Try to parse the error message from the response
                try
                {
                    var errorObject = JsonSerializer.Deserialize<Dictionary<string, string>>(errorResponse);

                    // Check if the error message exists and display it
                    if (errorObject != null && errorObject.ContainsKey("error"))
                    {
                        ErrorMessage.Text = errorObject["error"]; // Display the specific error message
                    }
                    else
                    {
                        ErrorMessage.Text = "Sign-up failed. Please try again."; // Fallback message
                    }
                }
                catch (Exception ex)
                {

                    ErrorMessage.Text = "An unexpected error occurred.";
                }

                ErrorMessage.Visibility = Visibility.Visible;
            }
        }

        private void OpenLogin_Click(object sender, RoutedEventArgs e)
        {
            var loginWindow = new MainWindow();
            loginWindow.Show();
            Close();
        }
    }
}