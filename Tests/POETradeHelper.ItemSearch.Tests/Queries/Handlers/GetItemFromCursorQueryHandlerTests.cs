using FluentAssertions;

using MediatR;

using NSubstitute;

using NUnit.Framework;

using POETradeHelper.ItemSearch.Contract.Models;
using POETradeHelper.ItemSearch.Contract.Services.Parsers;
using POETradeHelper.ItemSearch.Exceptions;
using POETradeHelper.ItemSearch.Queries;
using POETradeHelper.ItemSearch.Queries.Handlers;

namespace POETradeHelper.ItemSearch.Tests.Queries.Handlers
{
    public class GetItemFromCursorQueryHandlerTests
    {
        private readonly IMediator mediatorMock;
        private readonly IItemParserAggregator itemParserAggregatorMock;
        private readonly GetItemFromCursorQueryHandler handler;

        public GetItemFromCursorQueryHandlerTests()
        {
            this.mediatorMock = Substitute.For<IMediator>();
            this.itemParserAggregatorMock = Substitute.For<IItemParserAggregator>();
            this.handler = new GetItemFromCursorQueryHandler(this.mediatorMock, this.itemParserAggregatorMock);
        }

        [Test]
        public async Task HandleShouldSendGetItemTextFromCursorQuery()
        {
            CancellationToken cancellationToken = CancellationToken.None;
            this.itemParserAggregatorMock
                .IsParseable(Arg.Any<string>())
                .Returns(true);

            await this.handler.Handle(new GetItemFromCursorQuery(), cancellationToken);

            await this.mediatorMock
                .Received()
                .Send(Arg.Any<GetItemTextFromCursorQuery>(), cancellationToken);
        }

        [Test]
        public async Task HandleShouldCallIsParseableOnItemParser()
        {
            const string stringToParse = "item string to parse";
            this.mediatorMock.Send(Arg.Any<GetItemTextFromCursorQuery>(), Arg.Any<CancellationToken>())
                .Returns(stringToParse);
            this.itemParserAggregatorMock
                .IsParseable(Arg.Any<string>())
                .Returns(true);

            await this.handler.Handle(new GetItemFromCursorQuery(), default);

            this.itemParserAggregatorMock
                .Received()
                .IsParseable(stringToParse);
        }

        [Test]
        public void HandleShouldThrowInvalidItemStringExceptionIfIsParseableFromItemParserReturnsFalse()
        {
            this.itemParserAggregatorMock
                .IsParseable(Arg.Any<string>())
                .Returns(false);

            async Task Action()
            {
                await this.handler.Handle(new GetItemFromCursorQuery(), default);
            }

            Assert.ThrowsAsync<InvalidItemStringException>(Action);
        }

        [Test]
        public async Task HandleShouldCallParseOnItemParserIfIsParseableReturnsTrue()
        {
            const string stringToParse = "item string to parse";
            this.mediatorMock.Send(Arg.Any<GetItemTextFromCursorQuery>(), Arg.Any<CancellationToken>())
                .Returns(stringToParse);
            this.itemParserAggregatorMock.IsParseable(Arg.Any<string>())
                .Returns(true);

            await this.handler.Handle(new GetItemFromCursorQuery(), default);

            this.itemParserAggregatorMock
                .Received()
                .Parse(stringToParse);
        }

        [Test]
        public async Task HandleShouldReturnParseResult()
        {
            EquippableItem? expected = new(ItemRarity.Normal) { Name = "TestItem" };
            this.itemParserAggregatorMock.IsParseable(Arg.Any<string>())
                .Returns(true);
            this.itemParserAggregatorMock.Parse(Arg.Any<string>())
                .Returns(expected);

            Item result = await this.handler.Handle(new GetItemFromCursorQuery(), default);

            result.Should().Be(expected);
        }
    }
}