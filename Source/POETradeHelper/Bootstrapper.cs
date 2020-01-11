using Autofac;
using Gma.System.MouseKeyHook;
using POETradeHelper.Common.Contract;
using POETradeHelper.ItemSearch.Contract.Services;
using POETradeHelper.ItemSearch.Controllers;
using POETradeHelper.ItemSearch.Services;
using POETradeHelper.ItemSearch.ViewModels;
using POETradeHelper.PathOfExileTradeApi;
using POETradeHelper.Win32;
using Splat;
using Splat.Autofac;

namespace POETradeHelper
{
    public class Bootstrapper : IEnableLogger
    {
        public Bootstrapper()
        {
            RegisterDependencies();
        }

        private void RegisterDependencies()
        {
            var container = new ContainerBuilder();

            container.RegisterType<ItemSearchResultOverlayController>()
                .As<IItemSearchResultOverlayController>()
                .SingleInstance();

            container.RegisterType<UserInputEventProvider>()
                .As<IUserInputEventProvider>();

            container.RegisterInstance(Hook.GlobalEvents())
                .AsImplementedInterfaces();

            container.RegisterType<ItemSearchResultOverlayViewModel>()
                .As<IItemSearchResultOverlayViewModel>();

            container.RegisterType<ViewLocator>()
                .As<Common.Contract.IViewLocator>();

            container.RegisterType<SearchItemProvider>()
                .As<ISearchItemProvider>();

            container.RegisterType<TradeClient>()
                .As<ITradeClient>();

            container.RegisterType<PathOfExileProcessHelper>()
                .As<IPathOfExileProcessHelper>();

            container.RegisterType<CopyCommand>()
                .As<ICopyCommand>();

            container.RegisterType<ItemParserAggregator>()
                .As<IItemParserAggregator>();

            container.RegisterType<GemItemParser>()
                .As<IItemParser>();

            container.RegisterType<ClipboardHelper>()
                .As<IClipboardHelper>();

            container.UseAutofacDependencyResolver();
        }
    }
}