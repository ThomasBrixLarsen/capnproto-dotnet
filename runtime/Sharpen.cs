using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;

namespace Sharpen
{
	public abstract class EnumBase : IComparable<EnumBase>, IComparable
	{
		private static readonly Dictionary<Type, EnumBase[]> VALUES_MAP = new Dictionary<Type, EnumBase[]>();
		
		private readonly int _ordinal;
		private readonly string _name;
		
		protected EnumBase(int ordinal, string name)
		{
			_ordinal = ordinal;
			_name = name;
		}
		
		public int ordinal()
		{
			return _ordinal;
		}
		
		public string name()
		{
			return _name;
		}
		
		public override string ToString()
		{
			return _name;
		}
		
		public int CompareTo(object obj)
		{
			return CompareTo((EnumBase) obj);
		}
		
		public int CompareTo(EnumBase other)
		{
			return this._ordinal - other._ordinal;
		}
		
		public static bool IsEnum(Type t)
		{
			return VALUES_MAP.ContainsKey(t);
		}
		
		protected static void RegisterValues<T>(EnumBase[] values) where T : EnumBase
		{
			VALUES_MAP[typeof(T)] = values;
		}
		
		public static EnumBase[] GetEnumValue(Type enumType)
		{
			EnumBase[] result;
			if(VALUES_MAP.TryGetValue(enumType, out result))
				return result;
			RuntimeHelpers.RunClassConstructor(enumType.TypeHandle);
			return VALUES_MAP[enumType];
		}
	}
	
	public class System
	{
		public static readonly DateTime EPOCH = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
		
		public static int Compare(int x, int y)
		{
			return (x < y)? -1 : ((x == y) ? 0 : 1);
		}
		
		public static int Compare(long x, long y)
		{
			return (x < y)? -1 : ((x == y) ? 0 : 1);
		}
		
		public static long CurrentTimeMillis()
		{
			return (long)(DateTime.UtcNow - EPOCH).TotalMilliseconds;
		}
		
		public static int FloorDiv(int x, int y)
		{
			int r = x / y;
			if((x ^ y) < 0 && r * y != x)
				r--;
			return r;
		}
		
		public static int HighestOneBit(int i)
		{
			uint u = (uint) i;
			u |= (u >> 1);
			u |= (u >> 2);
			u |= (u >> 4);
			u |= (u >> 8);
			u |= (u >> 16);
			return (int)(u - (u >> 1));
		}
	}
	
	public class Arrays
	{
		public static void Fill<T>(T[] a, T val)
		{
			Fill(a, 0, a.Length, val);
		}
		
		public static void Fill<T>(T[] a, int from, int to, T val)
		{
			for(int i = from; i < to; i++)
				a[i] = val;
		}
		
		public static T[] CopyOf<T>(T[] a, int newSize)
		{
			T[] result = new T[newSize];
			a.CopyTo(result, 0);
			return result;
		}
		
		public static int HashCode<T>(T[] a)
		{
			if(a == null)
				return 0;
			
			int result = 1;
			foreach(var element in a)
				result = 31 * result + element.GetHashCode();
			return result;
		}
		
		public static string ToString<T>(T[] a)
		{
			StringBuilder sb = new StringBuilder();
			sb.Append("[");
			for(var i = 0; i < a.Length; i++)
			{
				if(i > 0)
					sb.Append(", ");
				sb.Append(a[i]);
			}
			sb.Append("]");
			return sb.ToString();
		}
	}
	
	public static class Collections
	{
		public static V Remove<K, V>(IDictionary<K, V> map, K key)
		{
			V result;
			if(map.TryGetValue(key, out result))
			{
				map.Remove(key);
				return result;
			}
			return default(V);
		}
		
		public static void PutAll<CK, CV, IK, IV>(IDictionary<CK, CV> collection, IDictionary<IK, IV> items) where IK : CK where IV : CV
		{
			foreach(var e in items)
				collection.Add(e.Key, e.Value);
		}
		
		public static void AddAll<T>(ICollection<T> collection, IEnumerable<T> items)
		{
			foreach(var item in items)
				collection.Add(item);
		}
		
		public static T[] ToArray<T>(ICollection<T> collection, T[] array)
		{
			int i = 0;
			foreach(var item in collection)
				array[i++] = item;
			return array;
		}
	}
	
	public static class Runtime
	{
		public static string substring(string s, int from, int to)
		{
			return s.Substring(from, to - from);
		}
		
		public static string GetSimpleName(this Type t)
		{
			string name = t.Name;
			int index = name.IndexOf('`');
			return index == -1? name : name.Substring(0, index);
		}
		
		public static FieldInfo[] GetDeclaredFields(Type clazz)
		{
			return clazz.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.DeclaredOnly | BindingFlags.Instance);
		}
		
		public static bool HasAttribute(FieldAttributes attributes, FieldAttributes flag)
		{
			return (attributes & flag) != 0;
		}
		
		public static CustomAttributeData GetCustomAttribute(FieldInfo field, Type attributeType)
		{
			foreach(var a in CustomAttributeData.GetCustomAttributes(field))
			{
				if(a.Constructor.DeclaringType == attributeType)
					return a;
			}
			return null;
		}
		
		public static byte[] GetBytesForString(string str)
		{
			return Encoding.UTF8.GetBytes(str);
		}
		
		public static byte[] GetBytesForString(string str, string encoding)
		{
			return Encoding.GetEncoding(encoding).GetBytes(str);
		}
		
		public static string GetStringForBytes(byte[] chars)
		{
			return Encoding.UTF8.GetString(chars);
		}
		
		public static string GetStringForBytes(byte[] chars, string encoding)
		{
			return GetEncoding(encoding).GetString(chars);
		}
		
		public static string GetStringForBytes(byte[] chars, int start, int len)
		{
			return Encoding.UTF8.GetString(chars, start, len);
		}
		
		public static string GetStringForBytes(byte[] chars, int start, int len, string encoding)
		{
			return GetEncoding(encoding).GetString(chars, start, len);
		}
		
		public static Encoding GetEncoding(string name)
		{
			Encoding e = Encoding.GetEncoding(name.Replace('_', '-'));
			if(e is UTF8Encoding)
				return new UTF8Encoding(false, true);
			return e;
		}
		
		public static float intBitsToFloat(int x)
		{
			byte[] bytes = BitConverter.GetBytes(x);
			return BitConverter.ToSingle(bytes, 0);
		}
		
		public static int floatToIntBits(float x)
		{
			byte[] bytes = BitConverter.GetBytes(x);
			return BitConverter.ToInt32(bytes, 0);
		}
		
		public static double longBitsToDouble(long x)
		{
			byte[] bytes = BitConverter.GetBytes(x);
			return BitConverter.ToDouble(bytes, 0);
		}
		
		public static long doubleToLongBits(double x)
		{
			byte[] bytes = BitConverter.GetBytes(x);
			return BitConverter.ToInt64(bytes, 0);
		}
	}
	
	public class IdentityHashMap<K, V> : Dictionary<K, V>
	{
		public IdentityHashMap() : base(new IdentityEqualityComparer<K>())
		{
			
		}
	}
	
	public class IdentityEqualityComparer<T> : IEqualityComparer<T>
	{
		public bool Equals(T x, T y)
		{
			return ReferenceEquals(x, y);
		}
		
		public int GetHashCode(T obj)
		{
			return RuntimeHelpers.GetHashCode(obj);
		}
	}
	
	public static class Lists
	{
		public static void Add<T>(this IList<T> list, int index, T value)
		{
			list.Insert(index, value);
		}
		
		public static T RemoveAtReturningValue<T>(this IList<T> list, int index)
		{
			T value = list[index];
			list.RemoveAt(index);
			return value;
		}
	}
	
	public static class Maps
	{
		public static V GetOrDefault<K, V>(this IDictionary<K, V> map, K key, V defaultValue)
		{
			V result;
			return map.TryGetValue(key, out result)? result : defaultValue;
		}
		
		public static V GetOrNull<K, V>(this IDictionary<K, V> map, K key) where V : class
		{
			V result;
			return map.TryGetValue(key, out result)? result : null;
		}
		
		public static V? GetOrNullable<K, V>(this IDictionary<K, V> map, K key) where V : struct
		{
			V result;
			return map.TryGetValue(key, out result)? result : new V? ();
		}
		
	}
}
