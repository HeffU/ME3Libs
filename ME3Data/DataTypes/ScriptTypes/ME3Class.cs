using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ME3Data.DataTypes.ScriptTypes
{
    public class ME3Class : ME3State
    {
        public Int32 ClassFlags;

        public ME3Class OuterClass;

        public Int32 ConfigNameIndex;

        public Int32 InterfaceCount;
        public List<ME3Class> ImplementedInterfaces;

        public Int32 ComponentCount;
        public List<ME3Object> Components;

        public Int32 DLLBindIndex;

        public Int32 DefaultPropertyIndex;

        public Int32 FunctionRefCount;
        public List<ME3Function> FunctionRefs;

    }
}
