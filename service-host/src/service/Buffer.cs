using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace service
{
    public class Buffer
    {
        private byte[] _content;
        private int _count;
        private int _size;

        public Buffer(int size)
        {
            this._size = size;
            this._content = new byte[size];
        }

        public void Flush(Stream writer)
        {
            writer.Write(_content, 0, _count);
            _count = 0;
        }

        public bool CanHold(int count)
        {
            return _size - this._count >= count;
        }

        public void Add(byte[] chunk)
        {
            System.Buffer.BlockCopy(chunk, 0, this._content, _count, chunk.Length);
            _count += chunk.Length;
        }

        public int Count
        {
            get { return _count; }
        }
    }
}
