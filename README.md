# Simple.MongoDB.Relation

You can create simple (one level deep) relations with attributes

## Installing

You can install package directly from nuget.org 

Install-Package Simple.MongoDB.Relation -Version 1.0.0

## Configuration

You can decorate your model properties with MongoRelation attirube.

### Definition of Attribute Properties

**FromCollection:** Name of principal (remote) collection  
**LocalFieldId:** Name of local property which will be used for relation.  
**ForeignValueField:** If relation property is a primitive type. This field will be taken from principal entity and will be setted to our relation field. Default value is "Name".  
**PrincipalFieldId:** Principal collection Id field, which will be matched with LocalFieldId. Default value is "_id".  

##
### Model Configurations

**IMPORTANT: You must decorate relation properties with BsonIgnoreIfDefault attribute.**


```
    public class Student
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        public string Name { get; set; }

        [BsonRepresentation(BsonType.ObjectId)]
        public string ClassId { get; set; }

        public int GenderId { get; set; }

        [MongoRelation(FromCollection = "Genders", LocalFieldId = nameof(GenderId), ForeignValueField = "Text", ForeignFieldId = "_id")]
        [BsonIgnoreIfDefault]
        public string GenderText { get; set; }

        [MongoRelation(FromCollection = "Classes", LocalFieldId = nameof(ClassId))]
        [BsonIgnoreIfDefault]
        public Class StudentClass { get; set; }
    }
```
**Explanation of relation at GenderText:** Relation will use GenderId in our class, and look for _id field at Genders collection. If a matched record found, it will get Text property in matched record and set the value to our GenderText field.

**Explanation of relation at StudentClass:** Relation will use ClassId in our class, and look for _id field at Classes collection. If a matched record found, it will get matched record and set the record to our StudentClass field.

##

```
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
```
**Explanation of relation at Students:** Relation will use StudentIds in our class, and look for _id field at Students collection. If matched records found, it will get matched records and set the value to our Students field. 

##

```
    public class Gender
    {
        [BsonId]
        public int Id { get; set; }

        public string Text { get; set; }
    }
```


## Usage

Import Reference
```
using Simple.MongoDB.Relation;
```


```
    var client = new MongoClient("yourconnectionstring");
    var database = client.GetDatabase("test");

    var classesCollection = database.GetCollection<Class>("Classes");
    var studentsCollection = database.GetCollection<Student>("Students");

    if (!classesCollection.Find(x => true).ToList().Any())
    {
        var classOne = new Class()
        {
            Id = ObjectId.GenerateNewId().ToString(),
            Name = "Class One"
        };

        classesCollection.InsertOne(classOne);

        var genderMale = new Gender()
        {
            Id = 1,
            Text = "Male"
        };

        var genderFemale = new Gender()
        {
            Id = 2,
            Text = "Female"
        };

        gendersCollection.InsertOne(genderMale);
        gendersCollection.InsertOne(genderFemale);


        var student1 = new Student()
        {
            Id = ObjectId.GenerateNewId().ToString(),
            ClassId = classOne.Id,
            GenderId = genderMale.Id,
            Name = "Barrack Obama"
        };

        var student2 = new Student()
        {
            Id = ObjectId.GenerateNewId().ToString(),
            ClassId = classOne.Id,
            GenderId = genderMale.Id,
            Name = "Vladimir Putin"
        };

        var student3 = new Student()
        {
            Id = ObjectId.GenerateNewId().ToString(),
            ClassId = classOne.Id,
            GenderId = genderFemale.Id,
            Name = "Hillary Clinton"
        };

        studentsCollection.InsertOne(student1);
        studentsCollection.InsertOne(student2);
        studentsCollection.InsertOne(student3);


        classOne.StudentIds = new List<string>()
        {
            student1.Id, student2.Id, student3.Id
        };

        classesCollection.ReplaceOne(x => x.Id == classOne.Id, classOne);
    }

    var students = studentsCollection.FindWithRelations();
    var classes = classesCollection.FindWithRelations();

```
### Results

Result for return Json(students.ToList());
```
  [
   {
      "id":"59fe19d9f05d8c237858b96e",
      "name":"Barrack Obama",
      "classId":"59fe19d6f05d8c237858b96d",
      "genderId":1,
      "genderText":"Male",
      "studentClass":{
         "id":"59fe19d6f05d8c237858b96d",
         "name":"Class One",
         "studentIds":[
            "59fe19d9f05d8c237858b96e",
            "59fe19d9f05d8c237858b96f",
            "59fe19d9f05d8c237858b970"
         ],
         "students":null
      }
   },
   {
      "id":"59fe19d9f05d8c237858b96f",
      "name":"Vladimir Putin",
      "classId":"59fe19d6f05d8c237858b96d",
      "genderId":1,
      "genderText":"Male",
      "studentClass":{
         "id":"59fe19d6f05d8c237858b96d",
         "name":"Class One",
         "studentIds":[
            "59fe19d9f05d8c237858b96e",
            "59fe19d9f05d8c237858b96f",
            "59fe19d9f05d8c237858b970"
         ],
         "students":null
      }
   },
   {
      "id":"59fe19d9f05d8c237858b970",
      "name":"Hillary Clinton",
      "classId":"59fe19d6f05d8c237858b96d",
      "genderId":2,
      "genderText":"Female",
      "studentClass":{
         "id":"59fe19d6f05d8c237858b96d",
         "name":"Class One",
         "studentIds":[
            "59fe19d9f05d8c237858b96e",
            "59fe19d9f05d8c237858b96f",
            "59fe19d9f05d8c237858b970"
         ],
         "students":null
      }
   }
  ]
```
##
Result for return Json(classes.ToList());
```
  [
   {
      "id":"59fe19d6f05d8c237858b96d",
      "name":"Class One",
      "studentIds":[
         "59fe19d9f05d8c237858b96e",
         "59fe19d9f05d8c237858b96f",
         "59fe19d9f05d8c237858b970"
      ],
      "students":[
         {
            "id":"59fe19d9f05d8c237858b96e",
            "name":"Barrack Obama",
            "classId":"59fe19d6f05d8c237858b96d",
            "genderId":1,
            "genderText":null,
            "studentClass":null
         },
         {
            "id":"59fe19d9f05d8c237858b96f",
            "name":"Vladimir Putin",
            "classId":"59fe19d6f05d8c237858b96d",
            "genderId":1,
            "genderText":null,
            "studentClass":null
         },
         {
            "id":"59fe19d9f05d8c237858b970",
            "name":"Hillary Clinton",
            "classId":"59fe19d6f05d8c237858b96d",
            "genderId":2,
            "genderText":null,
            "studentClass":null
         }
      ]
   }
  ]
```


## Authors

* **İlker Ünal** 


## License

This project is licensed under the MIT License.

