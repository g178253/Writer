using WriterCore.Database;

namespace WriterCore
{
    internal static class DbFactory
    {
        public static IDb Db { get { return new MemoryDb(); } }
    }
}
