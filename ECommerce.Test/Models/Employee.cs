namespace ECommerce.Test.Models
{
    public class Employee
    {
        public Employee(string firstName, string lastName, int employeeNumber, int birthDay)
        {
            FirstName = firstName ??
                throw new ArgumentNullException(nameof(firstName));
            LastName = lastName ??
                throw new ArgumentNullException(nameof(lastName));
            EmployeeNumber = employeeNumber;
            BirthDay = birthDay;
        }
        public string FirstName
        {
            get;
            set;
        }
        public string LastName
        {
            get;
            set;
        }
        public int EmployeeNumber
        {
            get;
            set;
        }
        public string FullName
        {
            get
            {
                return $"{FirstName} {MiddleName} {LastName}";
            }
        }

        public int BirthDay
        {
            get;
            set;
        }

        public string MiddleName
        {
            get;
            set;
        }
    }
}
