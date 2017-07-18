namespace java.nio.channels
{
	public interface ReadableByteChannel
	{
		bool IsOpen();
		void Close();
		int Read(ByteBuffer dst);
	}
}
