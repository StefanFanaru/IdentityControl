using IdentityControl.API.Data;
using MercuryBus.Helpers;
using MercuryBus.Local.Kafka.Consumer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace IdentityControl.API.Configuration
{
    public static class MercuryBus
    {
        private static readonly IConfiguration Configuration = Startup.StaticConfiguration;

        public static IServiceCollection AddMercuryBus(this IServiceCollection services)
        {
            services.AddMercuryBusSqlKafkaTransport("mercury", Configuration["Settings:KafkaBoostrapServers"],
                MercuryKafkaConsumerConfigurationProperties.Empty(),
                (serviceProvider, dbContextOptions) =>
                {
                    var appDbContext = serviceProvider.GetRequiredService<IdentityContext>();
                    dbContextOptions.UseSqlServer(appDbContext.Database.GetDbConnection());
                });

            services.AddMercuryBusEventsPublisher();

            return services;
        }
    }
}
