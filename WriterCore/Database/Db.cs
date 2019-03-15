using System;
using System.Collections.Generic;
using Microsoft.Data.Sqlite;
using WriterCore.Model;

namespace WriterCore
{
    public sealed class Db : IDb
    {
        #region 数据库辅助

        private readonly string m_connectionString = "Filename=notebook.db";

        public void InitializeDatabase()
        {
            using (var db = new SqliteConnection(m_connectionString))
            {
                db.Open();

                var cmd = "CREATE TABLE IF NOT EXISTS Fragments (" +
                    "Id             INTEGER             PRIMARY KEY AUTOINCREMENT, " +
                    "Title          NVARCHAR(128)       NOT NULL        DEFAULT '新作品', " +
                    "Summary        NVARCHAR(2048)      NOT NULL        DEFAULT '这本书主要写此什么？', " +
                    "CreateTime     NVARCHAR(32)        NOT NULL        DEFAULT  date('now'), " +
                    "Author         NVARCHAR(64)        NOT NULL        DEFAULT '作者是谁？'" +
                    ");";
                ExecuteNonQuery(cmd, db);

                db.Close();
            }
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

        public bool Add(Fragment child)
        {
            EnsureArgumentNotNull(child);

            var cmd = "INSERT INTO Fragments VALUES (NULL, @Title, @Summary, @Author, @CreateTime);";
            var dic = new Dictionary<string, object>();
            dic.Add("@Title", child.Title);
            dic.Add("@Summary", child.Summary);
            dic.Add("@Author", child.Author);
            dic.Add("@CreateTime", child.CreateTime);
            return 0 < ExecuteNonQuery(cmd, dic);
        }

        public bool Delete(Fragment child)
        {
            EnsureArgumentNotNull(child);

            var cmd = "DELETE FROM Event WHERE Id = " + child.Id;
            return 0 < ExecuteNonQuery(cmd);
        }

        public IEnumerable<Fragment> FindAll()
        {
            var tableName = "Fragments";
            var cmd = "SELECT * FROM " + tableName;
            foreach (var r in Query(cmd))
            {
                var s = new Fragment
                {
                    Id = r.GetInt32(0),
                    Title = r.GetString(1),
                    Summary = r.GetString(2),
                    CreateTime = r.GetDateTime(3),
                    Author = r.GetString(4)
                };
                yield return s;
            }
        }

        public bool Update(Fragment child)
        {
            EnsureArgumentNotNull(child);

            var cmd = "UPDATE Fragments SET " +
                "Title = @Title, " +
                "Summary = @Summary, " +
                "CreateTime = @CreateTime, " +
                "Author = @Author " +
                "WHERE Id = @Id;";
            var dic = new Dictionary<string, object>();
            dic.Add("@Id", child.Id);
            dic.Add("@Title", child.Title);
            dic.Add("@Summary", child.Summary);
            dic.Add("@CreateTime", child.CreateTime);
            dic.Add("@Author", child.Author);
            return 0 < ExecuteNonQuery(cmd, dic);
        }

        private void EnsureArgumentNotNull(Fragment fragment)
        {
            if (fragment == null)
                throw new ArgumentNullException(nameof(fragment));
        }
    }
}
