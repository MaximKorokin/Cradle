using System;
using System.Collections.Generic;
using System.Linq;

namespace Assets._Game.Scripts.Shared.Extensions
{
    public static class EnumExtensions
    {
        public static IEnumerable<T> GetAttributes<T>(this Enum enumVal) where T : Attribute
        {
            var type = enumVal.GetType();

            var memInfo = type.GetMember(enumVal.ToString());
            if (memInfo.Length > 0)
            {
                var attributes = memInfo[0].GetCustomAttributes(typeof(T), false);
                if (attributes.Length > 0)
                {
                    return attributes.Cast<T>();
                }
            }
            return Enumerable.Empty<T>();
        }
    }
}
