using ME3Data.DataTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ME3Data.Utility
{
    public class ObjectReader
    {
        private byte[] _data;
        public int Size;
        public int Position;

        public ObjectReader(byte[] data)
        {
            _data = data;
            Size = _data.Length;
        }

        public void Reset()
        {
            Position = 0;
        }

        private int _position(int delta)
        {
            int tmp = Position;
            Position += delta;
            return tmp;
        }

        public byte[] ReadRawData(int numBytes)
        {
            if (Position + numBytes > Size || numBytes <= 0)
                return null;

            byte[] raw = new byte[numBytes];
            Buffer.BlockCopy(_data, _position(numBytes), raw, 0, numBytes);
            return raw;
        }

        public byte ReadByte()
        {
            return Position + 1 > Size ? (byte)0 : _data[_position(1)];
        }

        public Int32 ReadIndex()
        {
            return Position + 4 > Size ? 0 : BitConverter.ToInt32(_data, _position(4));
        }

        public Int32 ReadInt32()
        {
            return Position + 4 > Size ? 0 : BitConverter.ToInt32(_data, _position(4));
        }

        public Int16 ReadInt16()
        {
            return Position + 2 > Size ? (Int16)0 : BitConverter.ToInt16(_data, _position(2));
        }

        public Int64 ReadInt64()
        {
            return Position + 8 > Size ? 0 : BitConverter.ToInt64(_data, _position(8));
        }

        public UInt32 ReadUInt32()
        {
            return Position + 4 > Size ? 0 : BitConverter.ToUInt32(_data, _position(4));
        }

        public UInt16 ReadUInt16()
        {
            return Position + 2 > Size ? (UInt16)0 : BitConverter.ToUInt16(_data, _position(2));
        }

        public UInt64 ReadUInt64()
        {
            return Position + 8 > Size ? 0 : BitConverter.ToUInt64(_data, _position(8));
        }

        public float ReadFloat()
        {
            return Position + 4 > Size ? 0 : BitConverter.ToSingle(_data, _position(4));
        }

        public NameReference ReadNameRef()
        {
            if (Position + 8 > Size)
                return new NameReference();
            var NameRef = new NameReference();
            NameRef.Index = BitConverter.ToInt32(_data, _position(4));
            NameRef.ModNumber = BitConverter.ToInt32(_data, _position(4)) - 1;

            return NameRef;
        }

        public String ReadString()
        {
            var size = ReadIndex();
            bool unicode = false;
            if (size < 0)
            {
                size = -size;
                unicode = true;
            } 
            else if (size == 0)
            {
                return String.Empty;
            }

            if (unicode)
            {
                var bytes = ReadRawData((size * 2));
                return Encoding.Unicode.GetString(bytes).Substring(0, size - 1);
            }
            else
            {
                var bytes = ReadRawData(size);
                return Encoding.ASCII.GetString(bytes).Substring(0, size - 1);
            }
        }
    }
}
