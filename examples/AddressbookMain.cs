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

using Capnproto;

namespace Capnproto.Examples
{
	public class AddressbookMain
	{
		/// <exception cref="System.IO.IOException"/>
		public static void writeAddressBook()
		{
			MessageBuilder message = new MessageBuilder();
			Addressbook.AddressBook.Builder addressbook = message.InitRoot(Addressbook.AddressBook.SingleFactory);
			StructList.Builder<Addressbook.Person.Builder> people = addressbook.InitPeople(2);
			Addressbook.Person.Builder alice = people[0];
			alice.SetId(123);
			alice.SetName("Alice");
			alice.SetEmail("alice@example.com");
			StructList.Builder<Addressbook.Person.PhoneNumber.Builder> alicePhones = alice.InitPhones(1);
			alicePhones[0].SetNumber("555-1212");
			alicePhones[0].SetType(Addressbook.Person.PhoneNumber.Type.Mobile);
			alice.GetEmployment().SetSchool("MIT");
			Addressbook.Person.Builder bob = people[1];
			bob.SetId(456);
			bob.SetName("Bob");
			bob.SetEmail("bob@example.com");
			StructList.Builder<Addressbook.Person.PhoneNumber.Builder> bobPhones = bob.InitPhones(2);
			bobPhones[0].SetNumber("555-4567");
			bobPhones[0].SetType(Addressbook.Person.PhoneNumber.Type.Home);
			bobPhones[1].SetNumber("555-7654");
			bobPhones[1].SetType(Addressbook.Person.PhoneNumber.Type.Work);
			bob.GetEmployment().SetUnemployed(Void.VOID);
			SerializePacked.WriteToUnbuffered((new FileDescriptor(Console.OpenStandardOutput())), message);
		}
		
		/// <exception cref="System.IO.IOException"/>
		public static void printAddressBook()
		{
			MessageReader message = SerializePacked.ReadFromUnbuffered(new FileDescriptor(Console.OpenStandardInput()));
			Addressbook.AddressBook.Reader addressbook = message.GetRoot(Addressbook.AddressBook.SingleFactory);
			foreach(Addressbook.Person.Reader person in addressbook.GetPeople())
			{
				Console.Out.WriteLine(person.GetName() + ": " + person.GetEmail());
				foreach(Addressbook.Person.PhoneNumber.Reader phone in person.GetPhones())
				{
					string typeName = "UNKNOWN";
					switch(phone.GetType())
					{
						case Addressbook.Person.PhoneNumber.Type.Mobile:
							typeName = "mobile";
							break;
						case Addressbook.Person.PhoneNumber.Type.Home:
							typeName = "home";
							break;
						case Addressbook.Person.PhoneNumber.Type.Work:
							typeName = "work";
							break;
					}
					Console.Out.WriteLine("  " + typeName + " phone: " + phone.GetNumber());
				}
				Addressbook.Person.Employment.Reader employment = person.GetEmployment();
				switch(employment.Which)
				{
					case Addressbook.Person.Employment.Which.Unemployed:
						System.Console.Out.WriteLine("  unemployed");
						break;
					case Addressbook.Person.Employment.Which.Employer:
						System.Console.Out.WriteLine("  employer: " + employment.GetEmployer());
						break;
					case Addressbook.Person.Employment.Which.School:
						System.Console.Out.WriteLine("  student at: " + employment.GetSchool());
						break;
					case Addressbook.Person.Employment.Which.SelfEmployed:
						System.Console.Out.WriteLine("  self-employed");
						break;
					default:
						break;
				}
			}
		}

		public static void usage()
		{
			Console.Out.WriteLine("usage: addressbook [write | read]");
		}

		public static void Main(string[] args)
		{
			try
			{
				if(args.Length < 1)
					usage();
				else if(args[0].Equals("write"))
					writeAddressBook();
				else if(args[0].Equals("read"))
					printAddressBook();
				else
					usage();
			}
			catch(System.IO.IOException e)
			{
				Console.Out.WriteLine("io exception: " + e);
			}
		}
	}
}
