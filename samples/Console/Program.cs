using StackExchange.Profiling;
using MongoDB.Driver;
using MongoDB.Bson;
using MiniProfiler.MongoDB;

Console.WriteLine("=========================");
Console.WriteLine("MiniProfilerMongoDbEventSubscriber Console Sample");
Console.WriteLine("=========================\n");

const string connectionString = "mongodb://localhost:27017/?readPreference=primary&ssl=false&directConnection=true";

var mongoClientSettings = MongoClientSettings.FromUrl(new MongoUrl(connectionString));
mongoClientSettings.ClusterConfigurator = cb =>
{
    cb.Subscribe(new MiniProfilerMongoDbEventSubscriber()); // Hook MongoDriver Events to add MiniProfiler custom profiling data.
};
var dbClient = new MongoClient(mongoClientSettings);

var mp = StackExchange.Profiling.MiniProfiler.StartNew("Mongo Sample Query"); // Start MiniProfiler Scope

Console.WriteLine("The list of databases on this server is: ");
var dbList = dbClient.ListDatabases().ToList();
foreach (var db in dbList)
{
    Console.WriteLine(db);
}

Console.WriteLine("Connection to sample_mflix database");

var database = dbClient.GetDatabase("sample_mflix");

Console.WriteLine("Querying..");
var collection = database.GetCollection<BsonDocument>("movies");
var firstDocument = collection.Find(new BsonDocument()).FirstOrDefault();
Console.WriteLine(firstDocument.ToString());

mp.Stop();

Console.WriteLine("\n=========================");
Console.WriteLine("MiniProfiler Result:");
Console.WriteLine(StackExchange.Profiling.MiniProfiler.Current.RenderPlainText());
