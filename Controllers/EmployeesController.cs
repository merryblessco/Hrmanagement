using Azure.Core;
using ClosedXML.Excel;
using HRbackend.Data;
using HRbackend.Lib;
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
using Microsoft.OpenApi.Models;
using OfficeOpenXml;
using RestSharp;
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
        private readonly IConfiguration _configuration;
        private readonly Notifications _notifications;
        private readonly ApplicationDbContext _dbContext;
        private readonly IWebHostEnvironment _environment;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IHttpContextAccessor _contextAccessor;

        public EmployeesController(
            ApplicationDbContext dbContext,
            IWebHostEnvironment environment,
            UserManager<ApplicationUser> userManager,
            IHttpContextAccessor contextAccessor,
            IConfiguration configuration,
            Notifications notifications) : base(userManager, contextAccessor)
        {
            _environment = environment ?? throw new ArgumentNullException(nameof(environment));
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
            _userManager = userManager;
            _contextAccessor = contextAccessor;
            _configuration = configuration;
            _notifications = notifications;
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
                        Department = contract.Department == null ? null : contract.Department.Name,
                        Position = contract.Position == null ? null : contract.Position.Name,
                        DOB = (DateTime)employee.DOB,
                        Address = employee.Address,
                        HireDate = contract.HireDate == null ? null : (DateTime)contract.HireDate,
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
                DOB = (DateTime)employee.DOB,
                Address = employee.Address,
                HireDate = (DateTime)contract.HireDate,
                Lga = employee.Lga.Name,
                State = employee.State.Name,
                EmployeeNumber = contract.EmployeeNumber,
                ManagerId = contract.Manager != null ? contract.ManagerId : null,
                ManagerName = contract.Manager != null ? contract.Manager.FirstName + " " + contract.Manager.LastName : null,
                PassportBytes = employee.PassportBytes,
                ResumeBytes = employee.ResumeBytes,
                Status = contract.Status,
                StatusName = contract.Status.GetDescription(),
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
                DOB = (DateTime)employee.DOB,
                PassportBytes = employee.PassportBytes,
                ResumeBytes = employee.ResumeBytes,

                // Contract Information
                JobTitle = contract.JobTitle,
                DepartmentId = (Guid)contract.DepartmentId,
                Department = contract.Department.Name,
                PositionId = (Guid)contract.PositionId,
                Position = contract.Position.Name,

                HireDate = (DateTime)contract.HireDate,

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

            // Convert Passport and Resume files to byte arrays
            byte[] passportBytes;
            byte[] resumeBytes;

            using (var passportStream = new MemoryStream())
            {
                await employeeDto.Passport.CopyToAsync(passportStream);
                passportBytes = passportStream.ToArray();
            }

            using (var resumeStream = new MemoryStream())
            {
                await employeeDto.Resume.CopyToAsync(resumeStream);
                resumeBytes = resumeStream.ToArray();
            }

            // Validate state and LGA
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
                CreationMode = EmployeeCreationMode.Single
            };

            await _dbContext.Employees.AddAsync(employee);

            // Create EmployeeContract object
            var employeeContract = new EmployeeContract
            {
                EmployeeId = employee.Id,
                JobTitle = employeeDto.JobTitle ?? string.Empty,
                DepartmentId = employeeDto.DepartmentId,
                PositionId = employeeDto.PositionId,
                HireDate = employeeDto.HireDate,
                ManagerId = employeeDto.ManagerId,
                Status = EmployeeStatus.Active,
            };

            await _dbContext.EmployeeContracts.AddAsync(employeeContract);

            await _dbContext.SaveChangesAsync();

            // Create ApplicationUser
            string password = $"{employeeDto.FirstName.ToUpper()}pass@123";
            var applicationUser = new ApplicationUser
            {
                UserName = employeeDto.Email,
                FirstName = employeeDto.FirstName,
                LastName = employeeDto.LastName,
                Email = employeeDto.Email,
                PhoneNumber = employeeDto.PhoneNumber,
                Role = ApplicationRoles.Employee,
                PasswordHash = _userManager.PasswordHasher.HashPassword(null, password)
            };

            var result = await _userManager.CreateAsync(applicationUser, password);
            if (!result.Succeeded)
            {
                if (result.Errors?.FirstOrDefault()?.Code == "DuplicateUserName")
                {
                    return BadRequest(new { message = $"Employee with {employeeDto.Email} already exists." });
                }
                return BadRequest(new { message = "Failed to create user." });
            }

            await _userManager.AddToRoleAsync(applicationUser, ApplicationRoles.Employee.ToString());

            var savedEmployee = await _dbContext.Employees.Where(x => x.Id == employee.Id).FirstOrDefaultAsync();
            savedEmployee.UserId = Guid.Parse(applicationUser.Id);

            await _dbContext.SaveChangesAsync();

            // Access the email template
            string emailTemplate = EmailTemplates.WelcomeEmail;

            // Replace placeholders with actual values
            string emailContent = emailTemplate
                .Replace("{FullName}", $"{employeeDto.FirstName + " " + employeeDto.LastName}")
                .Replace("[CompanyName]", _configuration["CompanySettings:CompanyName"])
                .Replace("{Passcode}", password);

            await _notifications.SendSmsNotificationAsync(new string[]
            {
                employee.PhoneNumber
            }, emailContent);

            // Optionally send a welcome email
            string subject = "Welcome to Tavai-Tech Solutions";
            string message = $"<h1>Welcome, {employee.FirstName} {employee.LastName}!</h1><p>Your account has been created successfully. Your login credentials are:</p>" +
                            $"<p>Email: {employee.Email}</p><p>Password: {password}</p><p>Please change your password after the first login.</p>";

            //await SendEmailAsync(employee.Email, subject, message);

            return CreatedAtAction(nameof(GetEmployee), new { id = employee.Id }, employee);
        }

        [HttpPost("bulk-upload")]
        public async Task<IActionResult> UploadEmployees(IFormFile excelFile)
        {
            if (excelFile == null || excelFile.Length == 0)
                return BadRequest("An Excel file is required.");

            var allowedExtensions = new[] { ".xls", ".xlsx" };
            var fileExtension = Path.GetExtension(excelFile.FileName).ToLower();

            if (!allowedExtensions.Contains(fileExtension))
                return BadRequest("Only Excel files (.xls, .xlsx) are allowed.");

            var employeeList = new List<EmployeeExcelDto>();

            using (var stream = new MemoryStream())
            {
                await excelFile.CopyToAsync(stream);
                using (var package = new ExcelPackage(stream))
                {
                    ExcelPackage.LicenseContext = LicenseContext.NonCommercial; // Set license context

                    var worksheet = package.Workbook.Worksheets[0]; // Access the first worksheet
                    var rowCount = worksheet.Dimension.Rows;

                    // Start reading from row 2, assuming row 1 is the header
                    for (int row = 2; row <= rowCount; row++)
                    {
                        var employee = new EmployeeExcelDto
                        {
                            LastName = worksheet.Cells[row, 1].Text,
                            FirstName = worksheet.Cells[row, 2].Text,
                            PhoneNumber = worksheet.Cells[row, 3].Text,
                            AlternativePhoneNumber = worksheet.Cells[row, 4].Text,
                            Address = worksheet.Cells[row, 5].Text,
                            Email = worksheet.Cells[row, 6].Text,
                            SpouseInfo = worksheet.Cells[row, 8].Text,
                            SpousePhoneNumber = worksheet.Cells[row, 9].Text,
                            //  DateJoined = String.IsNullOrEmpty((worksheet.Cells[row, 10].Text).Trim()) ? String.Empty : HelperMethods.formatExcelDate(worksheet.Cells[row, 10].Text)
                        };

                        employeeList.Add(employee);
                    }
                }
            }

            if (employeeList.Count == 0)
                return BadRequest("The Excel file contains no employee data.");

            foreach (var employeeDto in employeeList)
            {
                if (String.IsNullOrEmpty(employeeDto.Email) || String.IsNullOrEmpty(employeeDto.PhoneNumber))
                {
                    continue;
                    //return BadRequest( new { message = "The Excel file contains no employee data." });
                }
                // Check if the employee already exists by email
                var existingEmployee = await _dbContext.Employees
                    .Where(e => e.Email == employeeDto.Email)
                    .FirstOrDefaultAsync();

                if (existingEmployee != null)
                    continue; // Skip this employee if they already exist

                employeeDto.FirstName.Trim();
                employeeDto.LastName.Trim();
                employeeDto.Email.Trim();
                employeeDto.PhoneNumber.Trim();
                employeeDto.Address.Trim();
                employeeDto.SpouseInfo.Trim();
                employeeDto.LastName.Trim();

                // Create and save new employee
                var employee = new Employee
                {
                    FirstName = employeeDto.FirstName,
                    LastName = employeeDto.LastName,
                    Email = employeeDto.Email,
                    PhoneNumber = employeeDto.PhoneNumber,
                    Address = employeeDto.Address,
                    DOB = employeeDto.DOB,
                    SpouseInfo = employeeDto.SpouseInfo,
                    SpousePhoneNumber = employeeDto.SpousePhoneNumber,
                    CreationMode = EmployeeCreationMode.Bulk
                    // ... (set other fields as necessary)
                };

                await _dbContext.Employees.AddAsync(employee);

                // Create EmployeeContract object
                var employeeContract = new EmployeeContract
                {
                    EmployeeId = employee.Id,
                    //DateJoined = DateTime.Parse(employeeDto.DateJoined),
                    IsOnboardingComplete = true,
                    Status = EmployeeStatus.Active,
                };

                await _dbContext.EmployeeContracts.AddAsync(employeeContract);

                // Create ApplicationUser
                string password = $"{employeeDto.FirstName.Trim().ToUpper()}pass@123";
                var applicationUser = new ApplicationUser
                {
                    UserName = employeeDto.Email.Trim(),
                    FirstName = employeeDto.FirstName,
                    LastName = employeeDto.LastName,
                    Email = employeeDto.Email.Trim(),
                    PhoneNumber = employeeDto.PhoneNumber,
                    Role = ApplicationRoles.Employee,
                    PasswordHash = _userManager.PasswordHasher.HashPassword(null, password)
                };

                var result = await _userManager.CreateAsync(applicationUser, password);
                if (!result.Succeeded)
                {
                    if (result.Errors?.FirstOrDefault()?.Code == "DuplicateUserName")
                    {
                        return BadRequest(new { message = $"Employee with {employeeDto.Email} already exists." });
                    }
                    return BadRequest(new { message = $"Failed to create user.{employeeDto.FirstName}" });
                }

                await _userManager.AddToRoleAsync(applicationUser, ApplicationRoles.Employee.ToString());

                var savedEmployee = await _dbContext.Employees.Where(x => x.Id == employee.Id).FirstOrDefaultAsync();
                savedEmployee.UserId = Guid.Parse(applicationUser.Id);
            }

            await _dbContext.SaveChangesAsync();

            return Ok(new { message = "Employees uploaded successfully." });
        }


        [HttpGet("download-template")]
        public IActionResult DownloadEmployeeTemplate()
        {
            // Create a new workbook and worksheet
            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add("Employee Template");

                // Define the column headers
                worksheet.Cell(1, 1).Value = "LASTNAME";
                worksheet.Cell(1, 2).Value = "FIRSTNAME";
                worksheet.Cell(1, 3).Value = "PHONENUMBER";
                worksheet.Cell(1, 4).Value = "ALTERNATIVEPHONENUMBER";
                worksheet.Cell(1, 5).Value = "ADDRESS";
                worksheet.Cell(1, 6).Value = "EMAIL";
                worksheet.Cell(1, 7).Value = "DOB";
                worksheet.Cell(1, 8).Value = "SPOUSEINFO";
                worksheet.Cell(1, 9).Value = "SPOUSEPHONENUMBER";
                worksheet.Cell(1, 10).Value = "DATEJOINED";

                // Adjust column widths for better readability
                worksheet.Columns().AdjustToContents();

                // Prepare the Excel file for download
                using (var stream = new MemoryStream())
                {
                    workbook.SaveAs(stream);
                    var content = stream.ToArray();

                    // Return the Excel file as a downloadable content
                    return File(content, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "EmployeeTemplate.xlsx");
                }
            }
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
