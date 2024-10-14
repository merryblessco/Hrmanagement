using AutoMapper;
using HRbackend.Data;
using HRbackend.Models.Auth;
using HRbackend.Models.Entities.PayRoll;
using HRbackend.Models.Entities.Recruitment;
using HRbackend.Models.Entities.Setups;
using HRbackend.Models.Payroll;
using HRbackend.Models.SetupModels;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Net;
using System.Net.Mail;

namespace HRbackend.Controllers;
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
[Route("api/[controller]")]
[ApiController]
public class SetupsController : BaseController
{
    private readonly ApplicationDbContext _dbContext;
    private readonly IWebHostEnvironment _environment;
    private readonly IMapper _mapper;
    private readonly string _filePath = "";  // Set your file path here
    private readonly IHttpContextAccessor _contextAccessor;
    public SetupsController(ApplicationDbContext dbContext, IWebHostEnvironment environment, IMapper mapper, UserManager<ApplicationUser> userManager, IHttpContextAccessor contextAccessor) : base(userManager, contextAccessor)

    {
        _environment = environment ?? throw new ArgumentNullException(nameof(environment));
        _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        _mapper = mapper;
    }

    [HttpGet]
    [Route("positions")]
    public IActionResult GetPositions()
    {
        var positions = _dbContext.Positions.ToList();
        var response = positions.Select(x => new PositionDto
        {
            Id  = x.Id,
            Name = x.Name,
            Description = x.Description,
        });

        return Ok(response);
    }

    [HttpGet]
    [Route("states")]
    public IActionResult GetStates()
    {
        var records = _dbContext.States.ToList();
        var result = records.Select(x => new StateDto
        {
            Id = x.Id,
            Name = x.Name,
            Description = x.Description,
            StateCode = x.StateCode,
        }).ToList();
        return Ok(result);
    }

    [HttpGet]
    [Route("lga-by-state-code")]
    public async Task<ActionResult<List<LGADto>>> GetLgabyStateCode([FromQuery] string stateCode)
    {
        var records = await _dbContext.LGAs.Where(x => x.StateCode == stateCode.ToUpper()).ToListAsync();
        var result = records.Select(x => new LGADto
        {
            Id = x.Id,
            Name = x.Name,
            Description = x.Description,
            StateCode = x.StateCode,
        }).ToList();

        return Ok(result);
    }

    [HttpGet]
    [Route("lgas")]
    public IActionResult GetLgs()
    {
        var allLga = _dbContext.LGAs.ToList();

        return Ok(allLga);
    }

    [HttpPost]
    [Route("send-email")]
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

    [HttpGet("{ApplicantID}")]
    public async Task<ActionResult<Interview>> GetInterviews(Guid ApplicantID)
    {
        var interview = await _dbContext.Interviews.FindAsync(ApplicantID);
        if (interview == null) return NotFound();
        return interview;
    }

    [HttpGet]
    [Route("departments")]
    public async Task<ActionResult<IEnumerable<Department>>> GetDepartments()
    {
        return await _dbContext.Departments.ToListAsync();
    }

    [HttpGet]
    [Route("department-by-{id}")]
    public async Task<ActionResult<Department>> GetDepartment(Guid id)
    {
        var department = await _dbContext.Departments.FindAsync(id);

        if (department == null)
        {
            return NotFound();
        }

        return department;
    }

    [HttpPost]
    [Route("add-new-department")]
    public async Task<ActionResult<Department>> PostDepartment(Department department)
    {
        department.CreatedDate = DateTime.UtcNow;
        _dbContext.Departments.Add(department);
        await _dbContext.SaveChangesAsync();

        return CreatedAtAction(nameof(GetDepartment), new { id = department.Id }, department);
    }

    [HttpPut]
    [Route("update-department-by-{id}")]
    public async Task<IActionResult> PutDepartment(Guid id, Department department)
    {
        if (id != department.Id)
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

    [HttpDelete]
    [Route("delete-department-by-{id}")]
    public async Task<IActionResult> DeleteDepartment(Guid id)
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

    [HttpPost, Route("setup")]
    public async Task<IActionResult> Setup([FromBody] SetupModel setupModel)
    {
        // Validate the model
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        // Check if valid departments exist
        if (setupModel.Departments == null || setupModel.Departments.Count == 0)
        {
            return BadRequest("At least one department must be provided.");
        }

        var user = await GetCurrentUserAsync();

        // Save departments
        foreach (var department in setupModel.Departments)
        {
            _dbContext.Departments.Add(new Department
            {
                Name = department.Name,
                Description = department.Description,
                CreatedBy = Guid.Parse(user.Id),
                CreatedByIp = user.FullName
            });
        }

        if (RemoveEmptyTaxNames(setupModel.Taxes) != null || RemoveEmptyTaxNames(setupModel.Taxes).Count > 0)
        {
            // Save taxes
            foreach (var tax in setupModel.Taxes)
            {
                _dbContext.TaxInfo.Add(new Tax
                {
                    TaxName = tax.TaxName,
                    TaxPercentage = tax.TaxPercentage,
                    CreatedBy = Guid.Parse(user.Id),
                    CreatedByIp = user.FullName
                });
            }
        }


        user.InitialSetup = true;

        _dbContext.SaveChanges();

        return Ok(new { message = "Setup completed successfully." });
    }

    [HttpGet("summary")]
    public async Task<ActionResult<SummaryDto>> GetSummary()
    {
        // Query the count of departments and employees asynchronously
        int departmentCount = await _dbContext.Departments.Where(x => !x.IsDeleted).CountAsync();
        int employeeCount = await _dbContext.Employees.Where(x => !x.IsDeleted).CountAsync();

        var response = new SummaryDto
        {
            DepartmentCount = departmentCount,
            EmployeeCount = employeeCount,
            TotalExpense = 0
        };

        // Return the response
        return Ok(response);
    }

    private bool DepartmentExists(Guid id)
    {
        return _dbContext.Departments.Any(e => e.Id == id);
    }

    private List<TaxDto> RemoveEmptyTaxNames(List<TaxDto> taxes)
    {
        // Using LINQ to filter out items with an empty TaxName
        return taxes.Where(t => !string.IsNullOrEmpty(t.TaxName)).ToList();
    }

}

