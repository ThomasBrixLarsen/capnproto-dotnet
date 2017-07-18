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

using System;

namespace Capnproto
{
	public class EnumList
	{
		internal static T clampOrdinal<T>(T[] values, short ordinal)
		{
			int index = ordinal;
			if(ordinal < 0 || ordinal >= values.Length)
				index = values.Length - 1;
			return values[index];
		}
		
		public sealed class Factory<T> : Capnproto.ListFactory<Capnproto.EnumList.Builder<T>, Capnproto.EnumList.Reader<T>> where T : IConvertible
		{
			public readonly T[] values;
			
			public Factory(T[] values) : base(Capnproto.ElementSize.TWO_BYTES)
			{
				this.values = values;
			}
			
			public sealed override Capnproto.EnumList.Reader<T> ConstructReader(Capnproto.SegmentReader segment, int ptr, int elementCount, int step, int structDataSize, short structPointerCount, int nestingLimit)
			{
				return new Capnproto.EnumList.Reader<T>(values, segment, ptr, elementCount, step, structDataSize, structPointerCount, nestingLimit);
			}
			
			public sealed override Capnproto.EnumList.Builder<T> constructBuilder(Capnproto.SegmentBuilder segment, int ptr, int elementCount, int step, int structDataSize, short structPointerCount)
			{
				return new Capnproto.EnumList.Builder<T>(values, segment, ptr, elementCount, step, structDataSize, structPointerCount);
			}
		}
		
		public sealed class Reader<T> : Capnproto.ListReader where T : IConvertible
		{
			public readonly T[] values;
			
			public Reader(T[] values, Capnproto.SegmentReader segment, int ptr, int elementCount, int step, int structDataSize, short structPointerCount, int nestingLimit) : base(segment, ptr, elementCount, step, structDataSize, structPointerCount, nestingLimit)
			{
				this.values = values;
			}
			
			public T Get(int index)
			{
				return clampOrdinal(this.values, _getShortElement(index));
			}
			
			public T this[int index]
			{
				get { return Get(index); }
			}
		}
		
		public sealed class Builder<T> : Capnproto.ListBuilder where T : IConvertible
		{
			public readonly T[] values;
			
			public Builder(T[] values, Capnproto.SegmentBuilder segment, int ptr, int elementCount, int step, int structDataSize, short structPointerCount) : base(segment, ptr, elementCount, step, structDataSize, structPointerCount)
			{
				this.values = values;
			}
			
			public T Get(int index)
			{
				return clampOrdinal(this.values, _getShortElement(index));
			}
			
			public void Set(int index, T value)
			{
				_setShortElement(index, value.ToInt16(System.Globalization.CultureInfo.CurrentCulture));
			}
			
			public T this[int index]
			{
				get { return Get(index); }
				set { Set(index, value); }
			}
		}
	}
}
