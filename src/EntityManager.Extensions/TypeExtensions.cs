namespace System
{
    internal static class TypeExtensions
    {
        internal static Type UnwrapNullableType(this Type type) => Nullable.GetUnderlyingType(type) ?? type;
    }
}
