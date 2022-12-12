using System.ComponentModel;
using System.Reflection;

namespace Ornaments.Extensions;

public static class EnumExtensions
{
    public static string GetDescription(this Enum value)
    {
        FieldInfo? fi = value.GetType().GetField(value.ToString());

        return fi?.GetCustomAttributes(typeof(DescriptionAttribute), false) is DescriptionAttribute[] attributes && attributes.Any()
            ? attributes.First().Description
            : throw new InvalidEnumArgumentException(value.ToString());
    }
}
