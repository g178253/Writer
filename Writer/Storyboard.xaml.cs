using System;
using Windows.System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using MUXC = Microsoft.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Navigation;

using Writer.ViewModels;
using WriterCore;
using WriterCore.Model;

namespace Writer
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class Storyboard : Page
    {
        private readonly Story m_story = new Story();
        private readonly ItemViewModel m_root = new ItemViewModel(); // 图书。
        private readonly General m_general = new General(); // 通用编辑框。

        private ItemViewModel m_current = null; // 当前选中项。

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
            SetButtonStatus(false);
        }

        // 返回主页。
        private void Home_Click(object sender, RoutedEventArgs e)
        {
            Frame.GoBack();
        }

        // 开始添加新章节。
        private void AddNewDocument_Click(object sender, RoutedEventArgs e)
        {
            CreateItemViewModel(false);
        }

        // 开始添加新卷。
        private void AddNewCatalog_Click(object sender, RoutedEventArgs e)
        {
            if (m_current == null)
            {
                m_current = m_root;
            }

            CreateItemViewModel(true);
        }

        private void CreateItemViewModel(bool isCatalog)
        {
            if (m_current == null)
                throw new ArgumentNullException(nameof(m_current));

            var catalog = m_current;
            // 如果当前选中项是文档，需要找到其父节点，从父节点添加新章节。
            if (catalog.Model is Fragment)
            {
                catalog = GetParent(m_root, catalog);
            }

            var it = new ItemViewModel();
            if (isCatalog)
                it.InitChildren();
            catalog.Children.Add(it);

            m_general.InEdit = false;
            m_general.BeginEdit(it.Title);

            m_current = it; // 当前选中项即是新增项。
        }

        private ItemViewModel GetParent(ItemViewModel parent, ItemViewModel child)
        {
            if (object.Equals(parent, child)) return parent;
            if (parent.Children == null) return null;
            if (parent.Children.Contains(child)) return parent;

            foreach (var item in parent.Children)
            {
                var r = GetParent(item, child);
                if (r != null) return r;
            }

            return null;
        }

        // 删除章节或卷。
        private async void Delete_Click(object sender, RoutedEventArgs e)
        {
            var m = m_current.Model;

            var more = "后，";
            if (!(m is Fragment))
                more = "的同时，还会删除该目录下的所有内容。";

            var r = await Message.ShowWarningAsync(
                $"删除【{m.Title}】{more}该操作**无法**恢复！\n\n删除【{m.Title}】吗？",
                "是的");
            if (!r) return;

            if (!m_story.Delete(m))
            {
                await Message.ShowErrorAsync($"删除【{m.Title}】失败，请稍后重试……");
            }
            else
            {
                m_root.Children.Remove(m_current);
            }
        }

        // 选中书，列出所有卷的详细列表；
        // 选中卷，列出所有子卷或章的详细列表；
        // 选中章，加载章节内容。
        private void Outline_ItemInvoked(MUXC.TreeView sender, MUXC.TreeViewItemInvokedEventArgs args)
        {
            m_current = args.InvokedItem as ItemViewModel;
            SetButtonStatus(true);
        }

        private void SetButtonStatus(bool select)
        {
            AddFragment.IsEnabled = select;
            Delete.IsEnabled = select;
        }

        // 开始编辑条目。
        private void BeginEdit_Click(object sender, RoutedEventArgs e)
        {
            m_general.InEdit = true;
            m_general.BeginEdit(m_current.Title);
        }

        // 右键选中条目。
        private void TreeViewItem_RightTapped(object sender, RightTappedRoutedEventArgs e)
        {
            m_current = (e.OriginalSource as FrameworkElement).DataContext as ItemViewModel;
        }

        // 完成编辑或添加。
        private void EditOrAdd_KeyUp(object sender, KeyRoutedEventArgs e)
        {
            if (e.Key == VirtualKey.Escape)
            {
                m_general.CancelEditOrAdd(m_current.Title, () =>
                {
                    m_root.Children.Remove(m_current);
                });
            }

            if (e.Key == VirtualKey.Enter)
            {
                m_general.AddOrEdit(EditTitle, CreateNew);
            }
        }

        private void EditTitle()
        {
            if (m_current == null || m_general.LastText == null)
                throw new ArgumentNullException((m_current == null) ? nameof(m_current) : nameof(m_general.LastText));

            if (m_current.Title.IsReadOnly) return;

            var titleType = (m_current.Children == null) ? "章节名" : "卷名";

            var newName = m_current.Title.Text;
            if (string.IsNullOrEmpty(newName))
            {
                m_general.SetError(m_current.Title, "请输入新的" + titleType);
                return;
            }

            if (newName == m_general.LastText)
            {
                m_general.SetError(m_current.Title, titleType + "并未修改");
                return;
            }

            m_current.Title.Text = newName;
            if (m_story.Update(m_current.Model))
                m_general.EndEdit(m_current.Title);
            else
                m_general.SetError(m_current.Title, $"保存作品【{newName}】失败");
        }

        private void CreateNew()
        {
            if (m_current == null)
                throw new ArgumentNullException(nameof(m_current));

            var titleType = (m_current.Children == null) ? "章节名" : "卷名";

            var name = m_current.Title.Text;
            if (string.IsNullOrEmpty(name))
            {
                m_general.SetError(m_current.Title, "请输入" + titleType);
                return;
            }

            var parent = GetParent(m_root, m_current);
            var m = (m_current.Children == null)
                ? m_story.AddFragment(m_root.Model.Id, parent.Model.Id, name) as IOutline
                : m_story.AddCatalog(m_root.Model.Id, parent.Model.Id, name);
            if (m != null)
            {
                m_current.SetModel(m);
            }

            m_general.EndEdit(m_current.Title);
        }
    }
}
