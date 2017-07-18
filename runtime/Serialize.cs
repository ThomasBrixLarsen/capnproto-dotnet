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
	public sealed class Serialize
	{
		internal static java.nio.ByteBuffer makeByteBuffer(int bytes)
		{
			java.nio.ByteBuffer result = java.nio.ByteBuffer.prepare(bytes);
			result.order(java.nio.ByteOrder.LITTLE_ENDIAN);
			result.mark();
			return result;
		}
		
		/// <exception cref="System.IO.IOException"/>
		public static void fillBuffer(java.nio.ByteBuffer buffer, java.nio.channels.ReadableByteChannel bc)
		{
			while(buffer.hasRemaining())
			{
				int r = bc.Read(buffer);
				if(r < 0)
					throw new System.IO.IOException("premature EOF");
			}
		}
		
		//TODO: Check for r == 0?
		/// <exception cref="System.IO.IOException"/>
		public static Capnproto.MessageReader Read(java.nio.channels.ReadableByteChannel bc)
		{
			return Read(bc, Capnproto.ReaderOptions.DEFAULT_READER_OPTIONS);
		}
		
		/// <exception cref="System.IO.IOException"/>
		public static Capnproto.MessageReader Read(java.nio.channels.ReadableByteChannel bc, Capnproto.ReaderOptions options)
		{
			java.nio.ByteBuffer firstWord = makeByteBuffer(Capnproto.Constants.BYTES_PER_WORD);
			fillBuffer(firstWord, bc);
			int segmentCount = 1 + firstWord.getInt(0);
			int segment0Size = 0;
			if(segmentCount > 0)
				segment0Size = firstWord.getInt(4);
			int totalWords = segment0Size;
			if(segmentCount > 512)
				throw new System.IO.IOException("too many segments");
				
			//In words.
			System.Collections.Generic.List<int> moreSizes = new System.Collections.Generic.List<int>();
			if(segmentCount > 1)
			{
				java.nio.ByteBuffer moreSizesRaw = makeByteBuffer(4 * (segmentCount & ~1));
				fillBuffer(moreSizesRaw, bc);
				for(int ii = 0; ii < segmentCount - 1; ++ii)
				{
					int size = moreSizesRaw.getInt(ii * 4);
					moreSizes.Add(size);
					totalWords += size;
				}
			}
			if(totalWords > options.traversalLimitInWords)
				throw new Capnproto.DecodeException("Message size exceeds traversal limit.");
			java.nio.ByteBuffer allSegments = makeByteBuffer(totalWords * Capnproto.Constants.BYTES_PER_WORD);
			fillBuffer(allSegments, bc);
			java.nio.ByteBuffer[] segmentSlices = new java.nio.ByteBuffer[segmentCount];
			allSegments.rewind();
			segmentSlices[0] = allSegments.slice();
			segmentSlices[0].limit(segment0Size * Capnproto.Constants.BYTES_PER_WORD);
			segmentSlices[0].order(java.nio.ByteOrder.LITTLE_ENDIAN);
			int offset = segment0Size;
			for(int ii = 1; ii < segmentCount; ++ii)
			{
				allSegments.position(offset * Capnproto.Constants.BYTES_PER_WORD);
				segmentSlices[ii] = allSegments.slice();
				segmentSlices[ii].limit(moreSizes[ii - 1] * Capnproto.Constants.BYTES_PER_WORD);
				segmentSlices[ii].order(java.nio.ByteOrder.LITTLE_ENDIAN);
				offset += moreSizes[ii - 1];
			}
			return new Capnproto.MessageReader(segmentSlices, options);
		}
		
		/// <exception cref="System.IO.IOException"/>
		public static Capnproto.MessageReader Read(java.nio.ByteBuffer bb)
		{
			return Read(bb, Capnproto.ReaderOptions.DEFAULT_READER_OPTIONS);
		}
		
		///Upon return, `bb.position()` will be at the end of the message.
		/// <exception cref="System.IO.IOException"/>
		public static Capnproto.MessageReader Read(java.nio.ByteBuffer bb, Capnproto.ReaderOptions options)
		{
			bb.order(java.nio.ByteOrder.LITTLE_ENDIAN);
			int segmentCount = 1 + bb.getInt();
			if(segmentCount > 512)
				throw new System.IO.IOException("too many segments");
			java.nio.ByteBuffer[] segmentSlices = new java.nio.ByteBuffer[segmentCount];
			int segmentSizesBase = bb.position();
			int segmentSizesSize = segmentCount * 4;
			int align = Capnproto.Constants.BYTES_PER_WORD - 1;
			int segmentBase = (segmentSizesBase + segmentSizesSize + align) & ~align;
			int totalWords = 0;
			for(int ii = 0; ii < segmentCount; ++ii)
			{
				int segmentSize = bb.getInt(segmentSizesBase + ii * 4);
				bb.position(segmentBase + totalWords * Capnproto.Constants.BYTES_PER_WORD);
				segmentSlices[ii] = bb.slice();
				segmentSlices[ii].limit(segmentSize * Capnproto.Constants.BYTES_PER_WORD);
				segmentSlices[ii].order(java.nio.ByteOrder.LITTLE_ENDIAN);
				totalWords += segmentSize;
			}
			bb.position(segmentBase + totalWords * Capnproto.Constants.BYTES_PER_WORD);
			if(totalWords > options.traversalLimitInWords)
				throw new Capnproto.DecodeException("Message size exceeds traversal limit.");
			return new Capnproto.MessageReader(segmentSlices, options);
		}
		
		public static long computeSerializedSizeInWords(Capnproto.MessageBuilder message)
		{
			java.nio.ByteBuffer[] segments = message.GetSegmentsForOutput();
			//From the capnproto documentation:
			//"When transmitting over a stream, the following should be sent..."
			long bytes = 0;
			//"(4 bytes) The number of segments, minus one..."
			bytes += 4;
			//"(N * 4 bytes) The size of each segment, in words."
			bytes += segments.Length * 4;
			//"(0 or 4 bytes) Padding up to the next word boundary."
			if(bytes % 8 != 0)
				bytes += 4;
			//The content of each segment, in order.
			for(int i = 0; i < segments.Length; ++i)
			{
				java.nio.ByteBuffer s = segments[i];
				bytes += s.limit();
			}
			return bytes / Capnproto.Constants.BYTES_PER_WORD;
		}
		
		/// <exception cref="System.IO.IOException"/>
		public static void Write(java.nio.channels.WritableByteChannel outputChannel, Capnproto.MessageBuilder message)
		{
			java.nio.ByteBuffer[] segments = message.GetSegmentsForOutput();
			int tableSize = (segments.Length + 2) & (~1);
			java.nio.ByteBuffer table = java.nio.ByteBuffer.allocate(4 * tableSize);
			table.order(java.nio.ByteOrder.LITTLE_ENDIAN);
			table.putInt(0, segments.Length - 1);
			for(int i = 0; i < segments.Length; ++i)
				table.putInt(4 * (i + 1), segments[i].limit() / 8);
			//Any padding is already zeroed.
			while(table.hasRemaining())
				outputChannel.Write(table);
			foreach(java.nio.ByteBuffer buffer in segments)
			{
				while(buffer.hasRemaining())
					outputChannel.Write(buffer);
			}
		}
	}
}
