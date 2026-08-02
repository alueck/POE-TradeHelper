using AwesomeAssertions;

using NSubstitute;

using NUnit.Framework;

using POETradeHelper.Common.Contract;
using POETradeHelper.ItemSearch.Queries;

namespace POETradeHelper.ItemSearch.Tests.Queries
{
    public class GetItemTextFromCursorQueryTests
    {
        private readonly IClipboardHelper clipboardHelperMock;
        private readonly IUserInputSimulator userInputSimulatorMock;
        private readonly GetItemTextFromCursorQueryHandler handler;

        public GetItemTextFromCursorQueryTests()
        {
            this.clipboardHelperMock = Substitute.For<IClipboardHelper>();
            this.userInputSimulatorMock = Substitute.For<IUserInputSimulator>();
            this.handler = new GetItemTextFromCursorQueryHandler(this.clipboardHelperMock, this.userInputSimulatorMock);
        }

        [Test]
        public async Task Handle_ShouldCallGetTextAsyncOnClipboardHelper()
        {
            await this.handler.Handle(new GetItemTextFromCursorQuery(), default);

            await this.clipboardHelperMock
                .Received()
                .GetTextAsync();
        }

        [Test]
        public async Task Handle_ShouldCallSendCopyAdvancedItemStringCommandOnUserInputSimulator()
        {
            await this.handler.Handle(new GetItemTextFromCursorQuery(), default);

            this.userInputSimulatorMock
                .Received()
                .SendCopyAdvancedItemStringCommand();
        }

        [Test]
        public async Task Handle_ShouldReturnItemString()
        {
            const string expected = "itemString";
            this.clipboardHelperMock.GetTextAsync()
                .Returns("previously copied text");

            this.userInputSimulatorMock
                .WhenForAnyArgs(m => m.SendCopyAdvancedItemStringCommand())
                .Do(_ => this.clipboardHelperMock.GetTextAsync().Returns(expected));

            string result = await this.handler.Handle(new GetItemTextFromCursorQuery(), default);

            result.Should().BeEquivalentTo(expected);
        }

        [Test]
        public async Task Handle_ShouldRestoreClipboardTextToPreviousState()
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
        public async Task Handle_ShouldClearClipboardIfItWasEmpty()
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