﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;

using Microsoft.Extensions.Options;

namespace POETradeHelper.Common.WritableOptions
{
    public class WritableOptions<TOptions> : IWritableOptions<TOptions>
        where TOptions : class, new()
    {
        private static readonly JsonSerializerOptions ReadJsonSerializerOptions = new()
        {
            Converters = { new JsonStringEnumConverter() },
            TypeInfoResolver = new DefaultJsonTypeInfoResolver(),
        };

        private static readonly JsonSerializerOptions FormatJsonSerializerOptions = new()
        {
            WriteIndented = true,
            TypeInfoResolver = new DefaultJsonTypeInfoResolver(),
        };

        private readonly IOptionsMonitor<TOptions> options;
        private readonly string sectionName;
        private readonly string filePath;

        public WritableOptions(IOptionsMonitor<TOptions> options, string sectionName, string filePath)
        {
            this.filePath = filePath;
            this.sectionName = sectionName;
            this.options = options;
        }

        public TOptions Value => this.options.CurrentValue;

        public TOptions Get(string? name) => this.options.Get(name);

        public void Update(Action<TOptions> update)
        {
            var jsonDocument = JsonDocument.Parse(File.ReadAllText(this.filePath));
            var section = jsonDocument.RootElement.TryGetProperty(this.sectionName, out JsonElement sectionElement)
                ? JsonSerializer.Deserialize<TOptions>(sectionElement.ToString(), ReadJsonSerializerOptions) ?? new TOptions()
                : this.Value;

            update(section);

            string json = this.BuildNewConfigurationJson(jsonDocument, section);

            File.WriteAllText(this.filePath, FormatJson(json));
        }

        private string BuildNewConfigurationJson(JsonDocument currentConfiguration, TOptions updatedSection)
        {
            var foundSection = false;
            var sectionsJson = new List<string>();
            var jsonBuilder = new StringBuilder();

            foreach (var element in currentConfiguration.RootElement.EnumerateObject())
            {
                bool isCurrentSection = element.Name == this.sectionName;
                foundSection |= isCurrentSection;

                string elementJson = isCurrentSection
                    ? GetUpdatedJson(updatedSection)
                    : GetCurrentJson(element);

                sectionsJson.Add(GetSectionJson(element.Name, elementJson));
            }

            if (!foundSection)
            {
                sectionsJson.Add(GetSectionJson(this.sectionName, GetUpdatedJson(updatedSection)));
            }

            return jsonBuilder
                .AppendLine("{")
                .AppendJoin($",{Environment.NewLine}", sectionsJson)
                .AppendLine("}")
                .ToString();
        }

        private static string GetUpdatedJson(TOptions updatedSection)
        {
            return JsonSerializer.Serialize(updatedSection, ReadJsonSerializerOptions);
        }

        private static string GetCurrentJson(JsonProperty element)
        {
            return element.Value.ToString();
        }

        private static string GetSectionJson(string elementName, string elementJson)
        {
            return $"\"{elementName}\":{elementJson}";
        }

        private static string FormatJson(string json)
        {
            return JsonSerializer.Serialize(JsonDocument.Parse(json).RootElement, FormatJsonSerializerOptions);
        }
    }
}