namespace Mellon.MultiTenant.Base;

using System.Reflection;
using System.Text.RegularExpressions;

public static partial class TypeExtensions
{
	public static string ToGenericTypeString(this Type type)
	{
		if (!type.GetTypeInfo().IsGenericType)
		{
			return type.GetFullNameWithoutNamespace().ReplacePlusWithDotInNestedTypeName();
		}

		return type
			.GetGenericTypeDefinition()
			.GetFullNameWithoutNamespace()
			.ReplacePlusWithDotInNestedTypeName()
			.ReplaceGenericParametersInGenericTypeName(type);
	}

	public static Type[] GetAllGenericArguments(this TypeInfo type)
	{
		if (type.GenericTypeArguments.Length == 0)
		{
			return type.GenericTypeParameters;
		}

		return type.GenericTypeArguments;
	}

	private static bool TypesMatchRecursive(TypeInfo parameterType, TypeInfo actualType, IList<Type> genericArguments)
	{
		if (parameterType.IsGenericParameter)
		{
			var genericParameterPosition = parameterType.GenericParameterPosition;

			if (genericArguments[genericParameterPosition] != null && genericArguments[genericParameterPosition].GetTypeInfo() != actualType)
			{
				return false;
			}

			genericArguments[genericParameterPosition] = actualType.AsType();
			return true;
		}

		if (parameterType.ContainsGenericParameters)
		{
			if (parameterType.IsArray)
			{
				if (!actualType.IsArray)
				{
					return false;
				}

				var elementType = parameterType.GetElementType();
				return TypesMatchRecursive(actualType: actualType.GetElementType().GetTypeInfo(), parameterType: elementType.GetTypeInfo(), genericArguments: genericArguments);
			}

			if (!actualType.IsGenericType || parameterType.GetGenericTypeDefinition() != actualType.GetGenericTypeDefinition())
			{
				return false;
			}

			for (var i = 0; i < parameterType.GenericTypeArguments.Length; i++)
			{
				Type type = parameterType.GenericTypeArguments[i];
				if (!TypesMatchRecursive(actualType: actualType.GenericTypeArguments[i].GetTypeInfo(), parameterType: type.GetTypeInfo(), genericArguments: genericArguments))
				{
					return false;
				}
			}

			return true;
		}

		return parameterType == actualType;
	}

	private static string GetFullNameWithoutNamespace(this Type type)
	{
		if (type.IsGenericParameter)
		{
			return type.Name;
		}

		if (string.IsNullOrEmpty(type.Namespace))
		{
			return type.FullName;
		}

		return type.FullName![(type.Namespace!.Length + 1)..];
	}

	private static string ReplacePlusWithDotInNestedTypeName(this string typeName) => typeName.Replace('+', '.');

	private static string ReplaceGenericParametersInGenericTypeName(
		this string typeName, Type type)
	{
		var genericArguments = type.GetTypeInfo().GetAllGenericArguments();

		typeName = TypeNameRegex().Replace(typeName, match =>
		{
			var count = int.Parse(match.Value[1..]);
			var text = string.Join(",", genericArguments.Take(count).Select(new Func<Type, string>(ToGenericTypeString)));
			genericArguments = genericArguments.Skip(count).ToArray();
			return $"<{text}>";
		});

		return typeName;
	}

	[GeneratedRegex("`[1-9]\\d*", RegexOptions.IgnoreCase, matchTimeoutMilliseconds: 1000)]
	private static partial Regex TypeNameRegex();
}