using System.Windows;
using System.Windows.Input;
using Ycheb_practice.Service;

namespace Ycheb_practice.Windows
{
    public partial class LoginWindow : Window
    {
        public LoginWindow()
        {
            InitializeComponent();
            pbPassword.Password = "admin123";
        }

        private void BtnLogin_Click(object sender, RoutedEventArgs e) => DoLogin();

        private void Input_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter) DoLogin();
        }

        private void DoLogin()
        {
            ErrorBorder.Visibility = Visibility.Collapsed;

            string login    = tbLogin.Text.Trim();
            string password = pbPassword.Password;

            if (string.IsNullOrEmpty(login) || string.IsNullOrEmpty(password))
            {
                ShowError("Введите логин и пароль.");
                return;
            }

            // Авторизация через слой доступа к данным
            bool ok = ManagerService.DataService.Login(login, password);

            if (ok)
            {
                var main = new MainWindow();
                main.Show();
                this.Close();
            }
            else
            {
                ShowError("Неверный логин или пароль.");
            }
        }

        private void ShowError(string msg)
        {
            tbError.Text = msg;
            ErrorBorder.Visibility = Visibility.Visible;
        }
    }
}
