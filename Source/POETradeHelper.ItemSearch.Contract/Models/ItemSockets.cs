using System.Collections.Generic;
using System.Linq;

namespace POETradeHelper.ItemSearch.Contract.Models
{
    public class ItemSockets
    {
        public IEnumerable<SocketGroup> SocketGroups { get; set; } = Enumerable.Empty<SocketGroup>();

        public int Count => SocketGroups?.Sum(s => s.Links) ?? 0;
    }
}