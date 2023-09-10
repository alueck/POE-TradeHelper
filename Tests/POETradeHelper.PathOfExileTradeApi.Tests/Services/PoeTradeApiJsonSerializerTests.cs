using NUnit.Framework;

using POETradeHelper.Common;
using POETradeHelper.Common.Wrappers;
using POETradeHelper.PathOfExileTradeApi.Services;

using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;

using NSubstitute;

namespace POETradeHelper.PathOfExileTradeApi.Tests.Services
{
    public class PoeTradeApiJsonSerializerTests
    {
        private IJsonSerializerWrapper jsonSerializerWrapperMock;
        private PoeTradeApiJsonSerializer poeTradeApiJsonSerializer;

        [SetUp]
        public void Setup()
        {
            this.jsonSerializerWrapperMock = Substitute.For<IJsonSerializerWrapper>();
            this.poeTradeApiJsonSerializer = new PoeTradeApiJsonSerializer(this.jsonSerializerWrapperMock);
        }

        [Test]
        public void DeserializeShouldCallDeserializeOnJsonSerializerWithCamelCaseNamingPolicy()
        {
            const string json = "{ \"request\": { } }";

            this.poeTradeApiJsonSerializer.Deserialize<object>(json);

            this.jsonSerializerWrapperMock
                .Received()
                .Deserialize<object>(json, Arg.Is<JsonSerializerOptions>(o => o.PropertyNamingPolicy == JsonNamingPolicy.CamelCase));
        }

        [Test]
        public void SerializeShouldCallSerializeOnJsonSerializerWithSnakeCaseNamingPolicy()
        {
            IEnumerable<string> obj = new List<string>();

            this.poeTradeApiJsonSerializer.Serialize(obj);

            this.jsonSerializerWrapperMock
                .Received()
                .Serialize(obj, Arg.Is<JsonSerializerOptions>(o => o.PropertyNamingPolicy is JsonSnakeCaseNamingPolicy));
        }

        [Test]
        public void SerializeShouldCallSerializeOnJsonSerializerWithJsonStringEnumConverter()
        {
            IEnumerable<string> obj = new List<string>();

            this.poeTradeApiJsonSerializer.Serialize(obj);

            this.jsonSerializerWrapperMock
                .Received()
                .Serialize(obj, Arg.Is<JsonSerializerOptions>(o => o.Converters.OfType<JsonStringEnumConverter>().Any()));
        }

        [Test]
        public void SerializeShouldCallSerializeOnJsonSerializerWithIgnoreNullValues()
        {
            IEnumerable<string> obj = new List<string>();

            this.poeTradeApiJsonSerializer.Serialize(obj);

            this.jsonSerializerWrapperMock
                .Received()
                .Serialize(obj, Arg.Is<JsonSerializerOptions>(o => o.DefaultIgnoreCondition == JsonIgnoreCondition.WhenWritingNull));
        }
    }
}