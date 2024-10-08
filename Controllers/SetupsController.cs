using AutoMapper;
using HRbackend.Data;
using HRbackend.Models.SetupModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Net.Mail;
using System.Net;
using HRbackend.Models.EmployeeModels;
using HRbackend.Models.Entities.Employees;
using LinkOrgNet.Models;
using System.Linq;
using HRbackend.Models.ApplicantsModel;
using HRbackend.Models.Entities.Recruitment;
using HRbackend.Models.Enums;
using HRbackend.Models.Entities.Setups;

namespace HRbackend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SetupsController : ControllerBase
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly IWebHostEnvironment _environment;
        private readonly IMapper _mapper;
        private readonly string _filePath = "";  // Set your file path here
        public SetupsController(ApplicationDbContext dbContext, IWebHostEnvironment environment, IMapper mapper)
        {
            _environment = environment ?? throw new ArgumentNullException(nameof(environment));
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
            _mapper = mapper;
        }
        [HttpGet]
        [Route("GetAllStates")]
        public IActionResult GetStates()
        {
            var allStates = _dbContext.States.ToList();

            return Ok(allStates);
        }
        [HttpGet]
        [Route("lga-by-state-code")]
        public async Task<ActionResult<List<LGADto>>> GetLgabyStateCode([FromQuery] string stateCode)
        {
            var records = await _dbContext.LGAs.Where(x => x.StateCode == stateCode.ToUpper()).ToListAsync();
            var result = records.Select(x => new LGADto
            {
                Code = x.Code,
                Description = x.Description,
                StateCode = x.StateCode,
            }).ToList();

            return Ok(result);
        }
        [HttpGet]
        [Route("Get-All-LGA")]
        public IActionResult GetLgs()
        {
            var allLga = _dbContext.LGAs.ToList();

            return Ok(allLga);
        }
        [HttpPost]
        [Route("Send-email")]
        private async Task SendEmailAsync(string email, string subject, string message)
        {
            var smtpClient = new SmtpClient("smtp.gmail.com")
            {
                Port = 587, // Gmail uses port 587 for TLS
                Credentials = new NetworkCredential(
                    "hrsolutionsdev@gmail.com",
                    "P@$$4w0rld"
                ),
                EnableSsl = true,
            };

            var mailMessage = new MailMessage
            {
                From = new MailAddress("hrsolutionsdev@gmail.com"),
                Subject = subject,
                Body = message,
                IsBodyHtml = true,
            };
            mailMessage.To.Add(email);

            await smtpClient.SendMailAsync(mailMessage);
        }

      
        // GET: api/Intervie/5
        [HttpGet("{ApplicantID}")]
        public async Task<ActionResult<Interview>> GetInterviews(int ApplicantID)
        {
            var interview = await _dbContext.Interviews.FindAsync(ApplicantID);
            if (interview == null) return NotFound();
            return interview;
        }

        // GET: api/Department
        [HttpGet]
        [Route("get-all-department")]
        public async Task<ActionResult<IEnumerable<Department>>> GetDepartments()
        {
            return await _dbContext.Departments.ToListAsync();
        }
        // GET: api/Department/5
        [HttpGet]
        [Route("get-department-by-{id}")]
        public async Task<ActionResult<Department>> GetDepartment(int id)
        {
            var department = await _dbContext.Departments.FindAsync(id);

            if (department == null)
            {
                return NotFound();
            }

            return department;
        }
        // POST: api/Department
        [HttpPost]
        [Route("add-new-department")]
        public async Task<ActionResult<Department>> PostDepartment(Department department)
        {
            department.CreatedDate = DateTime.UtcNow;
            _dbContext.Departments.Add(department);
            await _dbContext.SaveChangesAsync();

            return CreatedAtAction(nameof(GetDepartment), new { id = department.DepartmentId }, department);
        }
        // PUT: api/Department/5
        [HttpPut]
        [Route("update-department-by-{id}")]
        public async Task<IActionResult> PutDepartment(int id, Department department)
        {
            if (id != department.DepartmentId)
            {
                return BadRequest();
            }

            _dbContext.Entry(department).State = EntityState.Modified;

            try
            {
                await _dbContext.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!DepartmentExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }
        // DELETE: api/Department/5
        [HttpDelete]
        [Route("delete-department-by-{id}")]
        public async Task<IActionResult> DeleteDepartment(int id)
        {
            var department = await _dbContext.Departments.FindAsync(id);
            if (department == null)
            {
                return NotFound();
            }

            _dbContext.Departments.Remove(department);
            await _dbContext.SaveChangesAsync();

            return NoContent();
        }

        private bool DepartmentExists(int id)
        {
            return _dbContext.Departments.Any(e => e.DepartmentId == id);
        }

    }
}
