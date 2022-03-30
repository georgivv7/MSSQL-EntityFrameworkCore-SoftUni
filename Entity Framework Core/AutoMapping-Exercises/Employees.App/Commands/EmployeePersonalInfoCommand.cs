namespace Employees.App.Commands
{
    using Contracts;
    using Employees.Services;
    using System;

    class EmployeePersonalInfoCommand : ICommand
    {
        private readonly EmployeeService employeeService;
        public EmployeePersonalInfoCommand(EmployeeService employeeService)
        {
            this.employeeService = employeeService;
        }
        public string Execute(params string[] args)
        {
            int employeeId = int.Parse(args[0]);

            var employee = employeeService.PersonalById(employeeId);

            var birthday = "[no birthday specified]";

            if (employee.Birthday != null)
            {
                birthday = employee.Birthday.Value.ToString("dd-MM-yyyy");
            }
                
            var address = employee.Address ?? "[no address specified]";

            string result = $"ID: {employeeId} - {employee.FirstName} {employee.LastName} - ${employee.Salary:F2}" + Environment.NewLine +
                             $"Birthday: {birthday}" + Environment.NewLine +
                             $"Address: {address}";

            return result;
        }
    }
}
