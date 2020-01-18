using Moq;
using NUnit.Framework;
using POETradeHelper.Common.Contract;
using System.Threading.Tasks;

namespace POETradeHelper.Win32.Tests
{
    public class CopyCommandTests
    {
        private Mock<IClipboardHelper> clipboardHelperMock;
        private CopyCommand copyCommand;

        [SetUp]
        public void Setup()
        {
            this.clipboardHelperMock = new Mock<IClipboardHelper>();
            this.copyCommand = new CopyCommand(this.clipboardHelperMock.Object);
        }

        [Test]
        public async Task ExecuteShouldCallGetTextAsyncOnClipboardHelper()
        {
            await this.copyCommand.ExecuteAsync();

            this.clipboardHelperMock.Verify(x => x.GetTextAsync(), Times.Exactly(2));
        }

        [Test]
        public async Task ExecuteShouldReturnItemString()
        {
            const string expected = "itemString";
            this.clipboardHelperMock.SetupSequence(x => x.GetTextAsync())
                .ReturnsAsync("previously copied text")
                .ReturnsAsync(expected);

            string result = await this.copyCommand.ExecuteAsync();

            Assert.That(result, Is.EqualTo(expected));
        }

        [Test]
        public async Task ExecuteShouldRestoreClipboardTextToPreviousState()
        {
            const string expected = "previously copied text";
            this.clipboardHelperMock.SetupSequence(x => x.GetTextAsync())
                .ReturnsAsync(expected)
                .ReturnsAsync("itemString");

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