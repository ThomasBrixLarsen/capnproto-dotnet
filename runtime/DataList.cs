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
	
	public sealed class DataList
	{
		public sealed class Factory : Capnproto.ListFactory<Capnproto.DataList.Builder, Capnproto.DataList.Reader>
		{
			internal Factory() : base(Capnproto.ElementSize.POINTER)
			{
				
			}
			
			public sealed override Capnproto.DataList.Reader ConstructReader(Capnproto.SegmentReader segment, int ptr, int elementCount, int step, int structDataSize, short structPointerCount, int nestingLimit)
			{
				return new Capnproto.DataList.Reader(segment, ptr, elementCount, step, structDataSize, structPointerCount, nestingLimit);
			}
			
			public sealed override Capnproto.DataList.Builder constructBuilder(Capnproto.SegmentBuilder segment, int ptr, int elementCount, int step, int structDataSize, short structPointerCount)
			{
				return new Capnproto.DataList.Builder(segment, ptr, elementCount, step, structDataSize, structPointerCount);
			}
		}
		
		public static readonly Capnproto.DataList.Factory SingleFactory = new Capnproto.DataList.Factory();
		
		public sealed class Reader : Capnproto.ListReader, System.Collections.Generic.IEnumerable<Capnproto.Data.Reader>
		{
			public Reader(Capnproto.SegmentReader segment, int ptr, int elementCount, int step, int structDataSize, short structPointerCount, int nestingLimit) : base(segment, ptr, elementCount, step, structDataSize, structPointerCount, nestingLimit)
			{
			}
			
			public Capnproto.Data.Reader Get(int index)
			{
				return _getPointerElement(Capnproto.Data.SingleFactory, index);
			}
			
			public Capnproto.Data.Reader this[int index]
			{
				get { return Get(index); }
			}
			
			public sealed class Iterator : System.Collections.Generic.IEnumerator<Capnproto.Data.Reader>
			{
				public Capnproto.DataList.Reader list;
				
				public int idx = 0;
				
				public Iterator(Reader _enclosing, Capnproto.DataList.Reader list)
				{
					this._enclosing = _enclosing;
					this.list = list;
				}
				
				public /*override*/ Capnproto.Data.Reader Current
				{
					get
					{
						return this.list._getPointerElement(Capnproto.Data.SingleFactory, this.idx++);
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
				
				private readonly Reader _enclosing;
			}
			
			public /*override*/ System.Collections.Generic.IEnumerator<Capnproto.Data.Reader> GetEnumerator()
			{
				return new Capnproto.DataList.Reader.Iterator(this, this);
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
		
		public sealed class Builder : Capnproto.ListBuilder, System.Collections.Generic.IEnumerable<Capnproto.Data.Builder>
		{
			public Builder(Capnproto.SegmentBuilder segment, int ptr, int elementCount, int step, int structDataSize, short structPointerCount) : base(segment, ptr, elementCount, step, structDataSize, structPointerCount)
			{
				
			}
			
			public Capnproto.Data.Builder Get(int index)
			{
				return _getPointerElement(Capnproto.Data.SingleFactory, index);
			}
			
			public Capnproto.Data.Builder this[int index]
			{
				get { return Get(index); }
			}
			
			public void Set(int index, Capnproto.Data.Reader value)
			{
				_setPointerElement(Capnproto.Data.SingleFactory, index, value);
			}
			
			public sealed class Iterator : System.Collections.Generic.IEnumerator<Capnproto.Data.Builder>
			{
				public Capnproto.DataList.Builder list;
				
				public int idx = 0;
				
				public Iterator(Builder _enclosing, Capnproto.DataList.Builder list)
				{
					this._enclosing = _enclosing;
					this.list = list;
				}
				
				public /*override*/ Capnproto.Data.Builder Current
				{
					get
					{
						return this.list._getPointerElement(Capnproto.Data.SingleFactory, this.idx++);
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
				
				private readonly Builder _enclosing;
			}
			
			public /*override*/ System.Collections.Generic.IEnumerator<Capnproto.Data.Builder> GetEnumerator()
			{
				return new Capnproto.DataList.Builder.Iterator(this, this);
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
