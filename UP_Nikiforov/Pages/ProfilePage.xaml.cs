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
            if (role != null)
            {
                tblRole.Text = role.Name;
            }

            int listsCount = Core.Context.ReadingLists.Count(rl => rl.UserID == Core.CurrentUser.ID);
            tblBooksInListsCount.Text = listsCount.ToString();

            if (Core.CurrentUser.RoleID == 2)
            {
                borderAuthorStats.Visibility = Visibility.Visible;
                int authorBooksCount = Core.Context.Books.Count(b => b.AuthorID == Core.CurrentUser.ID);
                tblAuthorBooksCount.Text = authorBooksCount.ToString();
            }
        }

        private void btnLogout_Click(object sender, RoutedEventArgs e)
        {
            Core.CurrentUser = null;

            var mainWindow = Application.Current.MainWindow as MainWindow;
            if (mainWindow != null)
            {
                mainWindow.SidebarBorder.Visibility = Visibility.Collapsed;
                mainWindow.btnAdmin.Visibility = Visibility.Collapsed;
                mainWindow.btnAuthor.Visibility = Visibility.Collapsed;
                mainWindow.btnFreezeWarning.Visibility = Visibility.Collapsed;
                mainWindow.MainFrame.Navigate(new LoginPage());
            }
        }
    }
}
