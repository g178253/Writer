using System;
using System.Collections.Generic;
using WriterCore.Model;

namespace WriterCore
{
    public sealed class Story
    {
        private LinkedList<Fragment> m_Fragments = new LinkedList<Fragment>();

        public bool Add(IDb db, Fragment fragment)
        {
            EnsureArgumentNotNull(db, fragment);
            m_Fragments.AddLast(fragment);
            return db.Add(fragment);
        }

        public bool Update(IDb db, Fragment fragment)
        {
            EnsureArgumentNotNull(db, fragment);
            return db.Update(fragment);
        }

        public bool Delete(IDb db, Fragment fragment)
        {
            EnsureArgumentNotNull(db, fragment);
            m_Fragments.Remove(fragment);
            return db.Delete(fragment);
        }

        private void EnsureArgumentNotNull(IDb db, Fragment fragment)
        {
            if (db == null || fragment == null)
                throw new ArgumentNullException((db == null) ? nameof(db) : nameof(fragment));
        }

        public IEnumerable<Fragment> GetFragments(IDb db)
        {
            if (m_Fragments.Count == 0)
            {
                var all = db.FindAll();
                foreach (var item in all)
                {
                    m_Fragments.AddLast(item);
                }
            }
            return m_Fragments;
        }
    }
}
