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
	public class SerializePackedSuite
	{
		[Test]
		public void SimplePacking()
		{
			expectPacksTo(new byte[0], new byte[0]);
			expectPacksTo(new byte[] {0,0,0,0,0,0,0,0}, new byte[] {0,0});
			expectPacksTo(new byte[] {0,0,12,0,0,34,0,0}, new byte[] {0x24,12,34});
			expectPacksTo(new byte[] {1,3,2,4,5,7,6,8}, new byte[] {0xff,1,3,2,4,5,7,6,8,0});
			expectPacksTo(new byte[] {0,0,0,0,0,0,0,0, 1,3,2,4,5,7,6,8}, new byte[] {0,0,0xff,1,3,2,4,5,7,6,8,0});
			expectPacksTo(new byte[] {0,0,12,0,0,34,0,0, 1,3,2,4,5,7,6,8}, new byte[] {0x24, 12, 34, 0xff,1,3,2,4,5,7,6,8,0});
			expectPacksTo(new byte[] {1,3,2,4,5,7,6,8, 8,6,7,4,5,2,3,1}, new byte[] {0xff,1,3,2,4,5,7,6,8,1,8,6,7,4,5,2,3,1});
			
			expectPacksTo(new byte[] {1,2,3,4,5,6,7,8, 1,2,3,4,5,6,7,8, 1,2,3,4,5,6,7,8, 1,2,3,4,5,6,7,8, 0,2,4,0,9,0,5,1},
			              new byte[] {0xff,1,2,3,4,5,6,7,8, 3, 1,2,3,4,5,6,7,8, 1,2,3,4,5,6,7,8, 1,2,3,4,5,6,7,8, 0xd6,2,4,9,5,1});
			
			expectPacksTo(new byte[] {1,2,3,4,5,6,7,8, 1,2,3,4,5,6,7,8, 6,2,4,3,9,0,5,1, 1,2,3,4,5,6,7,8, 0,2,4,0,9,0,5,1},
			              new byte[] {0xff,1,2,3,4,5,6,7,8, 3, 1,2,3,4,5,6,7,8, 6,2,4,3,9,0,5,1, 1,2,3,4,5,6,7,8, 0xd6,2,4,9,5,1});
			
			expectPacksTo(new byte[] {8,0,100,6,0,1,1,2, 0,0,0,0,0,0,0,0, 0,0,0,0,0,0,0,0, 0,0,0,0,0,0,0,0, 0,0,1,0,2,0,3,1},
			              new byte[] {0xed,8,100,6,1,1,2, 0,2, 0xd4,1,2,3,1});
			
			expectPacksTo(new byte[] {0,0,0,0,2,0,0,0, 0,0,0,0,0,0,1,0, 0,0,0,0,0,0,0,0}, new byte[] {0x10,2, 0x40,1, 0,0});
			
			//TODO: Somebody who knows the .NET stdlib should implement these:
			//expectPacksTo([cast(ubyte)0].replicate(8 * 200), [cast(ubyte)0, 199]);
			//expectPacksTo([cast(ubyte)1].replicate(8 * 200), cast(ubyte[])[0xff, 1,1,1,1,1,1,1,1, 199] ~ [cast(ubyte)1].replicate(8 * 199));
		}
		
		void expectPacksTo(byte[] unpacked, byte[] packed)
		{
			// ----
			// write
			{
				var bytes = new byte[packed.Length];
				var writer = new ArrayOutputStream(java.nio.ByteBuffer.wrap(bytes));
				var packedOutputStream = new PackedOutputStream(writer);
				var wrapped = java.nio.ByteBuffer.wrap(unpacked);
				packedOutputStream.Write(wrapped);
				
				Assert.AreEqual(bytes, packed);
			}
			
			// ------
			// read
			{
				var reader = new ArrayInputStream(java.nio.ByteBuffer.wrap(packed));
				var packedInputStream = new PackedInputStream(reader);
				var bytes = new byte[unpacked.Length];
				var wrapped = java.nio.ByteBuffer.wrap(bytes);
				var n = packedInputStream.Read(wrapped);
				
				Assert.AreEqual(n, unpacked.Length);
				Assert.AreEqual(bytes, unpacked);
			}
		}
	}
}
