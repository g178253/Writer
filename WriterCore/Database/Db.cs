using System;
using System.Collections.Generic;
using Microsoft.Data.Sqlite;

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

                var cmd = "CREATE TABLE IF NOT EXISTS Story (" +
                    "Id             INTEGER             PRIMARY KEY AUTOINCREMENT, " +
                    "Title          NVARCHAR(128)       NOT NULL        DEFAULT '新作品', " +
                    "Summary        NVARCHAR(2048)      NOT NULL        DEFAULT '这本书主要写此什么？', " +
                    "CreateTime     NVARCHAR(32)        NOT NULL        DEFAULT  date('now'), " +
                    "Author         NVARCHAR(64)        NOT NULL        DEFAULT '作者是谁？'" +
                    ");";
                ExecuteNonQuery(cmd, db);

                cmd = "CREATE TABLE IF NOT EXISTS Event (" +
                    "Id             INTEGER             PRIMARY KEY AUTOINCREMENT, " +
                    "FatherId       INTEGER, " +
                    "StoryId        INTEGER, " +
                    "Title          NVARCHAR(128)       NOT NULL        DEFAULT '新章节', " +
                    "Content        NVARCHAR(10240)     NOT NULL        DEFAULT '章节内容呢？', " +
                    "Remarks        NVARCHAR(2048)          NULL, " +
                    "SaveTime       NVARCHAR(32)        NOT NULL        DEFAULT  date('now')" +
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

        private void Query(string cmd, Action<SqliteDataReader> reader)
        {
            using (var db = new SqliteConnection(m_connectionString))
            {
                db.Open();

                var sc = new SqliteCommand(cmd, db);
                var query = sc.ExecuteReader();

                while (query.Read())
                {
                    reader.Invoke(query);
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
            Query(cmd, r => count = r.GetInt32(0));
            return count;
        }

        #endregion

        public void AddEvent(Event child)
        {
            if (child == null)
                throw new ArgumentNullException(nameof(child));

            var cmd = "INSERT INTO Event VALUES (NULL, @FatherId, @StoryId, @Title, @Content, @Remarks, @SaveTime);";
            var dic = new Dictionary<string, object>();
            dic.Add("@FatherId", child.FatherId);
            dic.Add("@StoryId", child.StoryId);
            dic.Add("@Title", child.Title);
            dic.Add("@Content", child.Content);
            dic.Add("@Remarks", child.Remarks);
            dic.Add("@SaveTime", child.SaveTime);
            ExecuteNonQuery(cmd, dic);
        }

        public void AddStory(Story story)
        {
            if (story == null)
                throw new ArgumentNullException(nameof(story));

            var cmd = "INSERT INTO Story VALUES (NULL, @Title, @Summary, @CreateTime, @Author);";
            var dic = new Dictionary<string, object>();
            dic.Add("@Title", story.Title);
            dic.Add("@Summary", story.Summary);
            dic.Add("@CreateTime", story.CreateTime);
            dic.Add("@Author", story.Author);
            ExecuteNonQuery(cmd, dic);
        }

        public void DeleteEvent(int id)
        {
            var cmd = "DELETE FROM Event WHERE Id = " + id;
            ExecuteNonQuery(cmd);
        }

        public void DeleteStory(int id)
        {
            var cmd = "DELETE FROM Story WHERE Id = " + id;
            ExecuteNonQuery(cmd);
        }

        public ICollection<Event> GetEvents(int storyId)
        {
            var tableName = "Event";
            var where = "StoryId = " + storyId;
            var count = Count(tableName, where);
            var list = new List<Event>(count);
            if (count > 0)
            {
                var cmd = $"SELECT * FROM {tableName} WHERE {where}";
                Query(cmd, r =>
                {
                    var e = new Event
                    {
                        Id = r.GetInt32(0),
                        FatherId = r.GetInt32(1),
                        StoryId = r.GetInt32(2),
                        Title = r.GetString(3),
                        Content = r.GetString(4),
                        Remarks = r.GetString(5),
                        SaveTime = r.GetDateTime(6),
                        Saved = true
                    };
                    list.Add(e);
                });
            }
            return list;
        }

        public ICollection<Story> GetStories()
        {
            var tableName = "Story";
            var count = Count(tableName);
            var list = new List<Story>(count);
            if (count > 0)
            {
                var cmd = "SELECT * FROM " + tableName;
                Query(cmd, r =>
                {
                    var s = new Story
                    {
                        Id = r.GetInt32(0),
                        Title = r.GetString(1),
                        Summary = r.GetString(2),
                        CreateTime = r.GetDateTime(3),
                        Author = r.GetString(4)
                    };
                    list.Add(s);
                });
            }
            return list;
        }

        public void UpdateEvent(Event e)
        {
            if (e == null)
                throw new ArgumentNullException(nameof(e));

            var cmd = "UPDATE Event SET " +
                "FatherId = @FatherId, " +
                "StoryId = @StoryId, " +
                "Title = @Title, " +
                "Content = @Content, " +
                "Remarks = @Remarks, " +
                "SaveTime = @SaveTime " +
                "WHERE Id = @Id;";
            var dic = new Dictionary<string, object>();
            dic.Add("@Id", e.Id);
            dic.Add("@FatherId", e.FatherId);
            dic.Add("@StoryId", e.StoryId);
            dic.Add("@Title", e.Title);
            dic.Add("@Content", e.Content);
            dic.Add("@Remarks", e.Remarks);
            dic.Add("@SaveTime", e.SaveTime);
            ExecuteNonQuery(cmd, dic);
        }

        public void UpdateStory(Story story)
        {
            if (story == null)
                throw new ArgumentNullException(nameof(story));

            var cmd = "UPDATE Story SET " +
                "Title = @Title, " +
                "Summary = @Summary, " +
                "CreateTime = @CreateTime, " +
                "Author = @Author " +
                "WHERE Id = @Id;";
            var dic = new Dictionary<string, object>();
            dic.Add("@Id", story.Id);
            dic.Add("@Title", story.Title);
            dic.Add("@Summary", story.Summary);
            dic.Add("@CreateTime", story.CreateTime);
            dic.Add("@Author", story.Author);
            ExecuteNonQuery(cmd, dic);
        }
    }
}
