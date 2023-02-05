using MongoDB.Bson;

namespace Web.Models
{
    public record Movie
    {
        public ObjectId id { get; set; }

        public string? title { get; set; }
    }
}
