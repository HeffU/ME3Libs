using ME3Data.FileFormats.PCC;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ME3Data.Utility
{
    public class PCCStreamReader
    {
        private FileStream File;

        public PCCStreamReader(FileStream file)
        {
            File = file;
        }

        private void Seek(int destination)
        {
            int seekOffset = destination >= (int)File.Position ? destination - (int)File.Position : destination;
            File.Seek(seekOffset, seekOffset != destination ? SeekOrigin.Current : SeekOrigin.Begin);
        }

        public ObjectReader GetReader(int startOffset, int size)
        {
            Seek(startOffset);
            byte[] data = new byte[size];
            File.Read(data, 0, size);
            return new ObjectReader(data);
        }

        public ObjectReader GetReader(UInt32 startOffset, UInt32 size)
        {
            return GetReader((int)startOffset, (int)size);
        }

        public ObjectReader GetReader(ExportTableEntry export)
        {
            return GetReader(export.FileOffset, export.Size);
        }

        public ObjectReader GetHeaderReader()
        {
            //TODO: is this fixed for ME3?
            return GetReader(0, 0x85);
        }
    }
}
