using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.Json;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using POETradeHelper.Common.Contract;
using POETradeHelper.Common.Extensions;
using POETradeHelper.ItemSearch.Contract.Configuration;
using POETradeHelper.QualityOfLife.Models;
using POETradeHelper.ViewModels;
using Serilog;
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

        private static void RegisterDependencies()
        {
            Assembly[] assemblies = Directory.EnumerateFiles(AppDomain.CurrentDomain.BaseDirectory, "POETradeHelper*.dll").Select(Assembly.LoadFrom).ToArray();

            ServiceCollection serviceCollection = ConfigureServiceCollection(assemblies);

            var container = new ContainerBuilder();

            container.RegisterAssemblyTypes(assemblies)
                .PublicOnly()
                .Where(t => !t.HasSingletonAttribute())
                .AsImplementedInterfaces()
                .Except<DebugSettingsViewModel>();

#if DEBUG
            container.RegisterType<DebugSettingsViewModel>().AsImplementedInterfaces();
#endif

            container.RegisterAssemblyTypes(assemblies)
                    .PublicOnly()
                    .Where(t => t.HasSingletonAttribute())
                    .AsImplementedInterfaces()
                    .SingleInstance();

            RegisterMediatR(container);

            container.Populate(serviceCollection);
            container.UseAutofacDependencyResolver();

            Locator.CurrentMutable.UseMicrosoftExtensionsLoggingWithWrappingFullLogger(Locator.Current.GetService<ILoggerFactory>());
        }

        private static ServiceCollection ConfigureServiceCollection(params Assembly[] applicationAssemblies)
        {
            var serviceCollection = new ServiceCollection();

            Serilog.Log.Logger = new LoggerConfiguration()
                            .MinimumLevel.Is(Serilog.Events.LogEventLevel.Warning)
                            .Enrich.WithExceptionDetails()
                            .WriteTo.Debug()
                            .WriteTo.File(Path.Combine(FileConfiguration.PoeTradeHelperAppDataFolder, "log.txt"), rollOnFileSizeLimit: true, retainedFileCountLimit: 1, fileSizeLimitBytes: 104857600).CreateLogger();

            serviceCollection.AddLogging(builder => builder.AddSerilog());
            serviceCollection.AddMemoryCache(builder => builder.SizeLimit = 20971520);

            ConfigureOptions(serviceCollection);
            RegisterModules(serviceCollection, applicationAssemblies);

            return serviceCollection;
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
                module.RegisterServices(serviceCollection);
            }
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

        private static void CreateAppSettingsFileIfMissing()
        {
            Directory.CreateDirectory(FileConfiguration.PoeTradeHelperAppDataFolder);

            if (!File.Exists(FileConfiguration.PoeTradeHelperAppSettingsPath))
            {
                string defaultAppSettingsJson = JsonSerializer.Serialize(new AppSettings(), new JsonSerializerOptions { WriteIndented = true });

                File.WriteAllText(FileConfiguration.PoeTradeHelperAppSettingsPath, defaultAppSettingsJson);
            }
        }

        private static void RegisterMediatR(ContainerBuilder container)
        {
            container
                .RegisterType<Mediator>()
                .As<IMediator>()
                .InstancePerLifetimeScope();
            container.Register<ServiceFactory>(context =>
            {
                var c = context.Resolve<IComponentContext>();
                return t => c.Resolve(t);
            });
        }
    }
}