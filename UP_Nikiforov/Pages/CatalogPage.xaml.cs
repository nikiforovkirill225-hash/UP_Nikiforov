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
    public partial class CatalogPage : Page
    {
        private List<Books> _allBooks = new List<Books>();

        public CatalogPage()
        {
            InitializeComponent();
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            _allBooks = Core.Context.Books.Where(b => b.IsFrozen == false).ToList();

            var genres = Core.Context.Genres.ToList();
            genres.Insert(0, new Genres { ID = 0, Name = "Все жанры" });
            cbGenre.ItemsSource = genres;
            cbGenre.DisplayMemberPath = "Name";
            cbGenre.SelectedIndex = 0;

            cbSort.SelectedIndex = 0;

            UpdateBooks();
        }

        private void UpdateBooks()
        {
            var currentBooks = _allBooks.AsEnumerable();

            if (!string.IsNullOrWhiteSpace(tbSearch.Text))
            {
                string search = tbSearch.Text.ToLower();
                currentBooks = currentBooks.Where(b => b.Title.ToLower().Contains(search) ||
                                                       (b.Users != null && b.Users.DisplayName.ToLower().Contains(search)));
            }

            if (cbGenre.SelectedIndex > 0)
            {
                var selectedGenre = cbGenre.SelectedItem as Genres;
                if (selectedGenre != null)
                {
                    currentBooks = currentBooks.Where(b => b.Genres.Any(g => g.ID == selectedGenre.ID));
                }
            }

            if (cbSort.SelectedIndex == 1)
            {
                currentBooks = currentBooks.OrderBy(b => b.Title);
            }
            else if (cbSort.SelectedIndex == 2)
            {
                currentBooks = currentBooks.OrderByDescending(b => b.Title);
            }

            lvBooks.ItemsSource = currentBooks.ToList();

            if (Core.CurrentUser == null || Core.CurrentUser.RoleID != 3)
            {
                currentBooks = currentBooks.Where(b => b.IsFrozen == false).ToList();
            }
        }

        private void tbSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            UpdateBooks();
        }

        private void cbSort_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateBooks();
        }

        private void cbGenre_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateBooks();
        }
        private void lvBooks_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            var selectedBook = lvBooks.SelectedItem as Books;
            if (selectedBook != null)
            {
                NavigationService.Navigate(new BookDetailsPage(selectedBook));
            }
        }
        private void ChangeBookStatusFromCatalog(object sender, int statusId)
        {
            if (Core.CurrentUser == null)
            {
                MessageBox.Show("Авторизуйтесь для добавления книг в списки.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            var menuItem = sender as MenuItem;
            if (menuItem == null) return;

            var contextMenu = menuItem.Parent as ContextMenu;
            if (contextMenu == null) return;

            var border = contextMenu.PlacementTarget as Border;
            if (border == null) return;

            var selectedBook = border.DataContext as Books;
            if (selectedBook == null) return;

            var existingRecord = Core.Context.ReadingLists.FirstOrDefault(rl => rl.UserID == Core.CurrentUser.ID && rl.BookID == selectedBook.ID);

            if (existingRecord != null)
            {
                existingRecord.StatusID = statusId;
            }
            else
            {
                ReadingLists newRecord = new ReadingLists
                {
                    UserID = Core.CurrentUser.ID,
                    BookID = selectedBook.ID,
                    StatusID = statusId
                };
                Core.Context.ReadingLists.Add(newRecord);
            }

            Core.Context.SaveChanges();
            MessageBox.Show("Статус книги успешно обновлен!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void MenuWantToRead_Click(object sender, RoutedEventArgs e)
        {
            ChangeBookStatusFromCatalog(sender, 1);
        }

        private void MenuReadingNow_Click(object sender, RoutedEventArgs e)
        {
            ChangeBookStatusFromCatalog(sender, 2);
        }

        private void MenuRead_Click(object sender, RoutedEventArgs e)
        {
            ChangeBookStatusFromCatalog(sender, 3);
        }
    }
}
