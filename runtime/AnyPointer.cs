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
	public sealed class AnyPointer
	{
		public sealed class Factory : Capnproto.PointerFactory<Capnproto.AnyPointer.Builder, Capnproto.AnyPointer.Reader>
		{
			public Capnproto.AnyPointer.Reader fromPointerReader(Capnproto.SegmentReader segment, int pointer, int nestingLimit)
			{
				return new Capnproto.AnyPointer.Reader(segment, pointer, nestingLimit);
			}
			
			public Capnproto.AnyPointer.Builder fromPointerBuilder(Capnproto.SegmentBuilder segment, int pointer)
			{
				return new Capnproto.AnyPointer.Builder(segment, pointer);
			}
			
			public Capnproto.AnyPointer.Builder initFromPointerBuilder(Capnproto.SegmentBuilder segment, int pointer, int elementCount)
			{
				Capnproto.AnyPointer.Builder result = new Capnproto.AnyPointer.Builder(segment, pointer);
				result.Clear();
				return result;
			}
		}
		
		public static readonly Capnproto.AnyPointer.Factory SingleFactory = new Capnproto.AnyPointer.Factory();
		
		public sealed class Reader
		{
			internal readonly Capnproto.SegmentReader segment;
			
			internal readonly int pointer;
			
			internal readonly int nestingLimit;
			
			public Reader(Capnproto.SegmentReader segment, int pointer, int nestingLimit)
			{
				this.segment = segment;
				this.pointer = pointer;
				this.nestingLimit = nestingLimit;
			}
			
			public bool IsNull()
			{
				return Capnproto.WirePointer.isNull(this.segment.buffer.getLong(this.pointer * Capnproto.Constants.BYTES_PER_WORD));
			}
			
			public T GetAs<T>(Capnproto.FromPointerReader<T> factory)
			{
				return factory.fromPointerReader(this.segment, this.pointer, this.nestingLimit);
			}
		}
		
		public sealed class Builder
		{
			internal readonly Capnproto.SegmentBuilder segment;
			
			internal readonly int pointer;
			
			public Builder(Capnproto.SegmentBuilder segment, int pointer)
			{
				this.segment = segment;
				this.pointer = pointer;
			}
			
			public bool IsNull()
			{
				return Capnproto.WirePointer.isNull(this.segment.buffer.getLong(this.pointer * Capnproto.Constants.BYTES_PER_WORD));
			}
			
			public T GetAs<T>(Capnproto.FromPointerBuilder<T> factory)
			{
				return factory.fromPointerBuilder(this.segment, this.pointer);
			}
			
			public T InitAs<T>(Capnproto.FromPointerBuilder<T> factory)
			{
				return factory.initFromPointerBuilder(this.segment, this.pointer, 0);
			}
			
			public T InitAs<T>(Capnproto.FromPointerBuilder<T> factory, int elementCount)
			{
				return factory.initFromPointerBuilder(this.segment, this.pointer, elementCount);
			}
			
			public void SetAs<T, U>(Capnproto.SetPointerBuilder<T, U> factory, U reader)
			{
				factory.setPointerBuilder(this.segment, this.pointer, reader);
			}
			
			public Capnproto.AnyPointer.Reader AsReader()
			{
				return new Capnproto.AnyPointer.Reader(segment, pointer, unchecked((int)(0x7fffffff)));
			}
			
			public void Clear()
			{
				Capnproto.WireHelpers.zeroObject(this.segment, this.pointer);
				this.segment.buffer.putLong(this.pointer * 8, 0L);
			}
		}
	}
}
