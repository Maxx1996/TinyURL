using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace TinyURL.Models
{
    public class UrlDbModel
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }
        public string CodedUrl { get; set; } = null!;
        public string LongUrl { get; set; } = null!;
    }
}

