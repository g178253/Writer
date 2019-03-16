using System;
using System.Collections.Generic;
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

        public bool Add(Fragment fragment)
        {
            EnsureArgumentNotNull(fragment);
            return m_db.Add(fragment);
        }

        public bool Update(Fragment fragment)
        {
            EnsureArgumentNotNull(fragment);
            return m_db.Update(fragment);
        }

        public bool Delete(Fragment fragment)
        {
            EnsureArgumentNotNull(fragment);
            return m_db.Delete(fragment);
        }

        private void EnsureArgumentNotNull(Fragment fragment)
        {
            if (m_db == null || fragment == null)
                throw new ArgumentNullException((m_db == null) ? nameof(m_db) : nameof(fragment));
        }

        public IEnumerable<Fragment> GetFragments()
        {
            if (m_db == null)
                throw new ArgumentNullException(nameof(m_db));
            return m_db.FindFragments();
        }
    }
}
