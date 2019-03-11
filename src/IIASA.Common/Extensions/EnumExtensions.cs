using System;
using System.ComponentModel;
using System.Linq;

namespace IIASA.Common.Extensions
{
    public static class EnumExtensions
    {
        public static string GetDisplayName(this Enum value)
        {
            var type = value.GetType();
            var name = Enum.GetName(type, value);

            if (name == null)
                return null;

            var field = type.GetField(name);

            if (field == null)
                return null;

            var attr = (DisplayNameAttribute)field.GetCustomAttributes(typeof(DisplayNameAttribute), false).SingleOrDefault();

            return attr?.DisplayName;
        }

        public static T GetAttribute<T>(this Enum value, bool customAttributes = false)
            where T : Attribute
        {
            var type = value.GetType();

            var name = Enum.GetName(type, value);

            return type.GetField(name)
                .GetCustomAttributes(customAttributes)
                .OfType<T>()
                .SingleOrDefault();
        }
    }
}
