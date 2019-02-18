using System;
using System.Collections.Generic;
using System.Linq;

namespace WriterCore
{
    public static class StoryFactory
    {
        private static IDb s_db;
        private static ICollection<Story> s_stories;

        public static ICollection<Story> GetStories(IDb db)
        {
            s_db = db ?? throw new ArgumentNullException(nameof(db));

            s_stories = s_db.GetStories();
            return s_stories;
        }

        public static void GetEvents(int storyId)
        {
            var story = GetStory(storyId);
            story.Events = s_db.GetEvents(storyId);
        }

        public static void AddStory(Story story)
        {
            if (story == null)
                throw new ArgumentNullException(nameof(story));

            s_stories.Add(story);
            s_db.AddStory(story);
        }

        public static void AddEvent(int storyId, int fatherId, Event child)
        {
            if (child == null)
                throw new ArgumentNullException(nameof(child));

            child.StoryId = storyId;
            child.FatherId = fatherId;
            child.Saved = true;
            child.SaveTime = DateTime.Now;

            var story = GetStory(storyId);
            story.Events.Add(child);
            s_db.AddEvent(child);
        }

        private static Story GetStory(int id)
        {
            var story = s_stories.FirstOrDefault(o => o.Id == id);
            if (story == null)
                throw new ArgumentNullException(nameof(story));
            return story;
        }

        public static void DeleteStory(Story story)
        {
            if (story == null)
                throw new ArgumentNullException(nameof(story));

            foreach (var e in story.Events)
            {
                DeleteEvent(e);
            }

            s_stories.Remove(story);
            s_db.DeleteStory(story.Id);
        }

        public static void DeleteEvent(Event e)
        {
            if (e == null)
                throw new ArgumentNullException(nameof(e));

            var story = GetStory(e.StoryId);
            if (HasChild(story, e.Id))
            {
                RemoveChildren(story, e);
            }

            RemoveEvent(story, e);
        }

        private static bool HasChild(Story story, int fatherId)
        {
            return story.Events.Any(e => e.FatherId == fatherId);
        }

        private static void RemoveEvent(Story from, Event e)
        {
            from.Events.Remove(e);
            s_db.DeleteEvent(e.Id);
        }

        private static void RemoveChildren(Story story, Event father)
        {
            var children = from e in story.Events
                           where e.FatherId == father.Id
                           select e;
            foreach (var e in children)
            {
                RemoveEvent(story, e);
            }
        }

        public static void Save(int storyId)
        {
            var story = GetStory(storyId);

            s_db.UpdateStory(story);

            var children = from e in story.Events
                           where !e.Saved
                           select e;
            foreach (var e in children)
            {
                s_db.UpdateEvent(e);
            }
        }
    }
}
