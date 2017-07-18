namespace java.nio
{
	using System;
	
	public class ByteBuffer
	{
		public byte[] buffer;
		private DataConverter c;
		private int capacity_;
		private int index;
		internal int limit_;
		private int mark_;
		public int offset;
		private ByteOrder order_;
		
		public ByteBuffer()
		{
			this.buffer = new byte[0];
			this.c = DataConverter.LittleEndian;
		}
		
		private ByteBuffer(byte[] buf, int start, int len)
		{
			this.buffer = buf;
			this.offset = 0;
			this.limit_ = start + len;
			this.index = start;
			this.mark_ = start;
			this.capacity_ = len;
			this.c = DataConverter.LittleEndian;
		}
		
		public static ByteBuffer allocate(int size)
		{
			return new ByteBuffer(new byte[size], 0, size);
		}
		
		public static ByteBuffer allocateDirect(int size)
		{
			return allocate(size);
		}
		
		public static ByteBuffer prepare(int size)
		{
			return new ByteBuffer(null, 0, size);
		}
		
		public byte[] array()
		{
			return buffer;
		}
		
		public int arrayOffset()
		{
			return offset;
		}
		
		public ByteBuffer asReadOnlyBuffer()
		{
			return this;
		}
		
		public int capacity()
		{
			return capacity_;
		}
		
		private void checkGetLimit(int index, int inc)
		{
			if(index + inc > limit_)
				throw new BufferUnderflowException();
		}
		
		private void checkGetLimit(int inc)
		{
			if(index + inc > limit_)
			{
				System.Console.WriteLine("index: " + index.ToString() + " + " + inc.ToString() + " > " + limit_.ToString() + ", offset: " + offset.ToString());
				throw new BufferUnderflowException();
			}
		}
		
		private void checkPutLimit(int index, int inc)
		{
			if(index + inc > limit_)
			{
				System.Console.WriteLine("index: " + index.ToString() + " + " + inc.ToString() + " > " + limit_.ToString() + ", offset: " + offset.ToString());
				throw new BufferUnderflowException();
			}
		}
		
		private void checkPutLimit(int inc)
		{
			if(index + inc > limit_)
			{
				System.Console.WriteLine("index: " + index.ToString() + " + " + inc.ToString() + " > " + limit_.ToString() + ", offset: " + offset.ToString());
				throw new BufferUnderflowException();
			}
		}
		
		public void clear()
		{
			index = offset;
			limit_ = offset + capacity_;
		}
		
		public ByteBuffer duplicate()
		{
			return new ByteBuffer(buffer, index, limit_);
		}
		
		public void flip()
		{
			limit_ = index;
			index = offset;
		}
		
		public byte get()
		{
			checkGetLimit(1);
			return buffer[index++];
		}
		
		public void get(byte[] data)
		{
			get(data, 0, data.Length);
		}
		
		public void get(byte[] data, int start, int len)
		{
			checkGetLimit(len);
			for(int i = 0; i < len; i++)
				data[i + start] = buffer[index++];
		}
		
		public short getShort()
		{
			checkGetLimit(2);
			short num = c.GetInt16(buffer, index);
			index += 2;
			return num;
		}
		
		public int getInt()
		{
			checkGetLimit(4);
			int num = c.GetInt32(buffer, index);
			index += 4;
			return num;
		}
		
		public long getLong()
		{
			checkGetLimit(8);
			long num = c.GetInt64(buffer, index);
			index += 8;
			return num;
		}
		
		public byte get(int index)
		{
			return buffer[index+offset];
		}
		
		public short getShort(int index)
		{
			checkGetLimit(index+offset, 2);
			return c.GetInt16(buffer, index+offset);
		}
		
		public int getInt(int index)
		{
			checkGetLimit(index+offset, 4);
			return c.GetInt32(buffer, index+offset);
		}
		
		public long getLong(int index)
		{
			checkGetLimit(index+offset, 8);
			return c.GetInt64(buffer, index+offset);
		}
		
		public float getFloat(int index)
		{
			checkGetLimit(index+offset, 4);
			return c.GetFloat(buffer, index+offset);
		}
		
		public double getDouble(int index)
		{
			checkGetLimit(index+offset, 8);
			return c.GetDouble(buffer, index+offset);
		}
		
		public bool hasArray()
		{
			return true;
		}
		
		public bool hasRemaining()
		{
			return remaining() > 0;
		}
		
		public int limit()
		{
			return limit_ - offset;
		}
		
		public void limit(int newLimit)
		{
			limit_ = newLimit + offset;
			if(index > limit_)
				position(newLimit);
		}
		
		public void mark()
		{
			mark_ = index;
		}
		
		public void order(ByteOrder order)
		{
			this.order_ = order;
			c = order_ == ByteOrder.BIG_ENDIAN? DataConverter.BigEndian : DataConverter.LittleEndian;
		}
		
		public int position()
		{
			return index - offset;
		}
		
		public void position(int pos)
		{
			if(pos < 0 || pos > limit())
				throw new BufferUnderflowException();
			index = pos + offset;
		}
		
		public void put(byte[] data)
		{
			put(data, 0, data.Length);
		}
		
		public void put(byte data)
		{
			checkPutLimit(1);
			buffer[index++] = data;
		}
		
		public void put(byte[] data, int start, int len)
		{
			checkPutLimit(len);
			for(int i = 0; i < len; i++)
				buffer[index++] = data[i + start];
		}
		
		public void put(byte[] data, int index)
		{
			checkPutLimit(offset+index, data.Length);
			for(int i = 0; i < data.Length; i++)
				buffer[offset+index+i] = data[i];
		}
		
		public void put(ByteBuffer src)
		{
			checkPutLimit(src.remaining());
			for(int i = 0; i < src.remaining(); i++)
				buffer[index+i] = src.buffer[src.index+i];
			index += src.remaining();
			src.index += src.remaining();
		}
		
		public void putInt(int i)
		{
			put(c.GetBytes(i));
		}
		
		public void putShort(short i)
		{
			put(c.GetBytes(i));
		}
		
		public void put(int pos, byte src)
		{
			buffer[pos+offset] = src;
		}
		
		public void putShort(int pos, short src)
		{
			put(c.GetBytes(src), pos);
		}
		
		public void putInt(int pos, int src)
		{
			put(c.GetBytes(src), pos);
		}
		
		public void putLong(int pos, long src)
		{
			put(c.GetBytes(src), pos);
		}
		
		public void putFloat(int pos, float src)
		{
			put(c.GetBytes(src), pos);
		}
		
		public void putDouble(int pos, double src)
		{
			put(c.GetBytes(src), pos);
		}
		
		public int remaining()
		{
			return limit() - position();
		}
		
		public void reset()
		{
			index = mark_;
		}
		
		public ByteBuffer rewind()
		{
			index = offset;
			return this;
		}
		
		public ByteBuffer slice()
		{
			ByteBuffer b = wrap(buffer, index, buffer == null? 0 : buffer.Length - index);
			b.offset = index;
			b.limit_ = limit_;
			b.order_ = order_;
			b.c = c;
			b.capacity_ = limit_ - index;
			return b;
		}
		
		public static ByteBuffer wrap(byte[] buf)
		{
			return new ByteBuffer(buf, 0, buf.Length);
		}
		
		public static ByteBuffer wrap(byte[] buf, int start, int len)
		{
			return new ByteBuffer(buf, start, len);
		}
	}
}
