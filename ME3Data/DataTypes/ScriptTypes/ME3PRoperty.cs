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

        public PropertyFlags PropertyFlags;

        // Network
        public UInt16 ReplicateOffset;
        public UInt16 ReplicateIndex;

        public String Category;

        private NameReference _CategoryNameRef;


        public ME3Property(ObjectReader data, ExportTableEntry exp, PCCFile pcc)
            : base(data, exp, pcc)
        {

        }

        public override bool Deserialize()
        {
            var result = base.Deserialize();

            var ArrayInfo = Data.ReadInt32();
            ArraySize = (UInt16)(ArrayInfo & 0x0000FFFFU);
            ArrayElementSize = (UInt16)(ArrayInfo >> 16);

            PropertyFlags = (PropertyFlags)Data.ReadUInt64();
            if (PropertyFlags.HasFlag(PropertyFlags.Net))
                ReplicateOffset = Data.ReadUInt16();

            _CategoryNameRef = Data.ReadNameRef();
            Category = PCC.GetName(_CategoryNameRef);

            ReplicateOffset = Data.ReadUInt16(); // TODO: verify, see code
            ReplicateIndex = Data.ReadUInt16();

            return result;
        }

        public override bool ResolveLinks()
        {
            var result = base.ResolveLinks();

            return result;
        }
    }
}
