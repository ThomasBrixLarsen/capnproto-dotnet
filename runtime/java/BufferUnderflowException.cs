namespace java.nio
{
	using System;
	
	[Serializable()]
	public class BufferUnderflowException : System.Exception
	{
		public BufferUnderflowException() : base()
		{
			
		}
		public BufferUnderflowException(string message) : base(message)
		{
			
		}
		public BufferUnderflowException(string message, System.Exception inner) : base(message, inner)
		{
			
		}
		
		//A constructor is needed for serialization when an
		//exception propagates from a remoting server to the client.
		protected BufferUnderflowException(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
		{
			
		}
	}
}
