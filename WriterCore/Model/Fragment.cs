using System;

namespace WriterCore.Model
{
    public sealed class Fragment
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Summary { get; set; }
        public DateTime CreateTime { get; set; }
        public string Author { get; set; }
    }
}
