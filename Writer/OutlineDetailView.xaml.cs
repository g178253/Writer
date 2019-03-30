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

namespace Writer
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class OutlineDetailView : Page
    {
        public OutlineDetailView()
        {
            this.InitializeComponent();
        }

        // 点击标题，可以进行编辑。
        private void Title_Tapped(object sender, TappedRoutedEventArgs e)
        {

        }

        // 点击内空，可以进行编辑。
        private void Content_Tapped(object sender, TappedRoutedEventArgs e)
        {

        }
    }
}
