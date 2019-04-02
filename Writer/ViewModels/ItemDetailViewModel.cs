using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using WriterCore.Model;

namespace Writer.ViewModels
{
    internal sealed class ItemDetailViewModel : NotifyPropertyChanged
    {
        public ItemDetailViewModel(string title)
        {
            SetTitle(title);
        }

        public ItemDetailViewModel(IOutline model = null)
        {
            SetModel(model);
        }

        public IOutline Model { get; private set; }

        public WritableTextBlockViewModel Title { get; } = new WritableTextBlockViewModel();
        public WritableTextBlockViewModel Content { get; } = new WritableTextBlockViewModel();

        private ICollection<ItemDetailViewModel> m_items;
        public ICollection<ItemDetailViewModel> Children
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
                m_items = new ObservableCollection<ItemDetailViewModel>();
        }
    }
}
