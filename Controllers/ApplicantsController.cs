﻿using AutoMapper;
using HRbackend.Data;
using HRbackend.Models.ApplicantsModel;
using HRbackend.Models.Entities.Recruitment;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HRbackend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ApplicantsController : ControllerBase
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly IWebHostEnvironment _environment;
        private readonly IMapper _mapper;
        public ApplicantsController(ApplicationDbContext dbContext, IWebHostEnvironment environment, IMapper mapper)
        {
            _environment = environment ?? throw new ArgumentNullException(nameof(environment));
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
            _mapper = mapper;
        }
        // GET: api/Applicants
        [HttpGet]
        public async Task<IActionResult> GetApplicants()
        {
            var applicants = await _dbContext.Applicants.ToListAsync();
            return Ok(applicants);
        }

        // GET: api/Applicants/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetApplicant(int id)
        {
            var applicant = await _dbContext.Applicants.FindAsync(id);

            if (applicant == null)
            {
                return NotFound();
            }

            return Ok(applicant);
        }
        // POST: api/Applicants
        [HttpPost]
        public async Task<IActionResult> CreateApplicant([FromForm] ApplicantsDto request)
        {
            var fileName = await SaveResumeFile(request.Resume);
            var applicant = new Applicants
            {
                JobID = request.JobID,
                FirstName = request.FirstName,
                LastName = request.LastName,
                Fullname = request.Fullname,
                PhoneNumber = request.PhoneNumber,
                Email = request.Email,
                ResumeFilePath = fileName,
                ApplicationDate = DateTime.Now,
                DOB = request.DOB,
                Status = request.Status,
                Coverletter = request.Coverletter
            };

            _dbContext.Applicants.Add(applicant);
            await _dbContext.SaveChangesAsync();

            return CreatedAtAction(nameof(GetApplicant), new { id = applicant.ApplicantID }, applicant);
        }

        // PUT: api/Applicants/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateApplicant(int id, [FromForm] ApplicantsDto request)
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
            applicant.Status = request.Status;
            applicant.Coverletter = request.Coverletter;

            if (request.Resume != null)
            {
                var fileName = await SaveResumeFile(request.Resume);
                applicant.ResumeFilePath = fileName;
            }

            _dbContext.Entry(applicant).State = EntityState.Modified;
            await _dbContext.SaveChangesAsync();

            return NoContent();
        }
        // DELETE: api/Applicants/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteApplicant(int id)
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

        // Save Resume File (PDF/Word)
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


        // Save Resume File (PDF/Word)
        /* private async Task<string> SaveResumeFile(IFormFile file)
         {
             if (file == null || (file.ContentType != "application/pdf" &&
                                  file.ContentType != "application/msword" &&
                                  file.ContentType != "application/vnd.openxmlformats-officedocument.wordprocessingml.document"))
             {
                 throw new Exception("Invalid file format. Only PDF and Word documents are allowed.");
             }

             var uploadsFolder = Path.Combine(_environment.WebRootPath, "resumes");
             if (!Directory.Exists(uploadsFolder))
             {
                 Directory.CreateDirectory(uploadsFolder);
             }

             var fileName = $"{Guid.NewGuid()}_{Path.GetFileName(file.FileName)}";
             var filePath = Path.Combine(uploadsFolder, fileName);

             using (var stream = new FileStream(filePath, FileMode.Create))
             {
                 await file.CopyToAsync(stream);
             }

             return fileName;
         }*/
    }

}

