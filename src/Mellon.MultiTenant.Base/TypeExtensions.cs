using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Mellon.MultiTenant.Base
{
    public static class TypeExtensions
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
                int genericParameterPosition = parameterType.GenericParameterPosition;
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

                    Type? elementType = parameterType.GetElementType();
                    return TypesMatchRecursive(actualType: actualType.GetElementType().GetTypeInfo(), parameterType: elementType.GetTypeInfo(), genericArguments: genericArguments);
                }

                if (!actualType.IsGenericType || parameterType.GetGenericTypeDefinition() != actualType.GetGenericTypeDefinition())
                {
                    return false;
                }

                for (int i = 0; i < parameterType.GenericTypeArguments.Length; i++)
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

            return type.FullName!.Substring(type.Namespace!.Length + 1);
        }

        private static string ReplacePlusWithDotInNestedTypeName(this string typeName)
        {
            return typeName.Replace('+', '.');
        }

        private static string ReplaceGenericParametersInGenericTypeName(this string typeName, Type type)
        {
            Type[] genericArguments = type.GetTypeInfo().GetAllGenericArguments();
            typeName = new Regex("`[1-9]\\d*").Replace(typeName, delegate (Match match)
            {
                int count = int.Parse(match.Value.Substring(1));
                string text = string.Join(",", genericArguments.Take(count).Select(new Func<Type, string>(ToGenericTypeString)));
                genericArguments = genericArguments.Skip(count).ToArray();
                return "<" + text + ">";
            });
            return typeName;
        }
    }
}
