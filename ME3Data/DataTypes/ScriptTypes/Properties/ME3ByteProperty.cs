using ME3Data.FileFormats.PCC;
using ME3Data.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ME3Data.DataTypes.ScriptTypes.Properties
{
    public class ME3ByteProperty : ME3Property
    {
        public ME3Enum Enum;

        private Int32 _EnumIndex;

        public ME3ByteProperty(ObjectReader data, ExportTableEntry exp, PCCFile pcc)
            : base(data, exp, pcc)
        {

        }

        public override bool Deserialize()
        {
            var result = base.Deserialize();

            _EnumIndex = Data.ReadIndex();

            return result;
        }

        public override bool ResolveLinks()
        {
            var result = base.ResolveLinks();

            var entry = PCC.GetObjectEntry(_EnumIndex);
            if (entry != null)
            {
                Enum = PCC.GetObjectEntry(_EnumIndex).Object as ME3Enum;
            }

            if (Enum == null)
                return false;

            return result;
        }
    }
}
