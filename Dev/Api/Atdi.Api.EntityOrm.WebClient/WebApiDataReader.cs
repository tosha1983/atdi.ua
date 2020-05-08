using Atdi.Api.EntityOrm.WebClient.DTO;
using Atdi.Contracts.Api.EntityOrm.WebClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
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

		public TValue GetValue<TValue>(System.Linq.Expressions.Expression<Func<TEntity, TValue>> pathExpression)
		{
			return _reader.GetValue<TValue>(pathExpression.Body.GetMemberName());
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

		public bool Read()
		{
			return _reader.Read();
		}


	}
}
