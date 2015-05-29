using ME3Data.FileFormats.PCC;
using ME3Data.Utility;
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

        public UInt64 PropertyFlags;

        // Network
        public UInt16 ReplicateOffset;

        public ME3Property(ObjectReader data, ExportTableEntry exp, PCCFile pcc)
            : base(data, exp, pcc)
        {

        }

        public bool Deserialize()
        {
            base.Deserialize();

            var ArrayInfo = Data.ReadInt32();
            ArraySize = (UInt16)(ArrayInfo & 0x0000FFFFU);
            ArrayElementSize = (UInt16)(ArrayInfo >> 16);

            PropertyFlags = Data.ReadUInt64();
            if (false) // Has .Net flag
                ReplicateOffset = Data.ReadUInt16();

            return true;
        }
    }
}
