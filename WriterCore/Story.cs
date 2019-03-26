using System;
using System.Collections.Generic;

using WriterCore.Database;
using WriterCore.Model;

namespace WriterCore
{
    public sealed class Story
    {
        private IDb m_db;
        public Story()
        {
            this.m_db = DbFactory.Db;
        }

        public bool Contains(string bookName)
        {
            return m_db.ContainsBook(bookName);
        }

        public Book Add(string bookName)
        {
            EnsureArgumentNotNull(bookName);

            var m = new Book
            {
                Title = bookName,
                CreateTime = DateTime.Now
            };

            var id = m_db.Add(m);
            if (id >= 0)
            {
                m.Id = id;
                return m;
            }

            return null;
        }

        public bool Add(Fragment fragment)
        {
            EnsureArgumentNotNull(fragment);
            return m_db.Add(fragment) > 0;
        }

        public bool Update(Book book)
        {
            EnsureArgumentNotNull(book);
            return m_db.Update(book);
        }

        public bool Update(Fragment fragment)
        {
            EnsureArgumentNotNull(fragment);
            return m_db.Update(fragment);
        }

        public bool Delete(Book book)
        {
            EnsureArgumentNotNull(book);
            return m_db.Delete(book);
        }

        public bool Delete(Fragment fragment)
        {
            EnsureArgumentNotNull(fragment);
            return m_db.Delete(fragment);
        }

        private void EnsureArgumentNotNull(object arg)
        {
            if (m_db == null || arg == null)
                throw new ArgumentNullException((m_db == null) ? nameof(m_db) : nameof(arg));
        }

        public IEnumerable<Book> GetBooks()
        {
            if (m_db == null)
                throw new ArgumentNullException(nameof(m_db));
            return m_db.FindBooks();
        }

        public IEnumerable<Catalog> GetCatalogs(Int64 bookId)
        {
            if (m_db == null)
                throw new ArgumentNullException(nameof(m_db));
            return m_db.FindCatalogs(bookId);
        }
    }
}
