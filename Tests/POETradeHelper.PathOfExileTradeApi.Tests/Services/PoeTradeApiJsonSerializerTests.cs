using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;

using NSubstitute;

using NUnit.Framework;

using POETradeHelper.Common.Wrappers;
using POETradeHelper.PathOfExileTradeApi.Services;

namespace POETradeHelper.PathOfExileTradeApi.Tests.Services
{
    public class PoeTradeApiJsonSerializerTests
    {
        private readonly IJsonSerializerWrapper jsonSerializerWrapperMock;
        private readonly PoeTradeApiJsonSerializer poeTradeApiJsonSerializer;

        public PoeTradeApiJsonSerializerTests()
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
                .Deserialize<object>(
                    json,
                    Arg.Is<JsonSerializerOptions>(o => o.PropertyNamingPolicy == JsonNamingPolicy.CamelCase));
        }

        [Test]
        public void SerializeShouldCallSerializeOnJsonSerializerWithSnakeCaseNamingPolicy()
        {
            IEnumerable<string> obj = [];

            this.poeTradeApiJsonSerializer.Serialize(obj);

            this.jsonSerializerWrapperMock
                .Received()
                .Serialize(obj, Arg.Is<JsonSerializerOptions>(o => o.PropertyNamingPolicy == JsonNamingPolicy.SnakeCaseLower));
        }

        [Test]
        public void SerializeShouldCallSerializeOnJsonSerializerWithJsonStringEnumConverter()
        {
            IEnumerable<string> obj = [];

            this.poeTradeApiJsonSerializer.Serialize(obj);

            this.jsonSerializerWrapperMock
                .Received()
                .Serialize(obj, Arg.Is<JsonSerializerOptions>(o => o.Converters.OfType<JsonStringEnumConverter>().Any()));
        }

        [Test]
        public void SerializeShouldCallSerializeOnJsonSerializerWithIgnoreNullValues()
        {
            IEnumerable<string> obj = [];

            this.poeTradeApiJsonSerializer.Serialize(obj);

            this.jsonSerializerWrapperMock
                .Received()
                .Serialize(
                    obj,
                    Arg.Is<JsonSerializerOptions>(o => o.DefaultIgnoreCondition == JsonIgnoreCondition.WhenWritingNull));
        }
    }
}