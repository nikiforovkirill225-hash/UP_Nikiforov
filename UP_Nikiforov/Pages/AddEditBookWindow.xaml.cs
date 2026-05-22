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
using System.Windows.Shapes;

namespace UP_Nikiforov.Pages
{
    public partial class AddEditBookWindow : Window
    {
        private Books _book;
        private bool _isEdit = false;

        public AddEditBookWindow(Books currentBook)
        {
            InitializeComponent();

            if (currentBook != null)
            {
                _book = currentBook;
                _isEdit = true;

                tbTitle.Text = _book.Title;
                tbCoverPath.Text = _book.CoverPath;
                tbDescription.Text = _book.Description;
                tbTextContent.Text = _book.TextContent;
            }
            else
            {
                _book = new Books();
            }
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(tbTitle.Text) || string.IsNullOrWhiteSpace(tbTextContent.Text))
            {
                MessageBox.Show("Поля 'Название' и 'Текст произведения' обязательны для заполнения.", "Предупреждение", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            _book.Title = tbTitle.Text;
            _book.CoverPath = string.IsNullOrWhiteSpace(tbCoverPath.Text) ? null : tbCoverPath.Text;
            _book.Description = tbDescription.Text;
            _book.TextContent = tbTextContent.Text;

            if (!_isEdit)
            {
                _book.AuthorID = Core.CurrentUser.ID;
                _book.IsFrozen = false;
                Core.Context.Books.Add(_book);
            }

            Core.Context.SaveChanges();
            MessageBox.Show("Данные успешно сохранены!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);

            DialogResult = true;
            this.Close();
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            this.Close();
        }
    }
}
