using AutoMapper;
using HRbackend.Models.EmployeeModels;
using HRbackend.Models.Entities;

namespace HRbackend.Models.Helper
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
           // CreateMap<LoginModel, Employee>();
            CreateMap<Employee, EmployeeDto>();
        }
    }
}
