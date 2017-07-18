namespace java.nio.channels
{
	public interface WritableByteChannel
	{
		bool IsOpen();
		void Close();
		int Write(ByteBuffer dst);
	}
}
