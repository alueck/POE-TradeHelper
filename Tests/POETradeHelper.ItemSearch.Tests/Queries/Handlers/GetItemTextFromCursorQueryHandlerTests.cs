using Moq;

using NUnit.Framework;

using POETradeHelper.Common.Contract;
using POETradeHelper.ItemSearch.Queries;
using POETradeHelper.ItemSearch.Queries.Handlers;

namespace POETradeHelper.ItemSearch.Tests.Queries.Handlers
{
    public class GetItemTextFromCursorQueryHandlerTests
    {
        private readonly Mock<IClipboardHelper> clipboardHelperMock;
        private readonly Mock<IUserInputSimulator> userInputSimulatorMock;
        private readonly GetItemTextFromCursorQueryHandler handler;

        public GetItemTextFromCursorQueryHandlerTests()
        {
            this.clipboardHelperMock = new Mock<IClipboardHelper>();
            this.userInputSimulatorMock = new Mock<IUserInputSimulator>();
            this.handler = new GetItemTextFromCursorQueryHandler(this.clipboardHelperMock.Object, this.userInputSimulatorMock.Object);
        }

        [Test]
        public async Task HandleShouldCallGetTextAsyncOnClipboardHelper()
        {
            await this.handler.Handle(new GetItemTextFromCursorQuery(), default);

            this.clipboardHelperMock.Verify(x => x.GetTextAsync(), Times.Exactly(2));
        }

        [Test]
        public async Task HandleShouldCallSendCopyCommandOnUserInputSimulator()
        {
            await this.handler.Handle(new GetItemTextFromCursorQuery(), default);

            this.userInputSimulatorMock.Verify(x => x.SendCopyCommand());
        }

        [Test]
        public async Task HandleShouldReturnItemString()
        {
            const string expected = "itemString";
            this.clipboardHelperMock.Setup(x => x.GetTextAsync())
                .ReturnsAsync("previously copied text");

            this.userInputSimulatorMock.Setup(x => x.SendCopyCommand())
                .Callback(() => this.clipboardHelperMock.Setup(x => x.GetTextAsync()).ReturnsAsync(expected));

            string result = await this.handler.Handle(new GetItemTextFromCursorQuery(), default);

            Assert.That(result, Is.EqualTo(expected));
        }

        [Test]
        public async Task HandleShouldRestoreClipboardTextToPreviousState()
        {
            const string expected = "previously copied text";
            this.clipboardHelperMock.Setup(x => x.GetTextAsync())
                .ReturnsAsync(expected);

            this.userInputSimulatorMock.Setup(x => x.SendCopyCommand())
                .Callback(() => this.clipboardHelperMock.Setup(x => x.GetTextAsync()).ReturnsAsync(expected));

            await this.handler.Handle(new GetItemTextFromCursorQuery(), default);

            this.clipboardHelperMock.Verify(x => x.SetTextAsync(expected));
        }

        [Test]
        public async Task HandleShouldClearClipboardIfItWasEmpty()
        {
            this.clipboardHelperMock.SetupSequence(x => x.GetTextAsync())
                .ReturnsAsync(string.Empty)
                .ReturnsAsync("itemString");

            await this.handler.Handle(new GetItemTextFromCursorQuery(), default);

            this.clipboardHelperMock.Verify(x => x.ClearAsync());
        }
    }
}