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
                .Received(2)
                .GetTextAsync();
        }

        [Test]
        public async Task HandleShouldCallSendCopyCommandOnUserInputSimulator()
        {
            await this.handler.Handle(new GetItemTextFromCursorQuery(), default);

            this.userInputSimulatorMock
                .Received()
                .SendCopyCommand();
        }

        [Test]
        public async Task HandleShouldReturnItemString()
        {
            const string expected = "itemString";
            this.clipboardHelperMock.GetTextAsync()
                .Returns("previously copied text");

            this.userInputSimulatorMock
                .WhenForAnyArgs(m => m.SendCopyCommand())
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
                .WhenForAnyArgs(m => m.SendCopyCommand())
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