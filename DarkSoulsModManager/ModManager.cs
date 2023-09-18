using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DarkSoulsModManager
{
    class ModManager
    {
        public bool merged;
        public bool mergeCheck()
        {
            merged = true;
            return merged;
        }
        public bool resetCheck()
        {
            merged = false;
            return merged;
        }
    }
}
