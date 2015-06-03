using ME3Data.FileFormats.PCC;
using ME3Data.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ME3Data.DataTypes.ScriptTypes
{
    public enum PropertyType
    {
        BoolProperty,
        ByteProperty,
        IntProperty,
        FloatProperty,
        StrProperty,
        StringRefProperty,
        NameProperty,
        ObjectProperty,
        DelegateProperty,
        StructProperty,
        ArrayProperty
    }

    public class ME3DefaultProperty
    {
        public String Name;
        public String TypeName;
        public PropertyType Type;

        // Common data
        public UInt32 Size;
        public UInt32 ArrayIndex;

        // Struct only
        public String MemberName;

        // Enum instance
        public String EnumName;

        // Bool only
        public Boolean BoolValue;

        //---

        private NameReference NameRef;
        private NameReference TypeNameRef;

        private PCCFile PCC;
        private ObjectReader Data;

        public ME3DefaultProperty(ObjectReader data, PCCFile pccObj)
        {
            Data = data;
            PCC = pccObj;
        }

        public bool Deserialize()
        {
            NameRef = Data.ReadNameRef();

            Name = PCC.GetName(NameRef);
            if (String.Equals(Name, "None", StringComparison.OrdinalIgnoreCase) || Name == String.Empty)
                return false;

            if (NameRef.ModNumber != 0) // Some weird inner name
                Data.ReadInt32();       // TODO: figure this out!

            TypeNameRef = Data.ReadNameRef();
            if (TypeNameRef.ModNumber != 0) // another weird thing, this type name is not valid at all.
                return false;

            TypeName = PCC.GetName(TypeNameRef);
            Type = (PropertyType)Enum.Parse(typeof(PropertyType), TypeName);

            Size = Data.ReadUInt32();
            switch (Type) // Adjust size for certain types:
            {
                case PropertyType.BoolProperty:
                    Size += 1;
                    break;
                case PropertyType.ByteProperty:
                    Size += 8;
                    break;
                case PropertyType.StructProperty:
                    Size += 8;
                    break;
            }

            ArrayIndex = Data.ReadUInt32();

            // Todo: read data depending on type.
            var skip = Data.ReadRawData((int)Size);

            return true;
        }
    }
}
