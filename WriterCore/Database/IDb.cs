using System.Collections.Generic;
using WriterCore.Model;

namespace WriterCore
{
    public interface IDb
    {
        bool Add(Fragment fragment);
        bool Delete(Fragment fragment);
        bool Update(Fragment fragment);
        IEnumerable<Fragment> FindAll();
    }
}