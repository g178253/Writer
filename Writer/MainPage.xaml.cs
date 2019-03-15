using System;
using System.Collections.Generic;
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
using System.Collections.ObjectModel;
using Writer.ViewModels;
using WriterCore;

namespace Writer
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private ICollection<FragmentViewModel> m_fragments = new ObservableCollection<FragmentViewModel>();
        public MainPage()
        {
            this.InitializeComponent();
            var db = DbFactory.Db;
            var story = new Story(db);
            foreach (var item in story.GetFragments())
            {
                m_fragments.Add(new FragmentViewModel(item));
            }
            m_fragments.Add(new FragmentViewModel(new WriterCore.Model.Fragment { Title = "123", Summary = "123456" }));
            Fragments.ItemsSource = m_fragments;
        }
    }
}
