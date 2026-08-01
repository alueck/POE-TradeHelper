using System.Text;

namespace POETradeHelper.ItemSearch.Tests.TestHelpers.ItemStringBuilders.Models;

public class NameAndRarityWithTypeGroup : NameAndRarityGroupBase
{
    public string Type { get; set; } = "TestType";

    public override string ToString()
    {
        StringBuilder sb = new(base.ToString());
        sb.AppendLineIfNotEmpty(this.Type);

        return sb.ToString();
    }
}