using System;
using System.Collections;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Web.Script.Serialization;

namespace Momo.Common
{
    public static class JsonHelper
    {
        public static string SerializeJson(this object source)
        {
            return new JavaScriptSerializer().Serialize(source);
        }

        public static T DeserializeJson<T>(this string json)
        {
            var javaScriptSerializer = new JavaScriptSerializer();
            if (typeof(T) == typeof(object))
                javaScriptSerializer.RegisterConverters(new[] {new DynamicJsonConverter()});
            return javaScriptSerializer.Deserialize<T>(json);
        }

        private sealed class DynamicJsonConverter : JavaScriptConverter
        {
            public override IEnumerable<Type> SupportedTypes
            {
                get { return new[] {typeof(object)}; }
            }

            public override object Deserialize(IDictionary<string, object> dictionary, Type type, JavaScriptSerializer serializer)
            {
                return new DynamicJsonObject(dictionary);
            }

            public override IDictionary<string, object> Serialize(object obj, JavaScriptSerializer serializer)
            {
                throw new NotImplementedException();
            }

            private sealed class DynamicJsonObject : DynamicObject
            {
                public DynamicJsonObject(IDictionary<string, object> dictionary)
                {
                    _dictionary = dictionary ?? new Dictionary<string, object>();
                }

                private readonly IDictionary<string, object> _dictionary;

                public override bool TryGetMember(GetMemberBinder binder, out object result)
                {
                    if (!_dictionary.TryGetValue(binder.Name, out result))
                        return false;

                    var dictionary = result as IDictionary<string, object>;
                    if (dictionary != null)
                    {
                        result = new DynamicJsonObject(dictionary);
                        return true;
                    }

                    var list = result as IList;
                    if (list != null)
                    {
                        if (list.Count > 0 && list[0] is IDictionary<string, object>)
                            result = list.Cast<IDictionary<string, object>>().Select(x => new DynamicJsonObject(x)).ToArray();
                        else
                            result = list.Cast<object>().Select<object, object>(x => (dynamic)x).ToArray();
                    }

                    return true;
                }
            }
        }
    }
}