using System;
using System.Collections.Generic;
using System.Text;

namespace WriterCore
{
    internal static class DbFactory
    {
        public static IDb Db { get { return new Db(); } }
    }
}
