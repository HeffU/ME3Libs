using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ME3Data.DataTypes.ScriptTypes
{
    public class ME3Property : ME3Field
    {
        // Array Info
        public UInt16 ArraySize;
        public UInt16 ArrayElementSize;

        public UInt64 Flags;

        // Network
        public UInt16 ReplicateOffset;
    }
}
