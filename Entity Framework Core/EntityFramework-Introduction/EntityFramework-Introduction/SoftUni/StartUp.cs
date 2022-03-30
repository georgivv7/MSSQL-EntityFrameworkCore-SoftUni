using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

using SoftUni.Data;
using SoftUni.Models;

namespace SoftUni
{
    public class StartUp
    {
        static void Main(string[] args)
        {
            var context = new SoftUniContext();
            Console.WriteLine(RemoveTown(context));
        }
        public static string RemoveTown(SoftUniContext context)
        {
            var town = context.Towns.First(t => 
                                   t.Name == "Seattle");

            context.Employees
                   .Where(e => e.Address.Town == town)
                   .ToList()
                   .ForEach(e => e.AddressId = null);

            var addressesToRemove = context.Addresses
                   .Where(a => a.Town == town)
                   .ToList();

            int count = addressesToRemove.Count();

            foreach (var a in addressesToRemove)
            {
                context.Addresses.Remove(a);
            }

            context.Towns.Remove(town);

            context.SaveChanges();
            
            return $"{count} addresses in Seattle were deleted";
        }
        public static string DeleteProjectById(SoftUniContext context)
        {
            var sb = new StringBuilder();

            var employeesProjectToDelete = context.EmployeesProjects
                                                  .Where(ep => ep.ProjectId == 2);
           
            var project = context.Projects
                                 .Where(p => p.ProjectId == 2)
                                 .Single();

            foreach (var ep in employeesProjectToDelete)
            {
                context.EmployeesProjects.Remove(ep);
            }

            context.Projects.Remove(project);

            context.SaveChanges();

            context.Projects
                   .Take(10)
                   .Select(p => p.Name)
                   .ToList()
                   .ForEach(p => sb.AppendLine(p));

            return sb.ToString().Trim();
        }
        public static string GetEmployeesByFirstNameStartingWithSa(SoftUniContext context)
        {
            var sb = new StringBuilder();

            var employees = context.Employees
                                   .Where(e => e.FirstName.StartsWith("Sa"))
                                   .OrderBy(e => e.FirstName)
                                   .ThenBy(e => e.LastName)
                                   .Select(e => new
                                   {
                                       e.FirstName,
                                       e.LastName,
                                       e.JobTitle,
                                       e.Salary
                                   })
                                   .ToList();

            foreach (var e in employees)
            {
                sb.AppendLine($"{e.FirstName} {e.LastName} - {e.JobTitle} - (${e.Salary:F2})");
            }

            return sb.ToString().Trim();
        }
        public static string IncreaseSalaries(SoftUniContext context)
        {
            var sb = new StringBuilder();

            context.Employees
                   .Where(e => new string[] { "Marketing", "Engineering", "Tool Design", "Information Services" }
                   .Contains(e.Department.Name))
                   .ToList()
                   .ForEach(e => e.Salary *= 1.12M);

            context.SaveChanges();

            context.Employees
                   .Where(e => new string[] { "Marketing", "Engineering", "Tool Design", "Information Services" }
                   .Contains(e.Department.Name))
                   .Select(e => new
                   {
                       e.FirstName,
                       e.LastName,
                       e.Salary
                   })
                   .OrderBy(e => e.FirstName)
                   .ThenBy(e => e.LastName)
                   .ToList()
                   .ForEach(e=> sb.AppendLine($"{e.FirstName} {e.LastName} (${e.Salary:f2})"));
            
            return sb.ToString().Trim();
        }
        public static string GetLatestProjects(SoftUniContext context)
        {
            var sb = new StringBuilder();

            var projects = context.Projects
                                  .OrderByDescending(p => p.StartDate)
                                  .Take(10)
                                  .Select(p => new
                                  {
                                      p.Name,
                                      p.Description,
                                      StartDate = p.StartDate.ToString("M/d/yyyy h:mm:ss tt", CultureInfo.InvariantCulture)
                                  })
                                  .OrderBy(p=>p.Name)
                                  .ToList();

            foreach (var p in projects)
            {
                sb.AppendLine(p.Name)
                  .AppendLine(p.Description)
                  .AppendLine(p.StartDate);
            }
            return sb.ToString().Trim();
        }
        public static string GetDepartmentsWithMoreThan5Employees(SoftUniContext context)
        {
            var sb = new StringBuilder();

            var departmentsData = context.Departments
                                     .Where(ep => ep.Employees.Count > 5)
                                     .OrderBy(ep => ep.Employees.Count)
                                     .ThenBy(d => d.Name)
                                     .Select(d => new
                                     {
                                         DepartmentName = d.Name,
                                         ManagerFirstName = d.Manager.FirstName,
                                         ManagerLastName = d.Manager.LastName,
                                         DepartmentEmployees = d.Employees
                                                                .Select(e => new
                                                                {
                                                                    e.FirstName,
                                                                    e.LastName,
                                                                    e.JobTitle
                                                                })
                                                                .OrderBy(e => e.FirstName)
                                                                .ThenBy(e => e.LastName)
                                                                .ToList()

                                     })
                                     .ToList();

            foreach (var department in departmentsData)
            {
                sb.AppendLine($"{department.DepartmentName} - " +
                    $"{department.ManagerFirstName} {department.ManagerLastName}");

                foreach (var employee in department.DepartmentEmployees)
                {
                    sb.AppendLine($"{employee.FirstName} {employee.LastName} - " +
                        $"{employee.JobTitle}");
                }
            }
            return sb.ToString().Trim();
        }
        public static string GetEmployee147(SoftUniContext context)
        {
            StringBuilder sb = new StringBuilder();
            var employeeData = context.Employees
                                            .Select(e => new
                                            {
                                                EmployeeId = e.EmployeeId,
                                                FirstName = e.FirstName,
                                                LastName = e.LastName,
                                                JobTitle = e.JobTitle,
                                                Projects = e.EmployeesProjects
                                                                     .Select(p => p.Project.Name)
                                                                     .OrderBy(p => p)
                                                                     .ToList()
                                            })
                                            .SingleOrDefault(e => e.EmployeeId == 147);

            sb.AppendLine($"{employeeData.FirstName} " +
                          $"{employeeData.LastName} - " +
                          $"{employeeData.JobTitle}");

            foreach (var project in employeeData.Projects)
            {
                sb.AppendLine($"{project}");
            }

            return sb.ToString().Trim();
        }
        public static string GetAddressesByTown(SoftUniContext context)
        {
            StringBuilder sb = new StringBuilder();

            var addresses = context.Addresses
                                   .OrderByDescending(a => a.Employees.Count)
                                   .ThenBy(t => t.Town.Name)
                                   .ThenBy(a => a.AddressText)
                                   .Take(10)
                                   .Select(a => new
                                   {
                                       AddressText = a.AddressText,
                                       TownName = a.Town.Name,
                                       EmployeeCount = a.Employees.Count
                                   })
                                   .ToList();

            foreach (var address in addresses)
            {
                sb.AppendLine($"{address.AddressText}, {address.TownName} - {address.EmployeeCount} employees");
            }

            return sb.ToString().Trim();
        }

        public static string GetEmployeesInPeriod(SoftUniContext context)
        {
            StringBuilder sb = new StringBuilder();

            var employees = context.Employees
                                   .Where(e => e.EmployeesProjects.Any(ep => ep.Project.StartDate.Year >= 2001 &&
                                     ep.Project.StartDate.Year <= 2003))
                                   .Take(10)
                                   .Select(e => new
                                   {
                                       e.FirstName,
                                       e.LastName,
                                       ManagerFirstName = e.Manager.FirstName,
                                       ManagerLastName = e.Manager.LastName,
                                       Projects = e.EmployeesProjects
                                       .Select(ep => new
                                       {
                                           ProjectName = ep.Project.Name,
                                           StartDate = ep.Project.StartDate.ToString("M/d/yyyy h:mm:ss tt", CultureInfo.InvariantCulture),
                                           EndDate = ep.Project.EndDate.HasValue ?
                                           ep.Project.EndDate.Value.ToString("M/d/yyyy h:mm:ss tt", CultureInfo.InvariantCulture) : "not finished"
                                       })
                                       .ToList()
                                   })
                                   .ToList();

            foreach (var employee in employees)
            {
                sb.AppendLine($"{employee.FirstName} {employee.LastName}" +
                    $" - Manager: {employee.ManagerFirstName} {employee.ManagerLastName}");

                foreach (var project in employee.Projects)
                {
                    sb.AppendLine($"--{project.ProjectName} - {project.StartDate} - {project.EndDate}");
                }
            }
            return sb.ToString().Trim();
        }
        public static string AddNewAddressToEmployee(SoftUniContext context)
        {
            StringBuilder sb = new StringBuilder();
            Address address = new Address()
            {
                AddressText = "Vitoshka 15",
                TownId = 4
            };

            context.Addresses.Add(address);
            Employee employee = context.Employees
                                       .First(e => e.LastName == "Nakov");
            employee.Address = address;
            context.SaveChanges();

            List<string> employeeAddresses = context.Employees
                                                    .OrderByDescending(e => e.AddressId)
                                                    .Take(10)
                                                    .Select(e => e.Address.AddressText)
                                                    .ToList();

            foreach (var employeeAddress in employeeAddresses)
            {
                sb.AppendLine(employeeAddress);
            }

            return sb.ToString().Trim();
        }
        public static string GetEmployeesFromResearchAndDevelopment(SoftUniContext context)
        {
            StringBuilder sb = new StringBuilder();

            var employees = context.Employees
                .Where(e => e.Department.Name == "Research and Development")
                .OrderBy(e => e.Salary)
                .ThenByDescending(e => e.FirstName)
                .Select(e => new
                {
                    e.FirstName,
                    e.LastName,
                    Department = e.Department.Name,
                    e.Salary
                })
                .ToList();

            foreach (var item in employees)
            {
                sb.AppendLine($"{item.FirstName} {item.LastName} from {item.Department} - ${item.Salary:f2}");
            }
            return sb.ToString().Trim();

        }

        public static string GetEmployeesWithSalaryOver50000(SoftUniContext context)
        {
            StringBuilder sb = new StringBuilder();

            var employees = context.Employees
                .Where(e => e.Salary > 50000)
                .OrderBy(e => e.FirstName)
                .Select(e => new Employee
                {
                    FirstName = e.FirstName,

                    Salary = e.Salary
                })
                .ToList();

            foreach (var item in employees)
            {
                sb.AppendLine($"{item.FirstName} - {item.Salary:f2}");
            }

            return sb.ToString().Trim();

        }

        public static string GetEmployeesFullInformation(SoftUniContext context)
        {
            StringBuilder sb = new StringBuilder();

            var employees = context.Employees
                .OrderBy(e => e.EmployeeId)
                .Select(e => new Employee
                {
                    FirstName = e.FirstName,
                    LastName = e.LastName,
                    MiddleName = e.MiddleName,
                    JobTitle = e.JobTitle,
                    Salary = e.Salary
                })
                .ToList();

            foreach (var item in employees)
            {
                sb.AppendLine($"{item.FirstName} {item.LastName} {item.MiddleName} {item.JobTitle} {item.Salary:f2}");
            }

            return sb.ToString().Trim();
        }
    }
}
