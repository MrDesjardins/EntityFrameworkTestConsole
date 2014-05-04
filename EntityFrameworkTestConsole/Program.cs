using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Runtime.Remoting.Contexts;
using System.Text;
using System.Threading.Tasks;
using EntityFrameworkTestConsole.DataAccessLayer;
using EntityFrameworkTestConsole.Model;

namespace EntityFrameworkTestConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            //BuildDatabase();
            //BuildByAddingEntity();
            //FindUser();
            //FindUserDoesNotExist();
            //FindUserWithSingleOrDefault();
            //FindUserNotInDatabaseButInContext();
            //DoubleSingleOrDefaultDatabase();
            //DoubleFindDatabase();
            //SingleOrDefaultNotInDatabaseButInContext();
            //QueryLocal();
            ExplicitLoadIntoLocal();
            Console.ReadLine();
        }

        private static void ExplicitLoadIntoLocal()
        {
            using (var context = new YourContext())
            {
                Console.WriteLine(context.Persons.Local.Count); //0
                context.Persons.Load();
                Console.WriteLine(context.Persons.Local.Count); //1
                context.Persons.Load(); //Call the database one more time
            }
        }

        private static void QueryLocal()
        {
            using (var context = new YourContext())
            {
                Console.WriteLine(context.Persons.Local.Count); //0
                var newPerson = new Person { Id = 500, Name = "New Person", BirthDate = new DateTime(2000, 1, 2) };
                context.Persons.Add(newPerson);
                Console.WriteLine(context.Persons.Local.Count); //1
                var person = context.Persons.SingleOrDefault(d => d.Id == 1); //Query the database
                Console.WriteLine(context.Persons.Local.Count); //2
                var person2 = context.Persons.SingleOrDefault(d => d.Id == 1); //Query the database again
                Console.WriteLine(context.Persons.Local.Count); //2
                var person3 = context.Persons.Local.SingleOrDefault(d => d.Id == 1); //Does not query the database
                Console.WriteLine(context.Persons.Local.Count); //2
            }
        }

        private static void SingleOrDefaultNotInDatabaseButInContext()
        {
            using (var context = new YourContext())
            {
                Console.WriteLine(context.Persons.Local.Count); //0
                var newPerson = new Person {Id=500, Name = "New Person", BirthDate = new DateTime(2000, 1, 2)};
                context.Persons.Add(newPerson);
                Console.WriteLine(context.Persons.Local.Count); //1
                var person = context.Persons.SingleOrDefault(d=>d.Id==500);
                //Console.WriteLine(person.Name);//Commented because would crash. Person is NULL
                Console.WriteLine(context.Persons.Local.Count); //1
            }
        }

        private static void DoubleFindDatabase()
        {
            using (var context = new YourContext())
            {
                Console.WriteLine(context.Persons.Local.Count); //0
                var person = context.Persons.Find(1);
                Console.WriteLine(person.Name);
                Console.WriteLine(context.Persons.Local.Count); //1
                var person2 = context.Persons.Find(1);
                Console.WriteLine(person2.Name);
                Console.WriteLine(context.Persons.Local.Count); //1
            } 
        }

        private static void DoubleSingleOrDefaultDatabase()
        {
            using (var context = new YourContext())
            {
                var person = context.Persons.SingleOrDefault(d => d.Id == 1);
                Console.WriteLine(person.Name);
                var person2 = context.Persons.SingleOrDefault(d => d.Id == 1);
                Console.WriteLine(person2.Name);
            } 
        }

        private static void FindUserNotInDatabaseButInContext()
        {
            using (var context = new YourContext())
            {
                Console.WriteLine(context.Persons.Local.Count); //0
                var newPerson = new Person {Id=500, Name = "New Person", BirthDate = new DateTime(2000, 1, 2)};
                context.Persons.Add(newPerson);
                Console.WriteLine(context.Persons.Local.Count); //1
                var person = context.Persons.Find(500);
                Console.WriteLine(person.Name);
                Console.WriteLine(context.Persons.Local.Count); //1
            }
        }

        private static void FindUserWithSingleOrDefault()
        {
            using (var context = new YourContext())
            {
                var person = context.Persons.SingleOrDefault(d=>d.Id==1);
                Console.WriteLine(person.Name);
            } 
        }

        private static void FindUser()
        {
            using (var context = new YourContext())
            {
                var person = context.Persons.Find(1);
                Console.WriteLine(person.Name);
            }
        }

        private static void FindUserDoesNotExist()
        {
            using (var context = new YourContext())
            {
                var person = context.Persons.Find(2);
                Console.WriteLine(person.Name);
            }
        }


        public static void BuildDatabase()
        {
            using (var context = new YourContext())
            {
                context.Database.Initialize(true);
            }
        }

        public static void BuildByAddingEntity()
        {
            using (var context = new YourContext())
            {
                var stud = new Person() { Name = "Person1", BirthDate = new DateTime(1990, 01, 01) };
                context.Persons.Add(stud);
                context.SaveChanges(); 
            }
        }


    }
}
