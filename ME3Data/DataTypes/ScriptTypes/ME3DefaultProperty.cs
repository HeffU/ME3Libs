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
        BoolProperty = 0,
        ByteProperty = 1,
        IntProperty = 2,
        FloatProperty = 3,
        StrProperty = 4,
        StringRefProperty = 5,
        NameProperty = 6,
        ObjectProperty = 7,
        DelegateProperty = 8,
        StructProperty = 9,
        ArrayProperty = 10,

        // Built-in struct properties:
        Vector = 11,
        Color = 12,
        LinearColor = 13,
        TwoVectors = 14,
        Vector4 = 15,
        Vector2D = 16,
        Rotator = 17,
        Guid = 18,
        Sphere = 19,
        Plane = 20,
        Scale = 21,
        Box = 22,
        Quat = 23,
        Matrix = 24,
        IntPoint = 25
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

        public BoolPropertyValue(ObjectReader data, PCCFile pcc)
            : base(data, pcc, 1) { }

        public override bool Deserialize()
        {
            Value = Data.ReadByte() > 0;
            return true;
        }
    }

    public class BytePropertyValue : DefaultPropertyValue
    {
        public byte Value;

        public BytePropertyValue(ObjectReader data, PCCFile pcc)
            : base(data, pcc, 1) { }

        public override bool Deserialize()
        {
            Value = Data.ReadByte();
            return true;
        }
    }

    public class EnumPropertyValue : DefaultPropertyValue
    {
        public String EnumName;
        public String EnumValue;

        public EnumPropertyValue(ObjectReader data, PCCFile pcc, UInt32 size, String name)
            : base(data, pcc, size)
        {
            EnumName = name;
        }


        public override bool Deserialize()
        {
            EnumValue = PCC.GetName(Data.ReadNameRef());
            return true;
        }
    }

    public class IntPropertyValue : DefaultPropertyValue
    {
        public Int32 Value;

        public IntPropertyValue(ObjectReader data, PCCFile pcc)
            : base(data, pcc, 4) { }

        public override bool Deserialize()
        {
            Value = Data.ReadInt32();
            return true;
        }
    }

    public class FloatPropertyValue : DefaultPropertyValue
    {
        public float Value;

        public FloatPropertyValue(ObjectReader data, PCCFile pcc)
            : base(data, pcc, 4) { }

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
            Value = PCC.GetName(Data.ReadNameRef());
            return true;
        }
    }

    public class StringRefPropertyValue : DefaultPropertyValue
    {
        public Int32 Value;

        public StringRefPropertyValue(ObjectReader data, PCCFile pcc)
            : base(data, pcc, 4) { }

        public override bool Deserialize()
        {
            Value = Data.ReadInt32();
            return true;
        }
    }

    public class NamePropertyValue : DefaultPropertyValue
    {
        public String Name;

        public NamePropertyValue(ObjectReader data, PCCFile pcc)
            : base(data, pcc, 8) { }

        public override bool Deserialize()
        {
            Name = PCC.GetName(Data.ReadNameRef());
            return true;
        }
    }

    public class ObjectPropertyValue : DefaultPropertyValue
    {
        public ME3Object Object { get { return PCC.GetExportObject(Index); } }
        public Int32 Index;

        public ObjectPropertyValue(ObjectReader data, PCCFile pcc)
            : base(data, pcc, 4) { }

        public override bool Deserialize()
        {
            Index = Data.ReadIndex();
            return true;
        }
    }

    public class DelegatePropertyValue : DefaultPropertyValue
    {
        public Int32 OuterIndex; // Object that contains the assigned delegate
        public String DelegateValue;
        public String DelegateName;

        public DelegatePropertyValue(ObjectReader data, PCCFile pcc, String name)
            : base(data, pcc, 12) 
        {
            DelegateName = name.Substring(2, name.Length - 12);
        }

        public override bool Deserialize()
        {
            OuterIndex = Data.ReadIndex();
            DelegateValue = PCC.GetName(Data.ReadNameRef());
            return true;
        }
    }

    public class StructPropertyValue : DefaultPropertyValue
    {
        public String StructName;
        public List<DefaultPropertyValue> MemberValues;

        public byte[] Value;
        public List<ME3DefaultProperty> MemberProperties;

        public StructPropertyValue(ObjectReader data, PCCFile pcc, UInt32 size, String name)
            : base(data, pcc, size) 
        {
            StructName = name;
        }

        public override bool Deserialize()
        {
            MemberProperties = new List<ME3DefaultProperty>();
            var current = new ME3DefaultProperty(Data, PCC);

            while (current.Deserialize())
            {
                MemberProperties.Add(current);
                current = new ME3DefaultProperty(Data, PCC);
            }
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
            if (TypeNameRef.ModNumber != 0) // another weird thing, this type name something unknown, but possibly the modnumber represents component type?
            {
                Data.ReadInt32();
                TypeName = PCC.Names[TypeNameRef.ModNumber];
                var pTypeName = PCC.GetName(Data.ReadNameRef());
                Type = (PropertyType)Enum.Parse(typeof(PropertyType), pTypeName);
            }
            else
            {
                TypeName = PCC.GetName(TypeNameRef);
                Type = (PropertyType)Enum.Parse(typeof(PropertyType), TypeName);
            }

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

            return DeserializeValue(Type, out Value, Size);
        }

        public bool DeserializeValue(PropertyType type, out DefaultPropertyValue value, UInt32 size)
        {
            value = null;
            switch (type) // Adjust size for certain types:
            {
                case PropertyType.BoolProperty:
                    value = new BoolPropertyValue(Data, PCC);
                    return value.Deserialize();

                case PropertyType.ByteProperty:
                    var enumName = PCC.GetName(Data.ReadNameRef());
                    if (size == 16) // if it's an enum-based byte value
                        value = new EnumPropertyValue(Data, PCC, size, enumName);
                    else
                        value = new BytePropertyValue(Data, PCC);
                    return value.Deserialize();

                case PropertyType.IntProperty:
                    value = new IntPropertyValue(Data, PCC);
                    return value.Deserialize();

                case PropertyType.FloatProperty:
                    value = new FloatPropertyValue(Data, PCC);
                    return value.Deserialize();

                case PropertyType.StrProperty:
                    value = new StrPropertyValue(Data, PCC, size);
                    return value.Deserialize();

                case PropertyType.StringRefProperty:
                    value = new StringRefPropertyValue(Data, PCC);
                    return value.Deserialize();

                case PropertyType.NameProperty:
                    value = new NamePropertyValue(Data, PCC);
                    return value.Deserialize();

                case PropertyType.ObjectProperty:
                    value = new ObjectPropertyValue(Data, PCC);
                    return value.Deserialize();

                case PropertyType.DelegateProperty:
                    value = new DelegatePropertyValue(Data, PCC, Name);
                    return value.Deserialize();

                case PropertyType.StructProperty:
                    var structName = PCC.GetName(Data.ReadNameRef());
                    // If it's a hardcoded struct type, deserialize it as that.
                    if (Enum.GetNames(typeof(PropertyType)).Skip(11).Contains(structName))
                        return DeserializeValue((PropertyType)Enum.Parse(typeof(PropertyType), structName), out value, size);

                    // otherwise use the deserialization of a user-defined struct:
                    value = new StructPropertyValue(Data, PCC, size, structName);
                    return value.Deserialize();

                case PropertyType.ArrayProperty:
                    value = new ArrayPropertyValue(Data, PCC, size);
                    return value.Deserialize();

                #region Hardcoded Structs

                case PropertyType.Vector:
                    value = new VectorPropertyValue(Data, PCC);
                    return value.Deserialize();

                case PropertyType.Color:
                    value = new ColorPropertyValue(Data, PCC);
                    return value.Deserialize();

                case PropertyType.LinearColor:
                    value = new LinearColorPropertyValue(Data, PCC);
                    return value.Deserialize();

                case PropertyType.TwoVectors:
                    value = new VectorPairPropertyValue(Data, PCC);
                    return value.Deserialize();

                case PropertyType.Vector4:
                    value = new Vector4PropertyValue(Data, PCC);
                    return value.Deserialize();

                case PropertyType.Vector2D:
                    value = new Vector2DPropertyValue(Data, PCC);
                    return value.Deserialize();

                case PropertyType.Rotator:
                    value = new RotatorPropertyValue(Data, PCC);
                    return value.Deserialize();

                case PropertyType.Guid:
                    value = new GUIDPropertyValue(Data, PCC);
                    return value.Deserialize();

                case PropertyType.Sphere:
                    value = new SpherePropertyValue(Data, PCC);
                    return value.Deserialize();

                case PropertyType.Plane:
                    value = new PlanePropertyValue(Data, PCC);
                    return value.Deserialize();

                case PropertyType.Scale:
                    value = new ScalePropertyValue(Data, PCC);
                    return value.Deserialize();

                case PropertyType.Box:
                    value = new BoxPropertyValue(Data, PCC);
                    return value.Deserialize();

                case PropertyType.Quat:
                    value = new QuatPropertyValue(Data, PCC);
                    return value.Deserialize();

                case PropertyType.Matrix:
                    value = new MatrixPropertyValue(Data, PCC);
                    return value.Deserialize();

                case PropertyType.IntPoint:
                    value = new IntPointPropertyValue(Data, PCC);
                    return value.Deserialize();

                #endregion

                default:
                    return false;
            }
        }
    }
}
