using System;

namespace WriterCore.Model
{
    public sealed class Fragment
    {
        public Int64 Id { get; set; }
        public Int64 BookId { get; set; }
        public string Title { get; set; }
        public string Summary { get; set; }
        public DateTime CreateTime { get; set; }
        public string Author { get; set; }
    }
}
