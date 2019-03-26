using System;

namespace WriterCore.Model
{
    public sealed class Catalog : IOutline
    {
        public Int64 Id { get; set; }
        public Int64 BookId { get; set; }
        public Int64 FatherId { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public DateTime CreateTime { get; set; }
        public string Author { get; set; }
    }
}
