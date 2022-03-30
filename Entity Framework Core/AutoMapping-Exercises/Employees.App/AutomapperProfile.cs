namespace Employees.App
{
    using AutoMapper;
    using Employees.DtoModels;
    using Employees.Models;

    public class AutomapperProfile : Profile
    {
        public AutomapperProfile()
        {
            CreateMap<Employee, EmployeeDto>();
            CreateMap<EmployeeDto, Employee>();
        }
    }
}
