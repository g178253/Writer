using System;
using WriterCore.Model;

namespace Writer.ViewModels
{
    internal sealed class BookViewModel : NotifyPropertyChanged
    {
        public BookViewModel(Book book)
        {
            Model = book ?? new Book();
            Model.CreateTime = DateTime.Now;
        }

        public Book Model { get; }

        public string Name
        {
            get { return Model.Name; }
            set { Model.Name = value; OnPropertyChanged(); }
        }
    }
}
