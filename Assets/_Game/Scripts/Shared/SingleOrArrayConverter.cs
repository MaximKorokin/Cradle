using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace Assets._Game.Scripts.Shared
{
    public class SingleOrArrayJsonConverter<T> : JsonConverter
    {
        public override bool CanWrite => false;

        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(List<T>);
        }

        public override object ReadJson(
            JsonReader reader,
            Type objectType,
            object existingValue,
            JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null)
                return null;

            if (reader.TokenType == JsonToken.StartArray)
                return serializer.Deserialize<T[]>(reader);

            var item = serializer.Deserialize<T>(reader);

            return item == null
                ? new T[0]
                : new T[] { item };
        }

        public override void WriteJson(
            JsonWriter writer,
            object value,
            JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }
    }
}
