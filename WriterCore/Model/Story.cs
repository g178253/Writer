using System;
using System.Collections.Generic;

namespace WriterCore
{
    public sealed class Story
    {
        public ICollection<Event> Events { get; set; }

        public int Id { get; set; }
        public string Title { get; set; }
        public string Summary { get; set; }
        public DateTime CreateTime { get; set; }
        public string Author { get; set; }
    }
}
