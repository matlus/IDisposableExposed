using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DomainLayer
{
    internal sealed class ConfigurationProvider
    {
        private readonly IConfigurationRoot _configurationRoot;
        public ConfigurationProvider()
        {
            var configurationBuilder = new ConfigurationBuilder();
            configurationBuilder.AddJsonFile("appsettings.json");
            LoadEnvironmentSpecificAppSettings(configurationBuilder);
            _configurationRoot = configurationBuilder.Build();
        }

        [ExcludeFromCodeCoverage]
        internal ConfigurationProvider(IConfigurationRoot configurationRoot)
        {
            _configurationRoot = configurationRoot;
        }

        private static void LoadEnvironmentSpecificAppSettings(ConfigurationBuilder configurationBuilder)
        {
            var aspNetCoreEnvironment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
            if (aspNetCoreEnvironment != null)
            {
                configurationBuilder.AddJsonFile($"appsettings.{aspNetCoreEnvironment}.json");
            }
        }

        private string RetrieveConfigurationSettingValue(string key)
        {
            return _configurationRoot[key];
        }

        public string GetImdbServiceBaseUrl()
        {
            return RetrieveConfigurationSettingValue("ImdbServiceBaseUrl");
        }

        public IConfiguration GetLoggingConfiguration()
        {
            return _configurationRoot.GetSection("Logging");
        }

        public string GetAppInsightsInstrumentationKey()
        {
            return RetrieveConfigurationSettingValue("ApplicationInsights:InstrumentationKey");
        }
    }
}
