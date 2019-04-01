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

        public bool ContainsBook(string title)
        {
            return m_db.ContainsBook(title);
        }

        public Book AddBook(string title)
        {
            EnsureArgumentNotNull(title);

            var m = new Book
            {
                Title = title,
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

        public Fragment AddFragment(Int64 bookId, Int64 catalogId, string title)
        {
            EnsureArgumentNotNull(title);

            var m = new Fragment
            {
                BookId = bookId,
                CatalogId = catalogId,
                Title = title,
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

        public Catalog AddCatalog(Int64 bookId, Int64 fatherId, string title)
        {
            EnsureArgumentNotNull(title);

            var m = new Catalog
            {
                BookId = bookId,
                FatherId = fatherId,
                Title = title,
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

        public bool Update(IOutline item)
        {
            EnsureArgumentNotNull(item);

            if (item is Book)
                return m_db.Update((Book)item);
            else if (item is Catalog)
                return m_db.Update((Catalog)item);
            else if (item is Fragment)
                return m_db.Update((Fragment)item);
            else
                throw new NotSupportedException("不支持的类型：" + item.GetType());
        }

        public bool Delete(IOutline item)
        {
            EnsureArgumentNotNull(item);

            if (item is Book)
                return m_db.Delete((Book)item);
            else if (item is Fragment)
                return m_db.Delete((Fragment)item);
            else if (item is Catalog)
                return m_db.Delete((Catalog)item);
            else
                throw new NotSupportedException("不支持的类型：" + item.GetType().ToString());
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
