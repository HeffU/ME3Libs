using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ME3Data.DataTypes.ScriptTypes
{
    public class ME3Struct : ME3Field
    {
        public Int32 FirstChildIndex;

        // Script-related
        public Int32 ByteScriptSize;
        public Int32 DataScriptSize;
        public byte[] DataScript;

    }
}
