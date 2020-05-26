using Atdi.Api.EntityOrm.WebClient.DTO;
using Atdi.Contracts.Api.EntityOrm.WebClient;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.Api.EntityOrm.WebClient
{
	internal sealed class WebApiDataReader : IDataReader
	{
		private readonly ReadQueryResponse _response;
		private readonly Dictionary<string, FieldDescriptor> _fields;
		private readonly long _maxIndex;
		private long _currentIndex;
		private object[] _currentRecord; 
		private long _count;

		public WebApiDataReader(ReadQueryResponse response)
		{
			_response = response;
			_fields = response.Fields.ToDictionary(k => k.Path, v => v);
			_currentIndex = -1;
			_count = response.Records.Length;
			_maxIndex = _count - 1;
		}

		public long Count => _count;

		public long Position => _currentIndex;

		public TValue GetValue<TValue>(string path)
		{
			if (_fields.TryGetValue(path, out var field))
			{
				var value = _currentRecord[field.Index];
				if (value == null)
				{
					return default(TValue);
				}

				var type = typeof(TValue);
				var valueType = value.GetType();

				if (type == valueType)
				{
					return (TValue)value;
				}

				if (type == typeof(byte) || type == typeof(byte?))
				{
					return (TValue)(object)Convert.ToByte(value);
				}
				if (type == typeof(short) || type == typeof(short?))
				{
					return (TValue)(object)Convert.ToInt16(value);
				}
				if (type == typeof(char) || type == typeof(char?))
				{
					return (TValue)(object)Convert.ToChar(value);
				}
				if (type == typeof(int) || type == typeof(int?))
				{
					return (TValue)(object)Convert.ToInt32(value);
				}
				if (type == typeof(long) || type == typeof(long?))
				{
					return (TValue)(object)Convert.ToInt64(value);
				}
				if (type == typeof(Guid) || type == typeof(Guid?))
				{
					return (TValue)(object)Guid.Parse(value.ToString());
				}
				if (type == typeof(DateTime) || type == typeof(DateTime?))
				{
					return (TValue)(object)Convert.ToDateTime(value);
				}
				if (type == typeof(DateTimeOffset) || type == typeof(DateTimeOffset?))
				{
					if (valueType == typeof(DateTime) || valueType == typeof(DateTime?))
					{
						var asDateTime = (DateTime) value;
						return (TValue) (object) new DateTimeOffset(asDateTime);
					}
					return (TValue)(object)DateTimeOffset.Parse(value.ToString());
				}
				if (type == typeof(TimeSpan) || type == typeof(TimeSpan?))
				{
					return (TValue)(object)TimeSpan.Parse(value.ToString());
				}
				if (type == typeof(float) || type == typeof(float?))
				{
					return (TValue)(object)Convert.ToSingle(value);
				}
				if (type == typeof(double) || type == typeof(double?))
				{
					return (TValue)(object)Convert.ToDouble(value);
				}
				if (type == typeof(decimal) || type == typeof(decimal?))
				{
					return (TValue)(object)Convert.ToDecimal(value);
				}
				if (type == typeof(bool) || type == typeof(bool?))
				{
					return (TValue)(object)Convert.ToBoolean(value);
				}
				if (type == typeof(sbyte) || type == typeof(sbyte?))
				{
					return (TValue)(object)Convert.ToSByte(value);
				}
				if (type == typeof(ushort) || type == typeof(ushort?))
				{
					return (TValue)(object)Convert.ToUInt16(value);
				}
				if (type == typeof(uint) || type == typeof(uint?))
				{
					return (TValue)(object)Convert.ToUInt32(value);
				}
				if (type == typeof(ulong) || type == typeof(ulong?))
				{
					return (TValue)(object)Convert.ToUInt64(value);
				}

				return (TValue) value;
			}
			throw new InvalidOperationException($"Field with path '{path}' not found");
		}

		public bool Has(string path)
		{
			return _fields.ContainsKey(path);
		}

		public bool IsNotNull(string path)
		{
			if (_fields.TryGetValue(path, out var field))
			{
				var value = _currentRecord[field.Index];
				return value != null;
			}
			throw new InvalidOperationException($"Field with path '{path}' not found");
		}

		public bool IsNull(string path)
		{
			if (_fields.TryGetValue(path, out var field))
			{
				var value = _currentRecord[field.Index];
				return value == null;
			}
			throw new InvalidOperationException($"Field with path '{path}' not found");
		}

		public bool MoveTo(int index)
		{
			var records = _response.Records;

			if (index >= 0 && index < records.Length)
			{
				_currentIndex = index;
				_currentRecord = records[_currentIndex];
				return true;
			}

			return false;
		}

		public bool Read()
		{
			if (_currentIndex  == _maxIndex)
			{
				return false;
			}

			++_currentIndex;
			_currentRecord = _response.Records[_currentIndex];
			return true;
		}
	}

	internal sealed class WebApiDataReader<TEntity> : IDataReader<TEntity>
	{
		private readonly IDataReader _reader;

		public WebApiDataReader(IDataReader reader)
		{
			_reader = reader;
		}

		public long Count => _reader.Count;

		public long Position => _reader.Position;

		public TValue GetValue<TValue>(System.Linq.Expressions.Expression<Func<TEntity, TValue>> pathExpression)
		{
			return _reader.GetValue<TValue>(pathExpression.Body.GetMemberName());
		}

		public TData GetValueAs<TData>(Expression<Func<TEntity, byte[]>> columnExpression)
		{
			var data = this.GetValue(columnExpression);
			var result = DeserializeAs<TData>(data);
			return result;
		}

		private static T DeserializeAs<T>(byte[] value)
		{
			if (value == null) return default(T);

			var binaryFormatter = new BinaryFormatter();
			using (var stream = new MemoryStream(value))
			{
				binaryFormatter.Binder = new DeserializationBinder();
				var result = binaryFormatter.Deserialize(stream);

				return (T)result;
			}
		}

		internal sealed class DeserializationBinder : SerializationBinder
		{
			public override Type BindToType(string assemblyName, string typeName)
			{

				var resultType = Type.GetType(String.Format("{0}, {1}",
					typeName, assemblyName));

				return resultType;
			}
		}
		public bool Has<TValue>(System.Linq.Expressions.Expression<Func<TEntity, TValue>> pathExpression)
		{
			return _reader.Has(pathExpression.Body.GetMemberName());
		}

		public bool IsNotNull<TValue>(System.Linq.Expressions.Expression<Func<TEntity, TValue>> pathExpression)
		{
			return _reader.IsNotNull(pathExpression.Body.GetMemberName());
		}

		public bool IsNull<TValue>(System.Linq.Expressions.Expression<Func<TEntity, TValue>> pathExpression)
		{
			return _reader.IsNull(pathExpression.Body.GetMemberName());
		}

		public bool MoveTo(int index)
		{
			return _reader.MoveTo(index);
		}

		public bool Read()
		{
			return _reader.Read();
		}


	}
}
