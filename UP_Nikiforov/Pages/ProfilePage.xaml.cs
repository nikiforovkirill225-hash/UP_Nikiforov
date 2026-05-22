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
    public partial class ProfilePage : Page
    {
        public ProfilePage()
        {
            InitializeComponent();
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            if (Core.CurrentUser == null) return;

            tblLogin.Text = Core.CurrentUser.Login;
            tblDisplayName.Text = Core.CurrentUser.DisplayName;
            tblEmail.Text = Core.CurrentUser.Email;

            var role = Core.Context.Roles.FirstOrDefault(r => r.ID == Core.CurrentUser.RoleID);
            tblRole.Text = role != null ? role.Name : "Пользователь";

            int booksInLists = Core.Context.ReadingLists.Count(rl => rl.UserID == Core.CurrentUser.ID);
            tblBooksInListsCount.Text = booksInLists.ToString();

            int userReviews = Core.Context.Reviews.Count(r => r.UserID == Core.CurrentUser.ID);
            tblUserReviewsCount.Text = userReviews.ToString();

            if (Core.CurrentUser.RoleID == 2 || Core.CurrentUser.RoleID == 3)
            {
                borderAuthorStats.Visibility = Visibility.Visible;
                int authorBooks = Core.Context.Books.Count(b => b.AuthorID == Core.CurrentUser.ID);
                tblAuthorBooksCount.Text = authorBooks.ToString();
                btnRequestAuthor.Visibility = Visibility.Collapsed;
            }
            else
            {
                borderAuthorStats.Visibility = Visibility.Collapsed;
                btnRequestAuthor.Visibility = Visibility.Visible;
            }
        }

        private void btnLogout_Click(object sender, RoutedEventArgs e)
        {
            Core.CurrentUser = null;
            NavigationService.Navigate(new LoginPage());
        }

        private void btnRequestAuthor_Click(object sender, RoutedEventArgs e)
        {
            var result = MessageBox.Show("Вы хотите подать заявку на получение роли автора? После подтверждения вы сможете публиковать свои книги.",
                "Заявка на роль автора", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    MessageBox.Show("Ваша заявка на роль автора отправлена администратору. Ожидайте подтверждения.",
                        "Заявка отправлена", MessageBoxButton.OK, MessageBoxImage.Information);

                    btnRequestAuthor.Visibility = Visibility.Collapsed;
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка при отправке заявки: {ex.Message}", "Ошибка",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
    }
}