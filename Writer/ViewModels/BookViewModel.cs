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
                Name = book.Title;
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

        private bool m_isReadOnly = true;
        public bool IsReadOnly
        {
            get { return m_isReadOnly; }
            set { m_isReadOnly = value; OnPropertyChanged(); }
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
