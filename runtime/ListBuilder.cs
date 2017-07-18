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
	public class ListBuilder
	{
		public interface Factory<T>
		{
			T constructBuilder(Capnproto.SegmentBuilder segment, int ptr, int elementCount, int step, int structDataSize, short structPointerCount);
		}
		
		internal readonly Capnproto.SegmentBuilder segment;
		
		internal readonly int ptr;
		
		internal readonly int elementCount;
		
		internal readonly int step;
		
		internal readonly int structDataSize;
		
		internal readonly short structPointerCount;
		
		public ListBuilder(Capnproto.SegmentBuilder segment, int ptr, int elementCount, int step, int structDataSize, short structPointerCount)
		{
			// byte offset to front of list
			// in bits
			// in bits
			this.segment = segment;
			this.ptr = ptr;
			this.elementCount = elementCount;
			this.step = step;
			this.structDataSize = structDataSize;
			this.structPointerCount = structPointerCount;
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
		
		protected internal virtual void _setBooleanElement(int index, bool value)
		{
			long bitOffset = index * this.step;
			byte bitnum = unchecked((byte)(bitOffset % 8));
			int position = (int)(this.ptr + (bitOffset / 8));
			byte oldValue = this.segment.buffer.get(position);
			this.segment.buffer.put(position, unchecked((byte)((oldValue & (~(1 << bitnum))) | ((value ? 1 : 0) << bitnum))));
		}
		
		protected internal virtual void _setSbyteElement(int index, sbyte value)
		{
			this.segment.buffer.put(this.ptr + (int)((long)index * this.step / Capnproto.Constants.BITS_PER_BYTE), (byte)value);
		}
		
		protected internal virtual void _setByteElement(int index, byte value)
		{
			this.segment.buffer.put(this.ptr + (int)((long)index * this.step / Capnproto.Constants.BITS_PER_BYTE), value);
		}
		
		protected internal virtual void _setShortElement(int index, short value)
		{
			this.segment.buffer.putShort(this.ptr + (int)((long)index * this.step / Capnproto.Constants.BITS_PER_BYTE), value);
		}
		
		protected internal virtual void _setUshortElement(int index, ushort value)
		{
			this.segment.buffer.putShort(this.ptr + (int)((long)index * this.step / Capnproto.Constants.BITS_PER_BYTE), (short)value);
		}
		
		protected internal virtual void _setIntElement(int index, int value)
		{
			this.segment.buffer.putInt(this.ptr + (int)((long)index * this.step / Capnproto.Constants.BITS_PER_BYTE), value);
		}
		
		protected internal virtual void _setUintElement(int index, uint value)
		{
			this.segment.buffer.putInt(this.ptr + (int)((long)index * this.step / Capnproto.Constants.BITS_PER_BYTE), (int)value);
		}
		
		protected internal virtual void _setLongElement(int index, long value)
		{
			this.segment.buffer.putLong(this.ptr + (int)((long)index * this.step / Capnproto.Constants.BITS_PER_BYTE), value);
		}
		
		protected internal virtual void _setUlongElement(int index, ulong value)
		{
			this.segment.buffer.putLong(this.ptr + (int)((long)index * this.step / Capnproto.Constants.BITS_PER_BYTE), (long)value);
		}
		
		protected internal virtual void _setFloatElement(int index, float value)
		{
			this.segment.buffer.putFloat(this.ptr + (int)((long)index * this.step / Capnproto.Constants.BITS_PER_BYTE), value);
		}
		
		protected internal virtual void _setDoubleElement(int index, double value)
		{
			this.segment.buffer.putDouble(this.ptr + (int)((long)index * this.step / Capnproto.Constants.BITS_PER_BYTE), value);
		}
		
		protected internal T _getStructElement<T>(Capnproto.StructBuilder.Factory<T> factory, int index)
		{
			long indexBit = (long)index * this.step;
			int structData = this.ptr + (int)(indexBit / Capnproto.Constants.BITS_PER_BYTE);
			int structPointers = (structData + (this.structDataSize / 8)) / 8;
			return factory.constructBuilder(this.segment, structData, structPointers, this.structDataSize, this.structPointerCount);
		}
		
		protected internal T _getPointerElement<T>(Capnproto.FromPointerBuilder<T> factory, int index)
		{
			return factory.fromPointerBuilder(this.segment, (this.ptr + (int)((long)index * this.step / Capnproto.Constants.BITS_PER_BYTE)) / Capnproto.Constants.BYTES_PER_WORD);
		}
		
		protected internal T _initPointerElement<T>(Capnproto.FromPointerBuilder<T> factory, int index, int elementCount)
		{
			return factory.initFromPointerBuilder(this.segment, (this.ptr + (int)((long)index * this.step / Capnproto.Constants.BITS_PER_BYTE)) / Capnproto.Constants.BYTES_PER_WORD, elementCount);
		}
		
		protected internal void _setPointerElement<Builder, Reader>(Capnproto.SetPointerBuilder<Builder, Reader> factory, int index, Reader value)
		{
			factory.setPointerBuilder(this.segment, (this.ptr + (int)((long)index * this.step / Capnproto.Constants.BITS_PER_BYTE)) / Capnproto.Constants.BYTES_PER_WORD, value);
		}
	}
}
