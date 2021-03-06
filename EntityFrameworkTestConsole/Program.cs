﻿using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Runtime.Remoting.Contexts;
using System.Text;
using System.Threading.Tasks;
using EntityFrameworkTestConsole.DataAccessLayer;
using EntityFrameworkTestConsole.Model;

namespace EntityFrameworkTestConsole
{
    internal class Program
    {
        private static void Main(string[] args)
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
            //ExplicitLoadIntoLocal();
            //EntryToAddNewEntity();
            //EntryToModifyExistingEntityWithoutLoadingFromDatabase();
            //EntryToModifyExistingEntityByLoadingFromDatabase();
            //EntryToModifyByPropertyChanged();
            //EntryToModifyByPropertyChangedWithoutUsingFind();
            //LoadASubSetOfACollection(); //<--- Still need work
            //LazyLoadingAndEagerLoadingSameResult();
            //DetectChanges();
            //CreateNewInstance();
            //AddEntityFromTheRootThroughReferences();
            //AddByIdInsteadOfProperty();
            WorkingWithEntryAndAllScalarProperties();
            UpdatingASpecificFieldOfATable();
            Console.ReadLine();
        }

        private static void UpdatingASpecificFieldOfATable()
        {
            Person person;

            //Create a person
            using (var context = new YourContext())
            {
                person = new Person { Name = "Update only a single property", BirthDate = DateTime.Now };
                context.Set<Person>().Add(person);
                context.SaveChanges();
            }

            //Update a single field
            using (var context = new YourContext())
            {
                context.Configuration.AutoDetectChangesEnabled = false;
                context.Configuration.ProxyCreationEnabled = false;
                context.Entry(person).State = EntityState.Unchanged; //In the context without any modification
                person.BirthDate = new DateTime(1984, 08, 01);
                person.Name = "This will not be saved in the database";
                context.Entry(person).Property(d => d.BirthDate).IsModified = true;
                //context.Entry(person).Property(d => d.Name).IsModified = false;
                context.SaveChanges();
            }
        }

        private static void WorkingWithEntryAndAllScalarProperties()
        {
            Person personLoaded;

            //Create a person
            using (var context = new YourContext())
            {
                var person = new Person { Name = "WorkingWithEntry", BirthDate = DateTime.Now };
                context.Set<Person>().Add(person);
                context.SaveChanges();
            }

            //Load the person
            using (var context = new YourContext())
            {
                personLoaded = context.Set<Person>().Single(sd => sd.Name == "WorkingWithEntry");
            }

            personLoaded.Name = "Modified Name";

            //Save the person (modified its properties)
            using (var context = new YourContext())
            {
                context.Configuration.AutoDetectChangesEnabled = false;
                context.Configuration.ProxyCreationEnabled = false;

                context.Entry(personLoaded).State = EntityState.Modified;
                context.SaveChanges();
          
                var copy = new Person();
                context.Entry(copy).State = EntityState.Added;//The context must know the entity to do the copy to.
                context.Entry(copy).CurrentValues.SetValues(personLoaded); //The context must also know the entity to copy from.
                copy.Name = "This is a copy AKA clone";
                context.SaveChanges();
            }
        }

        private static void AddByIdInsteadOfProperty()
        {
            using (var context = new YourContext())
            {
                var person = new Person { Name = "Automatically added from the property", BirthDate = DateTime.Now };
                context.Set<Person>().Add(person);
                context.SaveChanges();
                Console.WriteLine("Person id = " + person.Id);
                var car = new Car {OwnerId = person.Id, Type = "Honda"};
                context.Set<Car>().Add(car);
                context.SaveChanges();
                Console.WriteLine("Car id = " + car.Id);
            }
        }

        private static void AddEntityFromTheRootThroughReferences()
        {
            using (var context = new YourContext())
            {
                var newHouse = new House {Address = new Address{City="TestCity",Number = 123,Street="Street Name here"}};
                newHouse.Owner = new Person {Name = "Automatically added from the property", BirthDate = DateTime.Now};
                newHouse.Resident = new Collection<Person>(new []{new Person{BirthDate = DateTime.Now,Name = "Automatically added from the collection"}});
                context.Houses.Add(newHouse);
                context.SaveChanges();
            }
        }

        private static void CreateNewInstance()
        {
            using (var context = new YourContext())
            {
                // Create a new instance with Add
                var person = new Person {Name = "New person with New Keyword",BirthDate = DateTime.Now};
                Console.WriteLine("Person non-proxy state: " + context.Entry(person).State);
                context.Persons.Add(person);// Now Entity Framework added the object but was not tracking anything

                // Create a new instance with Entity Framework (proxy)
                var person2 = context.Persons.Create();
                Console.WriteLine("Person proxy state: " + context.Entry(person2).State);
                person2.Name = "New Person from EF";
                person2.BirthDate = DateTime.Now;
                context.Persons.Add(person2); // Still need to add but EF was tracking changes
                var x1 = context.Persons.Local.Count();
                Console.WriteLine("The count is at " + x1);

                // Detect Changes
                context.ChangeTracker.DetectChanges();
                Console.WriteLine("Person non-proxy state: " + context.Entry(person).State);
                Console.WriteLine("Person proxy state: " + context.Entry(person2).State);
                var x2 = context.Persons.Local.Count();
                Console.WriteLine("The count is at " + x2);

                context.SaveChanges();
            }
        }

        private static void DetectChanges()
        {
            using (var context = new YourContext())
            {
                context.Configuration.AutoDetectChangesEnabled = false;
                var personToModify = context.Persons.Find(1);
                personToModify.BirthDate = new DateTime(3051,12,12);
                Console.WriteLine(context.Entry(personToModify).State);
                context.ChangeTracker.DetectChanges(); //Remove this line and it will not save the entity modification
                Console.WriteLine(context.Entry(personToModify).State);
                context.SaveChanges();
            }
        }

        private static void LazyLoadingAndEagerLoadingSameResult()
        {
            using (var context = new YourContext())
            {
                Console.WriteLine("No Lazy Loading, Eager Loading");
                var person = context.Persons.Find(1);
                context.Entry(person).Reference(d=>d.Residence).Load();
                Console.WriteLine("City is " + person.Residence.Address.City);
            }
            using (var context = new YourContext())
            {
                context.Configuration.LazyLoadingEnabled = true;
                Console.WriteLine("Lazy Loading, No Eager Loading");
                var person = context.Persons.Find(1);
                Console.WriteLine("City is " + person.Residence.Address.City);
            }
        }

        /// <summary>
        /// This code does not work as it should.
        /// </summary>
        private static void LoadASubSetOfACollection()
        {
            var objectFromUser = new Person { Id = 1};
            using (var context = new YourContext())
            {
                objectFromUser = context.Persons.Find(objectFromUser.Id);
                //context.Entry(objectFromUser).Collection(d => d.Friends).Load();//Load all sub list

                //context.Entry(objectFromUser).Collection(d => d.Friends).Query().Where(f => f.Name.StartsWith("Seed")).Load(); // This fail and should not
                context.Entry(objectFromUser).Collection(d => d.Friends).Query().Load();// This also fail and should not

                Console.WriteLine(objectFromUser.Friends.Count);
            } 
        }

        private static void EntryToModifyByPropertyChangedWithoutUsingFind()
        {
            var objectFromUser = new Person { Id = 1, Name = "Tester #2", BirthDate = new DateTime(1941, 12, 25) };
            using (var context = new YourContext())
            {
                Console.WriteLine(context.Persons.Local.Count); //0
                context.Entry(objectFromUser).State = EntityState.Modified;
                Console.WriteLine(context.Persons.Local.Count); //1
                context.SaveChanges();
            }
        }


        private static void EntryToModifyByPropertyChanged()
        {
            var objectFromUser = new Person {Id = 1, Name="Test", BirthDate = new DateTime(1801, 12, 25)};
            using (var context = new YourContext())
            {
                Console.WriteLine(context.Persons.Local.Count); //0
                var existingPerson = context.Persons.Find(1);
                Console.WriteLine(context.Persons.Local.Count); //1
                context.Entry(existingPerson).CurrentValues.SetValues(objectFromUser);
                Console.WriteLine(context.Persons.Local.Count); //1
                context.SaveChanges();
            }
        }

        private static void EntryToModifyExistingEntityByLoadingFromDatabase()
        {
            using (var context = new YourContext())
            {
                Console.WriteLine(context.Persons.Local.Count); //0
                var existingPerson = context.Persons.Find(1);
                Console.WriteLine(context.Persons.Local.Count); //1
                existingPerson.Name = "Updated from database";
                Console.WriteLine(context.Persons.Local.Count); //1
                context.SaveChanges();
            }
        }

        private static void EntryToModifyExistingEntityWithoutLoadingFromDatabase()
        {
            using (var context = new YourContext())
            {
                Console.WriteLine(context.Persons.Local.Count); //0
                var existingPerson = new Person { Id=1, Name = "Updated Name"};
                context.Persons.Attach(existingPerson);
                var entryPerson = context.Entry(existingPerson);
                Console.WriteLine(context.Persons.Local.Count); //1
                entryPerson.Property(d => d.Name).IsModified = true;
                Console.WriteLine(context.Persons.Local.Count); //1
                context.SaveChanges();
            }
        }

        private static void EntryToAddNewEntity()
        {
            using (var context = new YourContext())
            {
                Console.WriteLine(context.Persons.Local.Count); //0
                var newPerson = new Person {Name = "New Person", BirthDate = new DateTime(1980, 1, 2) };
                var entryPerson = context.Entry(newPerson);
                Console.WriteLine(context.Persons.Local.Count); //0
                entryPerson.State = EntityState.Added;
                Console.WriteLine(context.Persons.Local.Count); //1
                context.SaveChanges();
            }
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
