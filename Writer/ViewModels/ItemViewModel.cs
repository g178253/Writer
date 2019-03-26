using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

using WriterCore.Model;

namespace Writer.ViewModels
{
    internal sealed class ItemViewModel : NotifyPropertyChanged
    {
        private ICollection<ItemViewModel> m_items;
        public ItemViewModel(IOutline model = null)
        {
            SetModel(model);
        }

        public IOutline Model { get; private set; }

        internal void SetModel(IOutline model)
        {
            if (model != null)
            {
                Name = model.Title;
                Model = model;
            }
        }

        private string m_name;
        public string Name
        {
            get { return m_name; }
            set { m_name = value; OnPropertyChanged(); }
        }

        private string m_newName;
        public string NewName
        {
            get { return m_newName; }
            set { m_newName = value; OnPropertyChanged(); }
        }

        private bool m_inEdit;
        public bool InEdit
        {
            get { return m_inEdit; }
            set { m_inEdit = value; OnPropertyChanged(); }
        }

        private bool m_notInEdit = true;
        public bool NotInEdit
        {
            get { return m_notInEdit; }
            set { m_notInEdit = value; OnPropertyChanged(); }
        }

        private bool m_inError;
        public bool InError
        {
            get { return m_inError; }
            set { m_inError = value; OnPropertyChanged(); }
        }

        private string m_editError;
        public string Error
        {
            get { return m_editError; }
            set { m_editError = value; OnPropertyChanged(); }
        }

        public void InitChildren()
        {
            if (m_items == null)
                m_items = new ObservableCollection<ItemViewModel>();
        }

        public ICollection<ItemViewModel> Children
        {
            get { return m_items; }
            set { m_items = value; OnPropertyChanged(); }
        }
    }
}
