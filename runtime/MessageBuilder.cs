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
	public sealed class MessageBuilder
	{
		private readonly Capnproto.BuilderArena arena;
		
		public MessageBuilder()
		{
			this.arena = new Capnproto.BuilderArena(Capnproto.BuilderArena.SUGGESTED_FIRST_SEGMENT_WORDS, Capnproto.BuilderArena.SUGGESTED_ALLOCATION_STRATEGY);
		}
		
		public MessageBuilder(int firstSegmentWords)
		{
			this.arena = new Capnproto.BuilderArena(firstSegmentWords, Capnproto.BuilderArena.SUGGESTED_ALLOCATION_STRATEGY);
		}
		
		public MessageBuilder(int firstSegmentWords, Capnproto.BuilderArena.AllocationStrategy allocationStrategy)
		{
			this.arena = new Capnproto.BuilderArena(firstSegmentWords, allocationStrategy);
		}
		
		private Capnproto.AnyPointer.Builder getRootInternal()
		{
			Capnproto.SegmentBuilder rootSegment = this.arena.segments[0];
			if(rootSegment.currentSize() == 0)
			{
				int location = rootSegment.allocate(1);
				if(location == Capnproto.SegmentBuilder.FAILED_ALLOCATION)
					throw new System.Exception("could not allocate root pointer");
				if(location != 0)
					throw new System.Exception("First allocated word of new segment was not at offset 0");
				return new Capnproto.AnyPointer.Builder(rootSegment, location);
			}
			return new Capnproto.AnyPointer.Builder(rootSegment, 0);
		}
		
		public T GetRoot<T>(Capnproto.FromPointerBuilder<T> factory)
		{
			return this.getRootInternal().GetAs(factory);
		}
		
		public void SetRoot<T, U>(Capnproto.SetPointerBuilder<T, U> factory, U reader)
		{
			this.getRootInternal().SetAs(factory, reader);
		}
		
		public T InitRoot<T>(Capnproto.FromPointerBuilder<T> factory)
		{
			return this.getRootInternal().InitAs(factory);
		}
		
		public java.nio.ByteBuffer[] GetSegmentsForOutput()
		{
			return this.arena.getSegmentsForOutput();
		}
	}
}
