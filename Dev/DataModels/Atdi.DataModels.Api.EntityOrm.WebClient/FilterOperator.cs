namespace Atdi.DataModels.Api.EntityOrm.WebClient
{
	/// <summary>
	/// Describes the type of comparison for two values (or expressions) in a condition expression
	/// </summary>
	public enum FilterOperator
	{
		/// <summary>
		/// The values are compared for equality.
		/// </summary>
		Equal,
		/// <summary>
		/// The value is greater than or equal to the compared value.
		/// </summary>
		GreaterEqual,
		/// <summary>
		/// The value is greater than the compared value.
		/// </summary>
		GreaterThan,
		/// <summary>
		/// The value is less than or equal to the compared value.
		/// </summary>
		LessEqual,
		/// <summary>
		/// The value is less than the compared value.
		/// </summary>
		LessThan,
		/// <summary>
		/// The two values are not equal.
		/// </summary>
		NotEqual,
		/// <summary>
		/// The value is null.
		/// </summary>
		IsNull,
		/// <summary>
		/// The value is not null. 
		/// </summary>
		IsNotNull,
		/// <summary>
		/// The character string is matched to the specified pattern.
		/// </summary>
		Like,
		/// <summary>
		/// The character string does not match the specified pattern.
		/// </summary>
		NotLike,
		/// <summary>
		/// The value exists in a list of values.
		/// </summary>
		In,
		/// <summary>
		/// The value does not exist in a list of values.
		/// </summary>
		NotIn,
		/// <summary>
		/// The value is between two values.
		/// </summary>
		Between,
		/// <summary>
		/// The value is not between two values.
		/// </summary>
		NotBetween,
		/// <summary>
		/// The character string is matched to the specified pattern.
		/// </summary>
		BeginWith,
		/// <summary>
		/// The character string is matched to the specified pattern.
		/// </summary>
		EndWith,
		/// <summary>
		/// The character string is matched to the specified pattern.
		/// </summary>
		Contains,
		/// <summary>
		/// The character string does not match the specified pattern.
		/// </summary>
		NotBeginWith,
		/// <summary>
		/// The character string does not match the specified pattern.
		/// </summary>
		NotEndWith,
		/// <summary>
		/// The character string does not match the specified pattern.
		/// </summary>
		NotContains

	}
}