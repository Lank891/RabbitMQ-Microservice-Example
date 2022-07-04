using Common.Logs;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Common
{
    public class Utility
    {
        public static Dictionary<TKey, TValue> BuildDictionary<TKey, TValue>(Func<Dictionary<TKey, TValue>> dictionaryBuilder) where TKey : notnull => dictionaryBuilder();

        public static IHostBuilder CreateHostBuilder(string[] args) =>
        Host.CreateDefaultBuilder(args)
            .ConfigureLogging(builder =>
                builder.ClearProviders()
                    .AddColorConsoleLogger(configuration =>
                    {
                    }));
    }
}
