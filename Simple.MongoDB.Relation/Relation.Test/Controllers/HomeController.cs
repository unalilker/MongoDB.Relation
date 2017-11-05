using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using MongoDB.Driver;
using Relation.Test.Models;
using Simple.MongoDB.Relation;

namespace Relation.Test.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
           // var client = new MongoClient("yourconnectionstring");
            var client = new MongoClient("mongodb://admin:9yuXy9bbw9SWiVVs@atlas-shard-00-00-vjqkb.mongodb.net:27017,atlas-shard-00-01-vjqkb.mongodb.net:27017,atlas-shard-00-02-vjqkb.mongodb.net:27017/test?ssl=true&replicaSet=Atlas-shard-0&authSource=admin");
            var database = client.GetDatabase("test");

            var classesCollection = database.GetCollection<Class>("Classes");
            var gendersCollection = database.GetCollection<Gender>("Genders");
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
      
            return Json(students.ToList());
        }

    }
}
