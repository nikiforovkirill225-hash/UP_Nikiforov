using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace UP_Nikiforov.Pages
{
    public partial class RegisterPage : Page
    {
        public RegisterPage()
        {
            InitializeComponent();
        }

        private void btnBack_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new LoginPage());
        }

        private void btnRegister_Click(object sender, RoutedEventArgs e)
        {
            string login = tbLogin.Text.Trim();
            string password = pbPassword.Password.Trim();
            string email = tbEmail.Text.Trim();
            string displayName = tbDisplayName.Text.Trim();

            if (string.IsNullOrWhiteSpace(login))
            {
                tbError.Text = "Введите логин!";
                tbError.Visibility = Visibility.Visible;
                return;
            }

            if (string.IsNullOrWhiteSpace(password))
            {
                tbError.Text = "Введите пароль!";
                tbError.Visibility = Visibility.Visible;
                return;
            }

            if (string.IsNullOrWhiteSpace(email))
            {
                tbError.Text = "Введите email!";
                tbError.Visibility = Visibility.Visible;
                return;
            }

            // Проверка на существующего пользователя
            var existingUser = Core.Context.Users.FirstOrDefault(u => u.Login == login || u.Email == email);
            if (existingUser != null)
            {
                tbError.Text = "Пользователь с таким логином или email уже существует!";
                tbError.Visibility = Visibility.Visible;
                return;
            }

            // Создание нового пользователя (без CreatedAt, если его нет в БД)
            Users newUser = new Users
            {
                Login = login,
                Password = password,
                Email = email,
                DisplayName = string.IsNullOrWhiteSpace(displayName) ? login : displayName,
                RoleID = 1,  // Обычный пользователь
                IsFrozen = false
                // CreatedAt удалён, если поле отсутствует в таблице Users
            };

            Core.Context.Users.Add(newUser);
            Core.Context.SaveChanges();

            MessageBox.Show("Регистрация успешно завершена! Теперь вы можете войти.", "Успех",
                MessageBoxButton.OK, MessageBoxImage.Information);

            NavigationService.Navigate(new LoginPage());
        }

        private void btnGoToLogin_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new LoginPage());
        }
    }
}