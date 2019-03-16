using System.Collections.Generic;
using WriterCore.Model;

namespace WriterCore
{
    public interface IDb
    {
        bool Add(Book book);
        bool Add(Fragment fragment);
        bool Delete(Book book);
        bool Delete(Fragment fragment);
        bool Update(Book book);
        bool Update(Fragment fragment);
        IEnumerable<Book> FindBooks();
        IEnumerable<Fragment> FindFragments();
    }
}