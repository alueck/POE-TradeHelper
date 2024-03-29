﻿using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;

using POETradeHelper.Common.Wrappers;

namespace POETradeHelper.PathOfExileTradeApi.Services
{
    public class PoeTradeApiJsonSerializer : IPoeTradeApiJsonSerializer
    {
        private static readonly JsonSerializerOptions CamelCaseJsonSerializerOptions =
            new()
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                TypeInfoResolver = new DefaultJsonTypeInfoResolver(),
            };

        private static readonly JsonSerializerOptions SnakeCaseJsonSerializerOptions = new()
        {
            PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower,
            Converters = { new JsonStringEnumConverter(JsonNamingPolicy.SnakeCaseLower) },
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
            TypeInfoResolver = new DefaultJsonTypeInfoResolver(),
        };

        private readonly IJsonSerializerWrapper jsonSerializer;

        public PoeTradeApiJsonSerializer(IJsonSerializerWrapper jsonSerializer)
        {
            this.jsonSerializer = jsonSerializer;
        }

        public T? Deserialize<T>(string json) => this.jsonSerializer.Deserialize<T>(json, CamelCaseJsonSerializerOptions);

        public string Serialize(object value) => this.jsonSerializer.Serialize(value, SnakeCaseJsonSerializerOptions);
    }
}