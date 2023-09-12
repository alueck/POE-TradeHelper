using POETradeHelper.ItemSearch.Contract.Models;
using POETradeHelper.ItemSearch.Contract.Services.Parsers;

namespace POETradeHelper.ItemSearch.Services.Parsers
{
    public class SocketsParser : ISocketsParser
    {
        private static readonly string[] SocketGroupSeparators = { " " };
        private static readonly string[] SocketLinkIndicators = { "-" };

        public ItemSockets Parse(string? socketsString)
        {
            ItemSockets itemSockets = new();

            if (!string.IsNullOrWhiteSpace(socketsString))
            {
                AddSocketGroups(itemSockets, socketsString);
            }

            return itemSockets;
        }

        private static void AddSocketGroups(ItemSockets itemSockets, string socketsString) =>
            socketsString.Split(SocketGroupSeparators, StringSplitOptions.RemoveEmptyEntries)
                .Select(GetSocketGroup)
                .ToList()
                .ForEach(itemSockets.SocketGroups.Add);

        private static SocketGroup GetSocketGroup(string socketGroupString)
        {
            SocketGroup socketGroup = new();

            AddSockets(socketGroup, socketGroupString);

            return socketGroup;
        }

        private static void AddSockets(SocketGroup socketGroup, string socketGroupString) =>
            socketGroupString.Split(SocketLinkIndicators, StringSplitOptions.RemoveEmptyEntries)
                .Select(GetSocket)
                .ToList()
                .ForEach(socketGroup.Sockets.Add);

        private static Socket GetSocket(string socketString) =>
            new()
            {
                SocketType = GetSocketType(socketString),
            };

        private static SocketType GetSocketType(string socketString) =>
            socketString switch
            {
                "R" => SocketType.Red,
                "B" => SocketType.Blue,
                "G" => SocketType.Green,
                "W" => SocketType.White,
                "A" => SocketType.Abyssal,
                _ => throw new ArgumentException($"Unknown socket type '{socketString}'.", nameof(socketString)),
            };
    }
}