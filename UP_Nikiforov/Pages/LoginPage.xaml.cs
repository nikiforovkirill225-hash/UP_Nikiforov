using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace UP_Nikiforov.Pages
{
    public partial class LoginPage : Page
    {
        public LoginPage()
        {
            InitializeComponent();
        }

        private void btnLogin_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(tbLogin.Text) || string.IsNullOrWhiteSpace(pbPassword.Password))
            {
                MessageBox.Show("Пожалуйста, заполните все поля.", "Предупреждение", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var user = Core.Context.Users.FirstOrDefault(u => u.Login == tbLogin.Text && u.Password == pbPassword.Password);

            if (user != null)
            {
                Core.CurrentUser = user;

                if (user.IsFrozen)
                {
                    MessageBox.Show("Внимание! Ваш аккаунт заморожен.", "Аккаунт заморожен", MessageBoxButton.OK, MessageBoxImage.Warning);
                }

                var mainWindow = Application.Current.MainWindow as MainWindow;
                if (mainWindow != null)
                {
                    mainWindow.ShowSidebarAndNavigate();
                }
            }
            else
            {
                MessageBox.Show("Неверный логин или пароль!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnGoToRegister_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new RegisterPage());
        }
    }
}
