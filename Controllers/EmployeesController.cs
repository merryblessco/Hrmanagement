using Azure.Core;
using HRbackend.Data;
using HRbackend.Models;
using HRbackend.Models.Auth;
using HRbackend.Models.EmployeeModels;
using HRbackend.Models.Entities.Employees;
using HRbackend.Models.Enums;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Security.Claims;

namespace HRbackend.Controllers
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Route("api/[controller]")]
    [ApiController]
    public class EmployeesController : BaseController
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly IWebHostEnvironment _environment;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IHttpContextAccessor _contextAccessor;

        private readonly string _smtpHost = "smtp.gmail.com";
        private readonly int _smtpPort = 587;
        private readonly string _senderEmail = "hrsolutionsdev@gmail.com";
        private readonly string _senderPassword = "P@$$4w0rld"; // Store this in a secure location, such as environment variables.

        public EmployeesController(ApplicationDbContext dbContext, IWebHostEnvironment environment, UserManager<ApplicationUser> userManager, IHttpContextAccessor contextAccessor) : base(userManager, contextAccessor)
        {
            _environment = environment ?? throw new ArgumentNullException(nameof(environment));
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
            _userManager = userManager;
            _contextAccessor = contextAccessor;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<EmployeeDto>>> GetEmployees()
        {

            var result = new List<EmployeeDto>();
            var employees = await _dbContext.Employees.Where(x => !x.IsDeleted).ToListAsync();

            if (employees.Any())
            {
                foreach (var employee in employees)
                {
                    var contract = await _dbContext.EmployeeContracts.Include(y => y.Department).Include(y => y.Position)
                   .Where(x => x.EmployeeId == employee.Id && !x.IsDeleted)
                   .FirstOrDefaultAsync();

                    var employeeInfo = new EmployeeDto
                    {
                        Id = employee.Id,
                        FirstName = employee.FirstName,
                        LastName = employee.LastName,
                        Email = employee.Email,
                        PhoneNumber = employee.PhoneNumber,
                        JobTitle = String.IsNullOrEmpty(contract.JobTitle) ? contract.JobTitle : contract.Position.Name,
                        Department = contract.Department.Name,
                        Position = contract.Position.Name,
                        DOB = employee.DOB,
                        Address = employee.Address,
                        HireDate = contract.HireDate,
                        EmployeeNumber = contract.EmployeeNumber
                    };

                    result.Add(employeeInfo);
                }
            }

            return Ok(result);
        }

        [HttpGet("employee-info")]
        public async Task<IActionResult> GetEmployeeInfo()
        {
            var user = await GetCurrentUserAsync();

            var employee = await _dbContext.Employees.Include(x => x.State)
                .Include(x => x.Lga)
                .Where(x => x.Email == user.Email && !x.IsDeleted)
                .FirstOrDefaultAsync();

            var contract = await _dbContext.EmployeeContracts.Include(x => x.Manager).Include(y => y.Department).Include(y => y.Position)
                .Where(x => x.EmployeeId == employee.Id && !x.IsDeleted)
                .FirstOrDefaultAsync();


            if (employee == null || contract == null)
            {
                return NotFound("Employee not found");
            }

            var employeeInfo = new EmployeeDto
            {
                Id = employee.Id,
                FirstName = employee.FirstName,
                LastName = employee.LastName,
                Email = employee.Email,
                PhoneNumber = employee.PhoneNumber,
                JobTitle = String.IsNullOrEmpty(contract.JobTitle) ? contract.Position.Name : contract.JobTitle,
                Department = contract.Department.Name,
                Position = contract.Position.Name,
                DOB = employee.DOB,
                Address = employee.Address,
                HireDate = contract.HireDate,
                Lga = employee.Lga.Name,
                State = employee.State.Name,

                ManagerId = contract.Manager != null ? contract.ManagerId : null,
                ManagerName = contract.Manager != null ? contract.Manager.FirstName + " " + contract.Manager.LastName : null,
                PassportBytes = employee.PassportBytes,
                ResumeBytes = employee.ResumeBytes,
            };

            return Ok(employeeInfo);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<EmployeeDto>> GetEmployee(Guid id)
        {
            var employee = await _dbContext.Employees
               .Where(x => x.Id == id && !x.IsDeleted)
               .FirstOrDefaultAsync();

            var contract = await _dbContext.EmployeeContracts.Include(y => y.Department)
                .Where(x => x.EmployeeId == employee.Id && !x.IsDeleted)
                .FirstOrDefaultAsync();

            if (employee == null || contract == null) return NotFound();

            var state = await _dbContext.States.Where(x => x.Id == employee.StateId).FirstOrDefaultAsync();
            var lga = await _dbContext.LGAs.Where(x => x.Id == id).FirstOrDefaultAsync();

            var employeeInfo = new EmployeeDto
            {
                // Personal Information
                FirstName = employee.FirstName,
                LastName = employee.LastName,
                Email = employee.Email,
                //Role = employee.Role.GetDescription(),
                PhoneNumber = employee.PhoneNumber,
                Address = employee.Address,
                State = state.Name,
                Lga = lga.Name,
                DOB = employee.DOB,
                PassportBytes = employee.PassportBytes,
                ResumeBytes = employee.ResumeBytes,

                // Contract Information
                JobTitle = contract.JobTitle,
                DepartmentId = contract.DepartmentId,
                Department = contract.Department.Name,
                PositionId = contract.PositionId,
                Position = contract.Position.Name,

                HireDate = contract.HireDate,

                // Manager Information
                ManagerId = contract.Manager != null ? contract.Manager.Id : new Guid(),
                ManagerName = contract.Manager != null ? $"{contract.Manager.FirstName} {contract.Manager.LastName}" : String.Empty
            };

            return employeeInfo;
        }

        [HttpPost("create-employee")]
        public async Task<IActionResult> CreateEmployee([FromForm] EmployeeDto employeeDto)
        {
            if (employeeDto.Passport == null || employeeDto.Resume == null)
                return BadRequest("Passport and Resume files are required.");

            // Validate file types (pdf, jpeg, word)
            var allowedExtensions = new[] { ".pdf", ".jpeg", ".jpg", ".doc", ".docx", ".png" };
            var passportFileExtension = Path.GetExtension(employeeDto.Passport.FileName).ToLower();
            var resumeFileExtension = Path.GetExtension(employeeDto.Resume.FileName).ToLower();

            if (!allowedExtensions.Contains(passportFileExtension) || !allowedExtensions.Contains(resumeFileExtension))
            {
                return BadRequest("Only PDF, JPEG, and Word files are allowed for Passport and Resume.");
            }

            // Convert Passport file to byte array
            byte[] passportBytes;
            using (var memoryStream = new MemoryStream())
            {
                await employeeDto.Passport.CopyToAsync(memoryStream);
                passportBytes = memoryStream.ToArray();
            }

            // Convert Resume file to byte array
            byte[] resumeBytes;
            using (var memoryStream = new MemoryStream())
            {
                await employeeDto.Resume.CopyToAsync(memoryStream);
                resumeBytes = memoryStream.ToArray();
            }


            var state = await _dbContext.States.Where(x => x.StateCode == employeeDto.StateCode).FirstOrDefaultAsync();
            var lga = await _dbContext.LGAs.Where(x => x.Id == employeeDto.LGAId).FirstOrDefaultAsync();

            if (lga == null || state == null)
            {
                return BadRequest(new { message = "State/Lga record not found" });
            }

            // Create Employee object
            var employee = new Employee
            {
                FirstName = employeeDto.FirstName,
                LastName = employeeDto.LastName,
                Email = employeeDto.Email,
                Role = ApplicationRoles.Employee,
                PhoneNumber = employeeDto.PhoneNumber,
                Address = employeeDto.Address,
                StateId = state.Id,
                LGAId = lga.Id,
                DOB = employeeDto.DOB,
                PassportBytes = passportBytes,
                ResumeBytes = resumeBytes,
            };

            // Add employee to the database
            await _dbContext.Employees.AddAsync(employee);

            // Create EmployeeContract object
            var employeeContract = new EmployeeContract
            {
                EmployeeId = employee.Id,
                JobTitle = employeeDto.JobTitle != null ? employeeDto.JobTitle : String.Empty,
                DepartmentId = employeeDto.DepartmentId,
                PositionId = employeeDto.PositionId,
                HireDate = employeeDto.HireDate,
                ManagerId = employeeDto.ManagerId != null ? employeeDto.ManagerId : null,
                Status = EmployeeStatus.Active,
            };

            // Add employee contract to the database
            await _dbContext.EmployeeContracts.AddAsync(employeeContract);
          

            await _dbContext.SaveChangesAsync();


            //Create Identity objects
            // Generate random password for the employee
            string password = $"{employeeDto.FirstName.ToUpper()}pass@123";

            // Create ApplicationUser (assuming Identity is being used)
            var applicationUser = new ApplicationUser
            {
                UserName = employeeDto.Email,
                FirstName = employeeDto.FirstName,
                LastName = employeeDto.LastName,
                Email = employeeDto.Email,
                PhoneNumber = employeeDto.PhoneNumber,
                Role = ApplicationRoles.Employee,
                PasswordHash = _userManager.PasswordHasher.HashPassword(null, password) // Hash password
            };

            var result = await _userManager.CreateAsync(applicationUser, password);


            if (!result.Succeeded)
            {

                if (result.Errors?.FirstOrDefault().Code == "DuplicateUserName")
                {
                    return BadRequest(new { message = $"Employee with {employeeDto.Email} already exist." });
                }
                return BadRequest(new { message = "Failed to create user." });
            }


            // Assign role to user
            await _userManager.AddToRoleAsync(applicationUser, ApplicationRoles.Employee.ToString());

            //Fetch the saved employee and save the userId
            var savedEmployee = await _dbContext.Employees.Where(x => x.Id == employee.Id).FirstOrDefaultAsync();
            savedEmployee.UserId = Guid.Parse(applicationUser.Id);
            await _dbContext.SaveChangesAsync();

            // Send welcome email
            string subject = "Welcome to Tavai-Tech Solutions";
            string message = $"<h1>Welcome, {employee.FirstName} {employee.LastName}!</h1><p>Your account has been created successfully. Your login credentials are:</p>" +
                             $"<p>Email: {employee.Email}</p><p>Password: {password}</p><p>Please change your password after the first login.</p>";
            //await SendEmailAsync(employee.Email, subject, message);

            return CreatedAtAction(nameof(GetEmployee), new { id = employee.Id }, employee);
        }

        private async Task<string> SaveFile(IFormFile file, string subFolder)
        {
            string uploadsFolder = Path.Combine(_environment.WebRootPath ?? Directory.GetCurrentDirectory(), "uploads", subFolder);

            if (!Directory.Exists(uploadsFolder))
                Directory.CreateDirectory(uploadsFolder);

            string fileExtension = Path.GetExtension(file.FileName).ToLowerInvariant();
            string[] allowedExtensions = { ".pdf", ".doc", ".docx", ".jpg", ".jpeg", ".png" };

            if (!allowedExtensions.Contains(fileExtension))
                throw new ArgumentException("Invalid file format. Only PDF, Word, and JPEG formats are allowed.");

            string uniqueFileName = Guid.NewGuid().ToString() + fileExtension;
            string filePath = Path.Combine(uploadsFolder, uniqueFileName);

            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(fileStream);
            }

            return Path.Combine(subFolder, uniqueFileName);
        }

        [HttpPut("updateEmployee/{id}")]
        public async Task<IActionResult> UpdateEmployee(Guid id, [FromForm] EmployeeDto employeeDto)
        {
            // Fetch the employee from the database
            var employee = await _dbContext.Employees.FirstOrDefaultAsync(e => e.Id == id);
            var contract = await _dbContext.EmployeeContracts.Include(y => y.Department)
              .Where(x => x.EmployeeId == employee.Id && !x.IsDeleted)
              .FirstOrDefaultAsync();

            if (employee == null || contract == null)
                return NotFound("Employee not found");

            // Validate and handle Passport file conversion to byte array
            if (employeeDto.Passport != null)
            {
                var allowedExtensions = new[] { ".jpeg", ".jpg", ".pdf" };
                var passportFileExtension = Path.GetExtension(employeeDto.Passport.FileName).ToLower();

                if (!allowedExtensions.Contains(passportFileExtension))
                    return BadRequest("Only PDF, JPEG files are allowed for Passport.");

                using (var memoryStream = new MemoryStream())
                {
                    await employeeDto.Passport.CopyToAsync(memoryStream);
                    employee.PassportBytes = memoryStream.ToArray();
                }
            }

            // Validate and handle Resume file conversion to byte array
            if (employeeDto.Resume != null)
            {
                var allowedExtensions = new[] { ".pdf", ".doc", ".docx" };
                var resumeFileExtension = Path.GetExtension(employeeDto.Resume.FileName).ToLower();

                if (!allowedExtensions.Contains(resumeFileExtension))
                    return BadRequest("Only PDF, Word files are allowed for Resume.");

                using (var memoryStream = new MemoryStream())
                {
                    await employeeDto.Resume.CopyToAsync(memoryStream);
                    employee.ResumeBytes = memoryStream.ToArray();
                }
            }

            var state = await _dbContext.States.Where(x => x.StateCode == employeeDto.StateCode).FirstOrDefaultAsync();
            var lga = await _dbContext.LGAs.Where(x => x.Id == employeeDto.LGAId).FirstOrDefaultAsync();

            if (lga == null || state == null)
            {
                return BadRequest(new { message = "State/Lga record not found" });
            }

            // Update Employee fields from DTO
            employee.FirstName = employeeDto.FirstName;
            employee.LastName = employeeDto.LastName;
            employee.Email = employeeDto.Email;
            employee.PhoneNumber = employeeDto.PhoneNumber;
            employee.Address = employeeDto.Address;
            employee.StateId = state.Id;
            employee.LGAId = lga.Id;
            employee.DOB = employeeDto.DOB;
            employee.Role = ApplicationRoles.Employee;


            // Update EmployeeContract if applicable
            if (contract != null)
            {
                contract.JobTitle = employeeDto.JobTitle;
                contract.DepartmentId = employeeDto.DepartmentId;
                contract.PositionId = employeeDto.PositionId;
                contract.HireDate = employeeDto.HireDate;
                contract.ManagerId = employeeDto.ManagerId;
            }
            else
            {
                // If the contract doesn't exist, create a new one
                var employeeContract = new EmployeeContract
                {
                    EmployeeId = employee.Id,
                    JobTitle = employeeDto.JobTitle,
                    DepartmentId = employeeDto.DepartmentId,
                    PositionId = employeeDto.PositionId,
                    HireDate = employeeDto.HireDate,
                    ManagerId = employeeDto.ManagerId
                };
                await _dbContext.EmployeeContracts.AddAsync(employeeContract);
            }

            // Save changes to the database
            _dbContext.Employees.Update(employee);
            await _dbContext.SaveChangesAsync();

            return Ok(new { employee.Id });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteEmployee(Guid id)
        {
            var employee = await _dbContext.Employees.Where(x => x.Id == id && !x.IsDeleted).FirstOrDefaultAsync();
            if (employee == null) return NotFound();

            employee.IsDeleted = true;

            await _dbContext.SaveChangesAsync();
            return NoContent();
        }

        private bool EmployeeExists(Guid id)
        {
            return _dbContext.Employees.Any(e => e.Id == id);
        }

        private void DeleteFile(string filePath)
        {
            if (!string.IsNullOrEmpty(filePath))
            {
                string fullPath = Path.Combine(_environment.WebRootPath, filePath);
                if (System.IO.File.Exists(fullPath))
                {
                    System.IO.File.Delete(fullPath);
                }
            }
        }

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


        private static readonly Random random = new Random();

        private static string GenerateUniqueIdentifier(Guid guid)
        {
            //// Generate a new GUID
            //Guid guid = Guid.NewGuid();

            // Convert GUID to string and take a subset for uniqueness
            string uniquePart = guid.ToString("N").Substring(0, 6).ToUpper();

            // Combine with the prefix
            string uniqueIdentifier = $"EMP-{uniquePart}";

            return uniqueIdentifier;
        }

    }
}
