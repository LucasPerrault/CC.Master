using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Tools
{
    public static class Serializer
    {
        public static T Deserialize<T>(string content)
        {
            if (string.IsNullOrEmpty(content))
            {
                return default;
            }

            return JsonSerializer.Deserialize<T>(content, Options.ForDeserialization());
        }

        public static Task<T> DeserializeAsync<T>(Stream content)
        {
            return DeserializeAsync<T>(content, Options.ForDeserialization());
        }

        public static async Task<T> DeserializeAsync<T>(Stream content, JsonSerializerOptions options)
        {
            if (!content.CanRead)
            {
                return default;
            }

            options.PropertyNameCaseInsensitive = true;
            return await JsonSerializer.DeserializeAsync<T>(content, options);
        }

        public static string Serialize(object o)
        {
            return JsonSerializer.Serialize(o, Options.ForSerialization());
        }

        internal static string Serialize(object o, JsonSerializerOptions options) => JsonSerializer.Serialize(o, options);
        internal static T Deserialize<T>(string content, JsonSerializerOptions options) => JsonSerializer.Deserialize<T>(content, options);

        internal static class Options
        {
            public static JsonSerializerOptions ForSerialization() => ForSerialization(new List<JsonConverter>());

            public static JsonSerializerOptions ForSerialization(IEnumerable<JsonConverter> converters)
            {
                var options = new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                    Converters = { new JsonStringEnumConverter() }
                };
                foreach (var converter in converters)
                {
                    options.Converters.Add(converter);
                }
                return options;
            }

            public static JsonSerializerOptions ForDeserialization() => ForDeserialization(new List<JsonConverter>());

            public static JsonSerializerOptions ForDeserialization(IEnumerable<JsonConverter> converters)
            {
                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true,
                    Converters = { new JsonStringEnumConverter(),  }
                };

                foreach (var converter in converters)
                {
                    options.Converters.Add(converter);
                }
                return options;
            }
        }
    }

    public interface IPolymorphicSerializerBuilder
    {
        IPolymorphicSerializerBuilder<TPoly, TDiscr> WithPolymorphism<TPoly, TDiscr>(string nameOfProperty)
            where TDiscr : Enum;

        IPolymorphicSerializer Build();
    }

    public interface IPolymorphicSerializerBuilder<in TPolymorphic, in TDiscriminator> : IPolymorphicSerializerBuilder
    {
        public IPolymorphicSerializerBuilder<TPolymorphic, TDiscriminator> AddMatch<T>(TDiscriminator discriminator) where T : TPolymorphic;
    }

    public class EmptyPolymorphicSerializerBuilder : IPolymorphicSerializerBuilder
    {
        public IPolymorphicSerializer Build()
        {
            throw new NotImplementedException();
        }

        public IPolymorphicSerializerBuilder<TPoly, TDiscr> WithPolymorphism<TPoly, TDiscr>(string nameOfProperty)
            where TDiscr : Enum
        {
            return new PolymorphicSerializerBuilder<TPoly, TDiscr>(nameOfProperty);
        }
    }

    internal class PolymorphicSerializerBuilder<TPolymorphic, TDiscriminator> : IPolymorphicSerializerBuilder<TPolymorphic, TDiscriminator>
        where TDiscriminator : Enum
    {
        private readonly List<JsonConverter> _converters;
        private readonly PolymorphicConverter<TPolymorphic, TDiscriminator> _currentConverter;

        public PolymorphicSerializerBuilder(string nameOfProperty) : this(new List<JsonConverter>(), nameOfProperty)
        { }
        private PolymorphicSerializerBuilder(List<JsonConverter> converters, string nameOfProperty)
        {
            _converters = converters;
            _currentConverter = new PolymorphicConverter<TPolymorphic, TDiscriminator>(nameOfProperty);
            _converters.Add(_currentConverter);
        }

        public IPolymorphicSerializerBuilder<TPolymorphic, TDiscriminator> AddMatch<T>(TDiscriminator discriminator)
            where T: TPolymorphic
        {
            _currentConverter.AddMatch(discriminator, typeof(T));
            return this;
        }

        public IPolymorphicSerializerBuilder<T, TKey> WithPolymorphism<T, TKey>(string nameOfProperty)
            where TKey : Enum
        {
            return new PolymorphicSerializerBuilder<T, TKey>(_converters, nameOfProperty);
        }

        public IPolymorphicSerializer Build()
        {
            return new PolymorphicSerializer(_converters);
        }
    }

    public interface IPolymorphicSerializer
    {
        T Deserialize<T>(string content);
        Task<T> DeserializeAsync<T>(Stream content);
        string Serialize(object o);
        IReadOnlyCollection<JsonConverter> GetConverters();
    }

    internal class PolymorphicSerializer : IPolymorphicSerializer
    {
        private readonly List<JsonConverter> _converters;

        public PolymorphicSerializer(List<JsonConverter> converters)
        {
            _converters = converters;
        }

        public T Deserialize<T>(string content)
        {
            return Serializer.Deserialize<T>(content, Serializer.Options.ForDeserialization(_converters));
        }

        public Task<T> DeserializeAsync<T>(Stream content)
        {
            return Serializer.DeserializeAsync<T>(content, Serializer.Options.ForDeserialization(_converters));
        }

        public string Serialize(object o)
        {
            return Serializer.Serialize(o, Serializer.Options.ForSerialization(_converters));
        }

        public IReadOnlyCollection<JsonConverter> GetConverters() => _converters;
    }

    public class PolymorphicConverter<T, TKey> : JsonConverter<T> where TKey : Enum
    {
        private readonly Dictionary<string, Type> _typesByName = new Dictionary<string, Type>(StringComparer.OrdinalIgnoreCase);
        private readonly Dictionary<int, Type> _typesByIntValue = new Dictionary<int, Type>();
        private readonly string _discriminatorPropertyName;

        public PolymorphicConverter(string discriminatorPropertyName)
        {
            _discriminatorPropertyName = discriminatorPropertyName;
        }

        public void AddMatch(TKey discriminator, Type type)
        {
            var name = Enum.GetName(typeof(TKey), discriminator);
            _typesByName[name] = type;

            var intValue = Convert.ToInt32(discriminator);
            _typesByIntValue[intValue] = type;
        }

        public override T Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType != JsonTokenType.StartObject)
            {
                throw new JsonException();
            }

            using var jsonDocument = JsonDocument.ParseValue(ref reader);

            if (!SerializerHelper.TryFind(jsonDocument, _discriminatorPropertyName, out var typeProperty))
            {
                throw new JsonException();
            }

            var type = GetDeserializationType(typeProperty);
            if (type == null)
            {
                throw new JsonException();
            }

            var jsonObject = jsonDocument.RootElement.GetRawText();
            var result = (T) JsonSerializer.Deserialize(jsonObject, type, options);

            return result;
        }

        public override void Write(Utf8JsonWriter writer, T value, JsonSerializerOptions options)
        {
            JsonSerializer.Serialize(writer, (object)value, options);
        }

        private Type GetDeserializationType(JsonElement typeProperty)
        {
            Type type = null;

            switch (typeProperty.ValueKind)
            {
                case JsonValueKind.Number:
                    _typesByIntValue.TryGetValue(typeProperty.GetInt32(), out type);
                    break;
                case JsonValueKind.String:
                    _typesByName.TryGetValue(typeProperty.GetString(), out type);
                    break;
            }

            return type;
        }
    }

    public static class SerializerHelper
    {
        public static bool TryFind(JsonDocument jsonDocument, string propertyName, out JsonElement jsonElement)
        {
            var typeProperty = jsonDocument.RootElement.EnumerateObject()
                .FirstOrDefault(p => string.Equals(p.Name, propertyName, StringComparison.OrdinalIgnoreCase));

            jsonElement = typeProperty.Value;
            return jsonElement.ValueKind != JsonValueKind.Undefined;
        }
    }

}
