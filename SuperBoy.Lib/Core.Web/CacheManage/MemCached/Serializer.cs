using System;
using System.IO;
using System.IO.Compression;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;

namespace Core.Web.CacheManage.Memcached
{
	internal enum SerializedType : ushort
	{
		ByteArray	= 0,
		Object		= 1,
		String		= 2,
		Datetime	= 3,
		Bool		= 4,
		//SByte		= 5, //Makes no sense.
		Byte		= 6,
		Short		= 7,
		UShort		= 8,
		Int			= 9,
		UInt		= 10,
		Long		= 11,
		ULong		= 12,
		Float		= 13,
		Double		= 14,

		CompressedByteArray	= 255,
		CompressedObject	= 256,
		CompressedString	= 257,
	}

	internal class Serializer
	{
		public static byte[] Serialize(object value, out SerializedType type, uint compressionThreshold) {
			byte[] bytes;
			if (value is byte[]) {
				bytes = (byte[])value;
				type = SerializedType.ByteArray;
				if (bytes.Length > compressionThreshold) {
					bytes = Compress(bytes);
					type = SerializedType.CompressedByteArray;
				}
			} else if (value is string) {
				bytes = Encoding.UTF8.GetBytes((string)value);
				type = SerializedType.String;
				if (bytes.Length > compressionThreshold) {
					bytes = Compress(bytes);
					type = SerializedType.CompressedString;
				}
			} else if (value is DateTime) {
				bytes = BitConverter.GetBytes(((DateTime)value).Ticks);
				type = SerializedType.Datetime;
			} else if (value is bool) {
				bytes = new byte[]{(byte)((bool)value ? 1 : 0)};
				type = SerializedType.Bool;
			} else if (value is byte) {
				bytes = new byte[]{(byte)value};
				type = SerializedType.Byte;
			} else if (value is short) {
				bytes = BitConverter.GetBytes((short)value);
				type = SerializedType.Short;
			} else if (value is ushort) {
				bytes = BitConverter.GetBytes((ushort)value);
				type = SerializedType.UShort;
			} else if (value is int) {
				bytes = BitConverter.GetBytes((int)value);
				type = SerializedType.Int;
			} else if (value is uint) {
				bytes = BitConverter.GetBytes((uint)value);
				type = SerializedType.UInt;
			} else if (value is long) {
				bytes = BitConverter.GetBytes((long)value);
				type = SerializedType.Long;
			} else if (value is ulong) {
				bytes = BitConverter.GetBytes((ulong)value);
				type = SerializedType.ULong;
			} else if (value is float) {
				bytes = BitConverter.GetBytes((float)value);
				type = SerializedType.Float;
			} else if (value is double) {
				bytes = BitConverter.GetBytes((double)value);
				type = SerializedType.Double;
			} else {
				//Object
				using(var ms = new MemoryStream()) {
					new BinaryFormatter().Serialize(ms, value);
					bytes = ms.ToArray();
					type = SerializedType.Object;
					if (bytes.Length > compressionThreshold) {
						bytes = Compress(bytes);
						type = SerializedType.CompressedObject;
					}
				}
			}
			return bytes;
		}

		private static byte[] Compress(byte[] bytes) {
			using (var ms = new MemoryStream()) {
				using (var gzs = new DeflateStream(ms, CompressionMode.Compress, false)) {
					gzs.Write(bytes, 0, bytes.Length);
				}
				ms.Close();
				return ms.ToArray();
			}
		}

		private static byte[] Decompress (byte[] bytes) {
			using (var ms = new MemoryStream(bytes, false)) {
				using(var gzs = new DeflateStream(ms, CompressionMode.Decompress, false)) {
					using(var dest = new MemoryStream()) {
						var tmp = new byte[bytes.Length];
						int read;
						while ((read = gzs.Read(tmp, 0, tmp.Length)) != 0) {
							dest.Write(tmp, 0, read);
						}
						dest.Close();
						return dest.ToArray();
					}
				}
			}
		}

		public static object DeSerialize(byte[] bytes, SerializedType type) {
			switch (type) {
				case SerializedType.String:
					return Encoding.UTF8.GetString(bytes);
				case SerializedType.Datetime:
					return new DateTime(BitConverter.ToInt64(bytes, 0));
				case SerializedType.Bool:
					return bytes[0] == 1;
				case SerializedType.Byte:
					return bytes[0];
				case SerializedType.Short:
					return BitConverter.ToInt16(bytes, 0);
				case SerializedType.UShort:
					return BitConverter.ToUInt16(bytes, 0);
				case SerializedType.Int:
					return BitConverter.ToInt32(bytes, 0);
				case SerializedType.UInt:
					return BitConverter.ToUInt32(bytes, 0);
				case SerializedType.Long:
					return BitConverter.ToInt64(bytes, 0);
				case SerializedType.ULong:
					return BitConverter.ToUInt64(bytes, 0);
				case SerializedType.Float:
					return BitConverter.ToSingle(bytes, 0);
				case SerializedType.Double:
					return BitConverter.ToDouble(bytes, 0);
				case SerializedType.Object:
					using(var ms = new MemoryStream(bytes)) {
						return new BinaryFormatter().Deserialize(ms);
					}
				case SerializedType.CompressedByteArray:
					return DeSerialize(Decompress(bytes), SerializedType.ByteArray);
				case SerializedType.CompressedString:
					return DeSerialize(Decompress(bytes), SerializedType.String);
				case SerializedType.CompressedObject:
					return DeSerialize(Decompress(bytes), SerializedType.Object);
				case SerializedType.ByteArray:
				default:
					return bytes;
			}
		}
	}
}