using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using WriterCore;
using Writer.ViewModels;

namespace Writer
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class StoryBooks : Page
    {
        private readonly Story m_story;
        private readonly ObservableCollection<BookViewModel> m_list;
        public StoryBooks()
        {
            this.InitializeComponent();

            m_story = new Story();
            m_list = new ObservableCollection<BookViewModel>();

            Books.ItemsSource = m_list;

            FillExistingBooks(m_story, m_list);
            SetStatus(m_list.Count);
        }

        private void SetStatus(int bookCount)
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

        private void AddNewBook_Click(object sender, RoutedEventArgs e)
        {
            var title = NewBookName.Text;
            if (string.IsNullOrEmpty(title))
            {
                SetError("请输入新作品的书名");
                return;
            }

            var m = CreateNewBook(title);
            if (m == null)
            {
                SetError($"新作品【{title}】创建失败");
                return;
            }

            m_list.Add(m);
            SetStatus(m_list.Count);
            SetError(null);

            Frame.Navigate(typeof(Storyboard), m_story);
        }

        private BookViewModel CreateNewBook(string bookName)
        {
            if (m_story.Contains(bookName))
            {
                SetError($"作品【{bookName}】已存在");
                return null;
            }

            var m = m_story.Add(bookName);
            if (m != null)
            {
                return new BookViewModel(m);
            }

            return null;
        }

        private void SetError(string message)
        {
            Error.Text = message;
            Error.Visibility = (string.IsNullOrEmpty(message))
                ? Visibility.Collapsed
                : Visibility.Visible;
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            AddNewBook.Flyout.Hide();
        }
    }
}
