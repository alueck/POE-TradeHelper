using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

using POETradeHelper.PathOfExileTradeApi.Properties;

namespace POETradeHelper.PathOfExileTradeApi.Models
{
    internal sealed record StatData : IStatData
    {
        private const string MonsterStatMarker = " (×#)";
        private static readonly string LocalStatMarker = $" ({Resources.LocalKeyword})";
        private static readonly Regex TierRegex = new(@" \(Tier \d+\)", RegexOptions.Compiled);

        public string Id { get; set; } = string.Empty;

        public string Text
        {
            get;
            init
            {
                field = value;
                this.IsLocal = field.Contains(LocalStatMarker);
            }
        } = string.Empty;

        public string Type { get; set; } = string.Empty;

        public int Lines
        {
            get
            {
                if (field == 0)
                {
                    field = this.Text.Count(x => x == '\n') + 1;
                }

                return field;
            }
        }

        public bool IsLocal { get; private set; }

        public Regex Regex
        {
            get
            {
                if (field == null)
                {
                    string regexText = this.Text.Replace(LocalStatMarker, string.Empty).Replace(MonsterStatMarker, string.Empty);
                    regexText = TierRegex.Replace(regexText, string.Empty);
                    field = new Regex($@"^{Regex.Escape(regexText).Replace(@"\#", @"[\+\-]?\d+(?:\.\d+)?")}(?: \([^\)]+\))?$");
                }

                return field;
            }
        }

        public IList<StatData> Alternatives { get; } = new List<StatData>();
    }
}