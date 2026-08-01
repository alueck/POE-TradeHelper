using System.Collections.Generic;

namespace POETradeHelper.ItemSearch.Contract.Models
{
    public class SocketGroup
    {
        public ICollection<Socket> Sockets { get; } = [];

        public int Links => this.Sockets.Count == 1 ? 0 : this.Sockets.Count;
    }
}