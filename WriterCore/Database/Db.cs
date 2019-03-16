﻿using System;
using System.Collections.Generic;
using Microsoft.Data.Sqlite;
using WriterCore.Model;

namespace WriterCore
{
    internal sealed class Db : IDb
    {
        public Db()
        {
            InitializeDatabase();
        }

        #region 数据库辅助

        private readonly string m_connectionString = "Filename=notebook.db"; // 数据库名。
        private readonly string m_books = "Books"; // 小说的表。
        private readonly string m_fragments = "Fragments"; // 情节片断的表。

        public void InitializeDatabase()
        {
            using (var db = new SqliteConnection(m_connectionString))
            {
                db.Open();

                if (!IsTableExist(m_books, db))
                {
                    var cmd = $"CREATE TABLE {m_books} (" +
                    "Id         INTEGER   NOT NULL PRIMARY KEY AUTOINCREMENT," +
                    "Name       TEXT(128) NOT NULL," +
                    "CreateTime TEXT(32)  NOT NULL DEFAULT( date('now') ) );";
                    ExecuteNonQuery(cmd, db);
                }

                if (!IsTableExist(m_fragments, db))
                {
                    var cmd = $"CREATE TABLE {m_fragments} (" +
                    "Id         INTEGER    NOT NULL PRIMARY KEY AUTOINCREMENT, " +
                   $"BookId     INTEGER    NOT NULL REFERENCES {m_books} (Id), " +
                    "Title      TEXT(128)  NOT NULL, " +
                    "Summary    TEXT(2048) NOT NULL, " +
                    "CreateTime TEXT(32)   NOT NULL DEFAULT(date('now')), " +
                    "Author     TEXT(64)   NOT NULL );";
                    ExecuteNonQuery(cmd, db);
                }

                db.Close();
            }
        }

        private bool IsTableExist(string tableName, SqliteConnection db)
        {
            var cmd = GetTableExistsCommand(tableName);
            var sc = new SqliteCommand(cmd, db);
            var r = (Int64)sc.ExecuteScalar();
            return r > 0;
        }

        private string GetTableExistsCommand(string tableName)
        {
            return $"SELECT COUNT(*) FROM sqlite_master WHERE type='table' AND name='{tableName}';";
        }

        private int ExecuteNonQuery(string cmd, SqliteConnection db, IDictionary<string, object> parameters = null)
        {
            var sc = new SqliteCommand(cmd, db);
            if (parameters != null)
            {
                foreach (var i in parameters)
                {
                    sc.Parameters.AddWithValue(i.Key, i.Value);
                }
            }
            return sc.ExecuteNonQuery();
        }

        private int ExecuteNonQuery(string cmd, IDictionary<string, object> parameters = null)
        {
            var count = 0;
            using (var db = new SqliteConnection(m_connectionString))
            {
                db.Open();

                count = ExecuteNonQuery(cmd, db, parameters);

                db.Close();
            }

            return count;
        }

        private IEnumerable<SqliteDataReader> Query(string cmd)
        {
            using (var db = new SqliteConnection(m_connectionString))
            {
                db.Open();

                var sc = new SqliteCommand(cmd, db);
                var query = sc.ExecuteReader();

                while (query.Read())
                {
                    yield return query;
                }

                db.Close();
            }
        }

        private int Count(string tableName, string where = null)
        {
            var count = 0;
            var cmd = "SELECT COUNT(*) FROM " + tableName;
            if (where != null)
            {
                cmd += " WHERE " + where;
            }

            foreach (var r in Query(cmd))
            {
                count += r.GetInt32(0);
            }
            return count;
        }

        #endregion

        public bool Add(Book child)
        {
            EnsureArgumentNotNull(child);

            var cmd = $"INSERT INTO {m_books} VALUES (NULL, @Name, @CreateTime);";
            var dic = new Dictionary<string, object>();
            dic.Add("@Name", child.Name);
            dic.Add("@CreateTime", child.CreateTime);
            return 0 < ExecuteNonQuery(cmd, dic);
        }

        public bool Add(Fragment child)
        {
            EnsureArgumentNotNull(child);

            var cmd = $"INSERT INTO {m_fragments} VALUES (NULL, @BookId, @Title, @Summary, @Author, @CreateTime);";
            var dic = new Dictionary<string, object>();
            dic.Add("@BookId", child.BookId);
            dic.Add("@Title", child.Title);
            dic.Add("@Summary", child.Summary);
            dic.Add("@Author", child.Author);
            dic.Add("@CreateTime", child.CreateTime);
            return 0 < ExecuteNonQuery(cmd, dic);
        }

        public bool Delete(Book child)
        {
            EnsureArgumentNotNull(child);

            var cmd = $"DELETE FROM {m_books} WHERE Id = " + child.Id;
            return 0 < ExecuteNonQuery(cmd);
        }

        public bool Delete(Fragment child)
        {
            EnsureArgumentNotNull(child);

            var cmd = $"DELETE FROM {m_fragments} WHERE Id = " + child.Id;
            return 0 < ExecuteNonQuery(cmd);
        }

        public IEnumerable<Book> FindBooks()
        {
            var cmd = "SELECT * FROM " + m_books;
            foreach (var r in Query(cmd))
            {
                var s = new Book
                {
                    Id = r.GetInt64(0),
                    Name = r.GetString(1),
                    CreateTime = r.GetDateTime(2)
                };
                yield return s;
            }
        }

        public IEnumerable<Fragment> FindFragments()
        {
            var cmd = "SELECT * FROM " + m_fragments;
            foreach (var r in Query(cmd))
            {
                var s = new Fragment
                {
                    Id = r.GetInt64(0),
                    BookId = r.GetInt64(1),
                    Title = r.GetString(2),
                    Summary = r.GetString(3),
                    CreateTime = r.GetDateTime(4),
                    Author = r.GetString(5)
                };
                yield return s;
            }
        }

        public bool Update(Book child)
        {
            EnsureArgumentNotNull(child);

            var cmd = $"UPDATE {m_books} SET " +
                "Name = @Name" +
                "CreateTime = @CreateTime, " +
                "WHERE Id = @Id;";
            var dic = new Dictionary<string, object>();
            dic.Add("@Id", child.Id);
            dic.Add("@Name", child.Name);
            dic.Add("@CreateTime", child.CreateTime);
            return 0 < ExecuteNonQuery(cmd, dic);
        }

        public bool Update(Fragment child)
        {
            EnsureArgumentNotNull(child);

            var cmd = $"UPDATE {m_fragments} SET " +
                "BookId = @BookId" +
                "Title = @Title, " +
                "Summary = @Summary, " +
                "CreateTime = @CreateTime, " +
                "Author = @Author " +
                "WHERE Id = @Id;";
            var dic = new Dictionary<string, object>();
            dic.Add("@Id", child.Id);
            dic.Add("@BookId", child.BookId);
            dic.Add("@Title", child.Title);
            dic.Add("@Summary", child.Summary);
            dic.Add("@CreateTime", child.CreateTime);
            dic.Add("@Author", child.Author);
            return 0 < ExecuteNonQuery(cmd, dic);
        }

        private void EnsureArgumentNotNull(object arg)
        {
            if (arg == null)
                throw new ArgumentNullException(nameof(arg));
        }
    }
}
