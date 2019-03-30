using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Writer.ViewModels
{
    internal sealed class ItemDetailViewModel : NotifyPropertyChanged
    {
        public WritableTextBlockViewModel Title { get; } = new WritableTextBlockViewModel();
        public WritableTextBlockViewModel Content { get; } = new WritableTextBlockViewModel();
    }
}
