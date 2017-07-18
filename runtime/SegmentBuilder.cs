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
	public sealed class SegmentBuilder : Capnproto.SegmentReader
	{
		public const int FAILED_ALLOCATION = -1;
		
		public int pos = 0;
		
		public int id = 0;
		
		public SegmentBuilder(java.nio.ByteBuffer buf, Capnproto.Arena arena) : base(buf, arena)
		{
			
		}
		
		///The total number of words the buffer can hold (in words).
		private int capacity()
		{
			this.buffer.rewind();
			return this.buffer.remaining() / 8;
		}
		
		// return how many words have already been allocated
		public int currentSize()
		{
			return this.pos;
		}
		
		///Allocate `amount` words.
		public int allocate(int amount)
		{
			System.Diagnostics.Debug.Assert(amount >= 0, "tried to allocate a negative number of words");
			if(amount > this.capacity() - this.currentSize())
				return FAILED_ALLOCATION;
			
			//No space left.
			int result = this.pos;
			this.pos += amount;
			return result;
		}
		
		public Capnproto.BuilderArena getArena()
		{
			return (Capnproto.BuilderArena)this.arena;
		}
		
		public bool isWritable()
		{
			//TODO: Support external non-writable segments.
			return true;
		}
		
		public void put(int index, long value)
		{
			buffer.putLong(index * Capnproto.Constants.BYTES_PER_WORD, value);
		}
	}
}
