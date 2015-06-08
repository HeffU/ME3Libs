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
        /// <summary>
        /// The enum type for this variable.
        /// Only valid is IsEnum is true.
        /// </summary>
        public ME3Enum Enum;

        public bool IsEnum;

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
                Enum = entry.Object as ME3Enum;
            }

            IsEnum = Enum == null ? false : true;

            return result;
        }
    }
}
