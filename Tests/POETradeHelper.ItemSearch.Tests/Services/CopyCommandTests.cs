using Moq;
using NUnit.Framework;
using POETradeHelper.Common.Contract;
using System.Threading.Tasks;

namespace POETradeHelper.ItemSearch.Services.Tests
{
    public class CopyCommandTests
    {
        private Mock<IClipboardHelper> clipboardHelperMock;
        private Mock<INativeKeyboard> nativeKeyBoardMock;
        private CopyCommand copyCommand;

        [SetUp]
        public void Setup()
        {
            this.clipboardHelperMock = new Mock<IClipboardHelper>();
            this.nativeKeyBoardMock = new Mock<INativeKeyboard>();
            this.copyCommand = new CopyCommand(this.clipboardHelperMock.Object, this.nativeKeyBoardMock.Object);
        }

        [Test]
        public async Task ExecuteShouldCallGetTextAsyncOnClipboardHelper()
        {
            await this.copyCommand.ExecuteAsync();

            this.clipboardHelperMock.Verify(x => x.GetTextAsync(), Times.Exactly(2));
        }

        [Test]
        public async Task ExecuteShouldCallSendCopyCommandOnNativeKeyboard()
        {
            await this.copyCommand.ExecuteAsync();

            this.nativeKeyBoardMock.Verify(x => x.SendCopyCommand());
        }

        [Test]
        public async Task ExecuteShouldReturnItemString()
        {
            const string expected = "itemString";
            this.clipboardHelperMock.Setup(x => x.GetTextAsync())
                .ReturnsAsync("previously copied text");

            this.nativeKeyBoardMock.Setup(x => x.SendCopyCommand())
                .Callback(() => this.clipboardHelperMock.Setup(x => x.GetTextAsync()).ReturnsAsync(expected));

            string result = await this.copyCommand.ExecuteAsync();

            Assert.That(result, Is.EqualTo(expected));
        }

        [Test]
        public async Task ExecuteShouldRestoreClipboardTextToPreviousState()
        {
            const string expected = "previously copied text";
            this.clipboardHelperMock.Setup(x => x.GetTextAsync())
                .ReturnsAsync(expected);

            this.nativeKeyBoardMock.Setup(x => x.SendCopyCommand())
                .Callback(() => this.clipboardHelperMock.Setup(x => x.GetTextAsync()).ReturnsAsync(expected));

            await this.copyCommand.ExecuteAsync();

            this.clipboardHelperMock.Verify(x => x.SetTextAsync(expected));
        }

        [Test]
        public async Task ExecuteShouldClearClipboardIfItWasEmpty()
        {
            this.clipboardHelperMock.SetupSequence(x => x.GetTextAsync())
                .ReturnsAsync((string)null)
                .ReturnsAsync("itemString");

            await this.copyCommand.ExecuteAsync();

            this.clipboardHelperMock.Verify(x => x.ClearAsync());
        }
    }
}