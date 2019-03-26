﻿using Windows.UI.Xaml.Controls;

namespace Writer
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();
            RootFrame.Navigate(typeof(StoryBooks));
        }
    }
}
