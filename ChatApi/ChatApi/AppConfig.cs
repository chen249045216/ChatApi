using Microsoft.Extensions.Configuration;
using MySql.Data.MySqlClient;
using Pomelo.EntityFrameworkCore.MySql.Storage;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace ChatApi
{
    public static class AppConfig
    {
        public static readonly int EfBatchSize =
            !string.IsNullOrWhiteSpace(Environment.GetEnvironmentVariable("EF_BATCH_SIZE"))
                ? Convert.ToInt32(Environment.GetEnvironmentVariable("EF_BATCH_SIZE"))
                : 1;

        public static readonly int EfRetryOnFailure =
            !string.IsNullOrWhiteSpace(Environment.GetEnvironmentVariable("EF_RETRY_ON_FAILURE"))
                ? Convert.ToInt32(Environment.GetEnvironmentVariable("EF_RETRY_ON_FAILURE"))
                : 0;

        public static readonly string EfDatabase = Environment.GetEnvironmentVariable("EF_DATABASE");
        public static readonly uint? EfPort = !string.IsNullOrWhiteSpace(Environment.GetEnvironmentVariable("EF_PORT"))
            ? (uint?)Convert.ToUInt32(Environment.GetEnvironmentVariable("EF_PORT"))
            : null;

        public static string ConnectionString
        {
            get
            {
                var csb = new MySqlConnectionStringBuilder(Config["Data:ConnectionString"]);

                if (EfPort.HasValue)
                {
                    csb.Port = EfPort.Value;
                }

                return csb.ConnectionString;
            }
        }
        public static Secret Secret
        {
            get
            {
                var secret = new Secret()
                {
                    Audience = Config["Secret:Audience"],
                    Issuer = Config["Secret:Issuer"],
                    JWT = Config["Secret:JWT"]
                };
                return secret;
            }
        }
        public static int ExpMinutes
        {
            get
            {
                return  int.Parse(Config["ExpMinutes"]);
            }
        }

        public static IConfigurationRoot Config => _lazyConfig.Value;

        private static readonly Lazy<IConfigurationRoot> _lazyConfig = new Lazy<IConfigurationRoot>(() =>
            new ConfigurationBuilder()
                .SetBasePath(Path.GetDirectoryName(new Uri(Assembly.GetExecutingAssembly().CodeBase).LocalPath))
                .AddJsonFile("appsettings.json", true)
                .AddJsonFile("config.json")
                .Build());
    }

    public class Secret
    {
        public string JWT { get; set; }
        public string Audience { get; set; }
        public string Issuer { get; set; }
    }
}
