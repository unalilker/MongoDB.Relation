using System.Collections.Generic;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using Simple.MongoDB.Relation.Attributes;

namespace Relation.Test.Models
{
    public class Class
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        public string Name { get; set; }

        [BsonRepresentation(BsonType.ObjectId)]
        [BsonIgnoreIfNull]
        public List<string> StudentIds { get; set; }

        [MongoRelation(FromCollection = "Students", LocalFieldId = nameof(StudentIds))]
        [BsonIgnoreIfDefault]
        public List<Student> Students { get; set; }
    }
}