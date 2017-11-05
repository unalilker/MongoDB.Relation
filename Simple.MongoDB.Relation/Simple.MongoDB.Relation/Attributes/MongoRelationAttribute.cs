using System;

namespace Simple.MongoDB.Relation.Attributes
{
    public class MongoRelationAttribute : Attribute
    {
        public string FromCollection;

        public string LocalFieldId;

        public string PrincipalFieldId = "_id";

        public string PrincipalValueField = "Name";
    }
}
