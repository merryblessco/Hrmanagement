using AutoMapper;
using Azure.Core;
using HRbackend.Data;
using HRbackend.Models.ApplicantsModel;
using HRbackend.Models.EmployeeModels;
using HRbackend.Models.Entities.Employees;
using HRbackend.Models.Entities.Recruitment;
using HRbackend.Models.Enums;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Net.Http.Headers;

namespace HRbackend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OnboardingController : ControllerBase
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly IWebHostEnvironment _environment;
        private readonly IMapper _mapper;
        private readonly string _filePath = "";  // Set your file path here
        public OnboardingController(ApplicationDbContext dbContext, IWebHostEnvironment environment, IMapper mapper)
        {
            _environment = environment ?? throw new ArgumentNullException(nameof(environment));
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
            _mapper = mapper;

           
        }
       
        [HttpGet("getallOnboarded")]
        public async Task<ActionResult<IEnumerable<OnboardingDto>>> GetOnboardings()
        {
            var onboarded = await _dbContext.Onboardings.ToListAsync();
            var onboarded2 = _mapper.Map<IEnumerable<OnboardingDto>>(onboarded);
           
            return Ok(onboarded2);
        }
        // GET: api/onboarding/{id}
        [HttpGet("getallOnboardedBy/{EmployeeID}")]
        public async Task<ActionResult<Onboarding>> GetOnboarding(int EmployeeID)
        {
            var onboarding = await _dbContext.Onboardings
                                             .FirstOrDefaultAsync(o => o.EmployeeID == EmployeeID);

            if (onboarding == null)
            {
                return NotFound();
            }

            return Ok(onboarding);
        }
        // POST: api/onboarding
        [HttpPost("AddNew-Onboarding")]
        public ActionResult<Onboarding> CreateOnboarding([FromForm] OnboardingDto onboarding)
        {
            var createdDate = DateTime.Now;
            onboarding.CreatedDate = createdDate;
           var onboardingRequest = new Onboarding
            {
                EmployeeID = onboarding.EmployeeID,
               ResumptionDate = onboarding.ResumptionDate,
                OfferLetterStatus = (int)OfferLetterStatus.Sent,
                WelcomeEmailStatus = (int)WelcomeEmailStatus.Pending,
                PaperworkStatus = (int)PaperworkStatus.Completed,
                EquipmentStatus = (int)EquipmentStatus.Inprogress,
                CreatedDate = onboarding.CreatedDate
            };
            // Send the request
            _dbContext.Onboardings.Add(onboardingRequest);
            _dbContext.SaveChanges();

            return CreatedAtAction(nameof(CreateOnboarding), new { id = onboarding.EmployeeID }, onboardingRequest);
        }
        [HttpPut("UpdateOnboardedBy/{EmployeeID}")]
        public async Task<IActionResult> UpdateOnboarding(int EmployeeID, [FromForm] OnboardingDto onboarding)
        {
            var onboardRequest = await _dbContext.Onboardings.FindAsync(EmployeeID);
            if (onboarding == null)
            {
                return NotFound();
            }

            onboardRequest.ResumptionDate = onboarding.ResumptionDate;
            onboardRequest.OfferLetterStatus = (int)onboarding.OfferLetterStatus;
            onboardRequest.WelcomeEmailStatus = (int)onboarding.WelcomeEmailStatus;
            onboardRequest.PaperworkStatus = (int)onboarding.PaperworkStatus;
            onboardRequest.EquipmentStatus = (int)onboarding.EquipmentStatus;

            _dbContext.Entry(onboardRequest).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
            _dbContext.SaveChanges();

            return NoContent();
        }
        [HttpDelete("DeleteOnboardedBy/{EmployeeID}")]
        public IActionResult DeleteOnboarding(int EmployeeID)
        {
            var onboarding = _dbContext.Onboardings.Find(EmployeeID);
            if (onboarding == null)
            {
                return NotFound();
            }

            _dbContext.Onboardings.Remove(onboarding);
            _dbContext.SaveChanges();

            return NoContent();
        }

    }
}
