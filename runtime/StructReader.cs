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
	public class StructReader
	{
		public interface Factory<T>
		{
			T ConstructReader(Capnproto.SegmentReader segment, int data, int pointers, int dataSize, short pointerCount, int nestingLimit);
		}
		
		protected internal readonly Capnproto.SegmentReader segment;
		
		protected internal readonly int data;
		
		protected internal readonly int pointers;
		
		protected internal readonly int dataSize;
		
		protected internal readonly short pointerCount;
		
		protected internal readonly int nestingLimit;
		
		public StructReader()
		{
			//byte offset to data section
			//word offset of pointer section
			//in bits
			this.segment = Capnproto.SegmentReader.EMPTY;
			this.data = 0;
			this.pointers = 0;
			this.dataSize = 0;
			this.pointerCount = 0;
			this.nestingLimit = unchecked((int)(0x7fffffff));
		}
		
		public StructReader(Capnproto.SegmentReader segment, int data, int pointers, int dataSize, short pointerCount, int nestingLimit)
		{
			this.segment = segment;
			this.data = data;
			this.pointers = pointers;
			this.dataSize = dataSize;
			this.pointerCount = pointerCount;
			this.nestingLimit = nestingLimit;
		}
		
		protected internal bool _getBoolField(int offset)
		{
			//XXX should use unsigned operations.
			if(offset < this.dataSize)
			{
				byte b = this.segment.buffer.get(this.data + offset / 8);
				return (b & (1 << (offset % 8))) != 0;
			}
			return false;
		}
		
		protected internal bool _getBoolField(int offset, bool mask)
		{
			return this._getBoolField(offset) ^ mask;
		}
		
		protected internal sbyte _getSbyteField(int offset)
		{
			if((offset + 1) * 8 <= this.dataSize)
				return (sbyte)this.segment.buffer.get(this.data + offset);
			return 0;
		}
		
		protected internal sbyte _getSbyteField(int offset, sbyte mask)
		{
			return unchecked((sbyte)(this._getSbyteField(offset) ^ mask));
		}
		
		protected internal byte _getByteField(int offset)
		{
			if((offset + 1) * 8 <= this.dataSize)
				return this.segment.buffer.get(this.data + offset);
			return 0;
		}
		
		protected internal byte _getByteField(int offset, byte mask)
		{
			return unchecked((byte)(this._getByteField(offset) ^ mask));
		}
		
		protected internal short _getShortField(int offset)
		{
			if((offset + 1) * 16 <= this.dataSize)
				return this.segment.buffer.getShort(this.data + offset * 2);
			return 0;
		}
		
		protected internal short _getShortField(int offset, short mask)
		{
			return (short)(this._getShortField(offset) ^ mask);
		}
		
		protected internal ushort _getUshortField(int offset)
		{
			if((offset + 1) * 16 <= this.dataSize)
				return (ushort)this.segment.buffer.getShort(this.data + offset * 2);
			return 0;
		}
		
		protected internal ushort _getUshortField(int offset, ushort mask)
		{
			return (ushort)(this._getUshortField(offset) ^ mask);
		}
		
		
		protected internal int _getIntField(int offset)
		{
			if((offset + 1) * 32 <= this.dataSize)
				return this.segment.buffer.getInt(this.data + offset * 4);
			return 0;
		}
		
		protected internal int _getIntField(int offset, int mask)
		{
			return this._getIntField(offset) ^ mask;
		}
		
		protected internal uint _getUintField(int offset)
		{
			if((offset + 1) * 32 <= this.dataSize)
				return (uint)this.segment.buffer.getInt(this.data + offset * 4);
			return 0;
		}
		
		protected internal uint _getUintField(int offset, uint mask)
		{
			return (uint)(this._getUintField(offset) ^ mask);
		}
		
		protected internal long _getLongField(int offset)
		{
			if((offset + 1) * 64 <= this.dataSize)
				return this.segment.buffer.getLong(this.data + offset * 8);
			return 0;
		}
		
		protected internal long _getLongField(int offset, long mask)
		{
			return this._getLongField(offset) ^ mask;
		}
		
		protected internal ulong _getUlongField(int offset)
		{
			if((offset + 1) * 64 <= this.dataSize)
				return (ulong)this.segment.buffer.getLong(this.data + offset * 8);
			return 0;
		}
		
		protected internal ulong _getUlongField(int offset, ulong mask)
		{
			return this._getUlongField(offset) ^ mask;
		}
		
		
		protected internal float _getFloatField(int offset)
		{
			if((offset + 1) * 32 <= this.dataSize)
				return this.segment.buffer.getFloat(this.data + offset * 4);
			return 0;
		}
		
		protected internal float _getFloatField(int offset, int mask)
		{
			if((offset + 1) * 32 <= this.dataSize)
				return Sharpen.Runtime.intBitsToFloat(this.segment.buffer.getInt(this.data + offset * 4) ^ mask);
			return Sharpen.Runtime.intBitsToFloat(mask);
		}
		
		protected internal double _getDoubleField(int offset)
		{
			if((offset + 1) * 64 <= this.dataSize)
				return this.segment.buffer.getDouble(this.data + offset * 8);
			return 0;
		}
		
		protected internal double _getDoubleField(int offset, long mask)
		{
			if((offset + 1) * 64 <= this.dataSize)
				return Sharpen.Runtime.longBitsToDouble(this.segment.buffer.getLong(this.data + offset * 8) ^ mask);
			return Sharpen.Runtime.longBitsToDouble(mask);
		}
		
		protected internal bool _pointerFieldIsNull(int ptrIndex)
		{
			return this.segment.buffer.getLong((this.pointers + ptrIndex) * Capnproto.Constants.BYTES_PER_WORD) == 0;
		}
		
		protected internal T _getPointerField<T>(Capnproto.FromPointerReader<T> factory, int ptrIndex)
		{
			if(ptrIndex < this.pointerCount)
				return factory.fromPointerReader(this.segment, this.pointers + ptrIndex, this.nestingLimit);
			return factory.fromPointerReader(Capnproto.SegmentReader.EMPTY, 0, this.nestingLimit);
		}
		
		protected internal T _getPointerField<T>(Capnproto.FromPointerReaderRefDefault<T> factory, int ptrIndex, Capnproto.SegmentReader defaultSegment, int defaultOffset)
		{
			if(ptrIndex < this.pointerCount)
				return factory.fromPointerReaderRefDefault(this.segment, this.pointers + ptrIndex, defaultSegment, defaultOffset, this.nestingLimit);
			return factory.fromPointerReaderRefDefault(Capnproto.SegmentReader.EMPTY, 0, defaultSegment, defaultOffset, this.nestingLimit);
		}
		
		protected internal T _getPointerField<T>(Capnproto.FromPointerReaderBlobDefault<T> factory, int ptrIndex, java.nio.ByteBuffer defaultBuffer, int defaultOffset, int defaultSize)
		{
			if(ptrIndex < this.pointerCount)
				return factory.fromPointerReaderBlobDefault(this.segment, this.pointers + ptrIndex, defaultBuffer, defaultOffset, defaultSize);
			return factory.fromPointerReaderBlobDefault(Capnproto.SegmentReader.EMPTY, 0, defaultBuffer, defaultOffset, defaultSize);
		}
	}
}
