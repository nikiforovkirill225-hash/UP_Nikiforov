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
    public partial class AuthorPage : Page
    {
        public AuthorPage()
        {
            InitializeComponent();
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            UpdateGrid();
        }

        public void UpdateGrid()
        {
            if (Core.CurrentUser == null) return;

            var authorBooks = Core.Context.Books.Where(b => b.AuthorID == Core.CurrentUser.ID).ToList();
            dgAuthorBooks.ItemsSource = authorBooks;

            if (Core.CurrentUser.IsFrozen)
            {
                btnAddBook.IsEnabled = false;
                btnAddBook.ToolTip = "Вы не можете добавлять книги, так как ваш аккаунт заморожен.";
            }
        }

        private void btnAddBook_Click(object sender, RoutedEventArgs e)
        {
            AddEditBookWindow addWin = new AddEditBookWindow(null);
            if (addWin.ShowDialog() == true)
            {
                UpdateGrid();
            }
        }

        private void dgAuthorBooks_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            var selectedBook = dgAuthorBooks.SelectedItem as Books;
            if (selectedBook != null)
            {
                if (Core.CurrentUser.IsFrozen)
                {
                    MessageBox.Show("Ваш аккаунт заморожен. Редактирование книг недоступно.", "Предупреждение", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                AddEditBookWindow editWin = new AddEditBookWindow(selectedBook);
                if (editWin.ShowDialog() == true)
                {
                    UpdateGrid();
                }
            }
        }
    }
}
