using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntityFrameworkTestConsole.Model;

namespace EntityFrameworkTestConsole.DataAccessLayer
{

    public class YourContext : DbContext
    {
        static YourContext()
        {
            //Database.SetInitializer<YourContext>(new CreateDatabaseIfNotExists<YourContext>()); //Default one
            //Database.SetInitializer<YourContext>(new DropCreateDatabaseIfModelChanges<YourContext>()); //Drop database if changes detected
            //Database.SetInitializer<YourContext>(new DropCreateDatabaseAlways<YourContext>()); //Drop database every times
            Database.SetInitializer<YourContext>(new CustomInitializer<YourContext>()); //Custom if model changed and seed values
            //Database.SetInitializer<YourContext>(null); //Nothing is done
            
        }

        public YourContext(): base("DefaultConnection")
        {
            
        }
        public DbSet<Person> Persons { get; set; }
        public DbSet<House> Houses { get; set; }
        //public DbSet<Address> Address { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Person>().HasMany<Person>(s => s.Friends).WithMany().Map(c =>
            {
                c.MapLeftKey("Person_id");
                c.MapRightKey("FriendPerson_id");
                c.ToTable("PersonFriend");
            });

            base.OnModelCreating(modelBuilder);
        }
    }

    public class CustomInitializer<T> : DropCreateDatabaseIfModelChanges<YourContext>
    {
        public override void InitializeDatabase(YourContext context)
        {
            context.Database.ExecuteSqlCommand(TransactionalBehavior.DoNotEnsureTransaction
                , string.Format("ALTER DATABASE {0} SET SINGLE_USER WITH ROLLBACK IMMEDIATE", context.Database.Connection.Database));
        
            base.InitializeDatabase(context);
        }

        protected override void Seed(YourContext context)
        {
  
            var person2 = new Person() { Id = 2, Name = "SeededPerson2", BirthDate = new DateTime(1900, 1, 1) };
            context.Persons.Add(person2);
            var person3 = new Person() { Id = 3, Name = "Person3", BirthDate = new DateTime(1900, 1, 1) };
            context.Persons.Add(person3);

            var person = new Person {Id = 1, Name = "SeededPerson", BirthDate = new DateTime(1900, 1, 1), Friends = new Collection<Person> {person2, person3}};
            person.Residence = new House {Id = 1, Address = new Address{City="Montreal", Number = 123, Street = "Owl"}, Price = 350000};
            context.Persons.Add(person);
          
            base.Seed(context);
        }
    }
}
