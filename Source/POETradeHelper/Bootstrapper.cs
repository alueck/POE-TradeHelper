﻿using Autofac;
using Autofac.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using POETradeHelper.ItemSearch;
using Serilog;
using Serilog.Exceptions;
using Splat;
using Splat.Autofac;
using Splat.Serilog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.Json;
using WindowsHook;

namespace POETradeHelper
{
    public class Bootstrapper : IEnableLogger
    {
        public void Build()
        {
            RegisterDependencies();
        }

        private void RegisterDependencies()
        {
            ServiceCollection serviceCollection = ConfigureServiceCollection();

            var container = new ContainerBuilder();

            container.Populate(serviceCollection);

            IEnumerable<Assembly> assemblies = Directory.EnumerateFiles(AppDomain.CurrentDomain.BaseDirectory, "POETradeHelper*.dll").Select(Assembly.LoadFrom);

            container.RegisterAssemblyTypes(assemblies.ToArray())
                .PublicOnly()
                .AsImplementedInterfaces();

            container.RegisterInstance(Hook.GlobalEvents());

            container.UseAutofacDependencyResolver();

            ConfigureLogging();
        }

        private static ServiceCollection ConfigureServiceCollection()
        {
            var serviceCollection = new ServiceCollection();
            serviceCollection.AddHttpClient();

            ConfigureOptions(serviceCollection);

            return serviceCollection;
        }

        private static void ConfigureOptions(ServiceCollection serviceCollection)
        {
            string appDataFolder = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            string poeTradeHelperAppDataFolder = Path.Combine(appDataFolder, "POETradeHelper");
            string poeTradeHelperAppSettingsPath = Path.Combine(poeTradeHelperAppDataFolder, "appsettings.json");

            CreateAppSettingsFileIfMissing(poeTradeHelperAppDataFolder, poeTradeHelperAppSettingsPath);

            IConfiguration config = new ConfigurationBuilder()
                          .AddJsonFile(poeTradeHelperAppSettingsPath, false, true)
                          .Build();

            serviceCollection
                .AddOptions()
                .Configure<AppSettings>(config)
                .Configure<ItemSearchOptions>(config.GetSection("ItemSearchOptions"));
        }

        private static void CreateAppSettingsFileIfMissing(string poeTradeHelperAppDataFolder, string poeTradeHelperAppSettingsPath)
        {
            Directory.CreateDirectory(poeTradeHelperAppDataFolder);

            if (!File.Exists(poeTradeHelperAppSettingsPath))
            {
                string defaultAppSettingsJson = JsonSerializer.Serialize(new AppSettings(), new JsonSerializerOptions { WriteIndented = true });

                File.WriteAllText(poeTradeHelperAppSettingsPath, defaultAppSettingsJson);
            }
        }

        private static void ConfigureLogging()
        {
            var logger = Serilog.Log.Logger = new LoggerConfiguration()
                            .MinimumLevel.Is(Serilog.Events.LogEventLevel.Verbose)
                            .Enrich.WithExceptionDetails()
                            .WriteTo.Debug()
                            .WriteTo.File(Path.Combine(Environment.CurrentDirectory, "log.txt"), rollOnFileSizeLimit: true, retainedFileCountLimit: 1, fileSizeLimitBytes: 104857600).CreateLogger();

            Locator.CurrentMutable.UseSerilogFullLogger();
        }
    }
}