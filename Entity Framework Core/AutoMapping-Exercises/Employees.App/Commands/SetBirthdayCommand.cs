namespace Employees.App.Commands
{
    using Contracts;
    using Employees.Services;
    using System;

    class SetBirthdayCommand : ICommand
    {
        private readonly EmployeeService employeeService;
        public SetBirthdayCommand(EmployeeService employeeService)
        {   
            this.employeeService = employeeService;
        }
        public string Execute(params string[] args)
        {
            int employeeId = int.Parse(args[0]);
            DateTime date = DateTime.ParseExact(args[1], "dd-MM-yyyy", null);

            var employeeName = employeeService.SetBirthday(employeeId, date);

            return $"{employeeName}'s birthday was set to {args[1]}";
        }
    }
}
