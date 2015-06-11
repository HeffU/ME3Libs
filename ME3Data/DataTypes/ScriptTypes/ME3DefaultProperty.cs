using ME3Data.DataTypes.ScriptTypes.DefaultProperties;
using ME3Data.DataTypes.ScriptTypes.DefaultProperties.BioWareValues;
using ME3Data.DataTypes.ScriptTypes.DefaultProperties.PredefinedStructs;
using ME3Data.DataTypes.ScriptTypes.DefaultProperties.PredefinedStructs.BioWareStructValues;
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
        InterfaceProperty = 11,

        // Built-in struct properties:
        Vector = InterfaceProperty + 1,
        Color = Vector + 1,
        LinearColor = Vector + 2,
        TwoVectors = Vector + 3,
        Vector4 = Vector + 4,
        Vector2D = Vector + 5,
        Rotator = Vector + 6,
        Guid = Vector + 7,
        Sphere = Vector + 8,
        Plane = Vector + 9,
        Scale = Vector + 10,
        Box = Vector + 11,
        Quat = Vector + 12,
        Matrix = Vector + 13,
        IntPoint = Vector + 14,

        // Bioware-specific:
        BioRwBox = IntPoint + 1,
        RwVector3 = BioRwBox + 1,
        BioMask4Property = BioRwBox + 2,
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

    public class ME3DefaultProperty
    {
        public String Name;
        public String SecondaryName;
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
            if (String.Equals(Name, "None", StringComparison.OrdinalIgnoreCase))
                return false;

            if (NameRef.ModNumber > 32) // Some weird inner name
            {                           // TODO: figure this out!
                NameReference secondary;
                secondary.Index = NameRef.ModNumber;
                secondary.ModNumber = Data.ReadInt32();
                SecondaryName = PCC.GetName(secondary);
            } else if (NameRef.ModNumber > 0)
            {
                Name = Name + "_" + NameRef.ModNumber;
            }

            if (Name == String.Empty && SecondaryName == String.Empty) //(SecondaryName == String.Empty || SecondaryName == null))
                return false;

            TypeNameRef = Data.ReadNameRef();
            if (TypeNameRef.ModNumber != 0) // another weird thing, this type name something unknown, but possibly the modnumber represents component type?
            {
                Data.ReadInt32();
                TypeName = PCC.Names[TypeNameRef.ModNumber];
                var pTypeName = PCC.GetName(Data.ReadNameRef());
                if (String.Equals(TypeName, "None", StringComparison.OrdinalIgnoreCase))
                    return false; //TODO: Sort this arcane shit out.
                Type = (PropertyType)Enum.Parse(typeof(PropertyType), pTypeName);
            }
            else
            {
                TypeName = PCC.GetName(TypeNameRef);
                if (TypeName == String.Empty)
                    return false;
                Type = (PropertyType)Enum.Parse(typeof(PropertyType), TypeName);
            }

            Size = Data.ReadUInt32();
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
                    if (size == 8) // if it's an enum-based byte value
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

                case PropertyType.InterfaceProperty:
                    value = new InterfacePropertyValue(Data, PCC);
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

                #region BioWare

                case PropertyType.BioMask4Property:
                    value = new BioMask4PropertyValue(Data, PCC);
                    return value.Deserialize();
                #endregion


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
                    value = new TwoVectorsPropertyValue(Data, PCC);
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

                case PropertyType.BioRwBox:
                    value = new BioRwBoxPropertyValue(Data, PCC);
                    return value.Deserialize();

                case PropertyType.RwVector3:
                    value = new RwVector3PropertyValue(Data, PCC);
                    return value.Deserialize();

                #endregion

                default:
                    return false;
            }
        }
    }
}
