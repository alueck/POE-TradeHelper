using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.Json;

using Autofac;
using Autofac.Extensions.DependencyInjection;
using Autofac.Extras.DynamicProxy;

using Avalonia.ReactiveUI;
using Avalonia.Threading;

using Castle.DynamicProxy;

using MediatR;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

using POETradeHelper.Common.Contract;
using POETradeHelper.Common.Extensions;
using POETradeHelper.Common.UI;
using POETradeHelper.Extensions;
using POETradeHelper.ItemSearch.Contract.Configuration;
using POETradeHelper.QualityOfLife.Models;
using POETradeHelper.ViewModels;

using Polly.Bulkhead;

using ReactiveUI;

using Serilog;
using Serilog.Events;
using Serilog.Exceptions;

using Splat;
using Splat.Autofac;
using Splat.Microsoft.Extensions.Logging;

namespace POETradeHelper
{
    [ExcludeFromCodeCoverage]
    public class Bootstrapper : IEnableLogger
    {
        public static void Configure()
        {
            RegisterDependencies();
        }

        public static void Shutdown()
        {
            var modules = Locator.Current.GetServices<IModule>();
            foreach (var module in modules.OfType<IDisposable>())
            {
                module.Dispose();
            }
        }

        private static void RegisterDependencies()
        {
            Assembly[] assemblies = Directory.EnumerateFiles(AppDomain.CurrentDomain.BaseDirectory, "POETradeHelper*.dll").Select(Assembly.LoadFrom).ToArray();
            ServiceCollection serviceCollection = ConfigureServiceCollection(assemblies);

            var builder = new ContainerBuilder();
            var autofacResolver = builder.UseAutofacDependencyResolver();
            builder.RegisterInstance(autofacResolver);
            
            RegisterInterceptors(builder, assemblies);
            RegisterNonSingletonTypes(builder, assemblies);
            RegisterSingletonTypes(builder, assemblies);

            RegisterMediatR(builder);

            builder.Populate(serviceCollection);

            Locator.CurrentMutable.InitializeSplat();
            Locator.CurrentMutable.InitializeReactiveUI();
            Locator.CurrentMutable.InitializeAvalonia();
            Locator.CurrentMutable.UseMicrosoftExtensionsLoggingWithWrappingFullLogger(() => Locator.Current.GetService<ILoggerFactory>());

            var container = builder.Build();
            autofacResolver.SetLifetimeScope(container);
        }

        private static ServiceCollection ConfigureServiceCollection(params Assembly[] applicationAssemblies)
        {
            var serviceCollection = new ServiceCollection();

            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Is(LogEventLevel.Warning)
                .Enrich.WithExceptionDetails()
                .WriteTo.Debug()
                .WriteTo.File(Path.Combine(FileConfiguration.PoeTradeHelperAppDataFolder, "log.txt"), rollOnFileSizeLimit: true, retainedFileCountLimit: 1, fileSizeLimitBytes: 104857600).CreateLogger();

            serviceCollection.AddLogging(builder => builder.AddSerilog());
            serviceCollection.AddMemoryCache();

            ConfigureOptions(serviceCollection);
            RegisterModules(serviceCollection, applicationAssemblies);

            return serviceCollection;
        }

        private static void RegisterInterceptors(ContainerBuilder builder, Assembly[] assemblies)
        {
            builder.RegisterAssemblyTypes(assemblies)
                .Where(t => t.GetInterfaces().Any(i => i == typeof(IInterceptor)))
                .AsSelf()
                .SingleInstance();
        }

        private static void RegisterNonSingletonTypes(ContainerBuilder builder, Assembly[] assemblies)
        {
#if DEBUG
            builder.RegisterType<DebugSettingsViewModel>().AsImplementedInterfaces();
#endif
            
            builder.RegisterAssemblyTypes(assemblies)
                .PublicOnly()
                .Where(t => !t.HasSingletonAttribute() && t.GetCustomAttributes<InterceptAttribute>().Any())
                .AsImplementedInterfaces()
                .EnableInterfaceInterceptors()
                .Except<DebugSettingsViewModel>();

            builder.RegisterAssemblyTypes(assemblies)
                .PublicOnly()
                .Where(t => !t.HasSingletonAttribute() && !t.GetCustomAttributes<InterceptAttribute>().Any())
                .AsImplementedInterfaces()
                .Except<DebugSettingsViewModel>();
        }

        private static void RegisterSingletonTypes(ContainerBuilder builder, Assembly[] assemblies)
        {
            builder.RegisterAssemblyTypes(assemblies)
                .PublicOnly()
                .Where(t => t.HasSingletonAttribute() && t.GetCustomAttributes<InterceptAttribute>().Any())
                .AsImplementedInterfaces()
                .EnableInterfaceInterceptors()
                .SingleInstance();

            builder.RegisterAssemblyTypes(assemblies)
                .PublicOnly()
                .Where(t => t.HasSingletonAttribute() && !t.GetCustomAttributes<InterceptAttribute>().Any())
                .AsImplementedInterfaces()
                .SingleInstance();
        }

        private static void RegisterMediatR(ContainerBuilder builder)
        {
            builder
                .RegisterType<Mediator>()
                .As<IMediator>()
                .InstancePerLifetimeScope();
            builder.Register<ServiceFactory>(context =>
            {
                var c = context.Resolve<IComponentContext>();
                return t => c.Resolve(t);
            });
        }

        private static void ConfigureOptions(ServiceCollection serviceCollection)
        {
            CreateAppSettingsFileIfMissing();

            IConfiguration config = new ConfigurationBuilder()
                          .AddJsonFile(FileConfiguration.PoeTradeHelperAppSettingsPath, false, true)
                          .Build();

            serviceCollection
                .AddOptions()
                .Configure<AppSettings>(config)
                .ConfigureWritable<ItemSearchOptions>(config.GetSection("ItemSearchOptions"), FileConfiguration.PoeTradeHelperAppSettingsPath)
                .ConfigureWritable<WikiOptions>(config.GetSection("WikiOptions"), FileConfiguration.PoeTradeHelperAppSettingsPath);
        }

        private static void RegisterModules(ServiceCollection serviceCollection, params Assembly[] applicationAssemblies)
        {
            var modules = applicationAssemblies
                .SelectMany(x => x.GetTypes())
                .Where(t => t.GetInterfaces().Any(i => i == typeof(IModule)))
                .Select(Activator.CreateInstance)
                .Cast<IModule>();

            foreach (var module in modules)
            {
                serviceCollection.AddSingleton(module);
                module.RegisterServices(serviceCollection);
            }
        }

        private static void CreateAppSettingsFileIfMissing()
        {
            Directory.CreateDirectory(FileConfiguration.PoeTradeHelperAppDataFolder);

            if (!File.Exists(FileConfiguration.PoeTradeHelperAppSettingsPath))
            {
                string defaultAppSettingsJson = JsonSerializer.Serialize(new AppSettings(), new JsonSerializerOptions { WriteIndented = true });

                File.WriteAllText(FileConfiguration.PoeTradeHelperAppSettingsPath, defaultAppSettingsJson);
            }
        }
    }
}
