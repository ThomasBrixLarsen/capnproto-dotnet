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
	using System;
	using System.Collections;
	
	public sealed class StructList
	{
		public sealed class Factory<ElementBuilder, ElementReader> : Capnproto.ListFactory<Capnproto.StructList.Builder<ElementBuilder>,
		                                                             Capnproto.StructList.Reader<ElementReader>>
		                                                             where ElementBuilder : Capnproto.StructBuilder
		                                                             where ElementReader : Capnproto.StructReader
		{
			public readonly Capnproto.StructFactory<ElementBuilder, ElementReader> SingleFactory;
			
			public Factory(Capnproto.StructFactory<ElementBuilder, ElementReader> factory) : base(Capnproto.ElementSize.INLINE_COMPOSITE)
			{
				this.SingleFactory = factory;
			}
			
			public sealed override Capnproto.StructList.Reader<ElementReader> ConstructReader(Capnproto.SegmentReader segment, int ptr, int elementCount, int step, int structDataSize, short structPointerCount, int nestingLimit)
			{
				return new Capnproto.StructList.Reader<ElementReader>(SingleFactory, segment, ptr, elementCount, step, structDataSize, structPointerCount, nestingLimit);
			}
			
			public sealed override Capnproto.StructList.Builder<ElementBuilder> constructBuilder(Capnproto.SegmentBuilder segment, int ptr, int elementCount, int step, int structDataSize, short structPointerCount)
			{
				return new Capnproto.StructList.Builder<ElementBuilder>(SingleFactory, segment, ptr, elementCount, step, structDataSize, structPointerCount);
			}
			
			public sealed override Capnproto.StructList.Builder<ElementBuilder> fromPointerBuilderRefDefault(Capnproto.SegmentBuilder segment, int pointer, Capnproto.SegmentReader defaultSegment, int defaultOffset)
			{
				return Capnproto.WireHelpers.getWritableStructListPointer(this, pointer, segment, SingleFactory.StructSize(), defaultSegment, defaultOffset);
			}
			
			public sealed override Capnproto.StructList.Builder<ElementBuilder> fromPointerBuilder(Capnproto.SegmentBuilder segment, int pointer)
			{
				return Capnproto.WireHelpers.getWritableStructListPointer(this, pointer, segment, SingleFactory.StructSize(), null, 0);
			}
			
			public sealed override Capnproto.StructList.Builder<ElementBuilder> initFromPointerBuilder(Capnproto.SegmentBuilder segment, int pointer, int elementCount)
			{
				return Capnproto.WireHelpers.initStructListPointer(this, pointer, segment, elementCount, SingleFactory.StructSize());
			}
		}
		
		public sealed class Reader<T> : Capnproto.ListReader, System.Collections.Generic.IEnumerable<T>
		{
			public readonly Capnproto.StructReader.Factory<T> SingleFactory;
			
			public Reader(Capnproto.StructReader.Factory<T> factory, Capnproto.SegmentReader segment, int ptr, int elementCount, int step, int structDataSize, short structPointerCount, int nestingLimit) : base(segment, ptr, elementCount, step, structDataSize, structPointerCount, nestingLimit)
			{
				this.SingleFactory = factory;
			}
			
			public T Get(int index)
			{
				return _getStructElement(SingleFactory, index);
			}
			
			public T this[int index]
			{
				get { return Get(index); }
			}
			
			public sealed class Iterator : System.Collections.Generic.IEnumerator<T>
			{
				public Capnproto.StructList.Reader<T> list;
				
				public int idx = 0;
				
				public Iterator(Reader<T> _enclosing, Capnproto.StructList.Reader<T> list)
				{
					this._enclosing = _enclosing;
					this.list = list;
				}
				
				public /*override*/ T Current
				{
					get
					{
						return this.list._getStructElement(this._enclosing.SingleFactory, this.idx++);
					}
				}
				
				public /*override*/ bool MoveNext()
				{
					return this.idx < this.list.Length;
				}
				
				public /*override*/ void remove()
				{
					throw new System.NotSupportedException();
				}
				
				public void Reset()
				{
					idx = -1;
				}
				
				void IDisposable.Dispose()
				{
					
				}
				
				object IEnumerator.Current
				{
					get
					{
						return Current;
					}
				}
				
				private readonly Reader<T> _enclosing;
			}
			
			public /*override*/ System.Collections.Generic.IEnumerator<T> GetEnumerator()
			{
				return new Capnproto.StructList.Reader<T>.Iterator(this, this);
			}
			
			private IEnumerator GetEnumerator1()
			{
				return this.GetEnumerator();
			}
			IEnumerator IEnumerable.GetEnumerator()
			{
				return GetEnumerator1();
			}
		}
		
		public sealed class Builder<T> : Capnproto.ListBuilder, System.Collections.Generic.IEnumerable<T>
		{
			public readonly Capnproto.StructBuilder.Factory<T> SingleFactory;
			
			public Builder(Capnproto.StructBuilder.Factory<T> factory, Capnproto.SegmentBuilder segment, int ptr, int elementCount, int step, int structDataSize, short structPointerCount) : base(segment, ptr, elementCount, step, structDataSize, structPointerCount)
			{
				this.SingleFactory = factory;
			}
			
			public T Get(int index)
			{
				return _getStructElement(SingleFactory, index);
			}
			
			public T this[int index]
			{
				get { return Get(index); }
			}
			
			public sealed class Iterator : System.Collections.Generic.IEnumerator<T>
			{
				public Capnproto.StructList.Builder<T> list;
				
				public int idx = 0;
				
				public Iterator(Builder<T> _enclosing, Capnproto.StructList.Builder<T> list)
				{
					this._enclosing = _enclosing;
					this.list = list;
				}
				
				public /*override*/ T Current
				{
					get
					{
						return this.list._getStructElement(this._enclosing.SingleFactory, this.idx++);
					}
				}
				
				public /*override*/ bool MoveNext()
				{
					return this.idx < this.list.Length;
				}
				
				public /*override*/ void remove()
				{
					throw new System.NotSupportedException();
				}
				
				public void Reset()
				{
					idx = -1;
				}
				
				void IDisposable.Dispose()
				{
					
				}
				
				object IEnumerator.Current
				{
					get
					{
						return Current;
					}
				}
				
				private readonly Builder<T> _enclosing;
			}
			
			public /*override*/ System.Collections.Generic.IEnumerator<T> GetEnumerator()
			{
				return new Capnproto.StructList.Builder<T>.Iterator(this, this);
			}
			
			private IEnumerator GetEnumerator1()
			{
				return this.GetEnumerator();
			}
			
			IEnumerator IEnumerable.GetEnumerator()
			{
				return GetEnumerator1();
			}
		}
	}
}
