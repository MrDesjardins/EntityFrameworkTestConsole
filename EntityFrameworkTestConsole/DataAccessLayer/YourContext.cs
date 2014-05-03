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
        public YourContext(): base("DefaultConnection")
        {

        }
        public DbSet<Person> Persons { get; set; }
        public DbSet<House> Houses { get; set; }
        //public DbSet<Address> Address { get; set; }
    }
    
}
