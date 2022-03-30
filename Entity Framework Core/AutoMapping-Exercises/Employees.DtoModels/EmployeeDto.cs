namespace Employees.DtoModels
{
    public class EmployeeDto
    {
        public EmployeeDto()
        { }
        public EmployeeDto(string firstName, string lastName, decimal salary)
        {
            FirstName = firstName;
            LastName = lastName;
            Salary = salary;
        }
        public int Id { get; set; } 
        public string FirstName { get; set; } 
        public string LastName { get; set; } 
        public decimal Salary { get; set; }     
    }
}
