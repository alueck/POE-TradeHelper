using Microsoft.Extensions.Options;
using System;
using System.IO;
using System.Text;
using System.Text.Json;

namespace POETradeHelper.Common
{
    public class WritableOptions<TOptions> : IWritableOptions<TOptions> where TOptions : class, new()
    {
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

        public TOptions Get(string name) => this.options.Get(name);

        public void Update(Action<TOptions> update)
        {
            var jsonDocument = JsonDocument.Parse(File.ReadAllText(this.filePath));
            var section = jsonDocument.RootElement.TryGetProperty(this.sectionName, out JsonElement sectionElement)
                ? JsonSerializer.Deserialize<TOptions>(sectionElement.ToString())
                : (this.Value ?? new TOptions());

            update(section);

            string json = BuildNewConfigurationJson(jsonDocument, section);

            File.WriteAllText(this.filePath, FormatJson(json));
        }

        private string BuildNewConfigurationJson(JsonDocument currentConfiguration, TOptions updatedSection)
        {
            StringBuilder jsonBuilder = new StringBuilder();
            jsonBuilder.AppendLine("{");

            foreach (var element in currentConfiguration.RootElement.EnumerateObject())
            {
                string elementJson = element.Name == this.sectionName
                   ? JsonSerializer.Serialize(updatedSection)
                   : element.ToString();

                jsonBuilder
                    .AppendLine($"\"{element.Name}\":")
                    .AppendLine(elementJson);
            }
            jsonBuilder.AppendLine("}");

            return jsonBuilder.ToString();
        }

        private static string FormatJson(string json)
        {
            return JsonSerializer.Serialize(JsonDocument.Parse(json).RootElement, new JsonSerializerOptions { WriteIndented = true });
        }
    }
}