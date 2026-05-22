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
    public partial class AdminPage : Page
    {
        public AdminPage()
        {
            InitializeComponent();
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            UpdateGrid();
        }

        private void UpdateGrid()
        {
            try
            {
                Core.Context.ChangeTracker.Entries().ToList().ForEach(p => p.Reload());

                dgComplaints.ItemsSource = Core.Context.Complaints.ToList();

                dgFrozenBooks.ItemsSource = Core.Context.Books.Where(b => b.IsFrozen == true).ToList();

                dgFrozenUsers.ItemsSource = Core.Context.Users.Where(u => u.IsFrozen == true).ToList();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при обновлении данных модерации: {ex.Message}", "Ошибка БД", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnReject_Click(object sender, RoutedEventArgs e)
        {
            var selectedComplaint = dgComplaints.SelectedItem as Complaints;
            if (selectedComplaint == null)
            {
                MessageBox.Show("Пожалуйста, выберите жалобу из списка.", "Предупреждение", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                Core.Context.Complaints.Remove(selectedComplaint);
                Core.Context.SaveChanges();

                UpdateGrid();
                MessageBox.Show("Жалоба успешно отклонена и удалена из списка.", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Не удалось отклонить жалобу: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnFreezeBook_Click(object sender, RoutedEventArgs e)
        {
            var selectedComplaint = dgComplaints.SelectedItem as Complaints;
            if (selectedComplaint == null)
            {
                MessageBox.Show("Пожалуйста, выберите жалобу из списка.", "Предупреждение", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (selectedComplaint.TargetBookID == null)
            {
                MessageBox.Show("Эта жалоба направлена не на книгу.", "Предупреждение", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                int bookId = Convert.ToInt32(selectedComplaint.TargetBookID);
                var book = Core.Context.Books.FirstOrDefault(b => b.ID == bookId);

                if (book != null)
                {
                    book.IsFrozen = true;
                    Core.Context.Complaints.Remove(selectedComplaint);
                    Core.Context.SaveChanges();

                    UpdateGrid();
                    MessageBox.Show($"Книга '{book.Title}' успешно заморожена. Вы можете увидеть её на соседней вкладке.", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при заморозке книги: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnFreezeUser_Click(object sender, RoutedEventArgs e)
        {
            var selectedComplaint = dgComplaints.SelectedItem as Complaints;
            if (selectedComplaint == null)
            {
                MessageBox.Show("Пожалуйста, выберите жалобу из списка.", "Предупреждение", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                Users userToFreeze = null;

                if (selectedComplaint.TargetReviewID != null)
                {
                    int reviewId = Convert.ToInt32(selectedComplaint.TargetReviewID);
                    var review = Core.Context.Reviews.FirstOrDefault(r => r.ID == reviewId);
                    if (review != null)
                    {
                        int userId = Convert.ToInt32(review.UserID);
                        userToFreeze = Core.Context.Users.FirstOrDefault(u => u.ID == userId);
                    }
                }
                else if (selectedComplaint.TargetBookID != null)
                {
                    int bookId = Convert.ToInt32(selectedComplaint.TargetBookID);
                    var book = Core.Context.Books.FirstOrDefault(b => b.ID == bookId);
                    if (book != null)
                    {
                        int authorId = Convert.ToInt32(book.AuthorID);
                        userToFreeze = Core.Context.Users.FirstOrDefault(u => u.ID == authorId);
                    }
                }

                if (userToFreeze != null)
                {
                    userToFreeze.IsFrozen = true;
                    Core.Context.Complaints.Remove(selectedComplaint);
                    Core.Context.SaveChanges();

                    UpdateGrid();
                    MessageBox.Show($"Пользователь {userToFreeze.Login} успешно заморожен. История находится во вкладке пользователей.", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при заморозке пользователя: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnUnfreezeBook_Click(object sender, RoutedEventArgs e)
        {
            var selectedBook = dgFrozenBooks.SelectedItem as Books;
            if (selectedBook == null)
            {
                MessageBox.Show("Выберите замороженную книгу для разблокировки.", "Предупреждение", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                var book = Core.Context.Books.FirstOrDefault(b => b.ID == selectedBook.ID);
                if (book != null)
                {
                    book.IsFrozen = false;
                    Core.Context.SaveChanges();

                    UpdateGrid();
                    MessageBox.Show($"Книга '{book.Title}' успешно разморожена и снова доступна в каталоге!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Не удалось разморозить книгу: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnUnfreezeUser_Click(object sender, RoutedEventArgs e)
        {
            var selectedUser = dgFrozenUsers.SelectedItem as Users;
            if (selectedUser == null)
            {
                MessageBox.Show("Выберите пользователя для разблокировки.", "Предупреждение", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                var user = Core.Context.Users.FirstOrDefault(u => u.ID == selectedUser.ID);
                if (user != null)
                {
                    user.IsFrozen = false;
                    Core.Context.SaveChanges();

                    UpdateGrid();
                    MessageBox.Show($"Пользователь {user.Login} успешно разблокирован. Все ограничения сняты.", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Не удалось разблокировать пользователя: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
