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

            var bookName = m_currentBook.Title.Text;
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

            var m = m_story.AddBook(bookName);
            if (m != null)
            {
                m_currentBook.SetModel(m);
            }

            SetBookCount(m_list.Count);
            EndEditBookName();
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

        private void AddOrEditBook()
        {
            if (m_isEdit)
                EditBookName();
            else
                CreateNewBook();
        }

        private void BeginEditBookName()
        {
            m_currentBook.Title.IsReadOnly = false;
            m_lastName = m_currentBook.Title.Text;
        }

        private void EditBookName()
        {
            if (m_currentBook == null || m_lastName == null)
                throw new ArgumentNullException((m_currentBook == null) ? nameof(m_currentBook) : nameof(m_lastName));

            if (m_currentBook.Title.IsReadOnly) return;

            var newName = m_currentBook.Title.Text;
            if (string.IsNullOrEmpty(newName))
            {
                SetError("请输入新的作品名");
                return;
            }

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
            book.Title.Text = newName;
            if (m_story.Update(book.Model))
                EndEditBookName();
            else
                SetError($"保存作品【{newName}】失败");
        }

        private void EndEditBookName()
        {
            m_currentBook.Title.IsReadOnly = true;
            m_currentBook.Title.InError = false;
            m_currentBook.Title.Error = null;
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
            m_currentBook.Title.InError = true;
            m_currentBook.Title.Error = v;
        }

        #endregion

        // 删除作品。
        private async void Delete_Click(object sender, RoutedEventArgs e)
        {
            var book = m_currentBook.Model;
            var r = await Message.ShowWarningAsync(
                $"删除作品【{book.Title}】后，该作品的相关内容也将一并删除。该操作**无法**恢复！\n\n删除【{book.Title}】吗？",
                "是的");
            if (!r) return;

            if (!m_story.Delete(book))
            {
                await Message.ShowErrorAsync($"删除作品【{book.Title}】失败，请稍后重试……");
            }
            else
            {
                m_list.Remove(m_currentBook);
                SetBookCount(m_list.Count);
            }
        }

        // 进入大纲视图。
        private void Outline_Click(object sender, RoutedEventArgs e)
        {
            var book = m_currentBook;
            Frame.Navigate(typeof(Storyboard), book);
        }

        // 左键进入大纲视图。
        private void Books_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count == 0) return;

            m_currentBook = e.AddedItems[0] as BookViewModel;
            Outline_Click(null, null);
        }

        // 右键选中作品。
        private void Books_RightTapped(object sender, RightTappedRoutedEventArgs e)
        {
            m_currentBook = (e.OriginalSource as FrameworkElement).DataContext as BookViewModel;
        }
    }
}
