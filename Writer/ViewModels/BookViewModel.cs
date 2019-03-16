using System;
using WriterCore.Model;

namespace Writer.ViewModels
{
    internal sealed class BookViewModel : NotifyPropertyChanged
    {
        private readonly Book m_book;
        public BookViewModel(Book book)
        {
            m_book = book ?? new Book();
            m_book.CreateTime = DateTime.Now;
        }

        public string Name
        {
            get { return m_book.Name; }
            set { m_book.Name = value; OnPropertyChanged(); }
        }
    }
}
