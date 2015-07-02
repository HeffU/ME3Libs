using ME3Data.DataTypes.ScriptTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ME3Data.DataTypes.NativeImports
{
    public class NativeME3Class : ME3Class
    {
        private String _name;
        public override String Name { get { return _name; } }

        public NativeME3Class(String name)
            :base(null, null, null)
        {
            _name = name;
        }
    }
}
