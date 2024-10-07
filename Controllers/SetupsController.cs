﻿using AutoMapper;
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

    }
}
