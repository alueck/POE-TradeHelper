using Autofac;
using Gma.System.MouseKeyHook;
using POETradeHelper.Common.Contract;
using POETradeHelper.ItemSearch.Controllers;
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

            container.RegisterType<UserInputEventProvider>()
                .As<IUserInputEventProvider>();

            container.RegisterInstance(Hook.GlobalEvents())
                .AsImplementedInterfaces();

            container.RegisterType<ViewLocator>()
                .As<Common.Contract.IViewLocator>();

            container.RegisterType<TradeClient>()
                .As<ITradeClient>();

            container.RegisterType<PathOfExileProcessHelper>()
                .As<IPathOfExileProcessHelper>();

            container.RegisterType<CopyCommand>()
                .As<ICopyCommand>();

            container.RegisterType<ClipboardHelper>()
                .As<IClipboardHelper>();

            container.RegisterType<ItemSearchResultOverlayController>()
                .As<IItemSearchResultOverlayController>()
                .SingleInstance();

            container.RegisterAssemblyTypes(typeof(ItemSearchResultOverlayController).Assembly)
                .PublicOnly()
                .Where(t => !t.Name.EndsWith("Controller"))
                .AsImplementedInterfaces();

            container.UseAutofacDependencyResolver();
        }
    }
}