using AutoMapper;
using HRbackend.Models.ApplicantsModel;
using HRbackend.Models.EmployeeModels;
using HRbackend.Models.Entities.Employees;
using HRbackend.Models.Entities.PayRoll;
using HRbackend.Models.Entities.Recruitment;
using HRbackend.Models.Entities.Setups;
using HRbackend.Models.RecruitmentModel;
using HRbackend.Models.SetupModels;

namespace HRbackend.Models.Helper
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Applicants, ApplicantsDto>();
            CreateMap<Employee, EmployeeDto>();
            CreateMap<Onboarding, OnboardingDto>();
            CreateMap<SalaryCalculation, SalaryCalculationDto>();
            CreateMap<PaySlip, PaySlipDto>();
            CreateMap<BenefitAdministration, BenefitAdministrationDto>();
            CreateMap<TaxManagement, TaxManagementDto>();
            CreateMap<JobPostings, JobPostingDto>();
            CreateMap<JobPostings, JobPostingResponseDto>();

            CreateMap<LGA, LGADto>();
            CreateMap<State, StateDto>();


        }
    }
}
