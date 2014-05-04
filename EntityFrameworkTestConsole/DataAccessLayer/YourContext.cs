using System;
using System.Collections.Generic;
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
            //Database.SetInitializer<YourContext>(new CustomInitializer<YourContext>()); //Custom if model changed and seed values
            Database.SetInitializer<YourContext>(null); //Nothing is done
        }

        public YourContext(): base("DefaultConnection")
        {

        }
        public DbSet<Person> Persons { get; set; }
        public DbSet<House> Houses { get; set; }
        //public DbSet<Address> Address { get; set; }
    }

    public class CustomInitializer<T> : DropCreateDatabaseAlways<YourContext>
    {
        public override void InitializeDatabase(YourContext context)
        {
            context.Database.ExecuteSqlCommand(TransactionalBehavior.DoNotEnsureTransaction
                , string.Format("ALTER DATABASE {0} SET SINGLE_USER WITH ROLLBACK IMMEDIATE", context.Database.Connection.Database));
        
            base.InitializeDatabase(context);
        }

        protected override void Seed(YourContext context)
        {
            var person = new Person() {Name = "SeededPerson", BirthDate = new DateTime(1900, 1, 1)};
            context.Persons.Add(person);
            base.Seed(context);
        }
    }
}
