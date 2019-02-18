using System.Collections.Generic;

namespace WriterCore
{
    public interface IDb
    {
        void AddStory(Story story);
        void AddEvent(Event child);

        void DeleteStory(int id);
        void DeleteEvent(int id);

        void UpdateEvent(Event e);
        void UpdateStory(Story story);

        ICollection<Story> GetStories();
        ICollection<Event> GetEvents(int storyId);
    }
}