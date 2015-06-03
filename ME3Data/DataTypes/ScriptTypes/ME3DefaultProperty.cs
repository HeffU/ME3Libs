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

    public abstract class DefaultPropertyValue
    {
        public ObjectReader Data;
        public PCCFile PCC;
        public UInt32 Size;

        public DefaultPropertyValue(ObjectReader data, PCCFile pcc, UInt32 size)
        {
            Data = data;
            PCC = pcc;
            Size = size;
        }

        public abstract bool Deserialize();
    }

    public class BoolPropertyValue : DefaultPropertyValue
    {
        public bool Value;

        public BoolPropertyValue(ObjectReader data, PCCFile pcc, UInt32 size)
            : base(data, pcc, size) { }

        public override bool Deserialize()
        {
            Value = Data.ReadByte() > 0;
            return true;
        }
    }

    public class BytePropertyValue : DefaultPropertyValue
    {
        public String EnumName;
        public String EnumValue;

        public BytePropertyValue(ObjectReader data, PCCFile pcc, UInt32 size)
            : base(data, pcc, size) { }

        public override bool Deserialize()
        {
            EnumName = PCC.GetName(Data.ReadNameRef());
            EnumValue = PCC.GetName(Data.ReadNameRef());
            return true;
        }
    }

    public class IntPropertyValue : DefaultPropertyValue
    {
        public Int32 Value;

        public IntPropertyValue(ObjectReader data, PCCFile pcc, UInt32 size)
            : base(data, pcc, size) { }

        public override bool Deserialize()
        {
            Value = Data.ReadInt32();
            return true;
        }
    }

    public class FloatPropertyValue : DefaultPropertyValue
    {
        public float Value;

        public FloatPropertyValue(ObjectReader data, PCCFile pcc, UInt32 size)
            : base(data, pcc, size) { }

        public override bool Deserialize()
        {
            Value = Data.ReadFloat();
            return true;
        }
    }

    public class StrPropertyValue : DefaultPropertyValue
    {
        public String Value;

        public StrPropertyValue(ObjectReader data, PCCFile pcc, UInt32 size)
            : base(data, pcc, size) { }

        public override bool Deserialize()
        {
            Value = Data.ReadString();
            return true;
        }
    }

    public class StringRefPropertyValue : DefaultPropertyValue
    {
        // TODO
        public byte[] Value;

        public StringRefPropertyValue(ObjectReader data, PCCFile pcc, UInt32 size)
            : base(data, pcc, size) { }

        public override bool Deserialize()
        {
            Value = Data.ReadRawData((int)Size);
            return true;
        }
    }

    public class NamePropertyValue : DefaultPropertyValue
    {
        public String Name;

        public NamePropertyValue(ObjectReader data, PCCFile pcc, UInt32 size)
            : base(data, pcc, size) { }

        public override bool Deserialize()
        {
            Name = PCC.GetName(Data.ReadNameRef());
            return true;
        }
    }

    public class ObjectPropertyValue : DefaultPropertyValue
    {
        public ME3Object Object { get { return PCC.GetObject(Index); } }
        public Int32 Index;

        public ObjectPropertyValue(ObjectReader data, PCCFile pcc, UInt32 size)
            : base(data, pcc, size) { }

        public override bool Deserialize()
        {
            Index = Data.ReadIndex();
            return true;
        }
    }

    public class DelegatePropertyValue : DefaultPropertyValue
    {
        // TODO
        public byte[] Value;

        public DelegatePropertyValue(ObjectReader data, PCCFile pcc, UInt32 size)
            : base(data, pcc, size) { }

        public override bool Deserialize()
        {
            Value = Data.ReadRawData((int)Size);
            return true;
        }
    }

    public class StructPropertyValue : DefaultPropertyValue
    {
        // TODO
        public String StructName;
        public byte[] Value;

        public StructPropertyValue(ObjectReader data, PCCFile pcc, UInt32 size)
            : base(data, pcc, size) { }

        public override bool Deserialize()
        {
            StructName = PCC.GetName(Data.ReadNameRef());
            Value = Data.ReadRawData((int)Size - 8);
            return true;
        }
    }

    public class ArrayPropertyValue : DefaultPropertyValue
    {
        // TODO
        public byte[] Value;

        public ArrayPropertyValue(ObjectReader data, PCCFile pcc, UInt32 size)
            : base(data, pcc, size) { }

        public override bool Deserialize()
        {
            Value = Data.ReadRawData((int)Size);
            return true;
        }
    }

    public class ME3DefaultProperty
    {
        public String Name;
        public String TypeName;
        public PropertyType Type;

        public UInt32 Size;
        public UInt32 ArrayIndex;

        public DefaultPropertyValue Value;

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

            switch (Type) // Adjust size for certain types:
            {
                case PropertyType.BoolProperty:
                    Value = new BoolPropertyValue(Data, PCC, Size);
                    if (!Value.Deserialize())
                        return false;
                    break;
                case PropertyType.ByteProperty:
                    Value = new BytePropertyValue(Data, PCC, Size);
                    if (!Value.Deserialize())
                        return false;
                    break;
                case PropertyType.IntProperty:
                    Value = new IntPropertyValue(Data, PCC, Size);
                    if (!Value.Deserialize())
                        return false;
                    break;
                case PropertyType.FloatProperty:
                    Value = new FloatPropertyValue(Data, PCC, Size);
                    if (!Value.Deserialize())
                        return false;
                    break;
                case PropertyType.StrProperty:
                    Value = new StrPropertyValue(Data, PCC, Size);
                    if (!Value.Deserialize())
                        return false;
                    break;
                case PropertyType.StringRefProperty:
                    Value = new StringRefPropertyValue(Data, PCC, Size);
                    if (!Value.Deserialize())
                        return false;
                    break;
                case PropertyType.NameProperty:
                    Value = new NamePropertyValue(Data, PCC, Size);
                    if (!Value.Deserialize())
                        return false;
                    break;
                case PropertyType.ObjectProperty:
                    Value = new ObjectPropertyValue(Data, PCC, Size);
                    if (!Value.Deserialize())
                        return false;
                    break;
                case PropertyType.DelegateProperty:
                    Value = new DelegatePropertyValue(Data, PCC, Size);
                    if (!Value.Deserialize())
                        return false;
                    break;
                case PropertyType.StructProperty:
                    Value = new StructPropertyValue(Data, PCC, Size);
                    if (!Value.Deserialize())
                        return false;
                    break;
                case PropertyType.ArrayProperty:
                    Value = new ArrayPropertyValue(Data, PCC, Size);
                    if (!Value.Deserialize())
                        return false;
                    break;
            }

            return true;
        }
    }
}
