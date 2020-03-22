using Autofac;
using Autofac.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using POETradeHelper.Common;
using POETradeHelper.Common.Extensions;
using POETradeHelper.ItemSearch;
using Serilog;
using Serilog.Exceptions;
using Splat;
using Splat.Autofac;
using Splat.Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.Json;
using System.Threading.Tasks;
using WindowsHook;

namespace POETradeHelper
{
    [ExcludeFromCodeCoverage]
    public class Bootstrapper : IEnableLogger
    {
        public async Task BuildAsync()
        {
            RegisterDependencies();
            await InitializeAsync();
        }

        private Task InitializeAsync()
        {
            var initializables = Locator.Current.GetServices<IInitializable>();

            return Task.WhenAll(initializables.Select(i => i.OnInitAsync()));
        }

        private void RegisterDependencies()
        {
            ServiceCollection serviceCollection = ConfigureServiceCollection();

            var container = new ContainerBuilder();

            container.Populate(serviceCollection);

            IEnumerable<Assembly> assemblies = Directory.EnumerateFiles(AppDomain.CurrentDomain.BaseDirectory, "POETradeHelper*.dll").Select(Assembly.LoadFrom);

            container.RegisterAssemblyTypes(assemblies.ToArray())
                .PublicOnly()
                .Where(t => !typeof(IInitializable).IsAssignableFrom(t))
                .AsImplementedInterfaces();

            container.RegisterAssemblyTypes(assemblies.ToArray())
                    .PublicOnly()
                    .Where(t => typeof(IInitializable).IsAssignableFrom(t))
                    .AsImplementedInterfaces()
                    .SingleInstance();

            container.RegisterInstance(Hook.GlobalEvents());

            container.UseAutofacDependencyResolver();

            Locator.CurrentMutable.UseMicrosoftExtensionsLoggingWithWrappingFullLogger(Locator.Current.GetService<ILoggerFactory>());
        }

        private static ServiceCollection ConfigureServiceCollection()
        {
            var serviceCollection = new ServiceCollection();
            serviceCollection.AddHttpClient();

            Serilog.Log.Logger = new LoggerConfiguration()
                            .MinimumLevel.Is(Serilog.Events.LogEventLevel.Warning)
                            .Enrich.WithExceptionDetails()
                            .WriteTo.Debug()
                            .WriteTo.File(Path.Combine(FileConfiguration.PoeTradeHelperAppDataFolder, "log.txt"), rollOnFileSizeLimit: true, retainedFileCountLimit: 1, fileSizeLimitBytes: 104857600).CreateLogger();

            serviceCollection.AddLogging(builder => builder.AddSerilog());
            serviceCollection.AddMemoryCache(builder => builder.SizeLimit = 20971520);

            ConfigureOptions(serviceCollection);

            return serviceCollection;
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
                .ConfigureWritable<ItemSearchOptions>(config.GetSection("ItemSearchOptions"), FileConfiguration.PoeTradeHelperAppSettingsPath);
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