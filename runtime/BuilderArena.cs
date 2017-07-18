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
	public sealed class BuilderArena : Capnproto.Arena
	{
		[System.Serializable]
		public sealed class AllocationStrategy : Sharpen.EnumBase
		{
			public static readonly Capnproto.BuilderArena.AllocationStrategy FIXED_SIZE = new Capnproto.BuilderArena.AllocationStrategy(0, "FIXED_SIZE");
			
			public static readonly Capnproto.BuilderArena.AllocationStrategy GROW_HEURISTICALLY = new Capnproto.BuilderArena.AllocationStrategy(1, "GROW_HEURISTICALLY");
			
			private AllocationStrategy(int ordinal, string name) : base(ordinal, name)
			{
			}
			
			public static AllocationStrategy[] values()
			{
				return new AllocationStrategy[] { FIXED_SIZE, GROW_HEURISTICALLY };
			}
			
			static AllocationStrategy()
			{
				RegisterValues<AllocationStrategy>(values());
			}
		}
		
		public const int SUGGESTED_FIRST_SEGMENT_WORDS = 1024;
		
		public static readonly Capnproto.BuilderArena.AllocationStrategy SUGGESTED_ALLOCATION_STRATEGY = Capnproto.BuilderArena.AllocationStrategy.GROW_HEURISTICALLY;
		
		public readonly System.Collections.Generic.List<Capnproto.SegmentBuilder> segments;
		
		public int nextSize;
		
		public readonly Capnproto.BuilderArena.AllocationStrategy allocationStrategy;
		
		public BuilderArena(int firstSegmentSizeWords, Capnproto.BuilderArena.AllocationStrategy allocationStrategy)
		{
			this.segments = new System.Collections.Generic.List<Capnproto.SegmentBuilder>();
			this.nextSize = firstSegmentSizeWords;
			this.allocationStrategy = allocationStrategy;
			Capnproto.SegmentBuilder segment0 = new Capnproto.SegmentBuilder(java.nio.ByteBuffer.allocate(firstSegmentSizeWords * Capnproto.Constants.BYTES_PER_WORD), this);
			segment0.buffer.order(java.nio.ByteOrder.LITTLE_ENDIAN);
			this.segments.Add(segment0);
		}
		
		public Capnproto.SegmentReader tryGetSegment(int id)
		{
			return this.segments[id];
		}
		
		public Capnproto.SegmentBuilder getSegment(int id)
		{
			return this.segments[id];
		}
		
		public void checkReadLimit(int numBytes)
		{
			
		}
		
		public class AllocateResult
		{
			public readonly Capnproto.SegmentBuilder segment;
			
			public readonly int offset;
			
			public AllocateResult(Capnproto.SegmentBuilder segment, int offset)
			{
				// offset to the beginning of the of allocated memory
				this.segment = segment;
				this.offset = offset;
			}
		}
		
		public Capnproto.BuilderArena.AllocateResult allocate(int amount)
		{
			int len = this.segments.Count;
			// we allocate the first segment in the constructor.
			int result = this.segments[len - 1].allocate(amount);
			if(result != Capnproto.SegmentBuilder.FAILED_ALLOCATION)
				return new Capnproto.BuilderArena.AllocateResult(this.segments[len - 1], result);
			// allocate_owned_memory
			int size = System.Math.Max(amount, this.nextSize);
			Capnproto.SegmentBuilder newSegment = new Capnproto.SegmentBuilder(java.nio.ByteBuffer.allocate(size * Capnproto.Constants.BYTES_PER_WORD), this);
			switch(this.allocationStrategy.ordinal())
			{
				case 1:
				{
					this.nextSize += size;
					break;
				}
				
				default:
				{
					break;
				}
			}
			// --------
			newSegment.buffer.order(java.nio.ByteOrder.LITTLE_ENDIAN);
			newSegment.id = len;
			this.segments.Add(newSegment);
			return new Capnproto.BuilderArena.AllocateResult(newSegment, newSegment.allocate(amount));
		}
		
		public java.nio.ByteBuffer[] getSegmentsForOutput()
		{
			java.nio.ByteBuffer[] result = new java.nio.ByteBuffer[this.segments.Count];
			for(int ii = 0; ii < this.segments.Count; ++ii)
			{
				Capnproto.SegmentBuilder segment = segments[ii];
				segment.buffer.rewind();
				java.nio.ByteBuffer slice = segment.buffer.slice();
				slice.limit(segment.currentSize() * Capnproto.Constants.BYTES_PER_WORD);
				slice.order(java.nio.ByteOrder.LITTLE_ENDIAN);
				result[ii] = slice;
			}
			return result;
		}
	}
}
