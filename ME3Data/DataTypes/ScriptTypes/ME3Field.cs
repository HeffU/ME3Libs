using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ME3Data.DataTypes.ScriptTypes
{
    public class ME3Field : ME3Object
    {
        public ME3Field SuperField;
        public ME3Field NextField;

        protected int _SuperIndex;
        protected int _NextIndex;
    }
}
