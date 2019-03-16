using System;

namespace WriterCore.Model
{
    public sealed class Book
    {
        public Int64 Id { get; set; }
        public string Name { get; set; }
        public DateTime CreateTime { get; set; }
    }
}
