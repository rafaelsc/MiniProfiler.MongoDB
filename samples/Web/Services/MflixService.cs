using MongoAsyncEnumerableAdapter;
using MongoDB.Driver;
using Web.Models;

namespace Web.Services
{
    public class MflixService
    {
        private readonly IMongoCollection<Movie> mongoCollection;

        public MflixService(IMongoClient mongoClient)
        {
            var database = mongoClient.GetDatabase("sample_mflix");
            mongoCollection = database.GetCollection<Movie>("movies");
        }

        public IEnumerable<Movie> ListAll()
        {
            var result = mongoCollection.Find(movie => true);
            return result.ToEnumerable();
        }

        public async IAsyncEnumerable<Movie> ListAllAsync()
        {
            var result = await mongoCollection.FindAsync(movie => true);
            await foreach (var movie in result.ToAsyncEnumerable())
            {
                yield return movie;
            }
        }
    }
}
