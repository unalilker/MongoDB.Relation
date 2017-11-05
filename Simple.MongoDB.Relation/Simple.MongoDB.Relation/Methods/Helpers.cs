using System;
using System.Collections;
using System.Reflection;
using Simple.MongoDB.Relation.Models;

namespace Simple.MongoDB.Relation.Methods
{
    internal static class Helpers
    {
        public static string FirstCharacterToLower(this string str)
        {
            if (string.IsNullOrEmpty(str) || char.IsLower(str, 0)) return str;

            return char.ToLowerInvariant(str[0]) + str.Substring(1);
        }

        public static ReturnTypes GetPropertyType(this PropertyInfo prop)
        {
            var type = prop.PropertyType;
            if (type == typeof(string)) return ReturnTypes.Value;
            if (typeof(IEnumerable).IsAssignableFrom(prop.PropertyType)) return ReturnTypes.List;

            return type.IsClass ? ReturnTypes.Object : ReturnTypes.Value;
        }

        public static string CreateUniqueString()
        {
            return Guid.NewGuid().ToString().Replace("-", "");
        }
    }
}
