using ME3Data.FileFormats.PCC;
using ME3Data.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ME3Data.DataTypes.ScriptTypes.Properties
{
    public class ME3StructProperty : ME3Property
    {
        public ME3Object Struct;

        private Int32 _StructIndex;

        public ME3StructProperty(ObjectReader data, ExportTableEntry exp, PCCFile pcc)
            : base(data, exp, pcc)
        {

        }

        public override bool Deserialize()
        {
            var result = base.Deserialize();

            _StructIndex = Data.ReadIndex();

            return result;
        }

        public override bool ResolveLinks()
        {
            var result = base.ResolveLinks();

            var entry = PCC.GetObjectEntry(_StructIndex);
            if (entry != null)
            {
                Struct = PCC.GetObjectEntry(_StructIndex).Object as ME3Object;
            }
            if (Struct == null) // TODO
                return true; // this should be false, but until native objects are handled by the library this will have to do.

            return result;
        }
    }
}
