using System;
using System.IO;
using IIASA.Repository.Migrations;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using NLog.Web;

namespace IIASA
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var logger = NLogBuilder.ConfigureNLog("NLog.config").GetCurrentClassLogger();

            try
            {
                logger.Info("Starting IIASA.API.");

                var host = CreateWebHostBuilder(args).Build();

                InitDatabase(host);

                host.Run();

                logger.Info("Host run IIASA.API.");
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Stopped IIASA.API because of exception.");
                throw;
            }
            finally
            {
                NLog.LogManager.Shutdown();
            }
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args)
        {
            return WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>()
                .ConfigureLogging(logging =>
                {
                    logging.ClearProviders();
                    logging.SetMinimumLevel(LogLevel.Trace);

                    logging.AddConsole();
                    logging.AddDebug();
                })
                .UseNLog();
        }

        private static void InitDatabase(IWebHost host)
        {
            using (var dataContext = new MigrationsContextFactory().CreateDbContext(null))
            {
                dataContext.Database.Migrate();
            }
        }
    }
}
