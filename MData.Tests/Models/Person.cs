using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MData.Tests.Models
{
    public sealed class Person
    {
        public string Name { get; set; }
        public string FamilyName { get; set; }
        public DateTime BirthDate { get; set; }
        public Color FavoriteColor { get; set; }
        public Person Father { get; set; }
        public Person Mother { get; set; }
        public List<Person> Children { get; set; }
    }
}