using System;
using System.Collections.Generic;
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
            BuildDatabase();
            //BuildByAddingEntity();
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
