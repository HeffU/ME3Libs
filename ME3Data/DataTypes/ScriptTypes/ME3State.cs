using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ME3Data.DataTypes.ScriptTypes
{
    public class ME3State : ME3Struct
    {
        // Function masks
        public Int32 ProbeMask;
        public Int64 IgnoreMask;

        public Int16 LabelTableOffset;
        public Int32 StateFlags;

        public Int32 FunctionMapCount;
        public List<ME3Function> FunctionMap;
    }
}
