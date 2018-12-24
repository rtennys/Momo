using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Dynamic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization.Formatters.Binary;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Xml.Serialization;

namespace Momo
{
    public static class ExtensionMethods
    {
        public static T NullCheck<T>(this T source, T defaultValue) where T : class
        {
            if (source == null)
                return defaultValue;

            var str = source as string;
            if (str != null && str == "")
                return defaultValue;

            return source;
        }


        public static IEnumerable<T> NullCheck<T>(this IEnumerable<T> source)
        {
            return source ?? Enumerable.Empty<T>();
        }

        public static IEnumerable<T> NonEmpties<T>(this IEnumerable<T> source) where T : class
        {
            return source
                .NullCheck()
                .Where(x => x != null)
                .Where(x => !(x is string) || !string.IsNullOrEmpty(x as string));
        }

        public static ReadOnlyCollection<T> AsReadOnly<T>(this IEnumerable<T> source)
        {
            if (source is ReadOnlyCollection<T> readOnly)
                return readOnly;

            var list = source as IList<T> ?? source.NullCheck().ToArray();

            return new ReadOnlyCollection<T>(list);
        }


        public static int IndexOf<T>(this IEnumerable<T> source, T value)
        {
            if (source is IList<T> list)
                return list.IndexOf(value);

            return source.IndexOf(value, EqualityComparer<T>.Default);
        }

        public static int IndexOf<T>(this IEnumerable<T> source, T value, IEqualityComparer<T> comparer)
        {
            var i = 0;
            foreach (var x in source)
            {
                if (comparer.Equals(x, value))
                    return i;
                i++;
            }

            return -1;
        }

        public static string Join<T>(this IEnumerable<T> source, string separator, Func<T, string> converter)
        {
            return source.NullCheck().Select(converter).Join(separator);
        }

        public static string Join(this IEnumerable<string> source, string separator)
        {
            return string.Join(separator, source.NullCheck());
        }

        public static string JoinNonEmpties(this IEnumerable<string> source, string separator)
        {
            return source
                .NullCheck()
                .Where(x => x != null)
                .Where(x => !string.IsNullOrEmpty(x.Trim()))
                .Join(separator);
        }


        public static int ToInt(this string source)
        {
            return string.IsNullOrEmpty(source) ? 0 : Convert.ToInt32(source);
        }

        public static uint ToUInt(this string source)
        {
            return string.IsNullOrEmpty(source) ? 0U : Convert.ToUInt32(source);
        }

        public static long ToLong(this string source)
        {
            return string.IsNullOrEmpty(source) ? 0L : Convert.ToInt64(source);
        }

        public static ulong ToULong(this string source)
        {
            return string.IsNullOrEmpty(source) ? 0UL : Convert.ToUInt64(source);
        }

        public static decimal ToDecimal(this string source)
        {
            return string.IsNullOrEmpty(source) ? 0M : decimal.Parse(source, NumberStyles.Any);
        }

        public static double ToDouble(this string source)
        {
            return string.IsNullOrEmpty(source) ? 0D : Convert.ToDouble(source);
        }

        public static DateTime ToDate(this string source)
        {
            return string.IsNullOrEmpty(source) ? DateTime.MinValue : DateTime.Parse(source);
        }

        public static Uri ToUri(this string source)
        {
            return string.IsNullOrEmpty(source) ? null : new Uri(source);
        }

        public static string ToTitleCase(this string source)
        {
            return Thread.CurrentThread.CurrentCulture.TextInfo.ToTitleCase(source.Replace('-', ' '));
        }

        public static dynamic ToDynamic(this object source)
        {
            if (source == null || source is ExpandoObject || source is string || source.GetType().IsPrimitive)
                return source;

            if (source is IList list)
                return list.Cast<object>().Select(x => x.ToDynamic()).AsReadOnly();

            IDictionary<string, object> expando = new ExpandoObject();

            if (source is IDictionary dictionary)
                foreach (DictionaryEntry entry in dictionary)
                    expando.Add(entry.Key.ToString(), entry.Value.ToDynamic());
            else
                foreach (PropertyDescriptor descriptor in TypeDescriptor.GetProperties(source))
                    expando.Add(descriptor.Name, descriptor.GetValue(source).ToDynamic());

            return expando;
        }


        public static TimeSpan Seconds(this int seconds)
        {
            return new TimeSpan(0, 0, seconds);
        }

        public static TimeSpan Minutes(this int minutes)
        {
            return new TimeSpan(0, minutes, 0);
        }

        public static TimeSpan Hours(this int hours)
        {
            return new TimeSpan(hours, 0, 0);
        }


        public static byte[] Serialize(this object source)
        {
            using (var memoryStream = new MemoryStream())
            {
                var formatter = new BinaryFormatter();
                formatter.Serialize(memoryStream, source);
                return memoryStream.ToArray();
            }
        }

        public static byte[] SerializeXml<T>(this T source)
        {
            using (var memoryStream = new MemoryStream())
            {
                new XmlSerializer(typeof(T)).Serialize(memoryStream, source);
                return memoryStream.ToArray();
            }
        }

        public static T Deserialize<T>(this byte[] source)
        {
            using (var memoryStream = new MemoryStream(source))
            {
                var formatter = new BinaryFormatter();
                return (T)formatter.Deserialize(memoryStream);
            }
        }

        public static T DeserializeXml<T>(this byte[] source)
        {
            using (var memoryStream = new MemoryStream(source))
                return (T)new XmlSerializer(typeof(T)).Deserialize(memoryStream);
        }


        public static bool Between(this DateTime date, DateTime left, DateTime right)
        {
            return left <= date && date <= right;
        }

        public static bool Between(this decimal value, decimal left, decimal right)
        {
            return left <= value && value <= right;
        }


        public static string Hash(this string source)
        {
            using (HashAlgorithm sha = new SHA256Managed())
            {
                var data = Encoding.UTF8.GetBytes(source);
                sha.TransformFinalBlock(data, 0, data.Length);
                return Convert.ToBase64String(sha.Hash);
            }
        }

        public static string ReplaceRegex(this string source, string pattern, string replacement)
        {
            return Regex.Replace(source, pattern, replacement);
        }

        public static TValue TryGet<TKey, TValue>(this IDictionary<TKey, TValue> source, TKey key)
        {
            return source.TryGetValue(key, out var value) ? value : default(TValue);
        }

        public static byte[] ReadAll(this Stream stream)
        {
            var buffer = new byte[16 * 1024];
            using (var ms = new MemoryStream())
            {
                int read;
                while ((read = stream.Read(buffer, 0, buffer.Length)) > 0)
                    ms.Write(buffer, 0, read);
                return ms.ToArray();
            }
        }

        public static string GetDisplayName(this MemberInfo member)
        {
            var displayAttr = member.GetCustomAttributes(false).OfType<DisplayAttribute>().FirstOrDefault();
            return displayAttr != null ? displayAttr.GetName() : member.Name;
        }

        public static string GetDisplayName(this Enum value)
        {
            return value.GetType().GetMember(value.ToString())[0].GetDisplayName();
        }
    }
}
