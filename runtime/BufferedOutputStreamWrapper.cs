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
	public sealed class BufferedOutputStreamWrapper : Capnproto.BufferedOutputStream
	{
		private readonly java.nio.channels.WritableByteChannel inner;
		
		private readonly java.nio.ByteBuffer buf;
		
		public BufferedOutputStreamWrapper(java.nio.channels.WritableByteChannel w)
		{
			this.inner = w;
			this.buf = java.nio.ByteBuffer.allocate(8192);
		}
		
		/// <exception cref="System.IO.IOException"/>
		public int Write(java.nio.ByteBuffer src)
		{
			int available = this.buf.remaining();
			int size = src.remaining();
			if(size <= available)
				this.buf.put(src);
			else if(size <= this.buf.capacity())
			{
				//# Too much for this buffer, but not a full buffer's worth,
				//# so we'll go ahead and copy.
				java.nio.ByteBuffer slice = src.slice();
				slice.limit(available);
				this.buf.put(slice);
				this.buf.rewind();
				while(this.buf.hasRemaining())
					this.inner.Write(this.buf);
				this.buf.rewind();
				src.position(src.position() + available);
				this.buf.put(src);
			}
			else
			{
				//# Writing so much data that we might as well write
				//# directly to avoid a copy.
				int pos = this.buf.position();
				this.buf.rewind();
				java.nio.ByteBuffer slice = this.buf.slice();
				slice.limit(pos);
				while(slice.hasRemaining())
					this.inner.Write(slice);
				while(src.hasRemaining())
					this.inner.Write(src);
			}
			return size;
		}
		
		public java.nio.ByteBuffer GetWriteBuffer()
		{
			return this.buf;
		}
		
		/// <exception cref="System.IO.IOException"/>
		public void Close()
		{
			this.inner.Close();
		}
		
		public bool IsOpen()
		{
			return this.inner.IsOpen();
		}
		
		/// <exception cref="System.IO.IOException"/>
		public void Flush()
		{
			int pos = this.buf.position();
			this.buf.rewind();
			this.buf.limit(pos);
			this.inner.Write(this.buf);
			this.buf.clear();
		}
	}
}
