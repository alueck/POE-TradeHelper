﻿using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Moq;
using NUnit.Framework;
using POETradeHelper.ItemSearch.Contract.Models;
using POETradeHelper.ItemSearch.Contract.Queries;
using POETradeHelper.ItemSearch.Contract.Services.Parsers;
using POETradeHelper.ItemSearch.Exceptions;
using POETradeHelper.ItemSearch.Queries.Handlers;

namespace POETradeHelper.ItemSearch.Tests.Queries.Handlers
{
    public class GetItemFromCursorQueryHandlerTests
    {
        private Mock<IMediator> mediatorMock;
        private Mock<IItemParserAggregator> itemParserAggregatorMock;
        private GetItemFromCursorQueryHandler handler;

        [SetUp]
        public void Setup()
        {
            this.mediatorMock = new Mock<IMediator>();
            this.itemParserAggregatorMock = new Mock<IItemParserAggregator>();
            this.handler = new GetItemFromCursorQueryHandler(this.mediatorMock.Object, this.itemParserAggregatorMock.Object);
        }

        [Test]
        public async Task HandleShouldSendGetItemTextFromCursorQuery()
        {
            var cancellationToken = new CancellationToken();
            this.itemParserAggregatorMock
                .Setup(x => x.IsParseable(It.IsAny<string>()))
                .Returns(true);

            await handler.Handle(new GetItemFromCursorQuery(), cancellationToken);

            this.mediatorMock.Verify(x => x.Send(It.IsAny<GetItemTextFromCursorQuery>(), cancellationToken));
        }

        [Test]
        public async Task HandleShouldCallIsParseableOnItemParser()
        {
            string stringToParse = "item string to parse";
            this.mediatorMock.Setup(x => x.Send(It.IsAny<GetItemTextFromCursorQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(stringToParse);
            this.itemParserAggregatorMock
                .Setup(x => x.IsParseable(It.IsAny<string>()))
                .Returns(true);

            await this.handler.Handle(new GetItemFromCursorQuery(), default);

            this.itemParserAggregatorMock.Verify(x => x.IsParseable(stringToParse));
        }

        [Test]
        public void HandleShouldThrowInvalidItemStringExceptionIfIsParseableFromItemParserReturnsFalse()
        {
            this.itemParserAggregatorMock
                .Setup(x => x.IsParseable(It.IsAny<string>()))
                .Returns(false);

            async Task Action() => await this.handler.Handle(new GetItemFromCursorQuery(), default);

            Assert.ThrowsAsync<InvalidItemStringException>(Action);
        }

        [Test]
        public async Task HandleShouldCallParseOnItemParserIfIsParseableReturnsTrue()
        {
            string stringToParse = "item string to parse";
            this.mediatorMock.Setup(x => x.Send( It.IsAny<GetItemTextFromCursorQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(stringToParse);
            this.itemParserAggregatorMock.Setup(x => x.IsParseable(It.IsAny<string>()))
                .Returns(true);

            await handler.Handle(new GetItemFromCursorQuery(), default);

            this.itemParserAggregatorMock.Verify(x => x.Parse(stringToParse));
        }

        [Test]
        public async Task HandleShouldReturnParseResult()
        {
            var expected = new EquippableItem(ItemRarity.Normal) { Name = "TestItem" };
            this.itemParserAggregatorMock.Setup(x => x.IsParseable(It.IsAny<string>()))
                .Returns(true);
            this.itemParserAggregatorMock.Setup(x => x.Parse(It.IsAny<string>()))
                .Returns(expected);

            Item result = await handler.Handle(new GetItemFromCursorQuery(), default);

            Assert.AreEqual(expected, result);
        }
    }
}