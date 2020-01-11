using System.Collections.Generic;

namespace POETradeHelper.ItemSearch.Contract.Models
{
    public class SocketGroup
    {
        public ICollection<Socket> Sockets { get; set; }

        public int Links => Sockets?.Count ?? 0;
    }
}