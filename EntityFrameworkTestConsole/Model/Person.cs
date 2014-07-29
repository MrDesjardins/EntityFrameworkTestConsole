using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EntityFrameworkTestConsole.Model
{
    public class Person
    {
        public int Id { get; set; }
        public string Name { get; set; }
        
        public DateTime BirthDate { get; set; }

        public ICollection<Person> Friends { get; set; }

        public virtual House Residence { set; get; }
    }

    public class House
    {
        public int Id { get; set; }
        public double Price { get; set; }

        public Address Address { get; set; }

        public ICollection<Person> Resident { get; set; }
        public Person Owner { get; set; }
    }

    public class Address
    {
        public string Street { get; set; }
        public int Number { get; set; }
        public string City { get; set; }
    }

    public class Car
    {
        public int Id { get; set; }
        public string Type { get; set; }
        public Person Owner { get; set; }
        public int OwnerId { get; set; }
    }


}
