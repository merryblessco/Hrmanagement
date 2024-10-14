using AutoMapper;
using HRbackend.Data;
using HRbackend.Models.ApplicantsModel;
using HRbackend.Models.Auth;
using HRbackend.Models.Entities.Recruitment;
using HRbackend.Models.Enums;
using HRbackend.Models.RecruitmentModel;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Net.Http;
using System.Net.Http.Headers;
using static System.Net.Mime.MediaTypeNames;

namespace HRbackend.Controllers
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Route("api/[controller]")]
    [ApiController]
    public class ApplicantsController : BaseController
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly IWebHostEnvironment _environment;
        private readonly IMapper _mapper;
        private readonly string _filePath = "";  // Set your file path here
        private readonly IHttpContextAccessor _contextAccessor;
        public ApplicantsController(ApplicationDbContext dbContext, IWebHostEnvironment environment, IMapper mapper, UserManager<ApplicationUser> userManager, IHttpContextAccessor contextAccessor) : base(userManager, contextAccessor)
        {
            _environment = environment ?? throw new ArgumentNullException(nameof(environment));
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
            _mapper = mapper;
        }
      
        [HttpGet("getallApplicants")]
        public async Task<ActionResult<IEnumerable<ApplicantsDto>>> GetAll()
        {
            var applications = await _dbContext.Applicants.ToListAsync();
            var applications2 = _mapper.Map<IEnumerable<ApplicantsDto>>(applications);
            foreach (var application in applications2)
            {
                application.StatusText = application.Status.GetDescription();
            }
            return Ok(applications2);
        }
      
        [HttpGet("{ApplicantID}")]
        public async Task<IActionResult> GetApplicant(Guid ApplicantID)
        {
            var applicant = await _dbContext.Applicants.FindAsync(ApplicantID);

            if (applicant == null)
            {
                return NotFound();
            }

            var applicant1 = _mapper.Map<ApplicantsDto>(applicant);
            applicant1.StatusText = applicant.Status.GetDescription();

            return Ok(applicant1);
        }
        
        [HttpGet("get-all-applicants-by-job-id/{JobId}")]
        public async Task<IActionResult> GetApplicants(Guid JobId)
        {
            var applicants = await _dbContext.Applicants
                                             .Where(a => a.JobID == JobId)
                                             .ToListAsync();

            if (applicants == null || !applicants.Any())
            {
                return NotFound();
            }

            var applications2 = _mapper.Map<IEnumerable<ApplicantsDto>>(applicants);
            foreach (var application in applications2)
            {
                application.StatusText = application.Status.GetDescription();
            }
            return Ok(applications2);
        }

        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> CreateApplicant([FromForm] ApplicantsDto request)
        {


            // Check if the file is valid
            if (request.Resume == null || request.Resume.Length == 0)
            {
                return BadRequest("Resume file is required.");
            }

            // Validate file types (pdf, jpeg, word)
            var allowedExtensions = new[] { ".pdf", ".jpeg", ".jpg", ".doc", ".docx" };
            var fileExtension = Path.GetExtension(request.Resume.FileName).ToLower();

            if (!allowedExtensions.Contains(fileExtension))
            {
                return BadRequest("Only PDF, JPEG, and Word files are allowed.");
            }

            // Convert the file to a byte array
            byte[] fileBytes;
            using (var memoryStream = new MemoryStream())
            {
                await request.Resume.CopyToAsync(memoryStream);
                fileBytes = memoryStream.ToArray();
            }

            var formData = new MultipartFormDataContent();
            var fileContent = new ByteArrayContent(fileBytes);
            fileContent.Headers.ContentType = MediaTypeHeaderValue.Parse("multipart/form-data");

            formData.Add(fileContent, "ResumeFile", "resume.pdf");

            // Send the request
            // await httpClient.PostAsync("api/your-endpoint", formData);

            var fileName = await SaveResumeFile(request.Resume);
            var applicant = new Applicants
            {
                JobID = request.JobID,
                FirstName = request.FirstName,
                LastName = request.LastName,
                PhoneNumber = request.PhoneNumber,
                Email = request.Email,
                ResumeFilePath = fileName,
                ApplicationDate = DateTime.Now,
                DOB = request.DOB,
                Status = ApplicationStatus.Applied,
                Coverletter = request.Coverletter,
                ResumeFile = fileBytes
            };

            _dbContext.Applicants.Add(applicant);
            await _dbContext.SaveChangesAsync();

            return CreatedAtAction(nameof(CreateApplicant), new { id = applicant.Id }, applicant);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateApplicant(Guid id, [FromForm] ApplicantsDto request)
        {
            var applicant = await _dbContext.Applicants.FindAsync(id);
            if (applicant == null)
            {
                return NotFound();
            }

            applicant.FirstName = request.FirstName;
            applicant.LastName = request.LastName;
            applicant.PhoneNumber = request.PhoneNumber;
            applicant.Email = request.Email;
            applicant.Status = (ApplicationStatus)request.Status;
            applicant.Coverletter = request.Coverletter;
            applicant.ResumeFile = request.ResumeFile;

            if (request.Resume != null)
            {
                var fileName = await SaveResumeFile(request.Resume);
                applicant.ResumeFilePath = fileName;
            }

            _dbContext.Entry(applicant).State = EntityState.Modified;
            await _dbContext.SaveChangesAsync();

            return NoContent();
        }
      
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteApplicant(Guid id)
        {
            var applicant = await _dbContext.Applicants.FindAsync(id);
            if (applicant == null)
            {
                return NotFound();
            }

            _dbContext.Applicants.Remove(applicant);
            await _dbContext.SaveChangesAsync();

            return NoContent();
        }

        private async Task<string> SaveResumeFile(IFormFile file)
        {
            // Ensure the file is valid
            if (file == null || (file.ContentType != "application/pdf" &&
                                 file.ContentType != "application/msword" &&
                                 file.ContentType != "application/vnd.openxmlformats-officedocument.wordprocessingml.document"))
            {
                throw new Exception("Invalid file format. Only PDF and Word documents are allowed.");
            }

            // Ensure _environment.WebRootPath is not null and fallback to a default path
            var uploadsFolder = _environment.WebRootPath != null
                ? Path.Combine(_environment.WebRootPath, "resumes")
                : Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "resumes");

            // Log the path to ensure it's correct (optional but useful for debugging)
            Console.WriteLine($"Saving file to: {uploadsFolder}");

            if (!Directory.Exists(uploadsFolder))
            {
                Directory.CreateDirectory(uploadsFolder);
            }

            // Generate a unique file name and combine it with the folder path
            var fileName = $"{Guid.NewGuid()}_{Path.GetFileName(file.FileName)}";
            var filePath = Path.Combine(uploadsFolder, fileName);

            // Save the file to the path
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            // Return the saved file name (not the full path)
            return fileName;
        }

        [HttpGet("download-file")]
        public IActionResult DownloadFile()
        {
            try
            {
                // Check if the file exists
                if (!System.IO.File.Exists(_filePath))
                {
                    return NotFound("File not found");
                }

                // Read the file as a byte array
                var fileBytes = System.IO.File.ReadAllBytes(_filePath);

                // Get the content type for the file (e.g., "application/pdf" for a PDF)
                string contentType = "application/octet-stream"; // Default content type for unknown files
                var extension = Path.GetExtension(_filePath);
                switch (extension.ToLower())
                {
                    case ".txt": contentType = "text/plain"; break;
                    case ".pdf": contentType = "application/pdf"; break;
                    case ".jpg": contentType = "image/jpeg"; break;
                    case ".png": contentType = "image/png"; break;
                    case ".json": contentType = "application/json"; break;
                        // Add other types if needed
                }

                // Optionally, specify a filename for download
                string fileName = Path.GetFileName(_filePath);

                // Return the file as a download
                return File(fileBytes, contentType, fileName);
            }
            catch (System.Exception ex)
            {
                // Handle any exceptions (file access errors, etc.)
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPost]
        [Route("send-invite")]
        public async Task<IActionResult> SendInvite([FromBody] SendInvitationRequest interviewDto)
        {

            var applicant = await _dbContext.Applicants.FindAsync(interviewDto.ApplicantID);

            if (applicant == null)
            {
                return NotFound();
            }

            var checkIfApplicationExist = await _dbContext.Interviews.Where(x => x.ApplicantID == applicant.Id && x.JobID == applicant.JobID && !x.IsDeleted).FirstOrDefaultAsync();
            if (checkIfApplicationExist != null)
            {
                return Conflict(new { message = "Invitation Already Sent." });
            }

            // Create Employee object and map fields from DTO
            var interview = new Interview
            {
                JobID = interviewDto.JobID,
                ApplicantID = interviewDto.ApplicantID,
                MeetingLink = interviewDto.MeetingLink,
                MeetingNote = interviewDto.MeetingNote,
                Interviewers = interviewDto.Interviewers,
                InterviewDate = interviewDto.InterviewDate,
                Status = InterViewStatus.scheduled,
                DateCreated = DateTime.Now
            };

            // Save employee to the datainterviewbase
            _dbContext.Interviews.Add(interview);
            await _dbContext.SaveChangesAsync();

            // Send welcome email
            string subject = "Interview Schedule";
            string message = $"<h1>Dear, {applicant.FirstName + " " + applicant.LastName}!</h1><p> Based on your portfolio, We're excited invite for an interview.</p>";
            //await SendEmailAsync(interview.ApplicantEmail, subject, message);

            return Ok(new { message = "Invitation Sent." });
        }

        [HttpPost]
        [Route("invitation")]
        public async Task<IActionResult> Invitation([FromBody] InterviewRequest interviewRequest)
        {
            var interview = await _dbContext.Interviews.Include(x => x.Applicant)
                                                       .Where(x => x.JobID == interviewRequest.JobID && x.ApplicantID == interviewRequest.ApplicantID)
                                                       .FirstOrDefaultAsync();

            if (interview == null)
            {
                return NotFound();
            }

            InterviewDto obj = new InterviewDto
            {
                ApplicantEmail = interview.Applicant.Email,
                ApplicantID = interview.Applicant.Id,
                ApplicatMobile = interview.Applicant.PhoneNumber,
                Fullname = interview.Applicant.FirstName + " " + interview.Applicant.LastName,
                InterviewDate = interview.Applicant.ApplicationDate,
                DateCreated = interview.DateCreated,
                MeetingLink = interview.MeetingLink,
                MeetingNote = interview.MeetingNote,
                Interviewers = interview.Interviewers,
                JobID = interviewRequest.JobID,
                StatusName = interview.Status.GetDescription()
            };

            return Ok(obj);
        }

        [HttpGet]
        [Route("invitations")]
        public async Task<IActionResult> Invitations([FromQuery] Guid jobId)
        {
            var interviews = await _dbContext.Interviews.Include(x => x.Applicant)
                                                       .Where(x => x.JobID == jobId)
                                                       .ToListAsync();

            if (interviews == null)
            {
                return NotFound();
            }

            var interviewsObj = new List<InterviewDto>();

            foreach (var item in interviews)
            {
                var interview = await _dbContext.Interviews.Include(x => x.Applicant)
                                                                      .Where(x => x.JobID == item.JobID && x.ApplicantID == item.ApplicantID)
                                                                      .FirstOrDefaultAsync();

                if (interview == null)
                {
                    return NotFound();
                }

                InterviewDto obj = new InterviewDto
                {
                    ApplicantEmail = interview.Applicant.Email,
                    ApplicantID = interview.Applicant.Id,
                    ApplicatMobile = interview.Applicant.PhoneNumber,
                    Fullname = interview.Applicant.FirstName + " " + interview.Applicant.LastName,
                    InterviewDate = interview.Applicant.ApplicationDate,
                    DateCreated = interview.DateCreated,
                    MeetingLink = interview.MeetingLink,
                    MeetingNote = interview.MeetingNote,
                    Interviewers = interview.Interviewers,
                    JobID = interview.JobID,
                    Status = interview.Status,
                    StatusName = interview.Status.GetDescription()
                };

                interviewsObj.Add(obj);
            }

            return Ok(interviewsObj);
        }
    }

}

