using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace UP_Nikiforov.Pages
{
    public partial class ListPage : Page
    {
        public ListPage()
        {
            InitializeComponent();
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                if (cbListFilter == null)
                {
                    MessageBox.Show("Ошибка инициализации фильтра.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                // Заполняем ComboBox из кода, а не из XAML
                var filterStatuses = Core.Context.ReadingListStatuses.ToList();

                // Очищаем ItemsSource перед заполнением
                cbListFilter.ItemsSource = null;
                cbListFilter.Items.Clear();

                // Добавляем пункт "Все списки" вручную
                var allItem = new ComboBoxItem { Content = "📚 Все списки", Tag = 0 };
                cbListFilter.Items.Add(allItem);

                // Добавляем статусы из БД
                foreach (var status in filterStatuses)
                {
                    var item = new ComboBoxItem { Content = GetStatusIcon(status.Name) + " " + status.Name, Tag = status.ID };
                    cbListFilter.Items.Add(item);
                }

                cbListFilter.SelectedIndex = 0;

                UpdateUserList();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки страницы: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private string GetStatusIcon(string statusName)
        {
            switch (statusName)
            {
                case "Хочу прочитать": return "⭐";
                case "Читаю": return "📖";
                case "Прочитано": return "✅";
                case "Заброшено": return "💔";
                default: return "📚";
            }
        }

        private void UpdateUserList()
        {
            try
            {
                if (Core.CurrentUser == null) return;
                if (lvUserBooks == null) return;

                // Очищаем ItemsSource перед установкой нового
                lvUserBooks.ItemsSource = null;

                var userList = Core.Context.ReadingLists
                    .Where(rl => rl.UserID == Core.CurrentUser.ID)
                    .ToList();

                if (cbListFilter != null && cbListFilter.SelectedIndex > 0)
                {
                    var selectedItem = cbListFilter.SelectedItem as ComboBoxItem;
                    if (selectedItem != null && selectedItem.Tag != null)
                    {
                        int selectedStatusId = Convert.ToInt32(selectedItem.Tag);
                        userList = userList.Where(rl => rl.StatusID == selectedStatusId).ToList();
                    }
                }

                lvUserBooks.ItemsSource = userList;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка обновления списка: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void cbListFilter_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateUserList();
        }

        private void lvUserBooks_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            try
            {
                var selectedRecord = lvUserBooks?.SelectedItem as ReadingLists;
                if (selectedRecord != null && selectedRecord.Books != null)
                {
                    NavigationService?.Navigate(new BookDetailsPage(selectedRecord.Books));
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка открытия книги: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}