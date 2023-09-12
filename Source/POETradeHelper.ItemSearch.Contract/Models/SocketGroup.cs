using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace POETradeHelper.ItemSearch.Contract.Models
{
    public class SocketGroup
    {
        public ICollection<Socket> Sockets { get; } = new Collection<Socket>();

        public int Links => this.Sockets.Count == 1 ? 0 : this.Sockets.Count;
    }
}