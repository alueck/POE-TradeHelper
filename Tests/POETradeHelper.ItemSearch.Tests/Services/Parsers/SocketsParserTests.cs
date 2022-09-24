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

            Assert.NotNull(result);
            Assert.That(result.Count, Is.EqualTo(1));
            Assert.That(result.SocketGroups.Count, Is.EqualTo(1));

            CheckSocketGroup(result.SocketGroups, 0, expected);
        }

        [TestCase("R B", SocketType.Red, SocketType.Blue)]
        [TestCase("W G", SocketType.White, SocketType.Green)]
        public void ParseShouldParseTwoSingleSockets(string socketsString, params SocketType[] expected)
        {
            ItemSockets result = this.socketsParser.Parse(socketsString);

            Assert.NotNull(result);
            Assert.That(result.Count, Is.EqualTo(2));
            Assert.That(result.SocketGroups.Count, Is.EqualTo(2));

            CheckSocketGroup(result.SocketGroups, 0, expected[0]);
            CheckSocketGroup(result.SocketGroups, 1, expected[1]);
        }

        [Test]
        public void ParseShouldParseLinkedSockets()
        {
            ItemSockets result = this.socketsParser.Parse("R-B");

            Assert.NotNull(result);
            Assert.That(result.Count, Is.EqualTo(2));
            Assert.That(result.SocketGroups.Count, Is.EqualTo(1));

            CheckSocketGroup(result.SocketGroups, 0, SocketType.Red, SocketType.Blue);
        }

        [Test]
        public void ParseShouldParseMultipleLinkedSocketGroups()
        {
            ItemSockets result = this.socketsParser.Parse("R-B W-G");

            Assert.NotNull(result);
            Assert.That(result.Count, Is.EqualTo(4));
            Assert.That(result.SocketGroups.Count, Is.EqualTo(2));

            CheckSocketGroup(result.SocketGroups, 0, SocketType.Red, SocketType.Blue);
            CheckSocketGroup(result.SocketGroups, 1, SocketType.White, SocketType.Green);
        }

        [TestCase(null)]
        [TestCase("  ")]
        public void ParseShouldReturnEmptyItemSocketsIfStringIsNullOrWhitespace(string socketsString)
        {
            ItemSockets result = this.socketsParser.Parse(socketsString);

            Assert.NotNull(result);
            Assert.That(result.Count, Is.EqualTo(0));
        }

        private static void CheckSocketGroup(ICollection<SocketGroup> socketGroups, int socketGroupIndex, params SocketType[] expectedSocketTypes)
        {
            ICollection<Socket> socketGroupSockets = socketGroups.Skip(socketGroupIndex).First().Sockets;
            Assert.That(socketGroupSockets, Has.Count.EqualTo(expectedSocketTypes.Length));

            CheckSocketColorCount(SocketType.Red, socketGroupSockets, expectedSocketTypes);
            CheckSocketColorCount(SocketType.Blue, socketGroupSockets, expectedSocketTypes);
            CheckSocketColorCount(SocketType.Green, socketGroupSockets, expectedSocketTypes);
            CheckSocketColorCount(SocketType.White, socketGroupSockets, expectedSocketTypes);
        }

        private static void CheckSocketColorCount(SocketType socketTypeToCheck, ICollection<Socket> socketGroupSockets, SocketType[] expectedSocketTypes)
        {
            Assert.That(socketGroupSockets.Count(s => s.SocketType == socketTypeToCheck), Is.EqualTo(expectedSocketTypes.Count(s => s == socketTypeToCheck)));
        }
    }
}