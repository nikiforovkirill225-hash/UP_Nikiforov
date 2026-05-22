using System;
using System.Collections.Generic;
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
            var filterStatuses = Core.Context.ReadingListStatuses.ToList();
            filterStatuses.Insert(0, new ReadingListStatuses { ID = 0, Name = "Все списки" });
            cbListFilter.ItemsSource = filterStatuses;
            cbListFilter.DisplayMemberPath = "Name";
            cbListFilter.SelectedValuePath = "ID";
            cbListFilter.SelectedIndex = 0;

            UpdateUserList();
        }

        private void UpdateUserList()
        {
            if (Core.CurrentUser == null) return;

            var userList = Core.Context.ReadingLists.Where(rl => rl.UserID == Core.CurrentUser.ID).ToList();

            if (cbListFilter.SelectedIndex > 0 && cbListFilter.SelectedValue != null)
            {
                int selectedStatusId = Convert.ToInt32(cbListFilter.SelectedValue);
                userList = userList.Where(rl => rl.StatusID == selectedStatusId).ToList();
            }

            lvUserBooks.ItemsSource = userList;
        }

        private void cbListFilter_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateUserList();
        }

        private void lvUserBooks_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var selectedRecord = lvUserBooks.SelectedItem as ReadingLists;
            if (selectedRecord != null && selectedRecord.Books != null)
            {
                NavigationService.Navigate(new BookDetailsPage(selectedRecord.Books));
            }
        }
    }
}