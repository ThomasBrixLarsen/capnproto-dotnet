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
	public sealed class ListList
	{
		public sealed class Factory<ElementBuilder, ElementReader> : Capnproto.ListFactory<Capnproto.ListList.Builder<ElementBuilder, ElementReader>,
		                                                             Capnproto.ListList.Reader<ElementReader>>
		                                                             where ElementReader : Capnproto.ListReader
		{
			public readonly Capnproto.ListFactory<ElementBuilder, ElementReader> SingleFactory;
			
			public Factory(Capnproto.ListFactory<ElementBuilder, ElementReader> factory) : base(Capnproto.ElementSize.POINTER)
			{
				this.SingleFactory = factory;
			}
			
			public sealed override Capnproto.ListList.Reader<ElementReader> ConstructReader(Capnproto.SegmentReader segment, int ptr, int elementCount, int step, int structDataSize, short structPointerCount, int nestingLimit)
			{
				return new Capnproto.ListList.Reader<ElementReader>(SingleFactory, segment, ptr, elementCount, step, structDataSize, structPointerCount, nestingLimit);
			}
			
			public sealed override Capnproto.ListList.Builder<ElementBuilder, ElementReader> constructBuilder(Capnproto.SegmentBuilder segment, int ptr, int elementCount, int step, int structDataSize, short structPointerCount)
			{
				return new Capnproto.ListList.Builder<ElementBuilder, ElementReader>(SingleFactory, segment, ptr, elementCount, step, structDataSize, structPointerCount);
			}
		}
		
		public sealed class Reader<T> : Capnproto.ListReader
		{
			private readonly Capnproto.FromPointerReader<T> SingleFactory;
			
			public Reader(Capnproto.FromPointerReader<T> factory, Capnproto.SegmentReader segment, int ptr, int elementCount, int step, int structDataSize, short structPointerCount, int nestingLimit) : base(segment, ptr, elementCount, step, structDataSize, structPointerCount, nestingLimit)
			{
				this.SingleFactory = factory;
			}
			
			public T Get(int index)
			{
				return _getPointerElement(this.SingleFactory, index);
			}
			
			public T this[int index]
			{
				get { return Get(index); }
			}
		}
		
		public sealed class Builder<T, U> : Capnproto.ListBuilder where U : Capnproto.ListReader
		{
			private readonly Capnproto.ListFactory<T, U> SingleFactory;
			
			public Builder(Capnproto.ListFactory<T, U> factory, Capnproto.SegmentBuilder segment, int ptr, int elementCount, int step, int structDataSize, short structPointerCount) : base(segment, ptr, elementCount, step, structDataSize, structPointerCount)
			{
				this.SingleFactory = factory;
			}
			
			public T Init(int index, int size)
			{
				return _initPointerElement(this.SingleFactory, index, size);
			}
			
			public T Get(int index)
			{
				return _getPointerElement(this.SingleFactory, index);
			}
			
			public T this[int index]
			{
				get { return Get(index); }
			}
		}
	}
}
