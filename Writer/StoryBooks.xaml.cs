using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

using Writer.ViewModels;
using WriterCore;

namespace Writer
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class StoryBooks : Page
    {
        private readonly Story m_story;
        private readonly ObservableCollection<BookViewModel> m_list;
        private string m_bookName;
        public StoryBooks()
        {
            this.InitializeComponent();

            m_story = new Story();
            m_list = new ObservableCollection<BookViewModel>();

            Books.ItemsSource = m_list;

            FillExistingBooks(m_story, m_list);
            SetBookCount(m_list.Count);
            SetButtonStatus(false);
        }

        private void SetBookCount(int bookCount)
        {
            Status.Text = "著作：" + bookCount.ToString();
        }

        private void FillExistingBooks(Story story, ObservableCollection<BookViewModel> list)
        {
            foreach (var item in story.GetBooks())
            {
                list.Add(new BookViewModel(item));
            }
        }

        // 添加新作品。
        private void AddNewBook_Click(object sender, RoutedEventArgs e)
        {
            var title = NewBookName.Text;
            if (string.IsNullOrEmpty(title))
            {
                SetAddError("请输入新作品的书名");
                return;
            }

            var m = CreateNewBook(title);
            if (m == null)
            {
                SetAddError($"新作品【{title}】创建失败");
                return;
            }

            m_list.Add(m);
            SetBookCount(m_list.Count);
            SetAddError(null);

            Frame.Navigate(typeof(Storyboard), m_story);
        }

        private BookViewModel CreateNewBook(string bookName)
        {
            if (m_story.Contains(bookName))
            {
                SetAddError($"作品【{bookName}】已存在");
                return null;
            }

            var m = m_story.Add(bookName);
            if (m != null)
            {
                return new BookViewModel(m);
            }

            return null;
        }

        private void SetAddError(string message)
        {
            SetError(ErrorAdd, message);
        }

        private void SetEditError(string message)
        {
            SetError(ErrorEdit, message);
        }

        private void SetError(TextBlock block, string message)
        {
            if (message != null)
                block.Text = message;
            block.Visibility = (string.IsNullOrEmpty(message))
                ? Visibility.Collapsed
                : Visibility.Visible;
        }

        private async Task ShowErrorAsync(string message)
        {
            var cd = new ContentDialog
            {
                Title = "出错啦",
                Content = message,
                CloseButtonText = "好的"
            };
            await cd.ShowAsync();
        }

        private async Task<bool> ShowWarningAsync(string message, string primaryButton)
        {
            var cd = new ContentDialog
            {
                Title = "警告",
                Content = message,
                CloseButtonText = "算了",
                PrimaryButtonText = primaryButton
            };
            var r = await cd.ShowAsync();
            return r == ContentDialogResult.Primary;
        }

        private void SetButtonStatus(bool bookSelected)
        {
            Add.IsEnabled = !bookSelected;
            Edit.IsEnabled = bookSelected;
            Delete.IsEnabled = bookSelected;
        }

        // 取消添加新作品。
        private void AddCancel_Click(object sender, RoutedEventArgs e)
        {
            Add.Flyout.Hide();
        }

        private BookViewModel GetSelectedBook()
        {
            var book = Books.SelectedItem as BookViewModel;
            if (book == null)
                throw new ArgumentNullException(nameof(book));
            return book;
        }

        // 当编辑菜单即将打开时发生。
        private void FlyoutEdit_Opening(object sender, object e)
        {
            var book = GetSelectedBook();
            m_bookName = book.Name;
            EditBookName.Text = m_bookName;
        }

        // 编辑图书名。
        private void EditBookName_Click(object sender, RoutedEventArgs e)
        {
            if (m_bookName == null)
                throw new ArgumentNullException(nameof(m_bookName));

            var newName = EditBookName.Text;
            if (newName == m_bookName)
            {
                SetEditError("作品名并未修改");
                return;
            }

            if (m_story.Contains(newName))
            {
                SetEditError($"作品【{newName}】已经存在");
                return;
            }

            var book = GetSelectedBook();
            book.Name = newName;
            if (!m_story.Update(book.Model))
            {
                SetEditError($"保存作品【{newName}】失败");
            }
        }

        // 取消编辑图书。
        private void EditCancel_Click(object sender, RoutedEventArgs e)
        {
            Edit.Flyout.Hide();
        }

        // 选中图书。
        private void Books_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var r = e.AddedItems.Count > 0;
            SetButtonStatus(r);
        }

        // 取消选中图书。
        private void Books_PointerReleased(object sender, Windows.UI.Xaml.Input.PointerRoutedEventArgs e)
        {
            if (Books.SelectedItem != null)
                Books.SelectedItem = null;
        }

        // 删除作品。
        private async void Delete_Click(object sender, RoutedEventArgs e)
        {
            var book = GetSelectedBook();
            var r = await ShowWarningAsync(
                $"删除作品【{book.Name}】后，将无法恢复。\n\n删除【{book.Name}】吗？",
                "是的");
            if (!r) return;

            if (!m_story.Delete(book.Model))
            {
                await ShowErrorAsync($"删除作品【{book.Name}】失败，请稍后重试……");
            }
            else
            {
                m_list.Remove(book);
                SetBookCount(m_list.Count);
            }
        }
    }
}
