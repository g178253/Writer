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
        private readonly ObservableCollection<BookViewModel> m_list; // 作品的集合。
        private readonly WritableTextBlock m_general; //通用操作。
        private BookViewModel m_current;    // 当前作品。

        public StoryBooks()
        {
            this.InitializeComponent();

            m_story = new Story();
            m_list = new ObservableCollection<BookViewModel>();
            m_general = new WritableTextBlock();

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
            m_current = new BookViewModel();
            m_list.Add(m_current);

            m_general.InEdit = false;
            m_general.BeginEdit(m_current.Title);
        }

        private void CreateNewBook()
        {
            if (m_current == null)
                throw new ArgumentNullException(nameof(m_current));

            var bookName = m_current.Title.Text;
            if (string.IsNullOrEmpty(bookName))
            {
                m_general.SetError(m_current.Title, "请输入新作品的书名");
                return;
            }

            if (m_story.ContainsBook(bookName))
            {
                m_general.SetError(m_current.Title, $"作品【{bookName}】已存在");
                return;
            }

            var m = m_story.AddBook(bookName);
            if (m != null)
            {
                m_current.SetModel(m);
            }

            SetBookCount(m_list.Count);
            m_general.EndEdit(m_current.Title);
        }

        #region 编辑图书名

        // 开始编辑书名。
        private void BeginEditBookName_Click(object sender, RoutedEventArgs e)
        {
            m_general.InEdit = true;
            m_general.BeginEdit(m_current.Title);
        }

        // 完成添加、编辑，或取消操作。
        private void EditBookName_KeyUp(object sender, KeyRoutedEventArgs e)
        {
            if (e.Key == VirtualKey.Escape)
            {
                m_general.CancelEditOrAdd(m_current.Title, () =>
                {
                    m_list.Remove(m_current);
                    SetBookCount(m_list.Count);
                });
            }

            if (e.Key == VirtualKey.Enter)
            {
                m_general.AddOrEdit(EditBookName, CreateNewBook);
            }
        }

        private void EditBookName()
        {
            if (m_current == null || m_general.LastText == null)
                throw new ArgumentNullException((m_current == null) ? nameof(m_current) : nameof(m_general.LastText));

            if (m_current.Title.IsReadOnly) return;

            var newName = m_current.Title.Text;
            if (string.IsNullOrEmpty(newName))
            {
                m_general.SetError(m_current.Title, "请输入新的作品名");
                return;
            }

            if (newName == m_general.LastText)
            {
                m_general.SetError(m_current.Title, "作品名并未修改");
                return;
            }

            if (m_story.ContainsBook(newName))
            {
                m_general.SetError(m_current.Title, $"作品【{newName}】已经存在");
                return;
            }

            var book = m_current;
            book.Title.Text = newName;
            if (m_story.Update(book.Model))
                m_general.EndEdit(m_current.Title);
            else
                m_general.SetError(m_current.Title, $"保存作品【{newName}】失败");
        }

        #endregion

        // 删除作品。
        private async void Delete_Click(object sender, RoutedEventArgs e)
        {
            var book = m_current.Model;
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
                m_list.Remove(m_current);
                SetBookCount(m_list.Count);
            }
        }

        // 进入大纲视图。
        private void Outline_Click(object sender, RoutedEventArgs e)
        {
            var book = m_current;
            Frame.Navigate(typeof(Storyboard), book);
        }

        // 左键进入大纲视图。
        private void Books_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count == 0) return;

            m_current = e.AddedItems[0] as BookViewModel;
            Outline_Click(null, null);
        }

        // 右键选中作品。
        private void Books_RightTapped(object sender, RightTappedRoutedEventArgs e)
        {
            m_current = (e.OriginalSource as FrameworkElement).DataContext as BookViewModel;
        }
    }
}
