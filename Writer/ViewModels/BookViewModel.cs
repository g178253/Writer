using System;
using WriterCore.Model;

namespace Writer.ViewModels
{
    internal sealed class BookViewModel : NotifyPropertyChanged
    {
        public BookViewModel(Book book = null)
        {
            SetModel(book);
        }

        public Book Model { get; private set; }

        internal void SetModel(Book book)
        {
            if (book != null)
            {
                Name = book.Name;
                Model = book;
                Model.CreateTime = DateTime.Now;
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
    }
}
