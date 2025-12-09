using System.ComponentModel;
using System.Reflection;

namespace CaseApp.Utils.Converters;

public static class CustomConverter
{
    public static string GetEnumDescription(Enum value)
    {
        var result = string.Empty;

        if (value is not null)
        {
            if (Enum.IsDefined(value.GetType(), value))
            {
                var fieldInfo = value.GetType().GetField(value.ToString());
                var attributes = (DescriptionAttribute[])fieldInfo.GetCustomAttributes(typeof(DescriptionAttribute), false);

                if (attributes != null && attributes.Length > 0)
                    result = attributes[0].Description;
            }
        }

        return result;
    }

    public static T GetEnumValueFromDescription<T>(string description) where T : Enum
    {
        foreach (FieldInfo field in typeof(T).GetFields())
        {
            var attribute = field.GetCustomAttributes(typeof(DescriptionAttribute), false)
                                 .Cast<DescriptionAttribute>()
                                 .FirstOrDefault();

            if (attribute != null && attribute.Description == description)
                return (T)field.GetValue(null);
        }

        return default(T);
    }
}