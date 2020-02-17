using System;

namespace AWSXRay.SqlClient.Extension.Extensions
{
    internal static class ObjectExtensions
    {
        public static T With<T>(this T obj, Action<T> action)
        {
            action(obj);

            return obj;
        }
    }
}
