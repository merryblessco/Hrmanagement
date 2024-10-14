using AutoMapper;
using HRbackend.Data;
using HRbackend.Models.EmployeeModels;
using HRbackend.Models.Entities.Recruitment;
using HRbackend.Models.Enums;
using HRbackend.Models.RecruitmentModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
namespace HRbackend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RecruitmentController : ControllerBase
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly IWebHostEnvironment _environment;
        private readonly IMapper _mapper;
        public RecruitmentController(ApplicationDbContext dbContext, IWebHostEnvironment environment, IMapper mapper)
        {
            _environment = environment ?? throw new ArgumentNullException(nameof(environment));
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
            _mapper = mapper;
        }
        [HttpGet("info")]
        //public async Task<IActionResult> GetEmployeeInfo()
        //{
        //    var identity = HttpContext.User.Identity as ClaimsIdentity;
        //    if (identity == null)
        //    {
        //        return Unauthorized();
        //    }

        //    var userClaims = identity.Claims;
        //    var email = userClaims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;

        //    if (string.IsNullOrEmpty(email))
        //    {
        //        return BadRequest("Invalid employee Email ID in token");
        //    }

        //    var employee = await _dbContext.Employees.FindAsync(email);
        //    if (employee == null)
        //    {
        //        return NotFound("Employee not found");
        //    }

        //    var employeeInfo = new EmployeeDto
        //    {
        //        FullName = employee.FullName,
        //        Email = employee.Email,
        //        JobTitle = employee.JobTitle,
        //        Department = employee.Department,
        //        IsAdmin = employee.IsAdmin
        //        // Add any other properties you want to include
        //    };

        //    return Ok(employeeInfo);


        //}
        [HttpGet("getallJobs")]
        public async Task<ActionResult<IEnumerable<JobPostingResponseDto>>> GetAll()
        {
                var jobPostings = new List<JobPostingResponseDto>();

            var records = _dbContext.JobPostings.Include(x => x.Department).Where(x => !x.IsDeleted).Select(x => new JobPostingResponseDto
            {
                Id = x.Id,
                JobTitle = x.JobTitle,
                DepartmentId = x.DepartmentId,
                DepartmentName = x.Department.Name,
                WorkModeName = x.WorkMode.GetDescription(),
                JobModeName = x.JobMode.GetDescription(),
                Description = x.Description,
                PostingDate = x.PostingDate,
                Status = x.Status,
                CompanyAddress = x.CompanyAddress,
                MaxSalaryRange = x.MaxSalaryRange,
                MinSalaryRange = x.MinSalaryRange,
                Benefits = x.Benefits,
                Responsibilities = x.Responsibilities,
                Qualifications = x.Qualifications,
            }).AsQueryable();

            jobPostings = await records.ToListAsync();

            return Ok(jobPostings);
        }

        [HttpGet("getJob/{id}")]
        public async Task<ActionResult<JobPostingResponseDto>> GetById(Guid id)
        {
            var record = await _dbContext.JobPostings.Include(x => x.Department).Where(x => x.Id == id && !x.IsDeleted).FirstOrDefaultAsync();
            if (record == null) return NotFound("Job posting record not found");

            var response = new JobPostingResponseDto
            {
                Id = record.Id,
                JobTitle = record.JobTitle,
                DepartmentId = record.DepartmentId,
                DepartmentName = record.Department.Name,
                WorkModeName = record.WorkMode.GetDescription(),
                JobModeName = record.JobMode.GetDescription(),
                Description = record.Description,
                PostingDate = record.PostingDate,
                Status = record.Status,
                CompanyAddress = record.CompanyAddress,
                MaxSalaryRange = record.MaxSalaryRange,
                MinSalaryRange = record.MinSalaryRange,
                Benefits = record.Benefits,
                Responsibilities = record.Responsibilities,
                Qualifications = record.Qualifications,
            };
          
            return Ok(response);
        }


        [HttpPost("PostJob")]
        public async Task<ActionResult<JobPostingDto>> Create([FromBody] JobPostingDto jobPostingDto)
        {
            var postingDate = DateTime.Now;
            jobPostingDto.PostingDate = postingDate;
            Random randR = new Random();
            int R = randR.Next(0000, 2222);
            int R1 = randR.Next(0000, 4444);

            int add = R + R1;
            var JobCode = "JB" + add.ToString();

            var posting = new JobPostings
            {
                JobTitle = jobPostingDto.JobTitle,
                JobCode = JobCode,
                CompanyAddress = jobPostingDto.CompanyAddress,
                DepartmentId = jobPostingDto.DepartmentId,
                Description = jobPostingDto.Description,
                PostingDate = jobPostingDto.PostingDate,
                Status = jobPostingDto.Status,
                JobMode = jobPostingDto.JobMode,
                WorkMode = jobPostingDto.WorkMode,
                MaxSalaryRange = jobPostingDto.MaxSalaryRange,
                MinSalaryRange = jobPostingDto.MinSalaryRange,
                Benefits = jobPostingDto.Benefits,
                Responsibilities = jobPostingDto.Responsibilities,
                Qualifications = jobPostingDto.Qualifications
            };

            // Save employee to the database
            _dbContext.JobPostings.Add(posting);
            await _dbContext.SaveChangesAsync();
            return CreatedAtAction(nameof(GetById), new { id = posting.Id }, posting);
        }

        //[HttpPut("{id}")]
        [HttpPut("updateJobposting/{id}")]
        public async Task<IActionResult> Update(Guid id, JobPostingDto jobPostingDto)
        {
            var posting = await _dbContext.JobPostings.FindAsync(id);
            if (id != jobPostingDto.Id) return BadRequest();
            posting.Id = jobPostingDto.Id;
            posting.JobTitle = jobPostingDto.JobTitle;
            posting.CompanyAddress = jobPostingDto.CompanyAddress;
            posting.DepartmentId = jobPostingDto.DepartmentId;
            posting.Description = jobPostingDto.Description;
            posting.PostingDate = jobPostingDto.PostingDate;
            posting.Status = jobPostingDto.Status;
            posting.JobMode = jobPostingDto.JobMode;
            posting.WorkMode = jobPostingDto.WorkMode;
            posting.MaxSalaryRange = jobPostingDto.MaxSalaryRange;
            posting.MinSalaryRange = jobPostingDto.MinSalaryRange;
            posting.Benefits = jobPostingDto.Benefits;
            posting.Responsibilities = jobPostingDto.Responsibilities;
            posting.Qualifications = jobPostingDto.Qualifications;

            _dbContext.JobPostings.Add(posting);
            await _dbContext.SaveChangesAsync();
            return Ok(new { posting.Id });
        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteJob(int id)
        {
            var job = await _dbContext.JobPostings.FindAsync(id);
            if (job == null) return NotFound();

            _dbContext.JobPostings.Remove(job);
            await _dbContext.SaveChangesAsync();
            return Ok("Job Successfuly Removed!");
        }

        private bool JobExists(Guid id)
        {
            return _dbContext.JobPostings.Any(e => e.Id == id);
        }
    }
}
