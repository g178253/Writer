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

using Writer.ViewModels;
using WriterCore;

namespace Writer
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class Storyboard : Page
    {
        private readonly Story m_story = new Story();
        private readonly ItemViewModel m_root = new ItemViewModel();
        public Storyboard()
        {
            this.InitializeComponent();
        }

        // 初始化作品目录。
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            var book = e.Parameter as BookViewModel;
            if (book == null || book.Model == null)
                throw new ArgumentNullException((book == null) ? nameof(book) : nameof(book.Model));

            InitOutline(book);
        }

        private void InitOutline(BookViewModel book)
        {
            m_root.SetModel(book.Model);
            m_root.InitChildren();

            var list = m_story.GetCatalogs(book.Model.Id);
            foreach (var i in list)
            {
                var it = new ItemViewModel(i);
                it.InitChildren();
                m_root.Children.Add(it);
            }
            Outline.ItemsSource = new[] { m_root };
        }

        // 添加新章节。
        private void AddNewDocument_Click(object sender, RoutedEventArgs e)
        {

        }

        // 添加新卷。
        private void AddNewCatalog_Click(object sender, RoutedEventArgs e)
        {

        }

        // 删除章节或卷。
        private void AppBarButton_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
