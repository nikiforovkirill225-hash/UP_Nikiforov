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

namespace UP_Nikiforov
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new LoginPage());
        }

        public void ShowSidebarAndNavigate()
        {
            if (Core.CurrentUser == null) return;

            SidebarBorder.Visibility = Visibility.Visible;

            if (Core.CurrentUser.RoleID == 3)
            {
                btnAdmin.Visibility = Visibility.Visible;
            }
            else if (Core.CurrentUser.RoleID == 2)
            {
                btnAuthor.Visibility = Visibility.Visible;
            }

            if (Core.CurrentUser.IsFrozen)
            {
                btnFreezeWarning.Visibility = Visibility.Visible;
            }

            MainFrame.Navigate(new CatalogPage());
        }

        private void btnCatalog_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new CatalogPage());
        }

        private void btnLists_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new ListsPage());
        }

        private void btnAuthor_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new AuthorPage());
        }

        private void btnAdmin_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new AdminPage());
        }

        private void btnFreezeWarning_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new FreezeWarningPage());
        }

        private void btnProfile_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new ProfilePage());
        }
    }
}
