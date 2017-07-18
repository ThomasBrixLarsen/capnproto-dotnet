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

using System.Text;
using NUnit.Framework;

using Capnproto;

namespace Capnproto.Tests.Test
{
	public class TestUtil
	{
		public static byte[] Data(string str)
		{
			return Encoding.ASCII.GetBytes(str);
		}
		
		public static void InitTestMessage(Test.TestAllTypes.Builder builder)
		{
			builder.SetVoidField(Void.VOID);
			builder.SetBoolField(true);
			builder.SetInt8Field(-123);
			builder.SetInt16Field(-12345);
			builder.SetInt32Field(-12345678);
			builder.SetInt64Field(-123456789012345L);
			builder.SetUInt8Field(0xea);
			builder.SetUInt16Field(0x4567);
			builder.SetUInt32Field(0x34567890);
			builder.SetUInt64Field(0x1234567890123456L);
			builder.SetFloat32Field(1234.5f);
			builder.SetFloat64Field(-123e45);
			builder.SetTextField("foo");
			builder.SetDataField(Data("bar"));
			
			{
				var subBuilder = builder.InitStructField();
				subBuilder.SetVoidField(Void.VOID);
				subBuilder.SetBoolField(true);
				subBuilder.SetInt8Field(-12);
				subBuilder.SetInt16Field(3456);
				subBuilder.SetInt32Field(-78901234);
				subBuilder.SetInt64Field(56789012345678L);
				subBuilder.SetUInt8Field(90);
				subBuilder.SetUInt16Field(1234);
				subBuilder.SetUInt32Field(56789012);
				subBuilder.SetUInt64Field(345678901234567890L);
				subBuilder.SetFloat32Field(-1.25e-10f);
				subBuilder.SetFloat64Field(345);
				subBuilder.SetTextField(new Text.Reader("baz"));
				subBuilder.SetDataField(Data("qux"));
				
				{
					var subSubBuilder = subBuilder.InitStructField();
					subSubBuilder.SetTextField(new Text.Reader("nested"));
					subSubBuilder.InitStructField().SetTextField(new Text.Reader("really nested"));
				}
				
				subBuilder.SetEnumField(Test.TestEnum.Baz);
				
				var boolList = subBuilder.InitBoolList(5);
				boolList.Set(0, false);
				boolList.Set(1, true);
				boolList.Set(2, false);
				boolList.Set(3, true);
				boolList.Set(4, true);
			}
			
			{
				builder.SetEnumField(Test.TestEnum.Corge);
				builder.InitVoidList(6);
				
				var boolList = builder.InitBoolList(4);
				boolList.Set(0, true);
				boolList.Set(1, false);
				boolList.Set(2, false);
				boolList.Set(3, true);
				
				var float64List = builder.InitFloat64List(4);
				float64List.Set(0, 7777.75);
				float64List.Set(1, System.Double.PositiveInfinity);
				float64List.Set(2, System.Double.NegativeInfinity);
				float64List.Set(3, System.Double.NaN);
				
				var textList = builder.InitTextList(3);
				textList.Set(0, new Text.Reader("plugh"));
				textList.Set(1, new Text.Reader("xyzzy"));
				textList.Set(2, new Text.Reader("thud"));
				
				var structList = builder.InitStructList(3);
				structList[0].SetTextField(new Text.Reader("structlist 1"));
				structList[1].SetTextField(new Text.Reader("structlist 2"));
				structList[2].SetTextField(new Text.Reader("structlist 3"));
				
				var enumList = builder.InitEnumList(2);
				enumList.Set(0, Test.TestEnum.Foo);
				enumList.Set(1, Test.TestEnum.Garply);
			}
		}
		
		public static void CheckTestMessage(Test.TestAllTypes.Builder builder)
		{
			builder.GetVoidField();
			Assert.AreEqual(builder.GetBoolField(), true);
			Assert.AreEqual(builder.GetInt8Field(), -123);
			Assert.AreEqual(builder.GetInt16Field(), -12345);
			Assert.AreEqual(builder.GetInt32Field(), -12345678);
			Assert.AreEqual(builder.GetInt64Field(), -123456789012345L);
			Assert.AreEqual(builder.GetUInt8Field(), 0xea);
			Assert.AreEqual(builder.GetUInt16Field(), 0x4567);
			Assert.AreEqual(builder.GetUInt32Field(), 0x34567890);
			Assert.AreEqual(builder.GetUInt64Field(), 0x1234567890123456L);
			Assert.AreEqual(builder.GetFloat32Field(), 1234.5f);
			Assert.AreEqual(builder.GetFloat64Field(), -123e45);
			Assert.AreEqual(builder.GetTextField().ToString(), "foo");
			
			{
				var subBuilder = builder.GetStructField();
				subBuilder.GetVoidField();
				Assert.AreEqual(subBuilder.GetBoolField(), true);
				Assert.AreEqual(subBuilder.GetInt8Field(), -12);
				Assert.AreEqual(subBuilder.GetInt16Field(), 3456);
				Assert.AreEqual(subBuilder.GetInt32Field(), -78901234);
				Assert.AreEqual(subBuilder.GetInt64Field(), 56789012345678L);
				Assert.AreEqual(subBuilder.GetUInt8Field(), 90);
				Assert.AreEqual(subBuilder.GetUInt16Field(), 1234);
				Assert.AreEqual(subBuilder.GetUInt32Field(), 56789012);
				Assert.AreEqual(subBuilder.GetUInt64Field(), 345678901234567890L);
				Assert.AreEqual(subBuilder.GetFloat32Field(), -1.25e-10f);
				Assert.AreEqual(subBuilder.GetFloat64Field(), 345);
				
				{
					var subSubBuilder = subBuilder.GetStructField();
					Assert.AreEqual(subSubBuilder.GetTextField().ToString(), "nested");
				}
				
				Assert.AreEqual(subBuilder.GetEnumField(), Test.TestEnum.Baz);
				
				var boolList = subBuilder.GetBoolList();
				Assert.AreEqual(boolList[0], false);
				Assert.AreEqual(boolList[1], true);
				Assert.AreEqual(boolList[2], false);
				Assert.AreEqual(boolList[3], true);
				Assert.AreEqual(boolList[4], true);
			}
			{
				Assert.AreEqual(builder.GetEnumField(), Test.TestEnum.Corge);
				
				Assert.AreEqual(builder.GetVoidList().Length, 6);
				
				var boolList = builder.GetBoolList();
				Assert.AreEqual(boolList[0], true);
				Assert.AreEqual(boolList[1], false);
				Assert.AreEqual(boolList[2], false);
				Assert.AreEqual(boolList[3], true);
				
				var float64List = builder.GetFloat64List();
				Assert.AreEqual(float64List[0], 7777.75);
				Assert.AreEqual(float64List[1], System.Double.PositiveInfinity);
				Assert.AreEqual(float64List[2], System.Double.NegativeInfinity);
				Assert.AreEqual(float64List[3], System.Double.NaN);
				
				var textList = builder.GetTextList();
				Assert.AreEqual(textList.Length, 3);
				Assert.AreEqual(textList[0].ToString(), "plugh");
				Assert.AreEqual(textList[1].ToString(), "xyzzy");
				Assert.AreEqual(textList[2].ToString(), "thud");
				
				var structList = builder.GetStructList();
				Assert.AreEqual(structList.Length, 3);
				Assert.AreEqual(structList[0].GetTextField().ToString(), "structlist 1");
				Assert.AreEqual(structList[1].GetTextField().ToString(), "structlist 2");
				Assert.AreEqual(structList[2].GetTextField().ToString(), "structlist 3");
				
				var enumList = builder.GetEnumList();
				Assert.AreEqual(enumList[0], Test.TestEnum.Foo);
				Assert.AreEqual(enumList[1], Test.TestEnum.Garply);
			}
		}
		
		public static void CheckTestMessage(Test.TestAllTypes.Reader reader)
		{
			reader.GetVoidField();
			Assert.AreEqual(reader.GetBoolField(), true);
			Assert.AreEqual(reader.GetInt8Field(), -123);
			Assert.AreEqual(reader.GetInt16Field(), -12345);
			Assert.AreEqual(reader.GetInt32Field(), -12345678);
			Assert.AreEqual(reader.GetInt64Field(), -123456789012345L);
			Assert.AreEqual(reader.GetUInt8Field(), 0xea);
			Assert.AreEqual(reader.GetUInt16Field(), 0x4567);
			Assert.AreEqual(reader.GetUInt32Field(), 0x34567890);
			Assert.AreEqual(reader.GetUInt64Field(), 0x1234567890123456L);
			Assert.AreEqual(reader.GetFloat32Field(), 1234.5f);
			Assert.AreEqual(reader.GetFloat64Field(), -123e45);
			Assert.AreEqual(reader.GetTextField().ToString(), "foo");
			
			{
				var subReader = reader.GetStructField();
				subReader.GetVoidField();
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
				
				{
					var subSubReader = subReader.GetStructField();
					Assert.AreEqual(subSubReader.GetTextField().ToString(), "nested");
				}
				var boolList = subReader.GetBoolList();
				Assert.AreEqual(boolList[0], false);
				Assert.AreEqual(boolList[1], true);
				Assert.AreEqual(boolList[2], false);
				Assert.AreEqual(boolList[3], true);
				Assert.AreEqual(boolList[4], true);
			}
			{
				Assert.AreEqual(reader.GetVoidList().Length, 6);
				
				var boolList = reader.GetBoolList();
				Assert.AreEqual(boolList[0], true);
				Assert.AreEqual(boolList[1], false);
				Assert.AreEqual(boolList[2], false);
				Assert.AreEqual(boolList[3], true);
				
				var float64List = reader.GetFloat64List();
				Assert.AreEqual(float64List[0], 7777.75);
				Assert.AreEqual(float64List[1], System.Double.PositiveInfinity);
				Assert.AreEqual(float64List[2], System.Double.NegativeInfinity);
				Assert.AreEqual(float64List[3], System.Double.NaN);
				
				var textList = reader.GetTextList();
				Assert.AreEqual(textList.Length, 3);
				Assert.AreEqual(textList[0].ToString(), "plugh");
				Assert.AreEqual(textList[1].ToString(), "xyzzy");
				Assert.AreEqual(textList[2].ToString(), "thud");
				
				var structList = reader.GetStructList();
				Assert.AreEqual(3, structList.Length);
				Assert.AreEqual(structList[0].GetTextField().ToString(), "structlist 1");
				Assert.AreEqual(structList[1].GetTextField().ToString(), "structlist 2");
				Assert.AreEqual(structList[2].GetTextField().ToString(), "structlist 3");
				
				var enumList = reader.GetEnumList();
				Assert.AreEqual(enumList[0], Test.TestEnum.Foo);
				Assert.AreEqual(enumList[1], Test.TestEnum.Garply);
			}
		}
		
		public static void CheckDefaultMessage(Test.TestDefaults.Builder builder)
		{
			builder.GetVoidField();
			Assert.AreEqual(builder.GetBoolField(), true);
			Assert.AreEqual(builder.GetInt8Field(), -123);
			Assert.AreEqual(builder.GetInt16Field(), -12345);
			Assert.AreEqual(builder.GetInt32Field(), -12345678);
			Assert.AreEqual(builder.GetInt64Field(), -123456789012345L);
			Assert.AreEqual(builder.GetUInt8Field(), 0xea);
			Assert.AreEqual(builder.GetUInt16Field(), 45678);
			Assert.AreEqual(builder.GetUInt32Field(), 0xce0a6a14);
			Assert.AreEqual(builder.GetUInt64Field(), 0xab54a98ceb1f0ad2L);
			Assert.AreEqual(builder.GetFloat32Field(), 1234.5f);
			Assert.AreEqual(builder.GetFloat64Field(), -123e45);
			Assert.AreEqual(builder.GetEnumField(), Test.TestEnum.Corge);
			
			Assert.AreEqual(builder.GetTextField().ToString(), "foo");
			Assert.AreEqual(builder.GetDataField().ToArray(), new byte[] {0x62, 0x61, 0x72});
		}
		
		public static void CheckDefaultMessage(Test.TestDefaults.Reader reader)
		{
			reader.GetVoidField();
			Assert.AreEqual(reader.GetBoolField(), true);
			Assert.AreEqual(reader.GetInt8Field(), -123);
			Assert.AreEqual(reader.GetInt16Field(), -12345);
			Assert.AreEqual(reader.GetInt32Field(), -12345678);
			Assert.AreEqual(reader.GetInt64Field(), -123456789012345L);
			Assert.AreEqual(reader.GetUInt8Field(), 0xea);
			Assert.AreEqual(reader.GetUInt16Field(), 45678);
			Assert.AreEqual(reader.GetUInt32Field(), 0xce0a6a14);
			Assert.AreEqual(reader.GetUInt64Field(), 0xab54a98ceb1f0ad2L);
			Assert.AreEqual(reader.GetFloat32Field(), 1234.5f);
			Assert.AreEqual(reader.GetFloat64Field(), -123e45);
			Assert.AreEqual(reader.GetTextField().ToString(), "foo");
			Assert.AreEqual(reader.GetDataField().ToArray(), new byte[] {0x62, 0x61, 0x72});
			
			{
				var subReader = reader.GetStructField();
				subReader.GetVoidField();
				Assert.AreEqual(subReader.GetBoolField(), true);
				Assert.AreEqual(subReader.GetInt8Field(), -12);
				Assert.AreEqual(subReader.GetInt16Field(), 3456);
				Assert.AreEqual(subReader.GetInt32Field(), -78901234);
				Assert.AreEqual(subReader.GetTextField().ToString(), "baz");
				
				{
					var subSubReader = subReader.GetStructField();
					Assert.AreEqual(subSubReader.GetTextField().ToString(), "nested");
				}
			}
			
			Assert.AreEqual(reader.GetEnumField(), Test.TestEnum.Corge);
			
			Assert.AreEqual(reader.GetVoidList().Length, 6);
			
			{
				var listReader = reader.GetBoolList();
				Assert.AreEqual(listReader.Length, 4);
				Assert.AreEqual(listReader[0], true);
				Assert.AreEqual(listReader[1], false);
				Assert.AreEqual(listReader[2], false);
				Assert.AreEqual(listReader[3], true);
			}
			
			{
				var listReader = reader.GetInt8List();
				Assert.AreEqual(listReader.Length, 2);
				Assert.AreEqual(listReader[0], 111);
				Assert.AreEqual(listReader[1], -111);
			}
		}
		
		public static void SetDefaultMessage(Test.TestDefaults.Builder builder)
		{
			builder.SetBoolField(false);
			builder.SetInt8Field(-122);
			builder.SetInt16Field(-12344);
			builder.SetInt32Field(-12345677);
			builder.SetInt64Field(-123456789012344L);
			builder.SetUInt8Field(0xe9);
			builder.SetUInt16Field(45677);
			builder.SetUInt32Field(0xce0a6a13);
			builder.SetUInt64Field(0xab54a98ceb1f0ad1L);
			builder.SetFloat32Field(1234.4f);
			builder.SetFloat64Field(-123e44);
			builder.SetTextField(new Text.Reader("bar"));
			builder.SetEnumField(Test.TestEnum.Qux);
		}
		
		public static void CheckSettedDefaultMessage(Test.TestDefaults.Reader reader)
		{
			Assert.AreEqual(reader.GetBoolField(), false);
			Assert.AreEqual(reader.GetInt8Field(), -122);
			Assert.AreEqual(reader.GetInt16Field(), -12344);
			Assert.AreEqual(reader.GetInt32Field(), -12345677);
			Assert.AreEqual(reader.GetInt64Field(), -123456789012344L);
			Assert.AreEqual(reader.GetUInt8Field(), 0xe9);
			Assert.AreEqual(reader.GetUInt16Field(), 45677);
			Assert.AreEqual(reader.GetUInt32Field(), 0xce0a6a13);
			Assert.AreEqual(reader.GetUInt64Field(), 0xab54a98ceb1f0ad1L);
			Assert.AreEqual(reader.GetFloat32Field(), 1234.4f);
			Assert.AreEqual(reader.GetFloat64Field(), -123e44);
			Assert.AreEqual(reader.GetEnumField(), Test.TestEnum.Qux);
		}
	}
}
