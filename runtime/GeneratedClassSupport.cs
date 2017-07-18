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
	public sealed class GeneratedClassSupport
	{
		public static Capnproto.SegmentReader decodeRawBytes(string s)
		{
			try
			{
				java.nio.ByteBuffer buffer = java.nio.ByteBuffer.wrap(Sharpen.Runtime.GetBytesForString(s, "ISO_8859-1")).asReadOnlyBuffer();
				buffer.order(java.nio.ByteOrder.LITTLE_ENDIAN);
				return new Capnproto.SegmentReader(buffer, new Capnproto.ReaderArena(new java.nio.ByteBuffer[0], unchecked((long)(0x7fffffffffffffffL))));
			}
			catch(System.Exception)
			{
				throw new System.Exception("could not decode raw bytes from String");
			}
		}
	}
}
