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
	public sealed class ArrayInputStream : Capnproto.BufferedInputStream
	{
		public readonly java.nio.ByteBuffer buf;
		
		public ArrayInputStream(java.nio.ByteBuffer buf)
		{
			this.buf = buf;
		}
		
		/// <exception cref="System.IO.IOException"/>
		public int Read(java.nio.ByteBuffer dst)
		{
			int size = System.Math.Min(dst.remaining(), this.buf.remaining());
			java.nio.ByteBuffer slice = this.buf.slice();
			slice.limit(size);
			dst.buffer = slice.buffer;
			dst.offset = slice.offset;
			dst.limit(size);
			dst.position(size);
			this.buf.position(this.buf.position() + size);
			return size;
		}
		
		public java.nio.ByteBuffer GetReadBuffer()
		{
			return this.buf;
		}
		
		/// <exception cref="System.IO.IOException"/>
		public void Close()
		{
			return;
		}
		
		public bool IsOpen()
		{
			return true;
		}
	}
}
