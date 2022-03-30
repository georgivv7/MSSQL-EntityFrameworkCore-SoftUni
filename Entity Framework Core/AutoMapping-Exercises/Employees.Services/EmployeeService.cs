namespace Employees.Services
{
    using Employees.Data;
    using Employees.Models;
    using Employees.DtoModels;
    using System;
    using AutoMapper;

    public class EmployeeService
    {
        private readonly EmployeesDbContext context;
        public EmployeeService(EmployeesDbContext context)
        {
            this.context = context;
        }

        public EmployeeDto ById(int employeeId)
        {
            var employee = context.Employees.Find(employeeId);
            var config = new MapperConfiguration(cfg => cfg.CreateMap<Employee, EmployeeDto>());
            var mapper = new Mapper(config);
            var employeeDto = mapper.Map<EmployeeDto>(employee);

            return employeeDto;
        }
        public void AddEmployee(EmployeeDto dto)
        {
            var config = new MapperConfiguration(cfg => cfg.CreateMap<EmployeeDto, Employee>());
            var mapper = new Mapper(config);
            var employee = mapper.Map<Employee>(dto);

            context.Employees.Add(employee);
            context.SaveChanges();
        }

        public string SetBirthday(int employeeId, DateTime date)
        {
            var employee = context.Employees.Find(employeeId);

            employee.Birthday = date;

            context.SaveChanges();

            return $"{employee.FirstName} {employee.LastName}";
        }

        public string SetAddress(int employeeId, string address)
        {
            var employee = context.Employees.Find(employeeId);

            employee.Address = address;

            context.SaveChanges();

            return $"{employee.FirstName} {employee.LastName}";
        }

        public EmployeePersonalDto PersonalById(int employeeId)
        {
            var employee = context.Employees.Find(employeeId);
            var config = new MapperConfiguration(cfg => cfg.CreateMap<Employee, EmployeePersonalDto>());
            var mapper = new Mapper(config);

            var employeeDto = mapper.Map<EmployeePersonalDto>(employee);
             
            return employeeDto;
        }
    }
}
