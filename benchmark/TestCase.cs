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

namespace Capnproto.Benchmark
{
	public abstract class TestCase<RequestFactory, RequestBuilder, RequestReader, ResponseFactory, ResponseBuilder, ResponseReader, Expectation>
	                      where RequestFactory : StructFactory<RequestBuilder, RequestReader>
	                      where RequestBuilder : StructBuilder
	                      where RequestReader : StructReader
	                      where ResponseFactory : StructFactory<ResponseBuilder, ResponseReader>
	                      where ResponseBuilder : StructBuilder
	                      where ResponseReader : StructReader
	{
		public abstract Expectation setupRequest(Common.FastRand rng, RequestBuilder request);
		
		public abstract void handleRequest(RequestReader request, ResponseBuilder response);
		
		public abstract bool checkResponse(ResponseReader response, Expectation expected);
		
		public virtual void passByObject(RequestFactory requestFactory, ResponseFactory responseFactory, Compression compression, long iters)
		{
			Common.FastRand rng = new Common.FastRand();
			for(int i = 0; i < iters; ++i)
			{
				MessageBuilder requestMessage = new MessageBuilder();
				MessageBuilder responseMessage = new MessageBuilder();
				RequestBuilder request = requestMessage.InitRoot(requestFactory);
				Expectation expected = this.setupRequest(rng, request);
				ResponseBuilder response = responseMessage.InitRoot(responseFactory);
				this.handleRequest(requestFactory.AsReader(request), response);
				if(!this.checkResponse(responseFactory.AsReader(response), expected))
					System.Console.Out.WriteLine("mismatch!");
			}
		}
		
		internal const int SCRATCH_SIZE = 128 * 1024;
		
		/// <exception cref="System.IO.IOException"/>
		public virtual void passByBytes(RequestFactory requestFactory, ResponseFactory responseFactory, Compression compression, long iters)
		{
			java.nio.ByteBuffer requestBytes = java.nio.ByteBuffer.allocate(SCRATCH_SIZE * 8);
			java.nio.ByteBuffer responseBytes = java.nio.ByteBuffer.allocate(SCRATCH_SIZE * 8);
			Common.FastRand rng = new Common.FastRand();
			for(int i = 0; i < iters; ++i)
			{
				MessageBuilder requestMessage = new MessageBuilder();
				MessageBuilder responseMessage = new MessageBuilder();
				RequestBuilder request = requestMessage.InitRoot(requestFactory);
				Expectation expected = this.setupRequest(rng, request);
				ResponseBuilder response = responseMessage.InitRoot(responseFactory);
				{
					ArrayOutputStream writer = new ArrayOutputStream(requestBytes);
					compression.writeBuffered(writer, requestMessage);
				}
				{
					MessageReader messageReader = compression.newBufferedReader(new ArrayInputStream(requestBytes));
					this.handleRequest(messageReader.GetRoot(requestFactory), response);
				}
				{
					ArrayOutputStream writer = new ArrayOutputStream(responseBytes);
					compression.writeBuffered(writer, responseMessage);
				}
				{
					MessageReader messageReader = compression.newBufferedReader(new ArrayInputStream(responseBytes));
					if(!this.checkResponse(messageReader.GetRoot(responseFactory), expected))
						throw new System.Exception("incorrect response");
				}
			}
		}
		
		/// <exception cref="System.IO.IOException"/>
		public virtual void syncServer(RequestFactory requestFactory, ResponseFactory responseFactory, Compression compression, long iters)
		{
			BufferedOutputStreamWrapper outBuffered = new BufferedOutputStreamWrapper(new FileDescriptor(Console.OpenStandardOutput()));
			BufferedInputStreamWrapper inBuffered = new BufferedInputStreamWrapper(new FileDescriptor(Console.OpenStandardInput()));
			for(int ii = 0; ii < iters; ++ii)
			{
				MessageBuilder responseMessage = new MessageBuilder();
				{
					ResponseBuilder response = responseMessage.InitRoot(responseFactory);
					MessageReader messageReader = compression.newBufferedReader(inBuffered);
					RequestReader request = messageReader.GetRoot(requestFactory);
					this.handleRequest(request, response);
				}
				compression.writeBuffered(outBuffered, responseMessage);
			}
		}
		
		/// <exception cref="System.IO.IOException"/>
		public virtual void syncClient(RequestFactory requestFactory, ResponseFactory responseFactory, Compression compression, long iters)
		{
			Common.FastRand rng = new Common.FastRand();
			BufferedOutputStreamWrapper outBuffered = new BufferedOutputStreamWrapper(new FileDescriptor(Console.OpenStandardOutput()));
			BufferedInputStreamWrapper inBuffered = new BufferedInputStreamWrapper(new FileDescriptor(Console.OpenStandardInput()));
			for(int ii = 0; ii < iters; ++ii)
			{
				MessageBuilder requestMessage = new MessageBuilder();
				RequestBuilder request = requestMessage.InitRoot(requestFactory);
				Expectation expected = this.setupRequest(rng, request);
				compression.writeBuffered(outBuffered, requestMessage);
				MessageReader messageReader = compression.newBufferedReader(inBuffered);
				ResponseReader response = messageReader.GetRoot(responseFactory);
				if(!this.checkResponse(response, expected))
					throw new System.Exception("incorrect response");
			}
		}
		
		public virtual void execute(string[] args, RequestFactory requestFactory, ResponseFactory responseFactory)
		{
			if(args.Length != 4)
			{
				System.Console.Out.WriteLine("USAGE: TestCase MODE REUSE COMPRESSION ITERATION_COUNT");
				return;
			}
			string mode = args[0];
			string reuse = args[1];
			Compression compression = null;
			if(args[2].Equals("packed"))
				compression = Compression.PACKED;
			else if(args[2].Equals("none"))
				compression = Compression.UNCOMPRESSED;
			else
				throw new System.Exception("unrecognized compression: " + args[2]);
			
			long iters = long.Parse(args[3]);
			try
			{
				if(mode.Equals("object"))
					passByObject(requestFactory, responseFactory, compression, iters);
				else if(mode.Equals("bytes"))
					passByBytes(requestFactory, responseFactory, compression, iters);
				else if(mode.Equals("client"))
					syncClient(requestFactory, responseFactory, compression, iters);
				else if(mode.Equals("server"))
					syncServer(requestFactory, responseFactory, compression, iters);
				else
					System.Console.Out.WriteLine("unrecognized mode: " + mode);
			}
			catch(System.IO.IOException e)
			{
				System.Console.Error.WriteLine("IOException: " + e);
			}
		}
	}
}
