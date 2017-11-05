using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using Simple.MongoDB.Relation.Attributes;

namespace Relation.Test.Models
{
    public class Student
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        public string Name { get; set; }

        [BsonRepresentation(BsonType.ObjectId)]
        public string ClassId { get; set; }

        public int GenderId { get; set; }

        [MongoRelation(FromCollection = "Genders", LocalFieldId = nameof(GenderId), PrincipalValueField = "Text", PrincipalFieldId = "_id")]
        [BsonIgnoreIfDefault]
        public string GenderText { get; set; }

        [MongoRelation(FromCollection = "Classes", LocalFieldId = nameof(ClassId))]
        [BsonIgnoreIfDefault]
        public Class StudentClass { get; set; }
    }
}