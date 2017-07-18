namespace Capnproto
{
	using System;
	using System.IO;
	
	using java.nio;
	using java.nio.channels;
	
	public class FileDescriptor : ReadableByteChannel, WritableByteChannel
	{
		public FileDescriptor(Stream file)
		{
			this.file = file;
		}
		
		public bool IsOpen()
		{
			return true;
		}
		
		public void Close()
		{
			file.Close();
		}
		
		///Reads from fd to dst.
		public int Read(ByteBuffer dst)
		{
			if(dst.buffer == null)
				dst.buffer = new byte[dst.remaining()];
			file.Read(dst.array(), 0, dst.capacity());
			dst.position(dst.capacity());
			dst.limit(dst.capacity());
			return dst.capacity();
		}
		
		///Writes from src to fd.
		public int Write(ByteBuffer src)
		{
			file.Write(src.array(), 0, src.limit());
			src.position(src.limit());
			return src.limit();
		}
		
		private Stream file;
	}
}
