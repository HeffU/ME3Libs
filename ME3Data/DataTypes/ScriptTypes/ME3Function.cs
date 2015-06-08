using ME3Data.FileFormats.PCC;
using ME3Data.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ME3Data.DataTypes.ScriptTypes
{
    public class ME3Function : ME3Struct
    {
        public UInt16 NativeToken;

        public Byte OperatorPrescedence;

        public Int32 FunctionFlags;

        public UInt16 ReplicateOffset;

        public List<ME3Property> Parameters;

        public ME3Property ReturnValue;

        public ME3Function(ObjectReader data, ExportTableEntry exp, PCCFile pcc)
            : base(data, exp, pcc)
        {

        }

        public override bool Deserialize()
        {
            var result = base.Deserialize();

            NativeToken = Data.ReadUInt16();

            //OperatorPrescedence = Data.ReadByte(); // Does not appear in ME3?

            FunctionFlags = Data.ReadInt32();

            if (false) // Has .Net flag
                ReplicateOffset = Data.ReadUInt16();

            return result;
        }

        public override bool ResolveLinks()
        {
            var result = base.ResolveLinks();

            Parameters = new List<ME3Property>();
            foreach(var variable in Variables)
            {
                //TODO: check flag, set returnval or param.
            }

            return result;
        }
    }
}
