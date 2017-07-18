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
	public sealed class PackedInputStream : java.nio.channels.ReadableByteChannel
	{
		internal readonly Capnproto.BufferedInputStream inner;
		
		public PackedInputStream(Capnproto.BufferedInputStream input)
		{
			this.inner = input;
		}
		
		/// <exception cref="System.IO.IOException"/>
		public int Read(java.nio.ByteBuffer outBuf)
		{
			if(outBuf.buffer == null)
				outBuf.buffer = new byte[outBuf.remaining()];
			int len = outBuf.remaining();
			if(len == 0)
				return 0;
			if(len % 8 != 0)
				throw new System.Exception("PackedInputStream reads must be word-aligned");
			int outPtr = outBuf.position();
			int outEnd = outPtr + len;
			java.nio.ByteBuffer inBuf = this.inner.GetReadBuffer();
			while(true)
			{
				byte tag = 0;
				if(inBuf.remaining() < 10)
				{
					if(outBuf.remaining() == 0)
						return len;
					if(inBuf.remaining() == 0)
					{
						inBuf = this.inner.GetReadBuffer();
						continue;
					}
					//# We have at least 1, but not 10, bytes available. We need to read
					//# slowly, doing a bounds check on each byte.
					tag = inBuf.get();
					for(int i = 0; i < 8; ++i)
					{
						if((tag & (1 << i)) != 0)
						{
							if(inBuf.remaining() == 0)
								inBuf = this.inner.GetReadBuffer();
							outBuf.put(inBuf.get());
						}
						else
							outBuf.put((byte)0);
					}
					if(inBuf.remaining() == 0 && (tag == 0 || tag == 0xff))
						inBuf = this.inner.GetReadBuffer();
				}
				else
				{
					tag = inBuf.get();
					for(int n = 0; n < 8; ++n)
					{
						bool isNonzero = (tag & (1 << n)) != 0;
						outBuf.put(unchecked((byte)(inBuf.get() & (isNonzero? -1 : 0))));
						inBuf.position(inBuf.position() + (isNonzero? 0 : -1));
					}
				}
				if(tag == 0)
				{
					if(inBuf.remaining() == 0)
						throw new System.Exception("Should always have non-empty buffer here.");
					int runLength = (unchecked((int)(0xff)) & (int)inBuf.get()) * 8;
					if(runLength > outEnd - outPtr)
						throw new System.Exception("Packed input did not end cleanly on a segment boundary");
					for(int i = 0; i < runLength; ++i)
						outBuf.put((byte)0);
				}
				else if(tag == 0xff)
				{
					int runLength = (unchecked((int)(0xff)) & (int)inBuf.get()) * 8;
					if(inBuf.remaining() >= runLength)
					{
						//# Fast path.
						java.nio.ByteBuffer slice = inBuf.slice();
						slice.limit(runLength);
						outBuf.put(slice);
						inBuf.position(inBuf.position() + runLength);
					}
					else
					{
						//# Copy over the first buffer, then do one big read for the rest.
						runLength -= inBuf.remaining();
						outBuf.put(inBuf);
						java.nio.ByteBuffer slice = outBuf.slice();
						slice.limit(runLength);
						this.inner.Read(slice);
						outBuf.position(outBuf.position() + runLength);
						if(outBuf.remaining() == 0)
							return len;
						inBuf = this.inner.GetReadBuffer();
						continue;
					}
				}
				if(outBuf.remaining() == 0)
					return len;
			}
		}
		
		/// <exception cref="System.IO.IOException"/>
		public void Close()
		{
			inner.Close();
		}
		
		public bool IsOpen()
		{
			return inner.IsOpen();
		}
	}
}
