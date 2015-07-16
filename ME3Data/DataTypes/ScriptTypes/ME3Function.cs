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

        public Byte OperatorPrecedence;

        public FunctionFlags FunctionFlags;

        public UInt16 ReplicateOffset;

        public List<ME3Property> Parameters;

        public List<ME3Property> LocalVariables;

        public ME3Property ReturnValue;

        public ME3Function(ObjectReader data, ExportTableEntry exp, PCCFile pcc)
            : base(data, exp, pcc)
        {

        }

        public override bool Deserialize()
        {
            var result = base.Deserialize();

            NativeToken = Data.ReadUInt16();

            //OperatorPrecedence = Data.ReadByte(); // Does not appear in ME3?

            FunctionFlags = (FunctionFlags)Data.ReadInt32();

            if (FunctionFlags.HasFlag(FunctionFlags.Net))
                ReplicateOffset = Data.ReadUInt16();

            return result;
        }

        public override bool ResolveLinks()
        {
            var result = base.ResolveLinks();

            Parameters = new List<ME3Property>();
            LocalVariables = new List<ME3Property>();
            foreach(var variable in Variables)
            {
                if (variable.PropertyFlags.HasFlag(PropertyFlags.ReturnParm))
                {
                    ReturnValue = variable;
                }
                else if (variable.PropertyFlags.HasFlag(PropertyFlags.Parm))
                {
                    Parameters.Add(variable);
                }
                else
                {
                    LocalVariables.Add(variable);
                }
            }

            return result;
        }
    }
}
