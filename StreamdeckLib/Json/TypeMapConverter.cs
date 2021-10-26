using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace StreamDeckLib.Json
{
    public class TypeMapConverter<T> : JsonConverter<T> where T : class
    {
        public TypeMapConverter(Dictionary<string, Type> typeMap, string selector)
        {
            TypeMap = typeMap;
            Selector = selector;
        }

        protected string Selector { get; init; }
        protected Dictionary<string, Type> TypeMap { get; init; }

        public new bool CanConvert(Type objectType)
        {
            return typeof(T).IsAssignableFrom(objectType);
        }

        public override bool CanRead => true;

        public override bool CanWrite => false;

        public override void WriteJson(JsonWriter writer, T? value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }

        public override T? ReadJson(JsonReader reader, Type objectType, T? existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            var jObject = JObject.Load(reader);
            var key = jObject[Selector]?.Value<string>() ?? string.Empty;
            if (string.IsNullOrEmpty(key))
                return jObject.ToObject<T>();

            if (!TypeMap.TryGetValue(jObject[Selector]?.Value<string>() ?? string.Empty, out var target))
                return jObject.ToObject<T>();
            
            return (T?)jObject.ToObject(target);
        }
    }
}