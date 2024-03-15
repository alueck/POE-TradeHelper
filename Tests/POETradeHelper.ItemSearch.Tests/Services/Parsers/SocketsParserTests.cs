using FluentAssertions;

using NUnit.Framework;

using POETradeHelper.ItemSearch.Contract.Models;
using POETradeHelper.ItemSearch.Services.Parsers;

namespace POETradeHelper.ItemSearch.Tests.Services.Parsers
{
    public class SocketsParserTests
    {
        private readonly SocketsParser socketsParser;

        public SocketsParserTests()
        {
            this.socketsParser = new SocketsParser();
        }

        [TestCase("R", SocketType.Red)]
        [TestCase("B", SocketType.Blue)]
        [TestCase("G", SocketType.Green)]
        [TestCase("W", SocketType.White)]
        [TestCase("A", SocketType.Abyssal)]
        public void ParseShouldParseSingleSocket(string socketsString, SocketType expected)
        {
            ItemSockets result = this.socketsParser.Parse(socketsString);

            result.Count.Should().Be(1);
            result.SocketGroups.Count.Should().Be(1);

            CheckSocketGroup(result.SocketGroups, 0, expected);
        }

        [TestCase("R B", SocketType.Red, SocketType.Blue)]
        [TestCase("W G", SocketType.White, SocketType.Green)]
        public void ParseShouldParseTwoSingleSockets(string socketsString, params SocketType[] expected)
        {
            ItemSockets result = this.socketsParser.Parse(socketsString);

            result.Count.Should().Be(2);
            result.SocketGroups.Count.Should().Be(2);

            CheckSocketGroup(result.SocketGroups, 0, expected[0]);
            CheckSocketGroup(result.SocketGroups, 1, expected[1]);
        }

        [Test]
        public void ParseShouldParseLinkedSockets()
        {
            ItemSockets result = this.socketsParser.Parse("R-B");

            result.Count.Should().Be(2);
            result.SocketGroups.Count.Should().Be(1);

            CheckSocketGroup(result.SocketGroups, 0, SocketType.Red, SocketType.Blue);
        }

        [Test]
        public void ParseShouldParseMultipleLinkedSocketGroups()
        {
            ItemSockets result = this.socketsParser.Parse("R-B W-G");

            result.Count.Should().Be(4);
            result.SocketGroups.Count.Should().Be(2);

            CheckSocketGroup(result.SocketGroups, 0, SocketType.Red, SocketType.Blue);
            CheckSocketGroup(result.SocketGroups, 1, SocketType.White, SocketType.Green);
        }

        [TestCase(null)]
        [TestCase("  ")]
        public void ParseShouldReturnEmptyItemSocketsIfStringIsNullOrWhitespace(string? socketsString)
        {
            ItemSockets result = this.socketsParser.Parse(socketsString);

            result.Count.Should().Be(0);
        }

        private static void CheckSocketGroup(ICollection<SocketGroup> socketGroups, int socketGroupIndex, params SocketType[] expectedSocketTypes)
        {
            ICollection<Socket> socketGroupSockets = socketGroups.Skip(socketGroupIndex).First().Sockets;
            socketGroupSockets.Count.Should().Be(expectedSocketTypes.Length);

            CheckSocketColorCount(SocketType.Red, socketGroupSockets, expectedSocketTypes);
            CheckSocketColorCount(SocketType.Blue, socketGroupSockets, expectedSocketTypes);
            CheckSocketColorCount(SocketType.Green, socketGroupSockets, expectedSocketTypes);
            CheckSocketColorCount(SocketType.White, socketGroupSockets, expectedSocketTypes);
        }

        private static void CheckSocketColorCount(SocketType socketTypeToCheck, ICollection<Socket> socketGroupSockets, SocketType[] expectedSocketTypes)
        {
            socketGroupSockets.Count(s => s.SocketType == socketTypeToCheck).Should()
                .Be(expectedSocketTypes.Count(s => s == socketTypeToCheck));
        }
    }
}