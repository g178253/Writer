using System;
using System.Collections.Generic;
using System.Linq;

using WriterCore.Model;

namespace WriterCore.Database
{
    internal sealed class MemoryDb : IDb
    {
        private static IDictionary<Int64, Book> s_books = new Dictionary<Int64, Book>();
        private static IDictionary<Int64, Catalog> s_catalogs = new Dictionary<Int64, Catalog>();
        private static IDictionary<Int64, Fragment> s_fragments = new Dictionary<Int64, Fragment>();

        public MemoryDb()
        {
            if (s_books.Count == 0)
            {
                Add(new Book { Title = "123", CreateTime = DateTime.Now });
            }
        }

        public long Add(Book book)
        {
            book.Id = s_books.Count;
            s_books.Add(book.Id, book);
            return book.Id;
        }

        public long Add(Fragment fragment)
        {
            fragment.Id = s_fragments.Count;
            s_fragments.Add(fragment.Id, fragment);
            return fragment.Id;
        }

        public long Add(Catalog catalog)
        {
            catalog.Id = s_catalogs.Count;
            s_catalogs.Add(catalog.Id, catalog);
            return catalog.Id;
        }

        public bool ContainsBook(string bookName)
        {
            var books = from book in s_books.Values
                        where book.Title == bookName
                        select book;
            return books.Count() > 0;
        }

        public bool Delete(Book book)
        {
            return s_books.Remove(book.Id);
        }

        public bool Delete(Fragment fragment)
        {
            return s_fragments.Remove(fragment.Id);
        }

        public bool Delete(Catalog catalog)
        {
            return s_catalogs.Remove(catalog.Id);
        }

        public IEnumerable<Book> FindBooks()
        {
            foreach (var item in s_books.Values)
            {
                yield return item;
            }
        }

        public IEnumerable<Catalog> FindCatalogs(long bookId)
        {
            foreach (var item in s_catalogs.Values)
            {
                if (item.BookId == bookId)
                {
                    yield return item;
                }
            }
        }

        public IEnumerable<Fragment> FindFragments(long bookId, long catalogId = -1)
        {
            foreach (var item in s_fragments.Values)
            {
                if (item.BookId == bookId)
                {
                    if (catalogId >= 0 && item.CatalogId != catalogId) continue;

                    yield return item;
                }
            }
        }

        public bool Update(Book book)
        {
            if (!s_books.ContainsKey(book.Id)) return false;

            s_books[book.Id] = book;
            return true;
        }

        public bool Update(Fragment fragment)
        {
            if (!s_fragments.ContainsKey(fragment.Id)) return false;

            s_fragments[fragment.Id] = fragment;
            return true;
        }

        public bool Update(Catalog catalog)
        {
            if (!s_catalogs.ContainsKey(catalog.Id)) return false;

            s_catalogs[catalog.Id] = catalog;
            return true;
        }
    }
}
