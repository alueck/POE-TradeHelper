using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.ReactiveUI;
using Moq;
using NUnit.Framework;
using POETradeHelper.Common;
using POETradeHelper.Common.Contract;
using POETradeHelper.ItemSearch.Contract.Services;
using POETradeHelper.ItemSearch.ViewModels;
using Splat;

namespace POETradeHelper.IntegrationTests
{
    public class ItemSearchResultOverlayViewModelIntegrationTests
    {
        private const string itemString = @"your item string here";

        private IItemSearchResultOverlayViewModel itemSearchOverlayViewModel;

        [SetUp]
        public async Task SetUp()
        {
            Bootstrapper.Configure();

            Locator.CurrentMutable.Register(() => Mock.Of<ICopyCommand>(x => x.ExecuteAsync(It.IsAny<CancellationToken>()) == Task.FromResult(itemString)));
            Locator.CurrentMutable.Register(() => Mock.Of<IPathOfExileProcessHelper>(x => x.IsPathOfExileActiveWindow() == true));

            foreach (var initializable in Locator.Current.GetServices<IInitializable>())
            {
                await initializable.OnInitAsync();
            }

            this.itemSearchOverlayViewModel = Locator.Current.GetService<IItemSearchResultOverlayViewModel>();

            AppBuilder.Configure<App>()
                .UsePlatformDetect()
                .LogToTrace()
                .UseReactiveUI()
                .SetupWithoutStarting();
        }

        [Test]
        [Category("Integration")]
        public async Task Test()
        {
            await this.itemSearchOverlayViewModel.SetListingForItemUnderCursorAsync(default);

            TestContext.WriteLine(JsonSerializer.Serialize(this.itemSearchOverlayViewModel, new JsonSerializerOptions { WriteIndented = true, Converters = { new JsonStringEnumConverter() } }));
        }
    }
}