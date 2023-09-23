using FluentAssertions;
using FluentAssertions.Extensions;

using NSubstitute;

using NUnit.Framework;

using POETradeHelper.Common.Contract;
using POETradeHelper.ItemSearch.Queries;
using POETradeHelper.ItemSearch.Queries.Handlers;

namespace POETradeHelper.ItemSearch.Tests.Queries.Handlers
{
    public class GetItemTextFromCursorQueryHandlerTests
    {
        private readonly IClipboardHelper clipboardHelperMock;
        private readonly IUserInputSimulator userInputSimulatorMock;
        private readonly GetItemTextFromCursorQueryHandler handler;

        public GetItemTextFromCursorQueryHandlerTests()
        {
            this.clipboardHelperMock = Substitute.For<IClipboardHelper>();
            this.userInputSimulatorMock = Substitute.For<IUserInputSimulator>();
            this.handler = new GetItemTextFromCursorQueryHandler(this.clipboardHelperMock, this.userInputSimulatorMock);
        }

        [Test]
        public async Task HandleShouldCallGetTextAsyncOnClipboardHelper()
        {
            await this.handler.Handle(new GetItemTextFromCursorQuery(), default);

            await this.clipboardHelperMock
                .Received()
                .GetTextAsync();
        }

        [Test]
        public async Task HandleShouldCallSendCopyAdvancedItemStringCommandOnUserInputSimulator()
        {
            await this.handler.Handle(new GetItemTextFromCursorQuery(), default);

            this.userInputSimulatorMock
                .Received()
                .SendCopyAdvancedItemStringCommand();
        }

        [Test]
        public async Task HandleShouldCallGetTextAsyncOnClipboardHelperDelayedUpToFiveTimesIfRetrievedTextFromClipboardDidNotChange()
        {
            this.clipboardHelperMock
                .GetTextAsync()
                .Returns(null, null, null, null, null, null, null, "string");

            Func<Task> action = () => this.handler.Handle(new GetItemTextFromCursorQuery(), default);

            action.ExecutionTime().Should().BeCloseTo(1000.Milliseconds(), 50.Milliseconds());
            await this.clipboardHelperMock
                .Received(6) // first call to save previous clipboard content for restore
                .GetTextAsync();
        }

        [Test]
        public async Task HandleShouldReturnItemString()
        {
            const string expected = "itemString";
            this.clipboardHelperMock.GetTextAsync()
                .Returns("previously copied text");

            this.userInputSimulatorMock
                .WhenForAnyArgs(m => m.SendCopyAdvancedItemStringCommand())
                .Do(_ => this.clipboardHelperMock.GetTextAsync().Returns(expected));

            string result = await this.handler.Handle(new GetItemTextFromCursorQuery(), default);

            Assert.That(result, Is.EqualTo(expected));
        }

        [Test]
        public async Task HandleShouldRestoreClipboardTextToPreviousState()
        {
            const string expected = "previously copied text";
            this.clipboardHelperMock.GetTextAsync()
                .Returns(expected);

            this.userInputSimulatorMock
                .WhenForAnyArgs(m => m.SendCopyAdvancedItemStringCommand())
                .Do(_ => this.clipboardHelperMock.GetTextAsync().Returns(expected));

            await this.handler.Handle(new GetItemTextFromCursorQuery(), default);

            await this.clipboardHelperMock
                .Received()
                .SetTextAsync(expected);
        }

        [Test]
        public async Task HandleShouldClearClipboardIfItWasEmpty()
        {
            this.clipboardHelperMock.GetTextAsync()
                .Returns(string.Empty, "itemString");

            await this.handler.Handle(new GetItemTextFromCursorQuery(), default);

            await this.clipboardHelperMock
                .Received()
                .ClearAsync();
        }
    }
}