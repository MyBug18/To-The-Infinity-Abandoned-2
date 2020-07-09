using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infinity
{
    public interface ISaveAndLoadable
    {
        string Save();

        void Load(string data);
    }
}
