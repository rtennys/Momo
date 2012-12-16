using System;

namespace Momo.Common
{
    public static class EnumHelper
    {
        public static string[] GetNames<TEnum>() where TEnum : struct
        {
            return Enum.GetNames(typeof(TEnum));
        }

        public static TEnum[] GetValues<TEnum>() where TEnum : struct
        {
            return Enum.GetValues(typeof(TEnum)).As<TEnum[]>();
        }

        public static TEnum Parse<TEnum>(string value) where TEnum : struct
        {
            return (TEnum)Enum.Parse(typeof(TEnum), value, true);
        }

        /// <summary>If value is one of the enum values, it is returned un-modified. Otherwise an empty string is returned.</summary>
        public static string Validate<TEnum>(string value) where TEnum : struct
        {
            return Validate<TEnum>(value, "");
        }

        /// <summary>If value is one of the enum values, it is returned un-modified. Otherwise defaultResult is returned.</summary>
        public static string Validate<TEnum>(string value, string defaultResult) where TEnum : struct
        {
            TEnum result;
            return Enum.TryParse(value, true, out result) ? value : defaultResult;
        }

        public static bool IsDefined<TEnum>(string value) where TEnum : struct
        {
            TEnum result;
            return Enum.TryParse(value, true, out result);
        }
    }
}