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
	public class PrimitiveList
	{
		public class Void
		{
			public sealed class Factory : Capnproto.ListFactory<Capnproto.PrimitiveList.Void.Builder, Capnproto.PrimitiveList.Void.Reader>
			{
				internal Factory() : base(Capnproto.ElementSize.VOID)
				{
					
				}
				
				public sealed override Capnproto.PrimitiveList.Void.Reader ConstructReader(Capnproto.SegmentReader segment, int ptr, int elementCount, int step, int structDataSize, short structPointerCount, int nestingLimit)
				{
					return new Capnproto.PrimitiveList.Void.Reader(segment, ptr, elementCount, step, structDataSize, structPointerCount, nestingLimit);
				}
				
				public sealed override Capnproto.PrimitiveList.Void.Builder constructBuilder(Capnproto.SegmentBuilder segment, int ptr, int elementCount, int step, int structDataSize, short structPointerCount)
				{
					return new Capnproto.PrimitiveList.Void.Builder(segment, ptr, elementCount, step, structDataSize, structPointerCount);
				}
			}
			
			public static readonly Capnproto.PrimitiveList.Void.Factory SingleFactory = new Capnproto.PrimitiveList.Void.Factory();
			
			public sealed class Reader : Capnproto.ListReader
			{
				public Reader(Capnproto.SegmentReader segment, int ptr, int elementCount, int step, int structDataSize, short structPointerCount, int nestingLimit) : base(segment, ptr, elementCount, step, structDataSize, structPointerCount, nestingLimit)
				{
					
				}
				
				public Capnproto.Void get(int index)
				{
					return Capnproto.Void.VOID;
				}
			}
			
			public sealed class Builder : Capnproto.ListBuilder
			{
				public Builder(Capnproto.SegmentBuilder segment, int ptr, int elementCount, int step, int structDataSize, short structPointerCount) : base(segment, ptr, elementCount, step, structDataSize, structPointerCount)
				{
					
				}
			}
		}
		
		public class Boolean
		{
			public sealed class Factory : Capnproto.ListFactory<Capnproto.PrimitiveList.Boolean.Builder, Capnproto.PrimitiveList.Boolean.Reader>
			{
				internal Factory() : base(Capnproto.ElementSize.BIT)
				{
					
				}
				
				public sealed override Capnproto.PrimitiveList.Boolean.Reader ConstructReader(Capnproto.SegmentReader segment, int ptr, int elementCount, int step, int structDataSize, short structPointerCount, int nestingLimit)
				{
					return new Capnproto.PrimitiveList.Boolean.Reader(segment, ptr, elementCount, step, structDataSize, structPointerCount, nestingLimit);
				}
				
				public sealed override Capnproto.PrimitiveList.Boolean.Builder constructBuilder(Capnproto.SegmentBuilder segment, int ptr, int elementCount, int step, int structDataSize, short structPointerCount)
				{
					return new Capnproto.PrimitiveList.Boolean.Builder(segment, ptr, elementCount, step, structDataSize, structPointerCount);
				}
			}
			
			public static readonly Capnproto.PrimitiveList.Boolean.Factory SingleFactory = new Capnproto.PrimitiveList.Boolean.Factory();
			
			public sealed class Reader : Capnproto.ListReader
			{
				public Reader(Capnproto.SegmentReader segment, int ptr, int elementCount, int step, int structDataSize, short structPointerCount, int nestingLimit) : base(segment, ptr, elementCount, step, structDataSize, structPointerCount, nestingLimit)
				{
					
				}
				
				public bool Get(int index)
				{
					return _getBooleanElement(index);
				}
				
				public bool this[int index]
				{
					get { return Get(index); }
				}
			}
			
			public sealed class Builder : Capnproto.ListBuilder
			{
				public Builder(Capnproto.SegmentBuilder segment, int ptr, int elementCount, int step, int structDataSize, short structPointerCount) : base(segment, ptr, elementCount, step, structDataSize, structPointerCount)
				{
				}
				
				public bool Get(int index)
				{
					return _getBooleanElement(index);
				}
				
				public void Set(int index, bool value)
				{
					_setBooleanElement(index, value);
				}
				
				public bool this[int index]
				{
					get { return Get(index); }
					set { Set(index, value); }
				}
			}
		}
		
		public class Sbyte
		{
			public sealed class Factory : Capnproto.ListFactory<Capnproto.PrimitiveList.Sbyte.Builder, Capnproto.PrimitiveList.Sbyte.Reader>
			{
				internal Factory() : base(Capnproto.ElementSize.BYTE)
				{
					
				}
				
				public sealed override Capnproto.PrimitiveList.Sbyte.Reader ConstructReader(Capnproto.SegmentReader segment, int ptr, int elementCount, int step, int structDataSize, short structPointerCount, int nestingLimit)
				{
					return new Capnproto.PrimitiveList.Sbyte.Reader(segment, ptr, elementCount, step, structDataSize, structPointerCount, nestingLimit);
				}
				
				public sealed override Capnproto.PrimitiveList.Sbyte.Builder constructBuilder(Capnproto.SegmentBuilder segment, int ptr, int elementCount, int step, int structDataSize, short structPointerCount)
				{
					return new Capnproto.PrimitiveList.Sbyte.Builder(segment, ptr, elementCount, step, structDataSize, structPointerCount);
				}
			}
			
			public static readonly Capnproto.PrimitiveList.Sbyte.Factory SingleFactory = new Capnproto.PrimitiveList.Sbyte.Factory();
			
			public sealed class Reader : Capnproto.ListReader
			{
				public Reader(Capnproto.SegmentReader segment, int ptr, int elementCount, int step, int structDataSize, short structPointerCount, int nestingLimit) : base(segment, ptr, elementCount, step, structDataSize, structPointerCount, nestingLimit)
				{
					
				}
				
				public sbyte Get(int index)
				{
					return _getSbyteElement(index);
				}
				
				public sbyte this[int index]
				{
					get { return Get(index); }
				}
			}
			
			public sealed class Builder : Capnproto.ListBuilder
			{
				public Builder(Capnproto.SegmentBuilder segment, int ptr, int elementCount, int step, int structDataSize, short structPointerCount) : base(segment, ptr, elementCount, step, structDataSize, structPointerCount)
				{
					
				}
				
				public sbyte Get(int index)
				{
					return _getSbyteElement(index);
				}
				
				public void Set(int index, sbyte value)
				{
					_setSbyteElement(index, value);
				}
				
				public sbyte this[int index]
				{
					get { return Get(index); }
					set { Set(index, value); }
				}
			}
		}
		
		public class Byte
		{
			public sealed class Factory : Capnproto.ListFactory<Capnproto.PrimitiveList.Byte.Builder, Capnproto.PrimitiveList.Byte.Reader>
			{
				internal Factory() : base(Capnproto.ElementSize.BYTE)
				{
					
				}
				
				public sealed override Capnproto.PrimitiveList.Byte.Reader ConstructReader(Capnproto.SegmentReader segment, int ptr, int elementCount, int step, int structDataSize, short structPointerCount, int nestingLimit)
				{
					return new Capnproto.PrimitiveList.Byte.Reader(segment, ptr, elementCount, step, structDataSize, structPointerCount, nestingLimit);
				}
				
				public sealed override Capnproto.PrimitiveList.Byte.Builder constructBuilder(Capnproto.SegmentBuilder segment, int ptr, int elementCount, int step, int structDataSize, short structPointerCount)
				{
					return new Capnproto.PrimitiveList.Byte.Builder(segment, ptr, elementCount, step, structDataSize, structPointerCount);
				}
			}
			
			public static readonly Capnproto.PrimitiveList.Byte.Factory SingleFactory = new Capnproto.PrimitiveList.Byte.Factory();
			
			public sealed class Reader : Capnproto.ListReader
			{
				public Reader(Capnproto.SegmentReader segment, int ptr, int elementCount, int step, int structDataSize, short structPointerCount, int nestingLimit) : base(segment, ptr, elementCount, step, structDataSize, structPointerCount, nestingLimit)
				{
					
				}
				
				public byte Get(int index)
				{
					return _getByteElement(index);
				}
				
				public byte this[int index]
				{
					get { return Get(index); }
				}
			}
			
			public sealed class Builder : Capnproto.ListBuilder
			{
				public Builder(Capnproto.SegmentBuilder segment, int ptr, int elementCount, int step, int structDataSize, short structPointerCount) : base(segment, ptr, elementCount, step, structDataSize, structPointerCount)
				{
					
				}
				
				public byte Get(int index)
				{
					return _getByteElement(index);
				}
				
				public void Set(int index, byte value)
				{
					_setByteElement(index, value);
				}
				
				public byte this[int index]
				{
					get { return Get(index); }
					set { Set(index, value); }
				}
			}
		}
		
		public class Short
		{
			public sealed class Factory : Capnproto.ListFactory<Capnproto.PrimitiveList.Short.Builder, Capnproto.PrimitiveList.Short.Reader>
			{
				internal Factory() : base(Capnproto.ElementSize.TWO_BYTES)
				{
					
				}
				
				public sealed override Capnproto.PrimitiveList.Short.Reader ConstructReader(Capnproto.SegmentReader segment, int ptr, int elementCount, int step, int structDataSize, short structPointerCount, int nestingLimit)
				{
					return new Capnproto.PrimitiveList.Short.Reader(segment, ptr, elementCount, step, structDataSize, structPointerCount, nestingLimit);
				}
				
				public sealed override Capnproto.PrimitiveList.Short.Builder constructBuilder(Capnproto.SegmentBuilder segment, int ptr, int elementCount, int step, int structDataSize, short structPointerCount)
				{
					return new Capnproto.PrimitiveList.Short.Builder(segment, ptr, elementCount, step, structDataSize, structPointerCount);
				}
			}
			
			public static readonly Capnproto.PrimitiveList.Short.Factory SingleFactory = new Capnproto.PrimitiveList.Short.Factory();
			
			public sealed class Reader : Capnproto.ListReader
			{
				public Reader(Capnproto.SegmentReader segment, int ptr, int elementCount, int step, int structDataSize, short structPointerCount, int nestingLimit) : base(segment, ptr, elementCount, step, structDataSize, structPointerCount, nestingLimit)
				{
					
				}
				
				public short Get(int index)
				{
					return _getShortElement(index);
				}
				
				public short this[int index]
				{
					get { return Get(index); }
				}
			}
			
			public sealed class Builder : Capnproto.ListBuilder
			{
				public Builder(Capnproto.SegmentBuilder segment, int ptr, int elementCount, int step, int structDataSize, short structPointerCount) : base(segment, ptr, elementCount, step, structDataSize, structPointerCount)
				{
					
				}
				
				public short Get(int index)
				{
					return _getShortElement(index);
				}
				
				public void Set(int index, short value)
				{
					_setShortElement(index, value);
				}
				
				public short this[int index]
				{
					get { return Get(index); }
					set { Set(index, value); }
				}
			}
		}
		
		public class Ushort
		{
			public sealed class Factory : Capnproto.ListFactory<Capnproto.PrimitiveList.Ushort.Builder, Capnproto.PrimitiveList.Ushort.Reader>
			{
				internal Factory() : base(Capnproto.ElementSize.TWO_BYTES)
				{
					
				}
				
				public sealed override Capnproto.PrimitiveList.Ushort.Reader ConstructReader(Capnproto.SegmentReader segment, int ptr, int elementCount, int step, int structDataSize, short structPointerCount, int nestingLimit)
				{
					return new Capnproto.PrimitiveList.Ushort.Reader(segment, ptr, elementCount, step, structDataSize, structPointerCount, nestingLimit);
				}
				
				public sealed override Capnproto.PrimitiveList.Ushort.Builder constructBuilder(Capnproto.SegmentBuilder segment, int ptr, int elementCount, int step, int structDataSize, short structPointerCount)
				{
					return new Capnproto.PrimitiveList.Ushort.Builder(segment, ptr, elementCount, step, structDataSize, structPointerCount);
				}
			}
			
			public static readonly Capnproto.PrimitiveList.Ushort.Factory SingleFactory = new Capnproto.PrimitiveList.Ushort.Factory();
			
			public sealed class Reader : Capnproto.ListReader
			{
				public Reader(Capnproto.SegmentReader segment, int ptr, int elementCount, int step, int structDataSize, short structPointerCount, int nestingLimit) : base(segment, ptr, elementCount, step, structDataSize, structPointerCount, nestingLimit)
				{
					
				}
				
				public ushort Get(int index)
				{
					return _getUshortElement(index);
				}
				
				public ushort this[int index]
				{
					get { return Get(index); }
				}
			}
			
			public sealed class Builder : Capnproto.ListBuilder
			{
				public Builder(Capnproto.SegmentBuilder segment, int ptr, int elementCount, int step, int structDataSize, short structPointerCount) : base(segment, ptr, elementCount, step, structDataSize, structPointerCount)
				{
					
				}
				
				public ushort Get(int index)
				{
					return _getUshortElement(index);
				}
				
				public void Set(int index, ushort value)
				{
					_setUshortElement(index, value);
				}
				
				public ushort this[int index]
				{
					get { return Get(index); }
					set { Set(index, value); }
				}
			}
		}
		
		public class Int
		{
			public sealed class Factory : Capnproto.ListFactory<Capnproto.PrimitiveList.Int.Builder, Capnproto.PrimitiveList.Int.Reader>
			{
				internal Factory() : base(Capnproto.ElementSize.FOUR_BYTES)
				{
					
				}
				
				public sealed override Capnproto.PrimitiveList.Int.Reader ConstructReader(Capnproto.SegmentReader segment, int ptr, int elementCount, int step, int structDataSize, short structPointerCount, int nestingLimit)
				{
					return new Capnproto.PrimitiveList.Int.Reader(segment, ptr, elementCount, step, structDataSize, structPointerCount, nestingLimit);
				}
				
				public sealed override Capnproto.PrimitiveList.Int.Builder constructBuilder(Capnproto.SegmentBuilder segment, int ptr, int elementCount, int step, int structDataSize, short structPointerCount)
				{
					return new Capnproto.PrimitiveList.Int.Builder(segment, ptr, elementCount, step, structDataSize, structPointerCount);
				}
			}
			
			public static readonly Capnproto.PrimitiveList.Int.Factory SingleFactory = new Capnproto.PrimitiveList.Int.Factory();
			
			public sealed class Reader : Capnproto.ListReader
			{
				public Reader(Capnproto.SegmentReader segment, int ptr, int elementCount, int step, int structDataSize, short structPointerCount, int nestingLimit) : base(segment, ptr, elementCount, step, structDataSize, structPointerCount, nestingLimit)
				{
					
				}
				
				public int Get(int index)
				{
					return _getIntElement(index);
				}
				
				public int this[int index]
				{
					get { return Get(index); }
				}
			}
			
			public sealed class Builder : Capnproto.ListBuilder
			{
				public Builder(Capnproto.SegmentBuilder segment, int ptr, int elementCount, int step, int structDataSize, short structPointerCount) : base(segment, ptr, elementCount, step, structDataSize, structPointerCount)
				{
					
				}
				
				public int Get(int index)
				{
					return _getIntElement(index);
				}
				
				public void Set(int index, int value)
				{
					_setIntElement(index, value);
				}
				
				public int this[int index]
				{
					get { return Get(index); }
					set { Set(index, value); }
				}
			}
		}
		
		public class Uint
		{
			public sealed class Factory : Capnproto.ListFactory<Capnproto.PrimitiveList.Uint.Builder, Capnproto.PrimitiveList.Uint.Reader>
			{
				internal Factory() : base(Capnproto.ElementSize.FOUR_BYTES)
				{
					
				}
				
				public sealed override Capnproto.PrimitiveList.Uint.Reader ConstructReader(Capnproto.SegmentReader segment, int ptr, int elementCount, int step, int structDataSize, short structPointerCount, int nestingLimit)
				{
					return new Capnproto.PrimitiveList.Uint.Reader(segment, ptr, elementCount, step, structDataSize, structPointerCount, nestingLimit);
				}
				
				public sealed override Capnproto.PrimitiveList.Uint.Builder constructBuilder(Capnproto.SegmentBuilder segment, int ptr, int elementCount, int step, int structDataSize, short structPointerCount)
				{
					return new Capnproto.PrimitiveList.Uint.Builder(segment, ptr, elementCount, step, structDataSize, structPointerCount);
				}
			}
			
			public static readonly Capnproto.PrimitiveList.Uint.Factory SingleFactory = new Capnproto.PrimitiveList.Uint.Factory();
			
			public sealed class Reader : Capnproto.ListReader
			{
				public Reader(Capnproto.SegmentReader segment, int ptr, int elementCount, int step, int structDataSize, short structPointerCount, int nestingLimit) : base(segment, ptr, elementCount, step, structDataSize, structPointerCount, nestingLimit)
				{
					
				}
				
				public uint Get(int index)
				{
					return _getUintElement(index);
				}
				
				public uint this[int index]
				{
					get { return Get(index); }
				}
			}
			
			public sealed class Builder : Capnproto.ListBuilder
			{
				public Builder(Capnproto.SegmentBuilder segment, int ptr, int elementCount, int step, int structDataSize, short structPointerCount) : base(segment, ptr, elementCount, step, structDataSize, structPointerCount)
				{
					
				}
				
				public uint Get(int index)
				{
					return _getUintElement(index);
				}
				
				public void Set(int index, uint value)
				{
					_setUintElement(index, value);
				}
				
				public uint this[int index]
				{
					get { return Get(index); }
					set { Set(index, value); }
				}
			}
		}
		
		public class Float
		{
			public sealed class Factory : Capnproto.ListFactory<Capnproto.PrimitiveList.Float.Builder, Capnproto.PrimitiveList.Float.Reader>
			{
				internal Factory() : base(Capnproto.ElementSize.FOUR_BYTES)
				{
					
				}
				
				public sealed override Capnproto.PrimitiveList.Float.Reader ConstructReader(Capnproto.SegmentReader segment, int ptr, int elementCount, int step, int structDataSize, short structPointerCount, int nestingLimit)
				{
					return new Capnproto.PrimitiveList.Float.Reader(segment, ptr, elementCount, step, structDataSize, structPointerCount, nestingLimit);
				}
				
				public sealed override Capnproto.PrimitiveList.Float.Builder constructBuilder(Capnproto.SegmentBuilder segment, int ptr, int elementCount, int step, int structDataSize, short structPointerCount)
				{
					return new Capnproto.PrimitiveList.Float.Builder(segment, ptr, elementCount, step, structDataSize, structPointerCount);
				}
			}
			
			public static readonly Capnproto.PrimitiveList.Float.Factory SingleFactory = new Capnproto.PrimitiveList.Float.Factory();
			
			public sealed class Reader : Capnproto.ListReader
			{
				public Reader(Capnproto.SegmentReader segment, int ptr, int elementCount, int step, int structDataSize, short structPointerCount, int nestingLimit) : base(segment, ptr, elementCount, step, structDataSize, structPointerCount, nestingLimit)
				{
					
				}
				
				public float Get(int index)
				{
					return _getFloatElement(index);
				}
				
				public float this[int index]
				{
					get { return Get(index); }
				}
			}
			
			public sealed class Builder : Capnproto.ListBuilder
			{
				public Builder(Capnproto.SegmentBuilder segment, int ptr, int elementCount, int step, int structDataSize, short structPointerCount) : base(segment, ptr, elementCount, step, structDataSize, structPointerCount)
				{
					
				}
				
				public float Get(int index)
				{
					return _getFloatElement(index);
				}
				
				public void Set(int index, float value)
				{
					_setFloatElement(index, value);
				}
				
				public float this[int index]
				{
					get { return Get(index); }
					set { Set(index, value); }
				}
			}
		}
		
		public class Long
		{
			public sealed class Factory : Capnproto.ListFactory<Capnproto.PrimitiveList.Long.Builder, Capnproto.PrimitiveList.Long.Reader>
			{
				internal Factory() : base(Capnproto.ElementSize.EIGHT_BYTES)
				{
					
				}
				
				public sealed override Capnproto.PrimitiveList.Long.Reader ConstructReader(Capnproto.SegmentReader segment, int ptr, int elementCount, int step, int structDataSize, short structPointerCount, int nestingLimit)
				{
					return new Capnproto.PrimitiveList.Long.Reader(segment, ptr, elementCount, step, structDataSize, structPointerCount, nestingLimit);
				}
				
				public sealed override Capnproto.PrimitiveList.Long.Builder constructBuilder(Capnproto.SegmentBuilder segment, int ptr, int elementCount, int step, int structDataSize, short structPointerCount)
				{
					return new Capnproto.PrimitiveList.Long.Builder(segment, ptr, elementCount, step, structDataSize, structPointerCount);
				}
			}
			
			public static readonly Capnproto.PrimitiveList.Long.Factory SingleFactory = new Capnproto.PrimitiveList.Long.Factory();
			
			public sealed class Reader : Capnproto.ListReader
			{
				public Reader(Capnproto.SegmentReader segment, int ptr, int elementCount, int step, int structDataSize, short structPointerCount, int nestingLimit) : base(segment, ptr, elementCount, step, structDataSize, structPointerCount, nestingLimit)
				{
					
				}
				
				public long Get(int index)
				{
					return _getLongElement(index);
				}
				
				public long this[int index]
				{
					get { return Get(index); }
				}
			}
			
			public sealed class Builder : Capnproto.ListBuilder
			{
				public Builder(Capnproto.SegmentBuilder segment, int ptr, int elementCount, int step, int structDataSize, short structPointerCount) : base(segment, ptr, elementCount, step, structDataSize, structPointerCount)
				{
					
				}
				
				public long Get(int index)
				{
					return _getLongElement(index);
				}
				
				public void Set(int index, long value)
				{
					_setLongElement(index, value);
				}
				
				public long this[int index]
				{
					get { return Get(index); }
					set { Set(index, value); }
				}
			}
		}
		
		public class Ulong
		{
			public sealed class Factory : Capnproto.ListFactory<Capnproto.PrimitiveList.Ulong.Builder, Capnproto.PrimitiveList.Ulong.Reader>
			{
				internal Factory() : base(Capnproto.ElementSize.EIGHT_BYTES)
				{
					
				}
				
				public sealed override Capnproto.PrimitiveList.Ulong.Reader ConstructReader(Capnproto.SegmentReader segment, int ptr, int elementCount, int step, int structDataSize, short structPointerCount, int nestingLimit)
				{
					return new Capnproto.PrimitiveList.Ulong.Reader(segment, ptr, elementCount, step, structDataSize, structPointerCount, nestingLimit);
				}
				
				public sealed override Capnproto.PrimitiveList.Ulong.Builder constructBuilder(Capnproto.SegmentBuilder segment, int ptr, int elementCount, int step, int structDataSize, short structPointerCount)
				{
					return new Capnproto.PrimitiveList.Ulong.Builder(segment, ptr, elementCount, step, structDataSize, structPointerCount);
				}
			}
			
			public static readonly Capnproto.PrimitiveList.Ulong.Factory SingleFactory = new Capnproto.PrimitiveList.Ulong.Factory();
			
			public sealed class Reader : Capnproto.ListReader
			{
				public Reader(Capnproto.SegmentReader segment, int ptr, int elementCount, int step, int structDataSize, short structPointerCount, int nestingLimit) : base(segment, ptr, elementCount, step, structDataSize, structPointerCount, nestingLimit)
				{
					
				}
				
				public ulong Get(int index)
				{
					return _getUlongElement(index);
				}
				
				public ulong this[int index]
				{
					get { return Get(index); }
				}
			}
			
			public sealed class Builder : Capnproto.ListBuilder
			{
				public Builder(Capnproto.SegmentBuilder segment, int ptr, int elementCount, int step, int structDataSize, short structPointerCount) : base(segment, ptr, elementCount, step, structDataSize, structPointerCount)
				{
					
				}
				
				public ulong Get(int index)
				{
					return _getUlongElement(index);
				}
				
				public void Set(int index, ulong value)
				{
					_setUlongElement(index, value);
				}
				
				public ulong this[int index]
				{
					get { return Get(index); }
					set { Set(index, value); }
				}
			}
		}
		
		public class Double
		{
			public sealed class Factory : Capnproto.ListFactory<Capnproto.PrimitiveList.Double.Builder, Capnproto.PrimitiveList.Double.Reader>
			{
				internal Factory() : base(Capnproto.ElementSize.EIGHT_BYTES)
				{
					
				}
				
				public sealed override Capnproto.PrimitiveList.Double.Reader ConstructReader(Capnproto.SegmentReader segment, int ptr, int elementCount, int step, int structDataSize, short structPointerCount, int nestingLimit)
				{
					return new Capnproto.PrimitiveList.Double.Reader(segment, ptr, elementCount, step, structDataSize, structPointerCount, nestingLimit);
				}
				
				public sealed override Capnproto.PrimitiveList.Double.Builder constructBuilder(Capnproto.SegmentBuilder segment, int ptr, int elementCount, int step, int structDataSize, short structPointerCount)
				{
					return new Capnproto.PrimitiveList.Double.Builder(segment, ptr, elementCount, step, structDataSize, structPointerCount);
				}
			}
			
			public static readonly Capnproto.PrimitiveList.Double.Factory SingleFactory = new Capnproto.PrimitiveList.Double.Factory();
			
			public sealed class Reader : Capnproto.ListReader
			{
				public Reader(Capnproto.SegmentReader segment, int ptr, int elementCount, int step, int structDataSize, short structPointerCount, int nestingLimit) : base(segment, ptr, elementCount, step, structDataSize, structPointerCount, nestingLimit)
				{
					
				}
				
				public double Get(int index)
				{
					return _getDoubleElement(index);
				}
				
				public double this[int index]
				{
					get { return Get(index); }
				}
			}
			
			public sealed class Builder : Capnproto.ListBuilder
			{
				public Builder(Capnproto.SegmentBuilder segment, int ptr, int elementCount, int step, int structDataSize, short structPointerCount) : base(segment, ptr, elementCount, step, structDataSize, structPointerCount)
				{
					
				}
				
				public double Get(int index)
				{
					return _getDoubleElement(index);
				}
				
				public void Set(int index, double value)
				{
					_setDoubleElement(index, value);
				}
				
				public double this[int index]
				{
					get { return Get(index); }
					set { Set(index, value); }
				}
			}
		}
	}
}
