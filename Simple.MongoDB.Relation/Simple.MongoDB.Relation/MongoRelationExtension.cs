using System;
using System.Collections.Generic;
using System.Linq;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;
using Simple.MongoDB.Relation.Attributes;
using Simple.MongoDB.Relation.Methods;
using Simple.MongoDB.Relation.Models;

namespace Simple.MongoDB.Relation
{
    public static class MongoRelationExtension
    {
        public static IAggregateFluent<T> FindWithRelations<T>(this IMongoCollection<T> collection, bool preserveNullAndEmptyArrays = true, bool isCamelCase = false)
        {
            var entity = collection.Aggregate();

            IAggregateFluent<BsonDocument> aggregateFluent = null;
            var projectDictionary = new Dictionary<string, object>();
            var typeofModel = typeof(T);
            var typeProperties = typeofModel.GetProperties();
           

            foreach (var property in typeProperties)
            {
                var propName = isCamelCase ? property.Name.FirstCharacterToLower() : property.Name;
                var bsonIgnoreAttributes = property.GetCustomAttributes(typeof(BsonIgnoreIfDefaultAttribute), true).ToList();
                if (!bsonIgnoreAttributes.Any()) projectDictionary.Add(propName, 1);

                var relationAttributes = property.GetCustomAttributes(typeof(MongoRelationAttribute), true).ToList();
                if (!relationAttributes.Any()) continue;


                var attribute = relationAttributes.Cast<MongoRelationAttribute>().First();
                if (string.IsNullOrWhiteSpace(attribute.LocalFieldId)) throw new ArgumentNullException(nameof(attribute.LocalFieldId), $"Please set a LocalFieldId value at MongoRelationAttribute. Model: ({typeofModel.FullName}) & Property : ({propName})");

                var fieldToGet = isCamelCase ? attribute.PrincipalValueField.ToLower() : attribute.PrincipalValueField;
                var unwindProperty = GetUnwindProperty(attribute);
                aggregateFluent = aggregateFluent.GetAggregateFluent(entity, attribute, unwindProperty);

                var unwindOptions = new AggregateUnwindOptions<BsonDocument> { PreserveNullAndEmptyArrays = preserveNullAndEmptyArrays };

                switch (property.GetPropertyType())
                {
                    case ReturnTypes.Value:
                        if (string.IsNullOrWhiteSpace(fieldToGet)) throw new ArgumentNullException(nameof(fieldToGet), $"ForeignValueField can not be empty if returntype is value. Model: ({typeofModel.FullName}) & Property : ({propName})");
                        aggregateFluent = aggregateFluent.Unwind(unwindProperty, unwindOptions);
                        projectDictionary.Add(propName, $"${unwindProperty}.{fieldToGet}");
                        break;
                    case ReturnTypes.Object:
                        aggregateFluent = aggregateFluent.Unwind(unwindProperty, unwindOptions);
                        projectDictionary.Add(propName, "$" + unwindProperty);
                        break;
                    case ReturnTypes.List:
                        projectDictionary.Add(propName, "$" + unwindProperty);
                        break;
                }
            }

            var projection = GetProjectQuery(projectDictionary);

            return aggregateFluent?.Project<T>(projection);
        }


        private static string GetUnwindProperty(MongoRelationAttribute attribute)
        {
            var uniqueStringForUnwind = Helpers.CreateUniqueString();
            return $"rel_{attribute.FromCollection}_{attribute.LocalFieldId}_{uniqueStringForUnwind}";
        }

        private static IAggregateFluent<BsonDocument> GetAggregateFluent<T>(this IAggregateFluent<BsonDocument> aggregateFluent, IAggregateFluent<T> entity, MongoRelationAttribute attribute, string unwindProperty)
        {
            return aggregateFluent == null ?
                entity.Lookup(attribute.FromCollection, attribute.LocalFieldId, attribute.PrincipalFieldId, unwindProperty) :
                aggregateFluent.Lookup(attribute.FromCollection, attribute.LocalFieldId, attribute.PrincipalFieldId, unwindProperty);
        }


        private static BsonDocument GetProjectQuery(Dictionary<string, object> projectDictionary)
        {
            var projection = new BsonDocument();
            projection.AddRange(projectDictionary);
            return projection;
        }
    }
}
