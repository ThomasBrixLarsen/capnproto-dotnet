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
	public class StructBuilder
	{
		public interface Factory<T>
		{
			T constructBuilder(Capnproto.SegmentBuilder segment, int data, int pointers, int dataSize, short pointerCount);
			
			Capnproto.StructSize StructSize();
		}
		
		protected internal readonly Capnproto.SegmentBuilder segment;
		
		protected internal readonly int data;
		
		protected internal readonly int pointers;
		
		protected internal readonly int dataSize;
		
		protected internal readonly short pointerCount;
		
		public StructBuilder(Capnproto.SegmentBuilder segment, int data, int pointers, int dataSize, short pointerCount)
		{
			// byte offset to data section
			// word offset of pointer section
			// in bits
			this.segment = segment;
			this.data = data;
			this.pointers = pointers;
			this.dataSize = dataSize;
			this.pointerCount = pointerCount;
		}
		
		protected internal bool _getBoolField(int offset)
		{
			int bitOffset = offset;
			int position = this.data + (bitOffset / 8);
			return (this.segment.buffer.get(position) & (1 << (bitOffset % 8))) != 0;
		}
		
		protected internal bool _getBoolField(int offset, bool mask)
		{
			return this._getBoolField(offset) ^ mask;
		}
		
		protected internal void _setBoolField(int offset, bool value)
		{
			int bitOffset = offset;
			byte bitnum = unchecked((byte)(bitOffset % 8));
			int position = this.data + (bitOffset / 8);
			byte oldValue = this.segment.buffer.get(position);
			this.segment.buffer.put(position, unchecked((byte)((oldValue & (~(1 << bitnum))) | ((value? 1 : 0) << bitnum))));
		}
		
		protected internal void _setBoolField(int offset, bool value, bool mask)
		{
			this._setBoolField(offset, value ^ mask);
		}
		
		protected internal sbyte _getSbyteField(int offset)
		{
			return (sbyte)this.segment.buffer.get(this.data + offset);
		}
		
		protected internal sbyte _getSbyteField(int offset, sbyte mask)
		{
			return unchecked((sbyte)(this._getByteField(offset) ^ mask));
		}
		
		protected internal void _setSbyteField(int offset, sbyte value)
		{
			this.segment.buffer.put(this.data + offset, (byte)value);
		}
		
		protected internal void _setSbyteField(int offset, sbyte value, sbyte mask)
		{
			this._setSbyteField(offset, unchecked((sbyte)(value ^ mask)));
		}
		
		protected internal byte _getByteField(int offset)
		{
			return this.segment.buffer.get(this.data + offset);
		}
		
		protected internal byte _getByteField(int offset, byte mask)
		{
			return unchecked((byte)(this._getByteField(offset) ^ mask));
		}
		
		protected internal void _setByteField(int offset, byte value)
		{
			this.segment.buffer.put(this.data + offset, value);
		}
		
		protected internal void _setByteField(int offset, byte value, byte mask)
		{
			this._setByteField(offset, unchecked((byte)(value ^ mask)));
		}
		
		protected internal short _getShortField(int offset)
		{
			return this.segment.buffer.getShort(this.data + offset * 2);
		}
		
		protected internal short _getShortField(int offset, short mask)
		{
			return (short)(this._getShortField(offset) ^ mask);
		}
		
		protected internal void _setShortField(int offset, short value)
		{
			this.segment.buffer.putShort(this.data + offset * 2, value);
		}
		
		protected internal void _setShortField(int offset, short value, short mask)
		{
			this._setShortField(offset, (short)(value ^ mask));
		}
		
		protected internal ushort _getUshortField(int offset)
		{
			return (ushort)this.segment.buffer.getShort(this.data + offset * 2);
		}
		
		protected internal ushort _getUshortField(int offset, ushort mask)
		{
			return (ushort)(this._getShortField(offset) ^ mask);
		}
		
		protected internal void _setUshortField(int offset, ushort value)
		{
			this.segment.buffer.putShort(this.data + offset * 2, (short)value);
		}
		
		protected internal void _setUshortField(int offset, ushort value, ushort mask)
		{
			this._setUshortField(offset, (ushort)(value ^ mask));
		}
		
		protected internal int _getIntField(int offset)
		{
			return this.segment.buffer.getInt(this.data + offset * 4);
		}
		
		protected internal int _getIntField(int offset, int mask)
		{
			return this._getIntField(offset) ^ mask;
		}
		
		protected internal void _setIntField(int offset, int value)
		{
			this.segment.buffer.putInt(this.data + offset * 4, value);
		}
		
		protected internal void _setIntField(int offset, int value, int mask)
		{
			this._setIntField(offset, value ^ mask);
		}
		
		protected internal uint _getUintField(int offset)
		{
			return (uint)this.segment.buffer.getInt(this.data + offset * 4);
		}
		
		protected internal uint _getUintField(int offset, uint mask)
		{
			return this._getUintField(offset) ^ mask;
		}
		
		protected internal void _setUintField(int offset, uint value)
		{
			this.segment.buffer.putInt(this.data + offset * 4, (int)value);
		}
		
		protected internal void _setUintField(int offset, uint value, uint mask)
		{
			this._setUintField(offset, value ^ mask);
		}
		
		protected internal long _getLongField(int offset)
		{
			return this.segment.buffer.getLong(this.data + offset * 8);
		}
		
		protected internal long _getLongField(int offset, long mask)
		{
			return this._getLongField(offset) ^ mask;
		}
		
		protected internal void _setLongField(int offset, long value)
		{
			this.segment.buffer.putLong(this.data + offset * 8, value);
		}
		
		protected internal void _setLongField(int offset, long value, long mask)
		{
			this._setLongField(offset, value ^ mask);
		}
		
		protected internal ulong _getUlongField(int offset)
		{
			return (ulong)this.segment.buffer.getLong(this.data + offset * 8);
		}
		
		protected internal ulong _getUlongField(int offset, ulong mask)
		{
			return this._getUlongField(offset) ^ mask;
		}
		
		protected internal void _setUlongField(int offset, ulong value)
		{
			this.segment.buffer.putLong(this.data + offset * 8, (long)value);
		}
		
		protected internal void _setUlongField(int offset, ulong value, ulong mask)
		{
			this._setUlongField(offset, value ^ mask);
		}
		
		protected internal float _getFloatField(int offset)
		{
			return this.segment.buffer.getFloat(this.data + offset * 4);
		}
		
		protected internal float _getFloatField(int offset, int mask)
		{
			return Sharpen.Runtime.intBitsToFloat(this.segment.buffer.getInt(this.data + offset
			                                      * 4) ^ mask);
		}
		
		protected internal void _setFloatField(int offset, float value)
		{
			this.segment.buffer.putFloat(this.data + offset * 4, value);
		}
		
		protected internal void _setFloatField(int offset, float value, int mask)
		{
			this.segment.buffer.putInt(this.data + offset * 4, Sharpen.Runtime.floatToIntBits(value) ^ mask);
		}
		
		protected internal double _getDoubleField(int offset)
		{
			return this.segment.buffer.getDouble(this.data + offset * 8);
		}
		
		protected internal double _getDoubleField(int offset, long mask)
		{
			return Sharpen.Runtime.longBitsToDouble(this.segment.buffer.getLong(this.data + offset * 8) ^ mask);
		}
		
		protected internal void _setDoubleField(int offset, double value)
		{
			this.segment.buffer.putDouble(this.data + offset * 8, value);
		}
		
		protected internal void _setDoubleField(int offset, double value, long mask)
		{
			this.segment.buffer.putLong(this.data + offset * 8, Sharpen.Runtime.doubleToLongBits(value) ^ mask);
		}
		
		protected internal bool _pointerFieldIsNull(int ptrIndex)
		{
			return this.segment.buffer.getLong((this.pointers + ptrIndex) * Capnproto.Constants.BYTES_PER_WORD) == 0;
		}
		
		protected internal void _clearPointerField(int ptrIndex)
		{
			int pointer = this.pointers + ptrIndex;
			Capnproto.WireHelpers.zeroObject(this.segment, pointer);
			this.segment.buffer.putLong(pointer * 8, 0L);
		}
		
		protected internal T _getPointerField<T>(Capnproto.FromPointerBuilder<T> factory, int index)
		{
			return factory.fromPointerBuilder(this.segment, this.pointers + index);
		}
		
		protected internal T _getPointerField<T>(Capnproto.FromPointerBuilderRefDefault<T> factory, int index, Capnproto.SegmentReader defaultSegment, int defaultOffset)
		{
			return factory.fromPointerBuilderRefDefault(this.segment, this.pointers + index, defaultSegment, defaultOffset);
		}
		
		protected internal T _getPointerField<T>(Capnproto.FromPointerBuilderBlobDefault<T> factory, int index, java.nio.ByteBuffer defaultBuffer, int defaultOffset, int defaultSize)
		{
			return factory.fromPointerBuilderBlobDefault(this.segment, this.pointers + index, defaultBuffer, defaultOffset, defaultSize);
		}
		
		protected internal T _initPointerField<T>(Capnproto.FromPointerBuilder<T> factory, int index, int elementCount)
		{
			return factory.initFromPointerBuilder(this.segment, this.pointers + index, elementCount);
		}
		
		protected internal void _setPointerField<Builder, Reader>(Capnproto.SetPointerBuilder<Builder, Reader> factory, int index, Reader value)
		{
			factory.setPointerBuilder(this.segment, this.pointers + index, value);
		}
	}
}
