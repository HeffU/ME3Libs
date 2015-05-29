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
        private int _size;
        private int _currentPosition;

        public ObjectReader(byte[] data)
        {
            _data = data;
            _size = _data.Length;
        }

        public void Reset()
        {
            _currentPosition = 0;
        }

        private int _position(int delta)
        {
            int tmp = _currentPosition;
            _currentPosition += delta;
            return tmp;
        }

        public byte[] ReadRawData(int numBytes)
        {
            if (_currentPosition + numBytes >= _size || numBytes <= 0)
                return null;

            byte[] raw = new byte[numBytes];
            Buffer.BlockCopy(_data, _position(numBytes), raw, 0, numBytes);
            return raw;
        }

        public Int32 ReadIndex()
        {
            return _currentPosition >= _size ? 0 : BitConverter.ToInt32(_data, _position(4));
        }

        public Int32 ReadInt32()
        {
            return _currentPosition >= _size ? 0 : BitConverter.ToInt32(_data, _position(4));
        }

        public Int16 ReadInt16()
        {
            return _currentPosition >= _size ? (Int16)0 : BitConverter.ToInt16(_data, _position(2));
        }

        public Int64 ReadInt64()
        {
            return _currentPosition >= _size ? 0 : BitConverter.ToInt64(_data, _position(8));
        }

        public UInt32 ReadUInt32()
        {
            return _currentPosition >= _size ? 0 : BitConverter.ToUInt32(_data, _position(4));
        }

        public UInt16 ReadUInt16()
        {
            return _currentPosition >= _size ? (UInt16)0 : BitConverter.ToUInt16(_data, _position(2));
        }

        public UInt64 ReadUInt64()
        {
            return _currentPosition >= _size ? 0 : BitConverter.ToUInt64(_data, _position(8));
        }
    }
}
