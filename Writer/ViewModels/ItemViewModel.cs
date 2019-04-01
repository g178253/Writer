using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

using WriterCore.Model;

namespace Writer.ViewModels
{
    internal sealed class ItemViewModel : NotifyPropertyChanged
    {
        public ItemViewModel(string title)
        {
            SetTitle(title);
        }

        public ItemViewModel(IOutline model = null)
        {
            SetModel(model);
        }

        public IOutline Model { get; private set; }

        public WritableTextBlockViewModel Title { get; } = new WritableTextBlockViewModel();

        private ICollection<ItemViewModel> m_items;
        public ICollection<ItemViewModel> Children
        {
            get { return m_items; }
            set { m_items = value; OnPropertyChanged(); }
        }

        internal void SetModel(IOutline model)
        {
            if (model != null)
            {
                SetTitle(model.Title);
                Model = model;
            }
        }

        internal void SetTitle(string title)
        {
            Title.Text = title;
        }

        internal void InitChildren()
        {
            if (m_items == null)
                m_items = new ObservableCollection<ItemViewModel>();
        }
    }
}
