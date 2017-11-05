using MongoDB.Bson.Serialization.Attributes;

namespace Relation.Test.Models
{
    public class Gender
    {
        [BsonId]
        public int Id { get; set; }

        public string Text { get; set; }
    }
}