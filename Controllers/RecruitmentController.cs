using AutoMapper;
using HRbackend.Data;
using HRbackend.Models.EmployeeModels;
using HRbackend.Models.Entities;
using HRbackend.Models.Entities.JobPosting;
using HRbackend.Models.RecruitmentModel;
using LinkOrgNet.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Reflection.Emit;
using System.Security.Claims;
using System.Threading.Tasks;
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
        public async Task<IActionResult> GetEmployeeInfo()
        {
            var identity = HttpContext.User.Identity as ClaimsIdentity;
            if (identity == null)
            {
                return Unauthorized();
            }

            var userClaims = identity.Claims;
            var email = userClaims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(email))
            {
                return BadRequest("Invalid employee Email ID in token");
            }

            var employee = await _dbContext.Employees.FindAsync(email);
            if (employee == null)
            {
                return NotFound("Employee not found");
            }

            var employeeInfo = new EmployeeDto
            {
                FullName = employee.FullName,
                Email = employee.Email,
                JobTitle = employee.JobTitle,
                Department = employee.Department,
                IsAdmin = employee.IsAdmin
                // Add any other properties you want to include
            };

            return Ok(employeeInfo);
        }
        [HttpGet("getallJobs")]
        public async Task<ActionResult<IEnumerable<JobPostingDto>>> GetAll()
        {
            var jobPostings = await _dbContext.JobPostings.ToListAsync();
            return Ok(_mapper.Map<IEnumerable<JobPostingDto>>(jobPostings));
        }

        //[HttpGet("{id}")]
        [HttpGet("getJob/{id}")]
        public async Task<ActionResult<JobPostingDto>> GetById(int id)
        {
            var jobPosting = await _dbContext.JobPostings.FindAsync(id);
            if (jobPosting == null) return NotFound("No Posting you Job search, pls try other Jobs");
            return Ok(_mapper.Map<JobPostingDto>(jobPosting));
        }

        
        [HttpPost("PostJob")]
        public async Task<ActionResult<JobPostingDto>> Create([FromBody] JobPostingDto jobPostingDto)
        {
            var jobPosting = _mapper.Map<JobPostings>(jobPostingDto);
            var postingDate = DateTime.Now;
            jobPostingDto.PostingDate = postingDate;
            Random randR = new Random();
            int R = randR.Next(0000, 2222);
            int R1 = randR.Next(0000, 4444);

            int add = R + R1;
            jobPostingDto.JobCode = "JB" + add.ToString();

            var posting = new JobPostings
            {
                
                JobTitle = jobPostingDto.JobTitle,
                JobCode = jobPostingDto.JobCode,
                CompanyAddress = jobPostingDto.CompanyAddress,
                Department = jobPostingDto.Department,
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
            return CreatedAtAction(nameof(GetById), new { id = posting.JobID }, posting);
        }

        //[HttpPut("{id}")]
        [HttpPut("updateJobposting/{id}")]
        public async Task<IActionResult> Update(int id, JobPostingDto jobPostingDto)
        {
            var posting = await _dbContext.JobPostings.FindAsync(id);
            if (id != jobPostingDto.JobID) return BadRequest();
            posting.JobID = jobPostingDto.JobID;
            posting.JobTitle = jobPostingDto.JobTitle;
            posting.JobCode = jobPostingDto.JobCode;
            posting.CompanyAddress = jobPostingDto.CompanyAddress;
            posting.Department = jobPostingDto.Department;
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
            return Ok(new { posting.JobID });
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

        private bool JobExists(int id)
        {
            return _dbContext.JobPostings.Any(e => e.JobID == id);
        }
    }
}
