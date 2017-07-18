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
	public sealed class ReaderArena : Capnproto.Arena
	{
		public long limit;
		
		public readonly System.Collections.Generic.List<Capnproto.SegmentReader> segments;
		
		public ReaderArena(java.nio.ByteBuffer[] segmentSlices, long traversalLimitInWords)
		{
			// current limit
			this.limit = traversalLimitInWords;
			this.segments = new System.Collections.Generic.List<Capnproto.SegmentReader>();
			for(int ii = 0; ii < segmentSlices.Length; ++ii)
				this.segments.Add(new Capnproto.SegmentReader(segmentSlices[ii], this));
		}
		
		public Capnproto.SegmentReader tryGetSegment(int id)
		{
			return segments[id];
		}
		
		public void checkReadLimit(int numBytes)
		{
			if(numBytes > limit)
				throw new Capnproto.DecodeException("Read limit exceeded.");
			limit -= numBytes;
		}
	}
}
