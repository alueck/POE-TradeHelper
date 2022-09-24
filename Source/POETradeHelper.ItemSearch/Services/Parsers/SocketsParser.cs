using POETradeHelper.ItemSearch.Contract.Models;
using POETradeHelper.ItemSearch.Contract.Services.Parsers;

namespace POETradeHelper.ItemSearch.Services.Parsers
{
    public class SocketsParser : ISocketsParser
    {
        private static readonly string[] socketGroupSeparators = new[] { " " };
        private static readonly string[] socketLinkIndicators = new[] { "-" };

        public ItemSockets Parse(string socketsString)
        {
            var itemSockets = new ItemSockets();

            if (!string.IsNullOrWhiteSpace(socketsString))
            {
                AddSocketGroups(itemSockets, socketsString);
            }

            return itemSockets;
        }

        private static void AddSocketGroups(ItemSockets itemSockets, string socketsString)
        {
            socketsString.Split(socketGroupSeparators, StringSplitOptions.RemoveEmptyEntries)
                            .Select(GetSocketGroup)
                            .ToList()
                            .ForEach(itemSockets.SocketGroups.Add);
        }

        private static SocketGroup GetSocketGroup(string socketGroupString)
        {
            var socketGroup = new SocketGroup();

            AddSockets(socketGroup, socketGroupString);

            return socketGroup;
        }

        private static void AddSockets(SocketGroup socketGroup, string socketGroupString)
        {
            socketGroupString.Split(socketLinkIndicators, StringSplitOptions.RemoveEmptyEntries)
                            .Select(GetSocket)
                            .ToList()
                            .ForEach(socketGroup.Sockets.Add);
        }

        private static Socket GetSocket(string socketString)
        {
            return new Socket
            {
                SocketType = GetSocketType(socketString)
            };
        }

        private static SocketType GetSocketType(string socketString)
        {
            switch (socketString)
            {
                case "R":
                    return SocketType.Red;

                case "B":
                    return SocketType.Blue;

                case "G":
                    return SocketType.Green;

                case "W":
                    return SocketType.White;

                case "A":
                    return SocketType.Abyssal;

                default:
                    throw new ArgumentException($"Unknown socket type '{socketString}'.", nameof(socketString));
            }
        }
    }
}