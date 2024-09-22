using AutoMapper;
using HRbackend.Models.ApplicantsModel;
using HRbackend.Models.EmployeeModels;
using HRbackend.Models.Entities;
using HRbackend.Models.Entities.Recruitment;

namespace HRbackend.Models.Helper
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Applicants, ApplicantsDto>();
            CreateMap<Employee, EmployeeDto>();
        }
    }
}
