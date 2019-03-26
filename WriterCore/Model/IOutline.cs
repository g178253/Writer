using System;

namespace WriterCore.Model
{
    public interface IOutline
    {
        Int64 Id { get; }
        string Title { get; set; }
    }
}
