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
                Title.Text = book.Title;
                Model = book;
                Model.CreateTime = DateTime.Now;
            }
        }

        public WritableTextBlockViewModel Title { get; } = new WritableTextBlockViewModel();
    }
}
