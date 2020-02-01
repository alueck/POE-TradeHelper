using Moq;
using NUnit.Framework;
using POETradeHelper.Common;
using POETradeHelper.Common.Wrappers;
using POETradeHelper.PathOfExileTradeApi.Services;
using System.Collections.Generic;
using System.Text.Json;

namespace POETradeHelper.PathOfExileTradeApiTests
{
    public class PoeTradeApiJsonSerializerTests
    {
        private Mock<IJsonSerializerWrapper> jsonSerializerWrapperMock;
        private PoeTradeApiJsonSerializer poeTradeApiJsonSerializer;

        [SetUp]
        public void Setup()
        {
            this.jsonSerializerWrapperMock = new Mock<IJsonSerializerWrapper>();
            this.poeTradeApiJsonSerializer = new PoeTradeApiJsonSerializer(this.jsonSerializerWrapperMock.Object);
        }

        [Test]
        public void DeserializeShouldCallDeserializeOnJsonSerializerWithCamelCaseNamingPolicy()
        {
            string json = "{ \"request\": { } }";

            this.poeTradeApiJsonSerializer.Deserialize<object>(json);

            this.jsonSerializerWrapperMock.Verify(x => x.Deserialize<object>(json, It.Is<JsonSerializerOptions>(o => o.PropertyNamingPolicy == JsonNamingPolicy.CamelCase)));
        }

        [Test]
        public void SerializeShouldCallSerializeOnJsonSerializerWithSnakeCaseNamingPolicy()
        {
            IEnumerable<string> obj = new List<string>();

            this.poeTradeApiJsonSerializer.Serialize(obj);

            this.jsonSerializerWrapperMock.Verify(x => x.Serialize(obj, It.Is<JsonSerializerOptions>(o => o.PropertyNamingPolicy is JsonSnakeCaseNamingPolicy)));
        }
    }
}