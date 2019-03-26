using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Windows.System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;

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
        private BookViewModel m_currentBook;
        private string m_lastName;
        private bool m_isEdit;

        public StoryBooks()
        {
            this.InitializeComponent();

            m_story = new Story();
            m_list = new ObservableCollection<BookViewModel>();

            Books.ItemsSource = m_list;

            FillExistingBooks(m_story, m_list);
            SetBookCount(m_list.Count);
        }

        private void SetBookCount(int bookCount)
        {
            Status.Text = "著作：" + bookCount.ToString();
        }

        private void FillExistingBooks(Story story, ObservableCollection<BookViewModel> list)
        {
            var books = story.GetBooks();
            foreach (var item in books)
            {
                list.Add(new BookViewModel(item));
            }
        }

        // 添加新作品。
        private void AddNewBook_Click(object sender, RoutedEventArgs e)
        {
            m_isEdit = false;

            m_currentBook = new BookViewModel();
            m_list.Add(m_currentBook);

            BeginEditBookName();
        }

        private void CreateNewBook()
        {
            if (m_currentBook == null)
                throw new ArgumentNullException(nameof(m_currentBook));

            var bookName = m_currentBook.NewName;
            if (string.IsNullOrEmpty(bookName))
            {
                SetError("请输入新作品的书名");
                return;
            }

            if (m_story.Contains(bookName))
            {
                SetError($"作品【{bookName}】已存在");
                return;
            }

            var m = m_story.Add(bookName);
            if (m != null)
            {
                m_currentBook.SetModel(m);
            }

            SetBookCount(m_list.Count);
            EndEditBookName();
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

        #region 编辑图书名

        // 开始编辑书名。
        private void BeginEditBookName_Click(object sender, RoutedEventArgs e)
        {
            m_isEdit = true;
            BeginEditBookName();
        }

        // 添加、编辑或取消。
        private void EditBookName_KeyUp(object sender, KeyRoutedEventArgs e)
        {
            if (e.Key == VirtualKey.Escape)
            {
                CancelEdit();
            }

            if (e.Key == VirtualKey.Enter)
            {
                AddOrEditBook();
            }
        }

        // 添加、编辑图书。
        private void Books_Tapped(object sender, TappedRoutedEventArgs e)
        {
            if (m_currentBook != null)
                AddOrEditBook();
        }

        private void AddOrEditBook()
        {
            if (m_isEdit)
                EditBookName();
            else
                CreateNewBook();
        }

        private void BeginEditBookName()
        {
            m_currentBook.InEdit = true;
            m_currentBook.NotInEdit = false;
            m_lastName = m_currentBook.Name;
            m_currentBook.NewName = m_lastName;
        }

        private void EditBookName()
        {
            if (m_currentBook == null || m_currentBook.NewName == null)
                throw new ArgumentNullException((m_currentBook == null) ? nameof(m_currentBook) : nameof(m_currentBook.NewName));

            if (m_currentBook.NotInEdit) return;

            var newName = m_currentBook.NewName;
            if (newName == m_lastName)
            {
                SetError("作品名并未修改");
                return;
            }

            if (m_story.Contains(newName))
            {
                SetError($"作品【{newName}】已经存在");
                return;
            }

            var book = m_currentBook;
            book.Name = newName;
            if (m_story.Update(book.Model))
                EndEditBookName();
            else
                SetError($"保存作品【{newName}】失败");
        }

        private void EndEditBookName()
        {
            m_currentBook.InEdit = false;
            m_currentBook.NotInEdit = true;
            m_currentBook.InError = false;
            m_currentBook.Error = null;
        }

        private void CancelEdit()
        {
            if (m_isEdit)
            {
                EndEditBookName();
            }
            else
            {
                m_list.Remove(m_currentBook);
                SetBookCount(m_list.Count);
            }
        }

        private void SetError(string v)
        {
            m_currentBook.InError = true;
            m_currentBook.Error = v;
        }

        #endregion

        // 删除作品。
        private async void Delete_Click(object sender, RoutedEventArgs e)
        {
            var book = m_currentBook;
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

        // 进入大纲视图。
        private void Outline_Click(object sender, RoutedEventArgs e)
        {
            var book = m_currentBook;
            Frame.Navigate(typeof(Storyboard), book);
        }

        // 右键选中图书。
        private void Books_RightTapped(object sender, RightTappedRoutedEventArgs e)
        {
            m_currentBook = (e.OriginalSource as FrameworkElement).DataContext as BookViewModel;
        }

        // 左键进入大纲视图。
        private void Books_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count == 0) return;

            m_currentBook = e.AddedItems[0] as BookViewModel;
            Outline_Click(null, null);
        }
    }
}
