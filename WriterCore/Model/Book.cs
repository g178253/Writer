using System;

namespace WriterCore.Model
{
    public sealed class Book : IOutline
    {
        public Int64 Id { get; set; }
        public string Title { get; set; }
        public DateTime CreateTime { get; set; }
    }
}
