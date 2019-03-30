using System;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;

namespace Writer
{
    internal static class Message
    {
        public static async Task ShowErrorAsync(string message)
        {
            var cd = new ContentDialog
            {
                Title = "出错啦",
                Content = message,
                CloseButtonText = "好的"
            };
            await cd.ShowAsync();
        }

        public static async Task<bool> ShowWarningAsync(string message, string primaryButton)
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
    }
}
