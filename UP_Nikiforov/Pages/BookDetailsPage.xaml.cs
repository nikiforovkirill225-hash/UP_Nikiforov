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
    public partial class BookDetailsPage : Page
    {
        private Books _currentBook;

        public BookDetailsPage(Books selectedBook)
        {
            InitializeComponent();
            _currentBook = selectedBook;
            DataContext = _currentBook;
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            if (_currentBook.Users != null)
            {
                tblAuthor.Text = "Автор: " + _currentBook.Users.DisplayName;
            }

            if (Core.CurrentUser != null && Core.CurrentUser.IsFrozen)
            {
                borderAddReview.Visibility = Visibility.Collapsed;
            }

            UpdateReviewsAndRating();

            var statuses = Core.Context.ReadingListStatuses.ToList();
            statuses.Insert(0, new ReadingListStatuses { ID = 0, Name = "Не выбрано" });
            cbBookStatus.ItemsSource = statuses;
            cbBookStatus.DisplayMemberPath = "Name";
            cbBookStatus.SelectedValuePath = "ID";

            if (Core.CurrentUser != null)
            {
                var existingRecord = Core.Context.ReadingLists.FirstOrDefault(rl => rl.UserID == Core.CurrentUser.ID && rl.BookID == _currentBook.ID);
                if (existingRecord != null)
                {
                    cbBookStatus.SelectedValue = existingRecord.StatusID;
                }
                else
                {
                    cbBookStatus.SelectedIndex = 0;
                }
            }
            if (Core.CurrentUser != null && Core.CurrentUser.RoleID == 3)
            {
                btnAdminFreezeBook.Visibility = Visibility.Visible;
            }
        }

        private void UpdateReviewsAndRating()
        {
            var reviews = Core.Context.Reviews.Where(r => r.BookID == _currentBook.ID).ToList();
            lvReviews.ItemsSource = reviews;

            if (reviews.Count > 0)
            {
                double avg = reviews.Average(r => r.Rating);
                tblRating.Text = $"★ Оценка: {avg:F1} из 10 (отзывов: {reviews.Count})";
            }
            else
            {
                tblRating.Text = "★ Нет оценок";
            }
        }

        private void btnSendReview_Click(object sender, RoutedEventArgs e)
        {
            if (Core.CurrentUser == null)
            {
                MessageBox.Show("Необходимо авторизоваться.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (string.IsNullOrWhiteSpace(tbReviewText.Text))
            {
                MessageBox.Show("Пожалуйста, введите текст отзыва.", "Предупреждение", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            int rating = Convert.ToInt32((cbRating.SelectedItem as ComboBoxItem).Content);

            Reviews newReview = new Reviews
            {
                BookID = _currentBook.ID,
                UserID = Core.CurrentUser.ID,
                Text = tbReviewText.Text,
                Rating = rating,
                CreatedAt = DateTime.Now
            };

            Core.Context.Reviews.Add(newReview);
            Core.Context.SaveChanges();

            tbReviewText.Text = string.Empty;
            UpdateReviewsAndRating();

            MessageBox.Show("Отзыв успешно добавлен!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void btnRead_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show(_currentBook.TextContent, _currentBook.Title, MessageBoxButton.OK, MessageBoxImage.None);
        }

        private void btnBack_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new CatalogPage());
        }
        private void cbBookStatus_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (Core.CurrentUser == null || cbBookStatus.SelectedValue == null) return;

            int selectedStatusId = Convert.ToInt32(cbBookStatus.SelectedValue);

            var existingRecord = Core.Context.ReadingLists.FirstOrDefault(rl => rl.UserID == Core.CurrentUser.ID && rl.BookID == _currentBook.ID);

            if (selectedStatusId == 0)
            {
                if (existingRecord != null)
                {
                    Core.Context.ReadingLists.Remove(existingRecord);
                }
            }
            else
            {
                if (existingRecord != null)
                {
                    existingRecord.StatusID = selectedStatusId;
                }
                else
                {
                    ReadingLists newRecord = new ReadingLists
                    {
                        UserID = Core.CurrentUser.ID,
                        BookID = _currentBook.ID,
                        StatusID = selectedStatusId
                    };
                    Core.Context.ReadingLists.Add(newRecord);
                }
            }

            Core.Context.SaveChanges();
        }
        private void btnComplainBook_Click(object sender, RoutedEventArgs e)
        {
            if (Core.CurrentUser == null)
            {
                MessageBox.Show("Необходимо авторизоваться для отправки жалобы.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            string reason = Microsoft.VisualBasic.Interaction.InputBox("Введите причину жалобы на книгу:", "Жалоба на произведение", "Нарушение авторских прав / Неприемлемый контент");

            if (string.IsNullOrWhiteSpace(reason)) return;

            Complaints newComplaint = new Complaints
            {
                SenderID = Core.CurrentUser.ID,
                TargetBookID = _currentBook.ID,
                TargetReviewID = null,
                Reason = reason,
                CreatedAt = DateTime.Now
            };

            Core.Context.Complaints.Add(newComplaint);
            Core.Context.SaveChanges();

            MessageBox.Show("Жалоба на книгу успешно отправлена администрации.", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
        }


        private void btnComplainAuthor_Click(object sender, RoutedEventArgs e)
        {
            if (Core.CurrentUser == null)
            {
                MessageBox.Show("Необходимо авторизоваться для отправки жалобы.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            string reason = Microsoft.VisualBasic.Interaction.InputBox("Введите причину жалобы на автора:", "Жалоба на автора", "Оскорбительное поведение / Спам");

            if (string.IsNullOrWhiteSpace(reason)) return;


            Complaints newComplaint = new Complaints
            {
                SenderID = Core.CurrentUser.ID,
                TargetBookID = _currentBook.ID,
                TargetReviewID = null,
                Reason = $"[Жалоба на Автора]: {reason}",
                CreatedAt = DateTime.Now
            };

            Core.Context.Complaints.Add(newComplaint);
            Core.Context.SaveChanges();

            MessageBox.Show("Жалоба на автора успешно отправлена администрации.", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
        }


        private void btnAdminFreezeBook_Click(object sender, RoutedEventArgs e)
        {
            var result = MessageBox.Show($"Вы уверены, что хотите заморозить книгу '{_currentBook.Title}'?", "Подтверждение блокировки", MessageBoxButton.YesNo, MessageBoxImage.Warning);

            if (result == MessageBoxResult.Yes)
            {
                var bookToFreeze = Core.Context.Books.FirstOrDefault(b => b.ID == _currentBook.ID);
                if (bookToFreeze != null)
                {
                    bookToFreeze.IsFrozen = true;
                    Core.Context.SaveChanges();

                    MessageBox.Show("Книга успешно заморожена и скрыта из общего доступа.", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                    NavigationService.Navigate(new CatalogPage());
                }
            }
        }
    }
}
