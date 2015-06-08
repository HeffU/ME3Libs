using ME3Data.FileFormats.PCC;
using ME3Data.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ME3Data.DataTypes.ScriptTypes.Properties
{
    public class ME3ObjectProperty : ME3Property
    {
        public ME3Object Object;

        private Int32 _ObjectIndex;

        public ME3ObjectProperty(ObjectReader data, ExportTableEntry exp, PCCFile pcc)
            : base(data, exp, pcc)
        {

        }

        public override bool Deserialize()
        {
            var result = base.Deserialize();

            _ObjectIndex = Data.ReadIndex();

            return result;
        }

        public override bool ResolveLinks()
        {
            var result = base.ResolveLinks();

            var entry = PCC.GetObjectEntry(_ObjectIndex); // links to name list, redo this whole thing.
            if (entry != null)
            {
                Object = PCC.GetObjectEntry(_ObjectIndex).Object as ME3Object;
            }
            if (Object == null)
                return true; // should be false.

            return result;
        }
    }
}
