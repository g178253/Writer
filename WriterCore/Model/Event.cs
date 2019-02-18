using System;

namespace WriterCore
{
    public sealed class Event
    {
        public string Title { get; set; }
        public string Content { get; set; }
        public string Remarks { get; set; }

        public int Id { get; set; }
        public int FatherId { get; set; }
        public int StoryId { get; set; }
        public DateTime SaveTime { get; set; }

        public bool Saved { get; set; }
    }
}
