using System;
using Microsoft.Extensions.Configuration;

namespace IIASA.Repository.Extensions
{
    public class SettingsExtensions
    {
        public static string GetConnectionString(string connStringName)
        {
            var environmentName = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
            var basePath = AppContext.BaseDirectory;

            var builder = new ConfigurationBuilder()
                .SetBasePath(basePath)
                .AddJsonFile("repositorysettings.json")
                .AddJsonFile($"repositorysettings.{environmentName}.json", true)
                .AddEnvironmentVariables();

            var config = builder.Build();

            var connectionString = config.GetValue<string>("Settings:ConnectionStrings:" + connStringName);

            if (string.IsNullOrWhiteSpace(connectionString))
                throw new Exception($"Empty connection string \"{connStringName}\".");

            if (string.IsNullOrEmpty(connectionString))
                throw new Exception($"{nameof(connectionString)} is null or empty.");

            return connectionString;
        }
    }
}
