using ME3Data.DataTypes;
using ME3Data.DataTypes.ScriptTypes;
using ME3Data.DataTypes.ScriptTypes.Properties;
using ME3Data.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ME3Data.FileFormats.PCC
{
    public class PCCFile
    {
        /// <summary>
        /// The name of this file.
        /// </summary>
        public String Name;

        /// <summary>
        /// List of all Export objects in this PCC.
        /// </summary>
        public List<ExportTableEntry> Exports { get; private set; }

        /// <summary>
        /// List of all Import objects in this PCC.
        /// </summary>
        public List<ImportTableEntry> Imports { get; private set; }

        /// <summary>
        /// List of all names in this PCC.
        /// </summary>
        public List<String> Names { get; private set; }

        /// <summary>
        /// File flags for this PCC.
        /// </summary>
        public PackageFlags PackageFlags;

        /// <summary>
        /// A list of all PCC file names that this file depends on for its imports.
        /// </summary>
        public List<String> ImportPackageNames;

        /// <summary>
        /// A list of all import PCC files that have been loaded.
        /// </summary>
        public List<PCCFile> ImportPackages;

        public PCCStreamReader Data;

        private UInt32 _unkn1;
        private String _folderName;
        private UInt32 _unknDummy;

        private Int32 _nameCount;
        private Int32 _exportCount;
        private Int32 _importCount;

        private UInt32 _nameOffset;
        private UInt32 _exportOffset;
        private UInt32 _importOffset;

        private UInt32 _unkn2;
        private UInt32 _unkn3;

        private UInt32 _GUID_A;
        private UInt32 _GUID_B;
        private UInt32 _GUID_C;
        private UInt32 _GUID_D;

        private UInt32 _generations;

        private UInt32 _unkn4;
        private UInt32 _unkn5;

        private FileCompressionFlags _compressionFlag;
        private UInt32 _chunkCount;


        private static List<String> _NativeOnlyPackages = new List<String>
        {
            "EngineMaterials",
            "EngineResources",
            "SFXWwise_Init",
	        "Wwise_Generic_Foley_Procedural",
            "BioVFX_C_Blood",
            "Textures",
            "BioVFX_C_Stealth",
            "Materials",
            "BioVFX_Env_Hologram",
            "Explosion",
            "glow",
            "sparks",
            "Planes",
            "Cube_Maps",
            "Distortion",
            "Fractals",
            "Normals",
            "BioVFX_Z_TEXTURES",
            "BioVFX_Z_MESHES",
            "BioVFX_Z_MATERIALS",
            "BioVFX_C_Weapons",
            "Wwise_Weapons_Generic",
            "Wwise_Weapons_Standard_LowAmmo"
        };

        private static List<String> _CoreObjectTypes = new List<String>
        {
            "ArrayProperty",
            "BoolProperty",
            "ByteProperty",
            "Class",
            "ClassProperty",
            "ComponentProperty",
            "Const",
            "DelegateProperty",
            "Enum",
            "FloatProperty",
            "Function",
            "InterfaceProperty",
            "IntProperty",
            "MapProperty",
            "NameProperty",
            "ObjectProperty",
            "Package",
            "Property",
            "ScriptStruct",
            "State",
            "StringRefProperty",
            "StrProperty",
            "StructProperty",

            // These are from engine, probably ought to not be here.
            "Level",
            "StaticMesh"
        };


        public PCCFile(PCCStreamReader data, String name)
        {
            Name = name;
            Data = data;
            Exports = new List<ExportTableEntry>();
            Imports = new List<ImportTableEntry>();
            Names = new List<String>();
            ImportPackageNames = new List<String>();
            ImportPackages = new List<PCCFile>();
        }

        public bool Deserialize()
        {
            var header = Data.GetHeaderReader();
            if (!DeserializeHeader(header))
                return false;

            // Unsure if import table is always after name table, make a more dynamic solution.
            if (!DeserializeNames())
                return false;

            if (!DeserializeImports())
                return false;

            if (!DeserializeExports())
                return false;

            // TODO: handle extra name table.

            foreach (ExportTableEntry export in Exports)
            {
                if (!DeserializeExportObject(export))
                    return false;
            }

            return true;
        }

        public bool ResolveLinks()
        {
            foreach (ExportTableEntry export in Exports)
            {
                if (!export.Object.ResolveLinks())
                    return false;
            }

            return true;
        }

        public void LoadDependencies(List<PCCFile> packages)
        {
            ImportPackages.AddRange(packages);
            foreach (var import in Imports)
            {
                var package = packages.FirstOrDefault(p => p.Name == import.SourcePCCName);
                if (package != null)
                    import.LoadFromSource(package);
            }
        }

        private bool DeserializeExportObject(ExportTableEntry entry)
        {
            switch (entry.ClassName)
            {
                case "Class":
                    var classObj = new ME3Class(Data.GetReader(entry.FileOffset, entry.Size), entry, this);
                    entry.Object = classObj;
                    return classObj.Deserialize();

                case "Function":
                    var funcObj = new ME3Function(Data.GetReader(entry.FileOffset, entry.Size), entry, this);
                    entry.Object = funcObj;
                    return funcObj.Deserialize();

                case "Struct":
                    var structObj = new ME3Struct(Data.GetReader(entry.FileOffset, entry.Size), entry, this);
                    entry.Object = structObj;
                    return structObj.Deserialize();

                case "ScriptStruct":
                    var scriptstructObj = new ME3ScriptStruct(Data.GetReader(entry.FileOffset, entry.Size), entry, this);
                    entry.Object = scriptstructObj;
                    return scriptstructObj.Deserialize();

                case "Enum":
                    var enumObj = new ME3Enum(Data.GetReader(entry.FileOffset, entry.Size), entry, this);
                    entry.Object = enumObj;
                    return enumObj.Deserialize();

                case "Const":
                    var constObj = new ME3Const(Data.GetReader(entry.FileOffset, entry.Size), entry, this);
                    entry.Object = constObj;
                    return constObj.Deserialize();

                case "State":
                    var stateObj = new ME3State(Data.GetReader(entry.FileOffset, entry.Size), entry, this);
                    entry.Object = stateObj;
                    return stateObj.Deserialize();

                case "ArrayProperty":
                    var arrayProp = new ME3ArrayProperty(Data.GetReader(entry.FileOffset, entry.Size), entry, this);
                    entry.Object = arrayProp;
                    return arrayProp.Deserialize();

                case "IntProperty":
                    var intProp = new ME3IntProperty(Data.GetReader(entry.FileOffset, entry.Size), entry, this);
                    entry.Object = intProp;
                    return intProp.Deserialize();

                case "BoolProperty":
                    var boolProp = new ME3BoolProperty(Data.GetReader(entry.FileOffset, entry.Size), entry, this);
                    entry.Object = boolProp;
                    return boolProp.Deserialize();

                case "ByteProperty":
                    var byteProp = new ME3ByteProperty(Data.GetReader(entry.FileOffset, entry.Size), entry, this);
                    entry.Object = byteProp;
                    return byteProp.Deserialize();

                case "ObjectProperty":
                    var objectProp = new ME3ObjectProperty(Data.GetReader(entry.FileOffset, entry.Size), entry, this);
                    entry.Object = objectProp;
                    return objectProp.Deserialize();

                case "ClassProperty":
                    var classProp = new ME3ClassProperty(Data.GetReader(entry.FileOffset, entry.Size), entry, this);
                    entry.Object = classProp;
                    return classProp.Deserialize();

                case "ComponentProperty":
                    var compProp = new ME3ComponentProperty(Data.GetReader(entry.FileOffset, entry.Size), entry, this);
                    entry.Object = compProp;
                    return compProp.Deserialize();

                case "DelegateProperty":
                    var deleProp = new ME3DelegateProperty(Data.GetReader(entry.FileOffset, entry.Size), entry, this);
                    entry.Object = deleProp;
                    return deleProp.Deserialize();

                case "FixedArrayProperty":
                    var fixedProp = new ME3FixedArrayProperty(Data.GetReader(entry.FileOffset, entry.Size), entry, this);
                    entry.Object = fixedProp;
                    return fixedProp.Deserialize();

                case "FloatProperty":
                    var floatProp = new ME3FloatProperty(Data.GetReader(entry.FileOffset, entry.Size), entry, this);
                    entry.Object = floatProp;
                    return floatProp.Deserialize();

                case "InterfaceProperty":
                    var interfaceProp = new ME3InterfaceProperty(Data.GetReader(entry.FileOffset, entry.Size), entry, this);
                    entry.Object = interfaceProp;
                    return interfaceProp.Deserialize();

                case "NameProperty":
                    var nameProp = new ME3NameProperty(Data.GetReader(entry.FileOffset, entry.Size), entry, this);
                    entry.Object = nameProp;
                    return nameProp.Deserialize();

                case "StrProperty":
                    var strProp = new ME3StrProperty(Data.GetReader(entry.FileOffset, entry.Size), entry, this);
                    entry.Object = strProp;
                    return strProp.Deserialize();

                case "StructProperty":
                    var structProp = new ME3StructProperty(Data.GetReader(entry.FileOffset, entry.Size), entry, this);
                    entry.Object = structProp;
                    return structProp.Deserialize();

                default :
                    var obj = new ME3Object(Data.GetReader(entry.FileOffset, entry.Size), entry, this);
                    entry.Object = obj;
                    return obj.Deserialize();
            }
        }

        public ExportTableEntry GetExportByName(String name)
        {
            return Exports.FirstOrDefault(x => String.Equals(x.ObjectName, name, StringComparison.OrdinalIgnoreCase));
        }

        public static ExportTableEntry GetExportFromImport(ImportTableEntry entry)
        {
            if (entry.ClassName == "Package")
            {
                return new ExportTableEntry(null, null);
                // TODO: this imports the whole package?
            }

            var OuterTree = entry.GetOuterTreeString().Split(new char[] {'.'}, 2, StringSplitOptions.RemoveEmptyEntries);
            if (OuterTree[0] == entry.CurrentPCC.Name 
                || _NativeOnlyPackages.Contains(OuterTree[0])
                || _CoreObjectTypes.Contains(entry.ObjectName))
            {
                return new ExportTableEntry(null, null);
                // TODO: this means its a fully native object, we should create those for all types required by ME3.
            }

            if (entry.SourcePCCName != OuterTree[0])
            {
                var realPCC = entry.CurrentPCC.ImportPackages.FirstOrDefault(x => x.Name == OuterTree[0]);
                if (realPCC == null)
                    return null; // Error, missing import?

                entry.SourcePCC = realPCC;
            }

            var objects = entry.SourcePCC.Exports.Where(x => // TODO: can probably be much quicker, perhaps by outer tree traversal?
                String.Equals(x.ObjectName, entry.ObjectName, StringComparison.OrdinalIgnoreCase) &&
                String.Equals(x.ClassName, entry.ClassName, StringComparison.OrdinalIgnoreCase));
            if (objects.Count() > 1)
            {
                if (OuterTree.Length == 2)
                {
                    objects = objects.Where(x => String.Equals(x.GetOuterTreeString(), OuterTree[1], StringComparison.OrdinalIgnoreCase));
                } 
                else
                {
                    objects = objects.Where(x => x.GetOuterTreeString() == String.Empty); // should not happen?
                }
            }
            if (objects.Count() != 1)
            {
                return null; // This should not happen unless we have a faulty import.
            }
            return objects.First();
        }

        public String GetName(NameReference reference)
        {
            /*if (reference.Index == 0 && reference.ModNumber >= 0)
                return String.Empty; // Error, weird mod number!*/
            return reference.Index >= 0 && reference.Index < Names.Count ? Names[reference.Index] : String.Empty;
        }

        public ME3Object GetExportObject(Int32 objIndex)
        {
            if (objIndex <= 0)
                return null;
            else
                return Exports[objIndex - 1].Object;
        }

        public ObjectTableEntry GetObjectEntry(Int32 index)
        {
            if (index < 0)
            {
                var proper = -index - 1;
                if (proper < 0 || proper >= Imports.Count)
                    return null;
                return Imports[proper];
            } 
            else if (index > 0 && index <= Exports.Count)
            {
                return Exports[index - 1];
            }

            return null;
        }

        public String GetClassName(int objIndex)
        {
            if (objIndex < 0)
                return Imports[(objIndex * -1) - 1].ObjectName;
            else if (objIndex > 0)
                return Exports[objIndex - 1].ObjectName;
            else
                return "Class";
        }

        private bool DeserializeHeader(ObjectReader header)
        {
            // Not a PCC file.
            if (header.ReadUInt32() != 0x9E2A83C1)
                return false;
            
            // Unsupported unreal engine version
            if (header.ReadUInt16() != 0x02AC || header.ReadUInt16() != 0xC2)
                return false;

            _unkn1 = header.ReadUInt32();
            _folderName = header.ReadString();
            PackageFlags = (PackageFlags)header.ReadUInt32();
            _unknDummy = header.ReadUInt32();

            _nameCount = header.ReadInt32();
            _nameOffset = header.ReadUInt32();
            _exportCount = header.ReadInt32();
            _exportOffset = header.ReadUInt32();
            _importCount = header.ReadInt32();
            _importOffset = header.ReadUInt32();

            _unkn2 = header.ReadUInt32(); // Both of these often match _unkn1.
            _unkn3 = header.ReadUInt32(); // UnHood references "first export offset", cant see that working here?

            header.ReadRawData(12); // seemingly 0's

            _GUID_A = header.ReadUInt32();
            _GUID_B = header.ReadUInt32();
            _GUID_C = header.ReadUInt32();
            _GUID_D = header.ReadUInt32();

            _generations = header.ReadUInt32();
            header.ReadRawData(4); // Copy of export count?
            header.ReadRawData(4); // Copy of name count?
            header.ReadRawData(4); // Another 0

            // Engine version mis-match
            if (header.ReadUInt32() != 0x18EF)
                return false;
            // Cooker version mis-match
            if (header.ReadUInt32() != 0x3006B)
                return false;

            _unkn4 = header.ReadUInt32(); // Seems to be the same for all files.
            _unkn5 = header.ReadUInt32(); // Same as above, UnHood possibly refers to this as package flags, old remnants?

            _compressionFlag = (FileCompressionFlags)header.ReadUInt32();
            _chunkCount = header.ReadUInt32();
            // TODO: support compressed files..

            return true;
        }

        private bool DeserializeNames()
        {
            ObjectReader table;
            Int32 size;
            UInt32 offset = 0;
            for (int n = 0; n < _nameCount; n++)
            {
                table = Data.GetReader(_nameOffset + offset, 4);
                size = -table.ReadInt32(); // assuming unicode

                table = Data.GetReader(_nameOffset + offset + 4, (UInt32)size * 2);
                Names.Add(Encoding.Unicode.GetString(table.ReadRawData(size * 2)).Substring(0, size - 1));

                offset += ((UInt32)size * 2) + 4;
            }
            return true;
        }

        private bool DeserializeImports()
        {
            for (int n = 0; n < _importCount; n++)
            {
                var import = new ImportTableEntry(this, 
                    Data.GetReader(_importOffset + (UInt32)(n * ImportTableEntry.SizeInBytes), 
                    ImportTableEntry.SizeInBytes));

                if (!import.Deserialize())
                    return false;

                Imports.Add(import);
                if (!ImportPackageNames.Contains(import.SourcePCCName))
                    ImportPackageNames.Add(import.SourcePCCName);
            }

            return true;
        }

        private bool DeserializeExports()
        {
            uint pos = 0;
            for (int n = 0; n < _exportCount; n++)
            {
                var export = new ExportTableEntry(this,
                    Data.GetReader(_exportOffset + pos,
                    ExportTableEntry.SizeInBytes));

                int result = export.Deserialize();
                if (result == -1)
                    return false;

                pos += (uint)result;
                Exports.Add(export);
            }

            return true;
        }
    }
}
