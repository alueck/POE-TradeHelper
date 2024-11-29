using System.Collections.Generic;
using System.Linq;

namespace POETradeHelper.ItemSearch.Contract.Models
{
    public class ItemSockets
    {
        public ICollection<SocketGroup> SocketGroups { get; } = [];

        public int Count => this.SocketGroups.Sum(s => s.Sockets.Count);
    }
}