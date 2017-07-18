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
	public abstract class StructFactory<Builder, Reader> : Capnproto.PointerFactory<Builder, Reader>,
	                                                       Capnproto.FromPointerBuilderRefDefault<Builder>,
	                                                       Capnproto.StructBuilder.Factory<Builder>,
	                                                       Capnproto.SetPointerBuilder<Builder, Reader>,
	                                                       Capnproto.FromPointerReaderRefDefault<Reader>,
	                                                       Capnproto.StructReader.Factory<Reader>
	                                                       where Reader : Capnproto.StructReader
	{
		public Reader fromPointerReaderRefDefault(Capnproto.SegmentReader segment, int pointer, Capnproto.SegmentReader defaultSegment, int defaultOffset, int nestingLimit)
		{
			return Capnproto.WireHelpers.readStructPointer(this, segment, pointer, defaultSegment, defaultOffset, nestingLimit);
		}
		
		public Reader fromPointerReader(Capnproto.SegmentReader segment, int pointer, int nestingLimit)
		{
			return fromPointerReaderRefDefault(segment, pointer, null, 0, nestingLimit);
		}
		
		public Builder fromPointerBuilderRefDefault(Capnproto.SegmentBuilder segment, int pointer, Capnproto.SegmentReader defaultSegment, int defaultOffset)
		{
			return Capnproto.WireHelpers.getWritableStructPointer(this, pointer, segment, this.StructSize(), defaultSegment, defaultOffset);
		}
		
		public Builder fromPointerBuilder(Capnproto.SegmentBuilder segment, int pointer)
		{
			return Capnproto.WireHelpers.getWritableStructPointer(this, pointer, segment, this.StructSize(), null, 0);
		}
		
		public Builder initFromPointerBuilder(Capnproto.SegmentBuilder segment, int pointer, int elementCount)
		{
			return Capnproto.WireHelpers.initStructPointer(this, pointer, segment, this.StructSize());
		}
		
		public void setPointerBuilder(Capnproto.SegmentBuilder segment, int pointer, Reader value)
		{
			Capnproto.WireHelpers.setStructPointer(segment, pointer, value);
		}
		
		public abstract Reader AsReader(Builder builder);
		
		public abstract Builder constructBuilder(Capnproto.SegmentBuilder arg1, int arg2, int arg3, int arg4, short arg5);
		
		public abstract Capnproto.StructSize StructSize();
		
		public abstract Reader ConstructReader(Capnproto.SegmentReader arg1, int arg2, int arg3, int arg4, short arg5, int arg6);
	}
}
