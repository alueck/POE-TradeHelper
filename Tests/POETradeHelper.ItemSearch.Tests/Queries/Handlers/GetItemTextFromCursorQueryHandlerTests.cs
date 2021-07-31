using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using POETradeHelper.Common.Contract;
using POETradeHelper.ItemSearch.Contract.Queries;
using POETradeHelper.ItemSearch.Queries.Handlers;

namespace POETradeHelper.ItemSearch.Tests.Queries.Handlers
{
    public class GetItemTextFromCursorQueryHandlerTests
    {
        private Mock<IClipboardHelper> clipboardHelperMock;
        private Mock<INativeKeyboard> nativeKeyBoardMock;
        private GetItemTextFromCursorQueryHandler handler;

        [SetUp]
        public void Setup()
        {
            this.clipboardHelperMock = new Mock<IClipboardHelper>();
            this.nativeKeyBoardMock = new Mock<INativeKeyboard>();
            this.handler = new GetItemTextFromCursorQueryHandler(this.clipboardHelperMock.Object, this.nativeKeyBoardMock.Object);
        }

        [Test]
        public async Task HandleShouldCallGetTextAsyncOnClipboardHelper()
        {
            await this.handler.Handle(new GetItemTextFromCursorQuery(), default);

            this.clipboardHelperMock.Verify(x => x.GetTextAsync(), Times.Exactly(2));
        }

        [Test]
        public async Task HandleShouldCallSendCopyCommandOnNativeKeyboard()
        {
            await this.handler.Handle(new GetItemTextFromCursorQuery(), default);

            this.nativeKeyBoardMock.Verify(x => x.SendCopyCommand());
        }

        [Test]
        public async Task HandleShouldReturnItemString()
        {
            const string expected = "itemString";
            this.clipboardHelperMock.Setup(x => x.GetTextAsync())
                .ReturnsAsync("previously copied text");

            this.nativeKeyBoardMock.Setup(x => x.SendCopyCommand())
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

            this.nativeKeyBoardMock.Setup(x => x.SendCopyCommand())
                .Callback(() => this.clipboardHelperMock.Setup(x => x.GetTextAsync()).ReturnsAsync(expected));

            string result = await this.handler.Handle(new GetItemTextFromCursorQuery(), default);

            this.clipboardHelperMock.Verify(x => x.SetTextAsync(expected));
        }

        [Test]
        public async Task HandleShouldClearClipboardIfItWasEmpty()
        {
            this.clipboardHelperMock.SetupSequence(x => x.GetTextAsync())
                .ReturnsAsync((string)null)
                .ReturnsAsync("itemString");

            string result = await this.handler.Handle(new GetItemTextFromCursorQuery(), default);

            this.clipboardHelperMock.Verify(x => x.ClearAsync());
        }
    }
}