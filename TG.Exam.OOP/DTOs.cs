using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Salestech.Exam.OOP;

namespace TG.Exam.OOP
{

    public class Employee : IOutputable
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public int Salary { get; set; }

        public string ToString2()
        {
            return $"{FirstName} {LastName}";
        }
    }

    public class SalesManager : Employee
    {
        public int BonusPerSale { get; set; }
        public int SalesThisMonth { get; set; }
    }

    public class CustomerServiceAgent : Employee
    {
        public int Customers { get; set; }
    }

    public class Dog : IOutputable
    {
        public string Name { get; set; }
        public int Age { get; set; }

        public string ToString2()
        {
            return $"{Name} {Age}";
        }
    }
}
