using HRbackend.Data;
using HRbackend.Models.EmployeeModels;
using HRbackend.Models.Entities;
using LinkOrgNet.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Net;
using System.Net.Mail;
using System.Security.Claims;

namespace HRbackend.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class EmployeesController : ControllerBase
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly IWebHostEnvironment _environment;
        public EmployeesController(ApplicationDbContext dbContext, IWebHostEnvironment environment)
        {
            _environment = environment ?? throw new ArgumentNullException(nameof(environment));
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        }
        // GET: api/Employees
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Employee>>> GetEmployees()
        {
            return await _dbContext.Employees.ToListAsync();
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

            if (string.IsNullOrEmpty(email) )
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

        // GET: api/Employees/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Employee>> GetEmployee(int id)
        {
            var employee = await _dbContext.Employees.FindAsync(id);
            if (employee == null) return NotFound();
            return employee;
        }
        // POST: api/Employees
        [HttpPost("createEmployee")]

        public async Task<IActionResult> CreateEmployee([FromForm] EmployeeDto employeeDto)
        {
            if (employeeDto.Passport == null || employeeDto.Resume == null)
                return BadRequest("Passport and Resume files are required.");

            // Generate unique filenames and define paths
            string uploadsFolder = _environment?.WebRootPath != null
                ? Path.Combine(_environment.WebRootPath, "uploads")
                : Path.Combine(Directory.GetCurrentDirectory(), "uploads");

            if (!Directory.Exists(uploadsFolder))
                Directory.CreateDirectory(uploadsFolder);

            // Handle Passport file
            string passportFileName = await SaveFile(employeeDto.Passport, "Passports");

            // Handle Resume file
            string resumeFileName = await SaveFile(employeeDto.Resume, "Resumes");

            // Generate random password
            Random randR = new Random();
            int R = randR.Next(0000, 2222);
            employeeDto.Password = "HR" + R.ToString();

            // Create Employee object and map fields from DTO
            var employee = new Employee
            {
                FullName = employeeDto.FullName,
                Email = employeeDto.Email,
                PhoneNumber = employeeDto.PhoneNumber,
                Address = employeeDto.Address,
                JobTitle = employeeDto.JobTitle,
                Department = employeeDto.Department,
                Position = employeeDto.Position,
                State = employeeDto.State,
                LGA = employeeDto.LGA,
                HireDate = employeeDto.HireDate,
                DOB = employeeDto.DOB,
                ManagerID = employeeDto.ManagerID,
                PassportPath = passportFileName,
                ResumePath = resumeFileName,
                LoginId = employeeDto.LoginId,
                Password = SecurityClass.FCODE(employeeDto.Password)
            };

            // Save employee to the database
            _dbContext.Employees.Add(employee);
            await _dbContext.SaveChangesAsync();

            // Send welcome email
            string subject = "Welcome to Tavai-Tech Solutions";
            string message = $"<h1>Welcome, {employee.FullName}!</h1><p>We're excited to have you join our team.</p>";
            await SendEmailAsync(employee.Email, subject, message);

            return CreatedAtAction(nameof(GetEmployee), new { id = employee.EmployeeID }, employee);
        }

        private async Task<string> SaveFile(IFormFile file, string subFolder)
        {
            string uploadsFolder = Path.Combine(_environment.WebRootPath ?? Directory.GetCurrentDirectory(), "uploads", subFolder);

            if (!Directory.Exists(uploadsFolder))
                Directory.CreateDirectory(uploadsFolder);

            string fileExtension = Path.GetExtension(file.FileName).ToLowerInvariant();
            string[] allowedExtensions = { ".pdf", ".doc", ".docx", ".jpg", ".jpeg" };

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

        //public async Task<IActionResult> CreateEmployee([FromForm] EmployeeDto employeeDto)
        //{
        //    if (employeeDto.Passport == null || employeeDto.Resume == null)
        //        return BadRequest("Passport and Resume files are required.");

        //    // Generate unique filenames and define paths
        //    string uploadsFolder = _environment?.WebRootPath != null    ? Path.Combine(_environment.WebRootPath, "uploads")   : Path.Combine(Directory.GetCurrentDirectory(), "uploads");
        //    //string uploadsFolder = Path.Combine(_environment.WebRootPath, "uploads");

        //    if (!Directory.Exists(uploadsFolder))
        //        Directory.CreateDirectory(uploadsFolder);

        //    string passportFileName = Guid.NewGuid().ToString() + "_" + employeeDto.Passport.ToString();
        //    string resumeFileName = Guid.NewGuid().ToString() + "_" + employeeDto.Resume.ToString();

        //    string passportFilePath = Path.Combine(uploadsFolder, passportFileName);
        //    string resumeFilePath = Path.Combine(uploadsFolder, resumeFileName);

        //    // Save the uploaded files

        //    using (var passportStream = new FileStream(passportFilePath, FileMode.Create))
        //    {
        //        // This is the correct use of CopyToAsync on an IFormFile (employeeDto.Passport)
        //        await employeeDto.Passport.CopyToAsync(passportStream);
        //    }

        //    using (var resumeStream = new FileStream(resumeFilePath, FileMode.Create))
        //    {
        //        // This is the correct use of CopyToAsync on an IFormFile (employeeDto.Resume)
        //        await employeeDto.Resume.CopyToAsync(resumeStream);
        //    }
        //    Random randR = new Random();
        //    int R = randR.Next(0000, 2222);
        //    employeeDto.Password = "HR" + R.ToString();
        //    // Create Employee object and map fields from DTO
        //    var employee = new Employee
        //    {
        //        FullName = employeeDto.FullName,
        //        Email = employeeDto.Email,
        //        PhoneNumber = employeeDto.PhoneNumber,
        //        Address = employeeDto.Address,
        //        JobTitle = employeeDto.JobTitle,
        //        Department = employeeDto.Department,
        //        Position = employeeDto.Position,
        //        State = employeeDto.State,
        //        LGA = employeeDto.LGA,
        //        HireDate = employeeDto.HireDate,
        //        DOB = employeeDto.DOB,
        //        ManagerID = employeeDto.ManagerID,
        //        PassportPath = passportFileName,
        //        ResumePath = resumeFileName,
        //        LoginId = employeeDto.LoginId,
        //        Password = SecurityClass.FCODE(employeeDto.Password)
        //    };

        //    // Save employee to the database
        //    _dbContext.Employees.Add(employee);
        //    await _dbContext.SaveChangesAsync();
        //    // Send welcome email
        //    string subject = "Welcome to Tavai-Tech Solutions";
        //    string message = $"<h1>Welcome, {employee.FullName}!</h1><p>We're excited to have you join our team.</p>";
        //    await SendEmailAsync(employee.Email, subject, message);

        //    return CreatedAtAction(nameof(GetEmployee), new { id = employee.EmployeeID }, employee);
        //    //return Ok(new { employee.EmployeeID });
        //}
        // PUT: api/Employees/5
        [HttpPut("updateEmployee/{id}")]
        public async Task<IActionResult> UpdateEmployee(int id, [FromForm] EmployeeDto employeeDto)
        {
            var employee = await _dbContext.Employees.FindAsync(id);

            if (employee == null)
                return NotFound("Employee not found");

            // Generate unique filenames and define paths if new files are provided
            string uploadsFolder = _environment?.WebRootPath != null ? Path.Combine(_environment.WebRootPath, "uploads") : Path.Combine(Directory.GetCurrentDirectory(), "uploads");

            if (!Directory.Exists(uploadsFolder))
                Directory.CreateDirectory(uploadsFolder);

            // Check if new Passport file is provided, if so, save it
            if (employeeDto.Passport != null)
            {
                string passportFileName = Guid.NewGuid().ToString() + "_" + employeeDto.Passport.FileName;
                string passportFilePath = Path.Combine(uploadsFolder, passportFileName);

                using (var passportStream = new FileStream(passportFilePath, FileMode.Create))
                {
                    await employeeDto.Passport.CopyToAsync(passportStream);
                }

                // Delete the old passport file if it exists
                if (!string.IsNullOrEmpty(employee.PassportPath))
                {
                    string oldPassportPath = Path.Combine(uploadsFolder, employee.PassportPath);
                    if (System.IO.File.Exists(oldPassportPath))
                    {
                        System.IO.File.Delete(oldPassportPath);
                    }
                }

                employee.PassportPath = passportFileName;
            }

            // Check if new Resume file is provided, if so, save it
            if (employeeDto.Resume != null)
            {
                string resumeFileName = Guid.NewGuid().ToString() + "_" + employeeDto.Resume.FileName;
                string resumeFilePath = Path.Combine(uploadsFolder, resumeFileName);

                using (var resumeStream = new FileStream(resumeFilePath, FileMode.Create))
                {
                    await employeeDto.Resume.CopyToAsync(resumeStream);
                }

                // Delete the old resume file if it exists
                if (!string.IsNullOrEmpty(employee.ResumePath))
                {
                    string oldResumePath = Path.Combine(uploadsFolder, employee.ResumePath);
                    if (System.IO.File.Exists(oldResumePath))
                    {
                        System.IO.File.Delete(oldResumePath);
                    }
                }

                employee.ResumePath = resumeFileName;
            }

            // Update other employee fields from DTO
            employee.FullName = employeeDto.FullName;
            employee.Email = employeeDto.Email;
            employee.PhoneNumber = employeeDto.PhoneNumber;
            employee.Address = employeeDto.Address;
            employee.JobTitle = employeeDto.JobTitle;
            employee.Department = employeeDto.Department;
            employee.Position = employeeDto.Position;
            employee.State = employeeDto.State;
            employee.LGA = employeeDto.LGA;
            employee.HireDate = employeeDto.HireDate;
            employee.DOB = employeeDto.DOB;
            employee.ManagerID = employeeDto.ManagerID;
            employee.LoginId = employeeDto.LoginId;
            employee.Password = SecurityClass.FCODE(employeeDto.Password);

            // Save changes to the database
            _dbContext.Employees.Update(employee);
            await _dbContext.SaveChangesAsync();

            return Ok(new { employee.EmployeeID });
        }


        // DELETE: api/Employees/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteEmployee(int id)
        {
            var employee = await _dbContext.Employees.FindAsync(id);
            if (employee == null) return NotFound();

            // Delete associated files
            DeleteFile(employee.PassportPath);
            DeleteFile(employee.ResumePath);

            _dbContext.Employees.Remove(employee);
            await _dbContext.SaveChangesAsync();
            return NoContent();
        }

        private bool EmployeeExists(int id)
        {
            return _dbContext.Employees.Any(e => e.EmployeeID == id);
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


    }
}
