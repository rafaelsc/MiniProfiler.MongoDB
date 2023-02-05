using MiniProfiler.MongoDB;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;
using Web.Models;
using Web.Services;

namespace Web;

public static class DependencyInjection
{
    public static IServiceCollection AddMongoClientWithProfiling(this IServiceCollection services, string connectionString)
    {
        services.AddSingleton<IMongoClient, MongoClient>(sp => CreateMongoClient(connectionString));
        RegisterMongoMapping();

        return services;

        static MongoClient CreateMongoClient(string connectionString)
        {
            var mongoClientSettings = MongoClientSettings.FromUrl(new MongoUrl(connectionString));
            mongoClientSettings.ClusterConfigurator = cb =>
            {
                cb.Subscribe(new MiniProfilerMongoDbEventSubscriber());
            };

            return new MongoClient(mongoClientSettings);
        }

        static void RegisterMongoMapping()
        {
            BsonClassMap.RegisterClassMap<Movie>(cm =>
            {
                cm.AutoMap(); 
                cm.SetIgnoreExtraElements(true);
            });
            //BsonClassMap.RegisterClassMap<Awards>(cm =>
            //{
            //    cm.AutoMap(); 
            //    cm.SetIgnoreExtraElements(true);
            //});
            //BsonClassMap.RegisterClassMap<Imdb>(cm =>
            //{
            //    cm.AutoMap(); 
            //    cm.SetIgnoreExtraElements(true);
            //});
            //BsonClassMap.RegisterClassMap<RottenTomatoes>(cm =>
            //{
            //    cm.AutoMap(); 
            //    cm.SetIgnoreExtraElements(true);
            //});
            //BsonClassMap.RegisterClassMap<TomatoRatingInfo>(cm =>
            //{
            //    cm.AutoMap();
            //    cm.SetIgnoreExtraElements(true);
            //});
        }
    }

    public static IServiceCollection AddMyServices(this IServiceCollection services)
    {
        services.AddScoped<MflixService>();
        return services;
    }
}