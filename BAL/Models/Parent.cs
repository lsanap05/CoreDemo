using System;
using System.Collections.Generic; // Required if adding relationships later

namespace BAL.Models
{
    public class Parent
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }

        // Potential relationship to Student(s) - can be added later
        // public virtual ICollection<Student> Children { get; set; }
    }
}
