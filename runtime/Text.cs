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
	public sealed class Text
	{
		public sealed class Factory : Capnproto.FromPointerReaderBlobDefault<Capnproto.Text.Reader>,
		                              Capnproto.FromPointerBuilderBlobDefault<Capnproto.Text.Builder>,
		                              Capnproto.PointerFactory<Capnproto.Text.Builder, Capnproto.Text.Reader>,
		                              Capnproto.SetPointerBuilder<Capnproto.Text.Builder, Capnproto.Text.Reader>
		{
			public Capnproto.Text.Reader fromPointerReaderBlobDefault(Capnproto.SegmentReader segment, int pointer, java.nio.ByteBuffer defaultBuffer, int defaultOffset, int defaultSize)
			{
				return Capnproto.WireHelpers.readTextPointer(segment, pointer, defaultBuffer, defaultOffset, defaultSize);
			}
			
			public Capnproto.Text.Reader fromPointerReader(Capnproto.SegmentReader segment, int pointer, int nestingLimit)
			{
				return Capnproto.WireHelpers.readTextPointer(segment, pointer, null, 0, 0);
			}
			
			public Capnproto.Text.Builder fromPointerBuilderBlobDefault(Capnproto.SegmentBuilder segment, int pointer, java.nio.ByteBuffer defaultBuffer, int defaultOffset, int defaultSize)
			{
				return Capnproto.WireHelpers.getWritableTextPointer(pointer, segment, defaultBuffer, defaultOffset, defaultSize);
			}
			
			public Capnproto.Text.Builder fromPointerBuilder(Capnproto.SegmentBuilder segment, int pointer)
			{
				return Capnproto.WireHelpers.getWritableTextPointer(pointer, segment, null, 0, 0);
			}
			
			public Capnproto.Text.Builder initFromPointerBuilder(Capnproto.SegmentBuilder segment, int pointer, int size)
			{
				return Capnproto.WireHelpers.initTextPointer(pointer, segment, size);
			}
			
			public void setPointerBuilder(Capnproto.SegmentBuilder segment, int pointer, Capnproto.Text.Reader value)
			{
				Capnproto.WireHelpers.setTextPointer(pointer, segment, value);
			}
		}
		
		public static readonly Capnproto.Text.Factory SingleFactory = new Capnproto.Text.Factory();
		
		public sealed class Reader
		{
			public readonly java.nio.ByteBuffer buffer;
			
			public readonly int offset;
			
			public readonly int size;
			
			public Reader()
			{
				//in bytes
				//in bytes, not including NUL terminator
				//TODO: What about the null terminator?
				this.buffer = java.nio.ByteBuffer.allocate(0);
				this.offset = 0;
				this.size = 0;
			}
			
			public Reader(java.nio.ByteBuffer buffer, int offset, int size)
			{
				this.buffer = buffer;
				this.offset = offset * 8;
				this.size = size;
			}
			
			public Reader(string value)
			{
				try
				{
					byte[] bytes = Sharpen.Runtime.GetBytesForString(value, "UTF-8");
					this.buffer = java.nio.ByteBuffer.wrap(bytes);
					this.offset = 0;
					this.size = bytes.Length;
				}
				catch(System.Exception)
				{
					throw new System.Exception("UTF-8 is unsupported");
				}
			}
			
			public int Size
			{
				get
				{
					return this.size;
				}
			}
			
			public java.nio.ByteBuffer asByteBuffer()
			{
				java.nio.ByteBuffer dup = this.buffer.asReadOnlyBuffer();
				dup.position(this.offset);
				java.nio.ByteBuffer result = dup.slice();
				result.limit(this.size);
				return result;
			}
			
			public sealed override string ToString()
			{
				byte[] bytes = new byte[this.size];
				java.nio.ByteBuffer dup = this.buffer.duplicate();
				dup.position(this.offset);
				dup.get(bytes, 0, this.size);
				try
				{
					return Sharpen.Runtime.GetStringForBytes(bytes, "UTF-8");
				}
				catch(System.Exception)
				{
					throw new System.Exception("UTF-8 is unsupported");
				}
			}
		}
		
		public sealed class Builder
		{
			public readonly java.nio.ByteBuffer buffer;
			
			public readonly int offset;
			
			public readonly int size;
			
			public Builder()
			{
				//in bytes
				//in bytes
				this.buffer = java.nio.ByteBuffer.allocate(0);
				this.offset = 0;
				this.size = 0;
			}
			
			public Builder(java.nio.ByteBuffer buffer, int offset, int size)
			{
				this.buffer = buffer;
				this.offset = offset;
				this.size = size;
			}
			
			public java.nio.ByteBuffer asByteBuffer()
			{
				java.nio.ByteBuffer dup = this.buffer.duplicate();
				dup.position(this.offset);
				java.nio.ByteBuffer result = dup.slice();
				result.limit(this.size);
				return result;
			}
			
			public sealed override string ToString()
			{
				byte[] bytes = new byte[this.size];
				java.nio.ByteBuffer dup = this.buffer.duplicate();
				dup.position(this.offset);
				dup.get(bytes, 0, this.size);
				try
				{
					return Sharpen.Runtime.GetStringForBytes(bytes, "UTF-8");
				}
				catch(System.Exception)
				{
					throw new System.Exception("UTF-8 is unsupported");
				}
			}
		}
	}
}
