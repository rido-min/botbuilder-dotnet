// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;

namespace Microsoft.Bot.Schema.Converters
{
    /// <summary>
    /// Converter which allows a MemoryStream instance to be used during JSON serialization/deserialization.
    /// </summary>
#pragma warning disable CA1812 // Avoid uninstantiated internal classes.
    internal class AttachmentMemoryStreamConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return typeof(MemoryStream).IsAssignableFrom(objectType);
        }

        /// <returns>
        ///     If the object is of type:<br/>
        ///     <list type="table">
        ///         <item>
        ///             <b>List/Array</b>
        ///             <list type="bullet">
        ///                 <item><i>Without MemoryStream</i>: it will return a JArray.</item>
        ///                 <item><i>With MemoryStream</i>: it will return a List.</item>
        ///             </list>            
        ///         </item>
        ///         <item>
        ///             <b>Dictionary/Object</b>
        ///             <list type="bullet">
        ///                 <item><i>Without MemoryStream</i>: it will return a JObject.</item>
        ///                 <item><i>With MemoryStream</i>: it will return a Dictionary.</item>
        ///             </list>            
        ///         </item>
        ///     </list>
        /// </returns>
        /// <inheritdoc/>
        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null)
            {
                return JValue.CreateNull();
            }

            if (reader.TokenType == JsonToken.StartArray)
            {
                var list = new List<object>();
                reader.Read();
                while (reader.TokenType != JsonToken.EndArray)
                {
                    var item = ReadJson(reader, objectType, existingValue, serializer);
                    list.Add(item);
                    reader.Read();
                }

                if (HasMemoryStream(list))
                {
                    return list;
                }
                else
                {
                    return JArray.FromObject(list);
                }
            }

            if (reader.TokenType == JsonToken.StartObject)
            {
                var deserialized = serializer.Deserialize<JToken>(reader);

                var isStream = deserialized.Type == JTokenType.Object && deserialized.Value<string>("$type") == nameof(MemoryStream);
                if (isStream)
                {
                    var stream = deserialized.ToObject<SerializedMemoryStream>();
                    return new MemoryStream(stream.Buffer.ToArray());
                }

                var newReader = deserialized.CreateReader();
                newReader.Read();
                string key = null;
                var dict = new Dictionary<string, object>();
                while (newReader.Read())
                {
                    if (newReader.TokenType == JsonToken.EndObject)
                    {
                        continue;
                    }

                    if (newReader.TokenType == JsonToken.PropertyName)
                    {
                        key = newReader.Value.ToString();
                        continue;
                    }

                    var item = ReadJson(newReader, objectType, existingValue, serializer);
                    dict.Add(key, item);
                }

                if (HasMemoryStream(dict))
                {
                    return dict;
                }
                else
                {
                    return JObject.FromObject(dict);
                }
            }

            return serializer.Deserialize(reader);
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            if (HasMemoryStream(value))
            {
                InternalWriteJson(writer, value, serializer, new HashSet<object>(ReferenceEqualityComparer.Instance));
                return;
            }

            JToken.FromObject(value, serializer).WriteTo(writer);
        }

        private static void InternalWriteJson(JsonWriter writer, object value, JsonSerializer serializer, HashSet<object> visited)
        {
            if (value == null || value is string)
            {
                // Avoid processing strings, since they implement IEnumerable.
                JToken.FromObject(value, serializer).WriteTo(writer);
                return;
            }

            if (value is MemoryStream)
            {
                var buffer = (value as MemoryStream).ToArray();
                var result = new SerializedMemoryStream
                {
                    Type = nameof(MemoryStream),
                    Buffer = buffer.ToList()
                };

                JToken.FromObject(result, serializer).WriteTo(writer);
                return;
            }

            // Skip value types as they cannot have circular references
            if (!value.GetType().IsValueType)
            {
                // Avoid infinite recursion by tracking visited objects
                if (!visited.Add(value))
                {
                    writer.WriteNull();
                    return;
                }
            }

            if (value is IDictionary dictionary)
            {
                writer.WriteStartObject();
                foreach (DictionaryEntry entry in dictionary)
                {
                    writer.WritePropertyName(entry.Key.ToString());
                    InternalWriteJson(writer, entry.Value, serializer, visited);
                }

                writer.WriteEndObject();
                return;
            }

            if (value is IEnumerable collection)
            {
                writer.WriteStartArray();
                foreach (var item in collection)
                {
                    InternalWriteJson(writer, item, serializer, visited);
                }

                writer.WriteEndArray();
                return;
            }

            var type = value.GetType();
            if (type.IsClass)
            {
                writer.WriteStartObject();
                foreach (var prop in type.GetProperties())
                {
                    writer.WritePropertyName(prop.Name);
                    InternalWriteJson(writer, prop.GetValue(value), serializer, visited);
                }

                writer.WriteEndObject();
                return;
            }

            JToken.FromObject(value, serializer).WriteTo(writer);
        }

        /// <summary>
        /// Check if an object contains at least one MemoryStream.
        /// </summary>
        /// <param name="value">Object contaning values that might have a MemoryStream instance.</param>
        /// <returns>True if there is at least one MemoryStream in the list, otherwise false.</returns>
        private static bool HasMemoryStream(object value)
        {
            return HasMemoryStream(value, new HashSet<object>(ReferenceEqualityComparer.Instance));
        }

        private static bool HasMemoryStream(object value, HashSet<object> visited)
        {
            if (value == null || value is string)
            {
                // Avoid processing strings, since they implement IEnumerable.
                return false;
            }

            if (value is MemoryStream)
            {
                return true;
            }

            // Skip value types as they cannot have circular references
            if (value.GetType().IsValueType)
            {
                return false;
            }

            // Avoid infinite recursion by tracking visited objects
            if (!visited.Add(value))
            {
                return false;
            }

            if (value is IDictionary dictionary)
            {
                foreach (DictionaryEntry entry in dictionary)
                {
                    if (HasMemoryStream(entry.Value, visited))
                    {
                        return true;
                    }
                }

                return false;
            }

            if (value is IEnumerable collection)
            {
                foreach (var item in collection)
                {
                    if (HasMemoryStream(item, visited))
                    {
                        return true;
                    }
                }

                return false;
            }

            var type = value.GetType();
            if (type.IsClass)
            {
                foreach (var prop in type.GetProperties())
                {
                    var propValue = prop.GetValue(value);
                    if (HasMemoryStream(propValue, visited))
                    {
                        return true;
                    }
                }

                return false;
            }

            return false;
        }

        internal class SerializedMemoryStream
        {
            [JsonProperty("$type")]
            public string Type { get; set; }

            [JsonProperty("buffer")]
            public List<byte> Buffer { get; set; }
        }

        private sealed class ReferenceEqualityComparer : IEqualityComparer<object>
        {
            private ReferenceEqualityComparer()
            {
            }

            public static ReferenceEqualityComparer Instance { get; } = new ReferenceEqualityComparer();

            public new bool Equals(object x, object y) => ReferenceEquals(x, y);

            public int GetHashCode(object obj) => System.Runtime.CompilerServices.RuntimeHelpers.GetHashCode(obj);
        }
    }
#pragma warning restore CA1812
}
