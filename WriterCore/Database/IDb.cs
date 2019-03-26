using System;
using System.Collections.Generic;
using WriterCore.Model;

namespace WriterCore.Database
{
    public interface IDb
    {
        Int64 Add(Book book);
        Int64 Add(Fragment fragment);
        bool Delete(Book book);
        bool Delete(Fragment fragment);
        bool Update(Book book);
        bool Update(Fragment fragment);
        bool ContainsBook(string bookName);
        IEnumerable<Book> FindBooks();
        IEnumerable<Catalog> FindCatalogs(Int64 bookId);
        IEnumerable<Fragment> FindFragments(Int64 bookId, Int64 catalogId = -1);
    }
}