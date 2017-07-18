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

using System;
using NUnit.Framework;

using Capnproto;

namespace Capnproto
{
	[TestFixture]
	public class LayoutSuite
	{
		class BareStructReader : StructReader.Factory<StructReader>
		{
			public StructReader ConstructReader(SegmentReader segment, int data, int pointers, int dataSize, short pointerCount, int nestingLimit)
			{
				return new StructReader(segment, data, pointers, dataSize, pointerCount, nestingLimit);
			}
		}
		
		[Test]
		public void SimpleRawDataStruct()
		{
			byte[] data = new byte[] { 0x00, 0x00, 0x00, 0x00, 0x01, 0x00, 0x00, 0x00, 0x01, 0x23, 0x45, 0x67, 0x89, 0xab, 0xcd, 0xef };
			
			var buffer = java.nio.ByteBuffer.wrap(data);
			
			var arena = new ReaderArena(new java.nio.ByteBuffer[] { buffer }, 0x7fffffffffffffffL);
			
			var reader = WireHelpers.readStructPointer(new BareStructReader(), arena.tryGetSegment(0), 0, null, 0, 0x7fffffff);
			
			Assert.AreEqual(reader._getUlongField(0), 0xefcdab8967452301L);
			Assert.AreEqual(reader._getLongField(1), 0L);
			
			Assert.AreEqual(reader._getUintField(0), 0x67452301);
			Assert.AreEqual(reader._getUintField(1), 0xefcdab89);
			Assert.AreEqual(reader._getUintField(2), 0);
			Assert.AreEqual(reader._getUshortField(0), 0x2301);
			Assert.AreEqual(reader._getUshortField(1), 0x6745);
			Assert.AreEqual(reader._getUshortField(2), 0xab89);
			Assert.AreEqual(reader._getUshortField(3), 0xefcd);
			Assert.AreEqual(reader._getUshortField(4), 0);
			
			// TODO masking
			
			Assert.AreEqual(reader._getBoolField(0), true);
			Assert.AreEqual(reader._getBoolField(1), false);
			Assert.AreEqual(reader._getBoolField(2), false);
			
			Assert.AreEqual(reader._getBoolField(3), false);
			Assert.AreEqual(reader._getBoolField(4), false);
			Assert.AreEqual(reader._getBoolField(5), false);
			Assert.AreEqual(reader._getBoolField(6), false);
			Assert.AreEqual(reader._getBoolField(7), false);
			
			Assert.AreEqual(reader._getBoolField(8), true);
			Assert.AreEqual(reader._getBoolField(9), true);
			Assert.AreEqual(reader._getBoolField(10), false);
			Assert.AreEqual(reader._getBoolField(11), false);
			Assert.AreEqual(reader._getBoolField(12), false);
			Assert.AreEqual(reader._getBoolField(13), true);
			Assert.AreEqual(reader._getBoolField(14), false);
			Assert.AreEqual(reader._getBoolField(15), false);
			
			Assert.AreEqual(reader._getBoolField(63), true);
			Assert.AreEqual(reader._getBoolField(64), false);
			
			// TODO masking
		}
		
		class BareStructBuilder : StructBuilder.Factory<StructBuilder>
		{
			public StructBuilder constructBuilder(SegmentBuilder segment, int data, int pointers, int dataSize, short pointerCount)
			{
				return new StructBuilder(segment, data, pointers, dataSize, pointerCount);
			}
			
			public Capnproto.StructSize StructSize()
			{
				return new Capnproto.StructSize(0, 0);
			}
		}
		
		[Test]
		public void StructRoundTrip_OneSegment()
		{
			var buffer = java.nio.ByteBuffer.wrap(new byte[1024 * 8]);
			
			var segment = new SegmentBuilder(buffer, new BuilderArena(BuilderArena.SUGGESTED_FIRST_SEGMENT_WORDS, BuilderArena.SUGGESTED_ALLOCATION_STRATEGY));
			var builder = WireHelpers.initStructPointer(new BareStructBuilder(), 0, segment, new StructSize(2, 4));
			setupStruct(builder);
			checkStruct(builder);
		}
		
		void setupStruct(StructBuilder builder)
		{
			builder._setLongField(0, 0x1011121314151617L);
			builder._setIntField(2, 0x20212223);
			builder._setShortField(6, 0x3031);
			builder._setByteField(14, 0x40);
			builder._setBoolField(120, false);
			builder._setBoolField(121, false);
			builder._setBoolField(122, true);
			builder._setBoolField(123, false);
			builder._setBoolField(124, true);
			builder._setBoolField(125, true);
			builder._setBoolField(126, true);
			builder._setBoolField(127, false);
		}
		
		void checkStruct(StructBuilder builder)
		{
			Assert.AreEqual(builder._getLongField(0), 0x1011121314151617L);
			Assert.AreEqual(builder._getIntField(2), 0x20212223);
			Assert.AreEqual(builder._getShortField(6), 0x3031);
			Assert.AreEqual(builder._getByteField(14), 0x40);
			Assert.AreEqual(builder._getBoolField(120), false);
			Assert.AreEqual(builder._getBoolField(121), false);
			Assert.AreEqual(builder._getBoolField(122), true);
			Assert.AreEqual(builder._getBoolField(123), false);
			Assert.AreEqual(builder._getBoolField(124), true);
			Assert.AreEqual(builder._getBoolField(125), true);
			Assert.AreEqual(builder._getBoolField(126), true);
			Assert.AreEqual(builder._getBoolField(127), false);
		}
	}
}

