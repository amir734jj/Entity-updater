using System;

namespace EntityUpdater.Extensions
{
    internal static class TypeExtension
    {
        public static T Instantiate<T>(this Type type)
        {
            return (T) Activator.CreateInstance(type);
        }
    }
}