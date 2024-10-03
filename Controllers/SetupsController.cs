using AutoMapper;
using HRbackend.Data;
using HRbackend.Models.SetupModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

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
        [Route("GetAllLGA")]
        public IActionResult GetLgs()
        {
            var allLga = _dbContext.LGAs.ToList();

            return Ok(allLga);
        }
    }
}
