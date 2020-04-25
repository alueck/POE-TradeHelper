using System.Collections.Generic;
using System.Linq;

namespace POETradeHelper.ItemSearch.Contract.Models
{
    public class ItemSockets
    {
        public ICollection<SocketGroup> SocketGroups { get; } = new List<SocketGroup>();

        public int Count => SocketGroups.Sum(s => s.Sockets.Count);
    }
}