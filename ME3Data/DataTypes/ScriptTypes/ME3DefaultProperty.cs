using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ME3Data.DataTypes.ScriptTypes
{
    public class ME3DefaultProperty
    {
        public String Name;
        public String TypeName;

        // Common data
        public UInt32 Size;
        public UInt32 ArrayIndex;

        // Struct only
        public String MemberName;

        // Enum instance
        public String EnumName;

        // Bool only
        public Boolean BoolValue;
    }
}
