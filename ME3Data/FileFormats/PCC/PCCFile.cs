using ME3Data.DataTypes;
using ME3Data.DataTypes.ScriptTypes;
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


        public PCCStreamReader Data;

        private UInt32 _unkn1;
        private String _folderName;
        private UInt32 _packageFlags;
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

        private UInt32 _compressionFlag;
        private UInt32 _chunkCount;

        public PCCFile(PCCStreamReader data)
        {
            Data = data;
            Exports = new List<ExportTableEntry>();
            Imports = new List<ImportTableEntry>();
            Names = new List<String>();
        }

        public bool Deserialize()
        {
            var header = Data.GetHeaderReader();
            if (!DeserializeHeader(header))
                return false;

            // Unsure if import table is always after name table, make a more dynamic solution.
            if (!DeserializeNames(Data.GetReader(_nameOffset, _importOffset - _nameOffset)))
                return false;

            if (!DeserializeImports())
                return false;

            if (!DeserializeExports())
                return false;

            foreach (ExportTableEntry export in Exports)
            {
                if (!DeserializeExportObject(export))
                    return false;
            }

            return true;
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

                case "ScriptStruct":
                    var scriptstructObj = new ME3ScriptStruct(Data.GetReader(entry.FileOffset, entry.Size), entry, this);
                    entry.Object = scriptstructObj;
                    return scriptstructObj.Deserialize();

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

        public String GetName(NameReference reference)
        {
            if (reference.Index == 0 && reference.ModNumber >= 0)
                return String.Empty; // Error, weird mod number!
            return reference.Index >= 0 && reference.Index < Names.Count ? Names[reference.Index] : String.Empty;
        }

        public ME3Object GetObject(Int32 objIndex)
        {
            if (objIndex <= 0)
                return null;
            else
                return Exports[objIndex - 1].Object;
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
            _packageFlags = header.ReadUInt32();
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

            _compressionFlag = header.ReadUInt32();
            _chunkCount = header.ReadUInt32();
            // TODO: support compressed files..

            return true;
        }

        private bool DeserializeNames(ObjectReader table)
        {
            for (int n = 0; n < _nameCount; n++)
            {
                var name = table.ReadString();
                // TODO: Add some sanity checking
                Names.Add(name);
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
