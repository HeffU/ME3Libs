using ME3Data.FileFormats.PCC;
using ME3Data.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ME3Data.DataTypes.ScriptTypes.Properties
{
    public class ME3DelegateProperty : ME3Property
    {
        public ME3Function Function;
        public ME3Function Delegate;

        public String FunctionName = "Unknown";
        public String DelegateName = "Unknown";

        private Int32 _FuncIndex;
        private Int32 _DeleIndex;

        public ME3DelegateProperty(ObjectReader data, ExportTableEntry exp, PCCFile pcc)
            : base(data, exp, pcc)
        {

        }

        public override bool Deserialize()
        {
            var result = base.Deserialize();

            _FuncIndex = Data.ReadIndex();
            _DeleIndex = Data.ReadIndex();

            return result;
        }

        public override bool ResolveLinks()
        {
            var result = base.ResolveLinks();

            var entry = PCC.GetObjectEntry(_FuncIndex);
            if (entry != null)
            {
                FunctionName = entry.ObjectName;
                Function = PCC.GetObjectEntry(_FuncIndex).Object as ME3Function;
            }
            if (Function == null) // TODO
                return true; // this should be false, but until native objects are handled by the library this will have to do.

            entry = PCC.GetObjectEntry(_DeleIndex);
            if (entry != null)
            {
                DelegateName = entry.ObjectName;
                Delegate = PCC.GetObjectEntry(_DeleIndex).Object as ME3Function;
            }
            if (Delegate == null) // TODO
                return true; // this should be false, but until native objects are handled by the library this will have to do.

            return result;
        }
    }
}
