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

namespace Capnproto.Tests.Test
{
	[TestFixture]
	public class EncodingSuite
	{
		[Test]
		public void AllTypes()
		{
			var message = new MessageBuilder();
			var allTypes = message.InitRoot(Test.TestAllTypes.SingleFactory);
			TestUtil.InitTestMessage(allTypes);
			TestUtil.CheckTestMessage(allTypes);
			TestUtil.CheckTestMessage(allTypes.AsReader());
		}
		
		[Test]
		public void AllTypesMultiSegment()
		{
			var message = new MessageBuilder(5, BuilderArena.AllocationStrategy.FIXED_SIZE);
			var allTypes = message.InitRoot(Test.TestAllTypes.SingleFactory);
			TestUtil.InitTestMessage(allTypes);
			TestUtil.CheckTestMessage(allTypes);
			TestUtil.CheckTestMessage(allTypes.AsReader());
		}
		
		[Test]
		public void Setters()
		{
			var message = new MessageBuilder();
			var allTypes = message.InitRoot(Test.TestAllTypes.SingleFactory);
			TestUtil.InitTestMessage(allTypes);
			
			var message2 = new MessageBuilder();
			var allTypes2 = message2.InitRoot(Test.TestAllTypes.SingleFactory);
			
			allTypes2.SetStructField(allTypes.AsReader());
			TestUtil.CheckTestMessage(allTypes2.GetStructField());
			var reader = allTypes2.AsReader().GetStructField();
			TestUtil.CheckTestMessage(reader);
		}
		
		[Test]
		public void Zeroing()
		{
			var message = new MessageBuilder();
			var allTypes = message.InitRoot(Test.TestAllTypes.SingleFactory);
			
			var structList = allTypes.InitStructList(3);
			TestUtil.InitTestMessage(structList[0]);
			
			var structField = allTypes.InitStructField();
			TestUtil.InitTestMessage(structField);
			
			TestUtil.InitTestMessage(structList[1]);
			TestUtil.InitTestMessage(structList[2]);
			TestUtil.CheckTestMessage(structList[0]);
			allTypes.InitStructList(0);
			
			TestUtil.CheckTestMessage(allTypes.GetStructField());
			var allTypesReader = allTypes.AsReader();
			TestUtil.CheckTestMessage(allTypesReader.GetStructField());
			
			var any = message.InitRoot(AnyPointer.SingleFactory);
			var segments = message.GetSegmentsForOutput();
			foreach(var segment in segments)
			{
				for(int jj = 0; jj < segment.limit() - 1; jj++)
					Assert.AreEqual(segment.get(jj), 0);
			}
		}
		
		[Test]
		public void DoubleFarPointers()
		{
			var bytes = new byte[] {2,0,0,0, 1,0,0,0, 2,0,  0,  0, 1,0,0,0,
			                        6,0,0,0, 1,0,0,0, 2,0,  0,  0, 2,0,0,0,
			                        0,0,0,0, 1,0,0,0, 1,7,255,127, 0,0,0,0};
			
			var input = new ArrayInputStream(java.nio.ByteBuffer.wrap(bytes));
			var message = Serialize.Read(input);
			var root = message.GetRoot(Test.TestAllTypes.SingleFactory);
			Assert.AreEqual(root.GetBoolField(), true);
			Assert.AreEqual(root.GetInt8Field(), 7);
			Assert.AreEqual(root.GetInt16Field(), 32767);
		}
		
		[Test]
		public void UpgradeStructInBuilder()
		{
			var builder = new MessageBuilder();
			var root = builder.InitRoot(Test.TestAnyPointer.SingleFactory);
			
			{
				var oldVersion = root.GetAnyPointerField().InitAs(Test.TestOldVersion.SingleFactory);
				oldVersion.SetOld1(123);
				oldVersion.SetOld2("foo");
				var sub = oldVersion.InitOld3();
				sub.SetOld1(456);
				sub.SetOld2("bar");
			}
			
			{
				var newVersion = root.GetAnyPointerField().GetAs(Test.TestNewVersion.SingleFactory);
				Assert.AreEqual(newVersion.GetOld1(), 123);
				Assert.AreEqual(newVersion.GetOld2().ToString(), "foo");
				Assert.AreEqual(newVersion.GetNew1(), 987);
				Assert.AreEqual(newVersion.GetNew2().ToString(), "baz");
				
				var sub = newVersion.GetOld3();
				Assert.AreEqual(sub.GetOld1(), 456);
				Assert.AreEqual(sub.GetOld2().ToString(), "bar");
				
				newVersion.SetOld1(234);
				newVersion.SetOld2("qux");
				newVersion.SetNew1(654);
				newVersion.SetNew2("quux");
			}
			
			{
				var oldVersion = root.GetAnyPointerField().GetAs(Test.TestOldVersion.SingleFactory);
				Assert.AreEqual(oldVersion.GetOld1(), 234);
				Assert.AreEqual(oldVersion.GetOld2().ToString(), "qux");
			}
		}
		
		[Test]
		public void StructListUpgrade()
		{
			var message = new MessageBuilder();
			var root = message.InitRoot(Test.TestAnyPointer.SingleFactory);
			var any = root.GetAnyPointerField();
			
			{
				var longs = any.InitAs(PrimitiveList.Long.SingleFactory, 3);
				longs.Set(0, 123);
				longs.Set(1, 456);
				longs.Set(2, 789);
			}
			
			{
				var olds = any.AsReader().GetAs(Test.TestOldVersion.ListFactory);
				Assert.AreEqual(olds[0].GetOld1(), 123);
				Assert.AreEqual(olds[1].GetOld1(), 456);
				Assert.AreEqual(olds[2].GetOld1(), 789);
			}
			
			{
				var olds = any.GetAs(Test.TestOldVersion.ListFactory);
				Assert.AreEqual(olds.Length, 3);
				Assert.AreEqual(olds[0].GetOld1(), 123);
				Assert.AreEqual(olds[1].GetOld1(), 456);
				Assert.AreEqual(olds[2].GetOld1(), 789);
				
				olds[0].SetOld2("zero");
				olds[1].SetOld2("one");
				olds[2].SetOld2("two");
			}
			
			{
				var news = any.GetAs(Test.TestNewVersion.ListFactory);
				Assert.AreEqual(news.Length, 3);
				Assert.AreEqual(news[0].GetOld1(), 123);
				Assert.AreEqual(news[0].GetOld2().ToString(), "zero");
				
				Assert.AreEqual(news[1].GetOld1(), 456);
				Assert.AreEqual(news[1].GetOld2().ToString(), "one");
				
				Assert.AreEqual(news[2].GetOld1(), 789);
				Assert.AreEqual(news[2].GetOld2().ToString(), "two");
			}
		}
		
		[Test]
		public void StructListUpgradeDoubleFar()
		{
			byte[] bytes = {
			           1, 0, 0, 0, 0x1f, 0, 0, 0, //List, inline composite, 3 words.
			           4, 0, 0, 0,    1, 0, 2, 0, //Struct tag. 1 element, 1 word data, 2 pointers.
			          91, 0, 0, 0,    0, 0, 0, 0, //Data: 91.
			        0x05, 0, 0, 0, 0x42, 0, 0, 0, //List pointer, offset 1, type = BYTE, length 8.
			           0, 0, 0, 0,    0, 0, 0, 0, //Null pointer.
			        0x68, 0x65, 0x6c, 0x6c, 0x6f, 0x21, 0x21, 0}; //"hello!!".
			
			var segment = java.nio.ByteBuffer.wrap(bytes);
			var messageReader = new MessageReader(new java.nio.ByteBuffer[] {segment}, ReaderOptions.DEFAULT_READER_OPTIONS);
			
			var oldFactory = new StructList.Factory<Test.TestOldVersion.Builder, Test.TestOldVersion.Reader>(Test.TestOldVersion.SingleFactory);
			var oldVersion = messageReader.GetRoot(Test.TestOldVersion.ListFactory);
			
			Assert.AreEqual(oldVersion.Length, 1);
			Assert.AreEqual(oldVersion[0].GetOld1(), 91);
			Assert.AreEqual(oldVersion[0].GetOld2().ToString(), "hello!!");
			
			//Make the first segment exactly large enough to fit the original message.
			//This leaves no room for a far pointer landing pad in the first segment.
			var message = new MessageBuilder(6);
			message.SetRoot(oldFactory, oldVersion);
			
			var segments = message.GetSegmentsForOutput();
			Assert.AreEqual(segments.Length, 1);
			Assert.AreEqual(segments[0].limit(), 6 * 8);
			
			var newVersion = message.GetRoot(Test.TestNewVersion.ListFactory);
			Assert.AreEqual(newVersion.Length, 1);
			Assert.AreEqual(newVersion[0].GetOld1(), 91);
			Assert.AreEqual(newVersion[0].GetOld2().ToString(), "hello!!");
			
			var segments1 = message.GetSegmentsForOutput();
			Assert.AreEqual(segments[0].limit(), 6 * 8);
			//Check the the old list, including the tag, was zeroed.
			for(int ii = 8; ii < (5 * 8) - 1; ii++)
				Assert.AreEqual(segments[0].get(ii), 0);
		}
		
		/*[Test]
		public void Generics()
		{
			var message = new MessageBuilder();
			var root = message.InitRoot!(TestGenerics!(TestAllTypes, Text))();
			TestUtil.InitTestMessage(root.GetFoo());
			root.GetDub().SetFoo(Text.Reader("Hello"));
			var bar = root.GetDub().InitBar(1);
			bar.Set(0, 11);
			var revBar = root.GetRev().GetBar();
			revBar.SetInt8Field(111);
			var boolList = revBar.InitBoolList(2);
			boolList.Set(0, false);
			boolList.Set(1, true);
			
			TestUtil.CheckTestMessage(root.GetFoo());
			var rootReader = root.AsReader();
			TestUtil.CheckTestMessage(rootReader.GetFoo());
			var dubReader = root.GetDub();
			Assert.AreEqual(dubReader.GetFoo().toString(), "Hello");
			var barReader = dubReader.GetBar();
			Assert.AreEqual(barReader.length, 1);
			Assert.AreEqual(barReader[0], 11);
		}
		
		[Test]
		public void UseGenerics()
		{
			var message = new MessageBuilder();
			var root = message.InitRoot!TestUseGenerics();
			
			{
				var message2 = new MessageBuilder();
				var root2 = message2.InitRoot!(TestGenerics!(AnyPointer, AnyPointer))();
				root2.InitDub().SetFoo(Text.Reader("foobar"));
				
				root.SetUnspecified(root2.AsReader());
			}
			
			var rootReader = root.AsReader();
			Assert.AreEqual(root.GetUnspecified().GetDub().GetFoo().toString(), "foobar");
		}*/
		
		[Test]
		public void Defaults()
		{
			var message = new MessageBuilder();
			var defaults = message.InitRoot(Test.TestDefaults.SingleFactory);
			TestUtil.CheckDefaultMessage(defaults);
			TestUtil.CheckDefaultMessage(defaults.AsReader());
			TestUtil.SetDefaultMessage(defaults);
			TestUtil.CheckSettedDefaultMessage(defaults.AsReader());
		}
		
		[Test]
		public void Unions()
		{
			var builder = new MessageBuilder();
			var root = builder.InitRoot(Test.TestUnion.SingleFactory);
			var u0 = root.InitUnion0();
			u0.InitU0f1sp(10);
			Assert.AreEqual(u0.Which, Test.TestUnion.Union0.Which.U0f1sp);
			
			u0.InitPrimitiveList(10);
			Assert.AreEqual(u0.Which, Test.TestUnion.Union0.Which.PrimitiveList);
		}
		
		[Test]
		public void Groups()
		{
			var builder = new MessageBuilder();
			var root = builder.InitRoot(Test.TestGroups.SingleFactory);
			
			{
				var foo = root.GetGroups().InitFoo();
				foo.SetCorge(12345678);
				foo.SetGrault(123456789012345L);
				foo.SetGarply(new Text.Reader("foobar"));
				
				Assert.AreEqual(foo.GetCorge(), 12345678);
				Assert.AreEqual(foo.GetGrault(), 123456789012345L);
				Assert.AreEqual(foo.GetGarply().ToString(), "foobar");
			}
			
			{
				var bar = root.GetGroups().InitBar();
				bar.SetCorge(23456789);
				bar.SetGrault(new Text.Reader("barbaz"));
				bar.SetGarply(234567890123456L);
				
				Assert.AreEqual(bar.GetCorge(), 23456789);
				Assert.AreEqual(bar.GetGrault().ToString(), "barbaz");
				Assert.AreEqual(bar.GetGarply(), 234567890123456L);
			}
			
			{
				var baz = root.GetGroups().InitBaz();
				baz.SetCorge(34567890);
				baz.SetGrault(new Text.Reader("bazqux"));
				baz.SetGarply(new Text.Reader("quxquux"));
				
				Assert.AreEqual(baz.GetCorge(), 34567890);
				Assert.AreEqual(baz.GetGrault().ToString(), "bazqux");
				Assert.AreEqual(baz.GetGarply().ToString(), "quxquux");
			}
		}
		
		[Test]
		public void NestedLists()
		{
			var builder = new MessageBuilder();
			var root = builder.InitRoot(Test.TestLists.SingleFactory);
			
			{
				var intListList = root.InitInt32ListList(2);
				var intList0 = intListList.Init(0, 4);
				intList0.Set(0, 1);
				intList0.Set(1, 2);
				intList0.Set(2, 3);
				intList0.Set(3, 4);
				var intList1 = intListList.Init(1, 1);
				intList1.Set(0, 100);
			}
			
			{
				var reader = root.AsReader();
				var intListList = root.GetInt32ListList();
				Assert.AreEqual(intListList.Length, 2);
				var intList0 = intListList[0];
				Assert.AreEqual(intList0.Length, 4);
				Assert.AreEqual(intList0[0], 1);
				Assert.AreEqual(intList0[1], 2);
				Assert.AreEqual(intList0[2], 3);
				Assert.AreEqual(intList0[3], 4);
				var intList1 = intListList[1];
				Assert.AreEqual(intList1.Length, 1);
				Assert.AreEqual(intList1[0], 100);
			}
		}
		
		[Test]
		public void Constants()
		{
			Assert.AreEqual(Test.TestConstants.VoidConst, Void.VOID);
			Assert.AreEqual(Test.TestConstants.BoolConst, true);
			Assert.AreEqual(Test.TestConstants.Int8Const, -123);
			Assert.AreEqual(Test.TestConstants.Int16Const, -12345);
			Assert.AreEqual(Test.TestConstants.Int32Const, -12345678);
			Assert.AreEqual(Test.TestConstants.Int64Const, -123456789012345L);
			
			Assert.AreEqual(Test.TestConstants.Uint8Const, unchecked((byte)-22));
			Assert.AreEqual(Test.TestConstants.Uint16Const, unchecked((ushort)-19858));
			Assert.AreEqual(Test.TestConstants.Uint32Const, unchecked((uint)-838178284));
			Assert.AreEqual(Test.TestConstants.Uint64Const, unchecked((ulong)-6101065172474983726L));
			
			Assert.AreEqual(Test.TestConstants.Float32Const, 1234.5f);
			Assert.AreEqual(Test.TestConstants.Float64Const, -123e45);
			
			Assert.AreEqual(Test.TestConstants.TextConst.ToString(), "foo");
			Assert.AreEqual(Test.TestConstants.DataConst.ToArray(), TestUtil.Data("bar"));
			
			Assert.AreEqual(Test.TestConstants.EnumConst, Test.TestEnum.Corge);
			
			{
				var subReader = Test.TestConstants.StructConst;
				Assert.AreEqual(subReader.GetBoolField(), true);
				Assert.AreEqual(subReader.GetInt8Field(), -12);
				Assert.AreEqual(subReader.GetInt16Field(), 3456);
				Assert.AreEqual(subReader.GetInt32Field(), -78901234);
				Assert.AreEqual(subReader.GetInt64Field(), 56789012345678L);
				Assert.AreEqual(subReader.GetUInt8Field(), 90);
				Assert.AreEqual(subReader.GetUInt16Field(), 1234);
				Assert.AreEqual(subReader.GetUInt32Field(), 56789012);
				Assert.AreEqual(subReader.GetUInt64Field(), 345678901234567890L);
				Assert.AreEqual(subReader.GetFloat32Field(), -1.25e-10f);
				Assert.AreEqual(subReader.GetFloat64Field(), 345);
				Assert.AreEqual(subReader.GetTextField().ToString(), "baz");
			}
			
			Assert.AreEqual(Test.TestConstants.VoidListConst.Length, 6);
			
			{
				var listReader = Test.TestConstants.BoolListConst;
				Assert.AreEqual(listReader.Length, 4);
				Assert.AreEqual(listReader[0], true);
				Assert.AreEqual(listReader[1], false);
				Assert.AreEqual(listReader[2], false);
				Assert.AreEqual(listReader[3], true);
			}
			
			{
				var listReader = Test.TestConstants.TextListConst;
				Assert.AreEqual(listReader.Length, 3);
				Assert.AreEqual(listReader[0].ToString(), "plugh");
				Assert.AreEqual(listReader[1].ToString(), "xyzzy");
				Assert.AreEqual(listReader[2].ToString(), "thud");
			}
			
			{
				var listReader = Test.TestConstants.StructListConst;
				Assert.AreEqual(listReader.Length, 3);
				Assert.AreEqual(listReader[0].GetTextField().ToString(), "structlist 1");
				Assert.AreEqual(listReader[1].GetTextField().ToString(), "structlist 2");
				Assert.AreEqual(listReader[2].GetTextField().ToString(), "structlist 3");
			}
		}
		
		[Test]
		public void GlobalConstants()
		{
			Assert.AreEqual(Test.GlobalInt, 12345);
		}
		
		[Test]
		public void EmptyStruct()
		{
			var builder = new MessageBuilder();
			var root = builder.InitRoot(Test.TestAnyPointer.SingleFactory);
			Assert.AreEqual(root.HasAnyPointerField(), false);
			var any = root.GetAnyPointerField();
			Assert.AreEqual(any.IsNull(), true);
			any.InitAs(Test.TestEmptyStruct.SingleFactory);
			Assert.AreEqual(any.IsNull(), false);
			Assert.AreEqual(root.HasAnyPointerField(), true);
			
			{
				var rootReader = root.AsReader();
				Assert.AreEqual(rootReader.HasAnyPointerField(), true);
				Assert.AreEqual(rootReader.GetAnyPointerField().IsNull(), false);
			}
		}
		
		[Test]
		public void TextBuilderIntUnderflow()
		{
			var message = new MessageBuilder();
			var root = message.InitRoot(Test.TestAnyPointer.SingleFactory);
			root.GetAnyPointerField().InitAs(Data.SingleFactory, 0);
			Assert.Throws<DecodeException>(() => { root.GetAnyPointerField().GetAs(Text.SingleFactory); });
		}
		
		[Test]
		public void InlineCompositeListIntOverflow()
		{
			var bytes = new byte[] {0,0,0,0,    0,0,1,0,
			                        1,0,0,0, 0x17,0,0,0, 0,0,0,0xff, 16,0,0,0,
			                        0,0,0,0,    0,0,0,0, 0,0,0,   0,  0,0,0,0};
			
			var segment = java.nio.ByteBuffer.wrap(bytes);
			var message = new MessageReader(new java.nio.ByteBuffer[] { segment }, ReaderOptions.DEFAULT_READER_OPTIONS);
			
			var root = message.GetRoot(Test.TestAnyPointer.SingleFactory);
			//TODO: Add this after we implement totalSize():
			//root.totalSize();
			
			Assert.Throws<DecodeException>(() => { root.GetAnyPointerField().GetAs(Test.TestAllTypes.ListFactory); });
			
			var messageBuilder = new MessageBuilder();
			var builderRoot = messageBuilder.InitRoot(Test.TestAnyPointer.SingleFactory);
			Assert.Throws<DecodeException>(() => { builderRoot.GetAnyPointerField().SetAs(Test.TestAnyPointer.SingleFactory, root); });
		}
		
		[Test]
		public void VoidListAmplification()
		{
			var builder = new MessageBuilder();
			builder.InitRoot(Test.TestAnyPointer.SingleFactory).GetAnyPointerField().InitAs(PrimitiveList.Void.SingleFactory, 1 << 28);
			
			var segments = builder.GetSegmentsForOutput();
			Assert.AreEqual(segments.Length, 1);
			
			var reader = new MessageReader(segments, ReaderOptions.DEFAULT_READER_OPTIONS);
			var root = reader.GetRoot(Test.TestAnyPointer.SingleFactory);
			Assert.Throws<DecodeException>(() => { root.GetAnyPointerField().GetAs(Test.TestAllTypes.ListFactory); });
		}
		
		[Test]
		public void EmptyStructListAmplification()
		{
			var builder = new MessageBuilder();
			builder.InitRoot(Test.TestAnyPointer.SingleFactory).GetAnyPointerField().InitAs(Test.TestEmptyStruct.ListFactory, (1 << 29) - 1);
			
			var segments = builder.GetSegmentsForOutput();
			Assert.AreEqual(segments.Length, 1);
			
			var reader = new MessageReader(segments, ReaderOptions.DEFAULT_READER_OPTIONS);
			var root = reader.GetRoot(Test.TestAnyPointer.SingleFactory);
			Assert.Throws<DecodeException>(() => { root.GetAnyPointerField().GetAs(Test.TestAllTypes.ListFactory); });
		}
		
		[Test]
		public void LongUint8List()
		{
			var message = new MessageBuilder();
			var allTypes = message.InitRoot(Test.TestAllTypes.SingleFactory);
			var length = (1 << 28) + 1;
			var list = allTypes.InitUInt8List(length);
			Assert.AreEqual(list.Length, length);
			list.Set(length - 1, 3);
			Assert.AreEqual(list.Get(length - 1), 3);
			Assert.AreEqual(allTypes.AsReader().GetUInt8List().Get(length - 1), 3);
		}
		
		[Test]
		public void LongUint16List()
		{
			var message = new MessageBuilder();
			var allTypes = message.InitRoot(Test.TestAllTypes.SingleFactory);
			var length = (1 << 27) + 1;
			var list = allTypes.InitUInt16List(length);
			Assert.AreEqual(list.Length, length);
			list.Set(length - 1, 3);
			Assert.AreEqual(list.Get(length - 1), 3);
			Assert.AreEqual(allTypes.AsReader().GetUInt16List().Get(length - 1), 3);
		}
		
		[Test]
		public void LongUint32List()
		{
			var message = new MessageBuilder();
			var allTypes = message.InitRoot(Test.TestAllTypes.SingleFactory);
			var length = (1 << 26) + 1;
			var list = allTypes.InitUInt32List(length);
			Assert.AreEqual(list.Length, length);
			list.Set(length - 1, 3);
			Assert.AreEqual(list.Get(length - 1), 3);
			Assert.AreEqual(allTypes.AsReader().GetUInt32List().Get(length - 1), 3);
		}
		
		[Test]
		public void LongUint64List()
		{
			var message = new MessageBuilder();
			var allTypes = message.InitRoot(Test.TestAllTypes.SingleFactory);
			var length = (1 << 25) + 1;
			var list = allTypes.InitUInt64List(length);
			Assert.AreEqual(list.Length, length);
			list.Set(length - 1, 3);
			Assert.AreEqual(list.Get(length - 1), 3);
			Assert.AreEqual(allTypes.AsReader().GetUInt64List().Get(length - 1), 3);
		}
		
		[Test]
		public void LongFloat32List()
		{
			var message = new MessageBuilder();
			var allTypes = message.InitRoot(Test.TestAllTypes.SingleFactory);
			var length = (1 << 26) + 1;
			var list = allTypes.InitFloat32List(length);
			Assert.AreEqual(list.Length, length);
			list.Set(length - 1, 3.14f);
			Assert.AreEqual(list.Get(length - 1), 3.14f);
			Assert.AreEqual(allTypes.AsReader().GetFloat32List().Get(length - 1), 3.14f);
		}
		
		[Test]
		public void LongFloat64List()
		{
			var message = new MessageBuilder();
			var allTypes = message.InitRoot(Test.TestAllTypes.SingleFactory);
			var length = (1 << 25) + 1;
			var list = allTypes.InitFloat64List(length);
			Assert.AreEqual(list.Length, length);
			list.Set(length - 1, 3.14);
			Assert.AreEqual(list.Get(length - 1), 3.14);
			Assert.AreEqual(allTypes.AsReader().GetFloat64List().Get(length - 1), 3.14);
		}
		
		[Test]
		public void LongStructList()
		{
			var message = new MessageBuilder();
			var allTypes = message.InitRoot(Test.TestAllTypes.SingleFactory);
			var length = (1 << 21) + 1;
			var list = allTypes.InitStructList(length);
			Assert.AreEqual(list.Length, length);
			list.Get(length - 1).SetUInt8Field(3);
			Assert.AreEqual(allTypes.AsReader().GetStructList().Get(length - 1).GetUInt8Field(), 3);
		}
		
		[Test]
		public void LongTextList()
		{
			var message = new MessageBuilder();
			var allTypes = message.InitRoot(Test.TestAllTypes.SingleFactory);
			var length = (1 << 25) + 1;
			var list = allTypes.InitTextList(length);
			Assert.AreEqual(list.Length, length);
			list.Set(length - 1, new Text.Reader("foo"));
			Assert.AreEqual(allTypes.AsReader().GetTextList().Get(length - 1).ToString(), "foo");
		}
		
		[Test]
		public void LongListList()
		{
			var message = new MessageBuilder();
			var root = message.InitRoot(Test.TestLists.SingleFactory);
			var length = (1 << 25) + 1;
			var list = root.InitStructListList(length);
			Assert.AreEqual(list.Length, length);
			list.Init(length - 1, 3);
			Assert.AreEqual(list.Get(length - 1).Length, 3);
			Assert.AreEqual(root.AsReader().GetStructListList().Get(length - 1).Length, 3);
		}
		
		[Test]
		public void StructSetters()
		{
			var builder = new MessageBuilder();
			var root = builder.InitRoot(Test.TestAllTypes.SingleFactory);
			TestUtil.InitTestMessage(root);
			
			{
				var builder2 = new MessageBuilder();
				builder2.SetRoot(Test.TestAllTypes.SingleFactory, root.AsReader());
				TestUtil.CheckTestMessage(builder2.GetRoot(Test.TestAllTypes.SingleFactory));
			}
			
			{
				var builder2 = new MessageBuilder();
				var root2 = builder2.GetRoot(Test.TestAllTypes.SingleFactory);
				root2.SetStructField(root.AsReader());
				TestUtil.CheckTestMessage(root2.GetStructField());
			}
			
			{
				var builder2 = new MessageBuilder();
				var root2 = builder2.GetRoot(Test.TestAnyPointer.SingleFactory);
				root2.GetAnyPointerField().SetAs(Test.TestAllTypes.SingleFactory, root.AsReader());
				TestUtil.CheckTestMessage(root2.GetAnyPointerField().GetAs(Test.TestAllTypes.SingleFactory));
			}
		}
		
		[Test]
		public void SerializedSize()
		{
			var builder = new MessageBuilder();
			var root = builder.InitRoot(Test.TestAnyPointer.SingleFactory);
			root.GetAnyPointerField().SetAs(Text.SingleFactory, new Text.Reader("12345"));
			
			//One word for segment table, one for the root pointer,
			//one for the body of the TestAnyPointer struct,
			//and one for the body of the Text.
			Assert.AreEqual(Serialize.computeSerializedSizeInWords(builder), 4);
		}
		
		[Test]
		public void Import()
		{
			var builder = new MessageBuilder();
			var root = builder.InitRoot(TestImport.Foo.SingleFactory);
			var field = root.InitImportedStruct();
			TestUtil.InitTestMessage(field);
			TestUtil.CheckTestMessage(field);
			TestUtil.CheckTestMessage(field.AsReader());
		}
		
		/*[Test]
		public void GenericMap()
		{
			var builder = new MessageBuilder();
			var root = builder.InitRoot!(GenericMap!(Text, TestAllTypes))();
			
			{
				var entries = root.InitEntries(3);
				
				var entry0 = entries[0];
				entry0.SetKey(Text.Reader("foo"));
				var value0 = entry0.InitValue();
				value0.SetInt64Field(101);
				
				var entry1 = entries[1];
				entry1.SetKey(Text.Reader("bar"));
				var value1 = entry1.InitValue();
				value1.SetInt64Field(202);
				
				var entry2 = entries[2];
				entry2.SetKey(Text.Reader("baz"));
				var value2 = entry2.InitValue();
				value2.SetInt64Field(303);
			}
			
			{
				var entries = root.AsReader().GetEntries();
				var entry0 = entries[0];
				Assert.AreEqual(entry0.GetKey().toString(), "foo");
				Assert.AreEqual(entry0.GetValue().GetInt64Field(), 101);
				
				var entry1 = entries[1];
				Assert.AreEqual(entry1.GetKey().toString(), "bar");
				Assert.AreEqual(entry1.GetValue().GetInt64Field(), 202);
				
				var entry2 = entries[2];
				Assert.AreEqual(entry2.GetKey().toString(), "baz");
				Assert.AreEqual(entry2.GetValue().GetInt64Field, 303);
			}
		}*/
	}
}
