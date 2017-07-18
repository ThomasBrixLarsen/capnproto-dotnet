// Copyright (c) 2013-2014 Sandstorm Development Group, Inc. and contributors
// Licensed under the MIT License:
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.

namespace Capnproto
{
	public class ListReader
	{
		public interface Factory<T>
		{
			T ConstructReader(Capnproto.SegmentReader segment, int ptr, int elementCount, int step, int structDataSize, short structPointerCount, int nestingLimit);
		}
		
		internal readonly Capnproto.SegmentReader segment;
		
		internal readonly int ptr;
		
		internal readonly int elementCount;
		
		internal readonly int step;
		
		internal readonly int structDataSize;
		
		internal readonly short structPointerCount;
		
		internal readonly int nestingLimit;
		
		public ListReader()
		{
			// byte offset to front of list
			// in bits
			// in bits
			this.segment = null;
			this.ptr = 0;
			this.elementCount = 0;
			this.step = 0;
			this.structDataSize = 0;
			this.structPointerCount = 0;
			this.nestingLimit = unchecked((int)(0x7fffffff));
		}
		
		public ListReader(Capnproto.SegmentReader segment, int ptr, int elementCount, int step, int structDataSize, short structPointerCount, int nestingLimit)
		{
			this.segment = segment;
			this.ptr = ptr;
			this.elementCount = elementCount;
			this.step = step;
			this.structDataSize = structDataSize;
			this.structPointerCount = structPointerCount;
			this.nestingLimit = nestingLimit;
		}
		
		public virtual int Length
		{
			get { return this.elementCount; }
		}
		
		protected internal virtual bool _getBooleanElement(int index)
		{
			int bindex = index * this.step;
			byte b = this.segment.buffer.get(this.ptr + (int)(bindex / Capnproto.Constants.BITS_PER_BYTE));
			return (b & (1 << (bindex % 8))) != 0;
		}
		
		protected internal virtual sbyte _getSbyteElement(int index)
		{
			return (sbyte)this.segment.buffer.get(this.ptr + (int)((long)index * this.step / Capnproto.Constants.BITS_PER_BYTE));
		}
		
		protected internal virtual byte _getByteElement(int index)
		{
			return this.segment.buffer.get(this.ptr + (int)((long)index * this.step / Capnproto.Constants.BITS_PER_BYTE));
		}
		
		protected internal virtual short _getShortElement(int index)
		{
			return this.segment.buffer.getShort(this.ptr + (int)((long)index * this.step / Capnproto.Constants.BITS_PER_BYTE));
		}
		
		protected internal virtual ushort _getUshortElement(int index)
		{
			return (ushort)this.segment.buffer.getShort(this.ptr + (int)((long)index * this.step / Capnproto.Constants.BITS_PER_BYTE));
		}
		
		protected internal virtual int _getIntElement(int index)
		{
			return this.segment.buffer.getInt(this.ptr + (int)((long)index * this.step / Capnproto.Constants.BITS_PER_BYTE));
		}
		
		protected internal virtual uint _getUintElement(int index)
		{
			return (uint)this.segment.buffer.getInt(this.ptr + (int)((long)index * this.step / Capnproto.Constants.BITS_PER_BYTE));
		}
		
		protected internal virtual long _getLongElement(int index)
		{
			return this.segment.buffer.getLong(this.ptr + (int)((long)index * this.step / Capnproto.Constants.BITS_PER_BYTE));
		}
		
		protected internal virtual ulong _getUlongElement(int index)
		{
			return (ulong)this.segment.buffer.getLong(this.ptr + (int)((long)index * this.step / Capnproto.Constants.BITS_PER_BYTE));
		}
		
		protected internal virtual float _getFloatElement(int index)
		{
			return this.segment.buffer.getFloat(this.ptr + (int)((long)index * this.step / Capnproto.Constants.BITS_PER_BYTE));
		}
		
		protected internal virtual double _getDoubleElement(int index)
		{
			return this.segment.buffer.getDouble(this.ptr + (int)((long)index * this.step / Capnproto.Constants.BITS_PER_BYTE));
		}
		
		protected internal virtual T _getStructElement<T>(Capnproto.StructReader.Factory<T> factory, int index)
		{
			// TODO check nesting limit
			long indexBit = (long)index * this.step;
			int structData = this.ptr + (int)(indexBit / Capnproto.Constants.BITS_PER_BYTE);
			int structPointers = structData + (this.structDataSize / Capnproto.Constants.BITS_PER_BYTE);
			return factory.ConstructReader(this.segment, structData, structPointers / 8, this.structDataSize, this.structPointerCount, this.nestingLimit - 1);
		}
		
		protected internal virtual T _getPointerElement<T>(Capnproto.FromPointerReader<T> factory, int index)
		{
			return factory.fromPointerReader(this.segment, (this.ptr + (int)((long)index * this.step / Capnproto.Constants.BITS_PER_BYTE)) / Capnproto.Constants.BYTES_PER_WORD, this.nestingLimit);
		}
		
		protected internal virtual T _getPointerElement<T>(Capnproto.FromPointerReaderBlobDefault<T> factory, int index, java.nio.ByteBuffer defaultBuffer, int defaultOffset, int defaultSize)
		{
			return factory.fromPointerReaderBlobDefault(this.segment, (this.ptr + (int)((long)index * this.step / Capnproto.Constants.BITS_PER_BYTE)) / Capnproto.Constants.BYTES_PER_WORD, defaultBuffer, defaultOffset, defaultSize);
		}
	}
}
