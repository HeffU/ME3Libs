using ME3Data.FileFormats.PCC;
using ME3Data.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ME3Data.DataTypes.ScriptTypes.Properties
{
    public class ME3ClassProperty : ME3ObjectProperty
    {
        public ME3Class Class;

        private Int32 _ClassIndex;

        public ME3ClassProperty(ObjectReader data, ExportTableEntry exp, PCCFile pcc)
            : base(data, exp, pcc)
        {

        }

        public override bool Deserialize()
        {
            var result = base.Deserialize();

            _ClassIndex = Data.ReadIndex();

            return result;
        }

        public override bool ResolveLinks()
        {
            var result = base.ResolveLinks();

            var entry = PCC.GetObjectEntry(_ClassIndex);
            if (entry != null)
            {
                Class = PCC.GetObjectEntry(_ClassIndex).Object as ME3Class;
            }

            if (Class == null) // TODO
                return true; // this should be false, but until native objects are handled by the library this will have to do.

            return result;
        }
    }
}
