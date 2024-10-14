using AutoMapper;
using HRbackend.Data;
using HRbackend.Models.EmployeeModels;
using HRbackend.Models.Entities.Employees;
using HRbackend.Models.Entities.PayRoll;
using HRbackend.Models.Entities.Recruitment;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HRbackend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PayRollController : ControllerBase
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly IWebHostEnvironment _environment;
        private readonly IMapper _mapper;
        private readonly string _filePath = "";  // Set your file path here

        public PayRollController(ApplicationDbContext dbContext, IWebHostEnvironment environment, IMapper mapper)
        {
            _environment = environment ?? throw new ArgumentNullException(nameof(environment));
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
            _mapper = mapper;


        }
        [HttpGet("getallSalaries")]
        public async Task<ActionResult<IEnumerable<SalaryCalculationDto>>> GetSalaries()
        {
            var salaries = await _dbContext.SalaryCalculations.ToListAsync();
            var salaries2 = _mapper.Map<IEnumerable<SalaryCalculationDto>>(salaries);

            return Ok(salaries2);
        }
        [HttpGet("getallSalariesby/{EmployeeId}")]
        public async Task<ActionResult<IEnumerable<SalaryCalculation>>> GetSalariesByEmployeId(Guid EmployeeId)
        {
            var salaries = await _dbContext.SalaryCalculations.FirstOrDefaultAsync(o => o.Id == EmployeeId);

            if (salaries == null)
            {
                return NotFound();
            }

            return Ok(salaries);
        }
        [HttpPost]
        public async Task<ActionResult<SalaryCalculation>> CreateSalaryCalculation([FromForm] SalaryCalculationDto salaryCalculation)
        {
            var dateCreated = DateTime.Now;
            salaryCalculation.CalculationDate= dateCreated;
            var salary =new SalaryCalculation 
            { 
                EmployeeId = salaryCalculation.EmployeeId,
                Fullname = salaryCalculation.Fullname,
                BasicSalary = salaryCalculation.BasicSalary,
                AdjustmentType = salaryCalculation.AdjustmentType,
                Bonus = salaryCalculation.Bonus,
                Deductions = salaryCalculation.Deductions,
                Resoans = salaryCalculation.Resoans,
                CalculationDate = salaryCalculation.CalculationDate

            };
            _dbContext.SalaryCalculations.Add(salary);
            await _dbContext.SaveChangesAsync();

            return CreatedAtAction(nameof(CreateSalaryCalculation), new { id = salaryCalculation.EmployeeId }, salaryCalculation);
        }
        [HttpPut("UpdateSalaryBy/{EmployeeId}")]
        public async Task<IActionResult> UpdateSalaryCalculation(Guid EmployeeId, [FromForm] SalaryCalculation salaryCalculation)
        {
            var salary = await _dbContext.SalaryCalculations.FindAsync(EmployeeId);
            if (EmployeeId != salaryCalculation.EmployeeId)
            {
                return BadRequest();
            }
            var dateCreated = DateTime.Now;
            salaryCalculation.CalculationDate = dateCreated;
            salary.EmployeeId = salaryCalculation.EmployeeId;
            salary.Fullname = salaryCalculation.Fullname;
            salary.AdjustmentType = salaryCalculation.AdjustmentType;
            salary.BasicSalary = salaryCalculation.BasicSalary;
            salary.Bonus = salaryCalculation.Bonus;
            salary.Deductions = salaryCalculation.Deductions;
            salary.Resoans = salaryCalculation.Resoans;
            salary.CalculationDate = salaryCalculation.CalculationDate;

            _dbContext.Entry(salary).State = EntityState.Modified;
            await _dbContext.SaveChangesAsync();

            return NoContent();
        }
        // DELETE: api/SalaryCalculation/{id}
        [HttpDelete("DeleteSalaryBy/{EmployeeId}")]
        public async Task<IActionResult> DeleteSalaryCalculation(int EmployeeId)
        {
            var salaryCalculation = await _dbContext.SalaryCalculations.FindAsync(EmployeeId);
            if (salaryCalculation == null)
            {
                return NotFound();
            }

            _dbContext.SalaryCalculations.Remove(salaryCalculation);
            await _dbContext.SaveChangesAsync();

            return NoContent();
        }
        [HttpGet("GetAllPayslips")]
        public async Task<ActionResult<IEnumerable<PaySlipDto>>> GetPaySlips()
        {
            var payslip = await _dbContext.PaySlips.ToListAsync();
            var payslip2 = _mapper.Map<IEnumerable<PaySlipDto>>(payslip);

            return Ok(payslip2);
        }
        [HttpGet("GetpaySlipsby/{EmployeeId}")]
        public async Task<ActionResult<PaySlip>> GetPaySlip(Guid EmployeeId)
        {
            var paySlip = await _dbContext.PaySlips
                                            .FirstOrDefaultAsync(o => o.Id == EmployeeId);

            if (paySlip == null)
            {
                return NotFound();
            }

            return Ok(paySlip);
        }
        // POST: api/PaySlip
        [HttpPost("Generate-Payslip")]
        public async Task<ActionResult<PaySlip>> CreatePaySlip([FromForm] PaySlipDto paySlip)
        {
            // Generate the PaySlip DateGenerated value automatically
            paySlip.DateGenerated = DateTime.Now;
            var payer=new PaySlip 
            { 
                DateGenerated = DateTime.Now,
                PayPeriod = paySlip.PayPeriod,
                TotalEarnings = paySlip.TotalEarnings,
                TotalDeductions = paySlip.TotalDeductions,
                EmployeeID = paySlip.EmployeeId
            };


            _dbContext.PaySlips.Add(payer);
            await _dbContext.SaveChangesAsync();

            return CreatedAtAction(nameof(CreatePaySlip), new { id = paySlip.EmployeeId }, paySlip);
        }
        // PUT: api/PaySlip/{id}
        [HttpPut("Update-Payslip-By/{EmployeeId}")]
        public async Task<IActionResult> UpdatePaySlip(Guid EmployeeId, [FromForm] PaySlipDto paySlip)
        {
            var payslip = await _dbContext.PaySlips.FindAsync(EmployeeId);
            if (EmployeeId != paySlip.EmployeeId)
            {
                return BadRequest();
            }
            payslip.EmployeeID = paySlip.EmployeeId;
            payslip.PayPeriod = paySlip.PayPeriod;
            payslip.TotalEarnings = paySlip.TotalEarnings;
            payslip.TotalDeductions = paySlip.TotalDeductions;
            _dbContext.Entry(paySlip).State = EntityState.Modified;

            try
            {
                await _dbContext.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PaySlipExists(EmployeeId))
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
        [HttpDelete("Delete-PaySlip-By/{EmployeeId}")]
        public async Task<IActionResult> DeletePaySlip(int EmployeeId)
        {
            var paySlip = await _dbContext.PaySlips.FindAsync(EmployeeId);
            if (paySlip == null)
            {
                return NotFound();
            }

            _dbContext.PaySlips.Remove(paySlip);
            await _dbContext.SaveChangesAsync();

            return NoContent();
        }

        private bool PaySlipExists(Guid EmployeeId)
        {
            return _dbContext.PaySlips.Any(e => e.Id == EmployeeId);
        }

        // GET: api/BenefitAdministration
        [HttpGet("AllBenefits")]
        public async Task<ActionResult<IEnumerable<BenefitAdministrationDto>>> GetBenefitAdministrations()
        {
            var benefit = await _dbContext.BenefitAdmin.ToListAsync();
            var benefit2 = _mapper.Map<IEnumerable<BenefitAdministrationDto>>(benefit);

            return Ok(benefit2);
        }
        [HttpGet("AllBenefits-By/{EmployeeId}")]
        public async Task<ActionResult<IEnumerable<BenefitAdministrationDto>>> GetBenefitAdministrationsById(Guid EmployeeId)
        {
            var salaries = await _dbContext.BenefitAdmin.FirstOrDefaultAsync(o => o.Id == EmployeeId);

            if (salaries == null)
            {
                return NotFound();
            }
            return Ok(salaries);
        }
        // POST: api/BenefitAdministration
        [HttpPost("Add-Staff-Benefit")]
        public async Task<ActionResult<BenefitAdministration>> CreateBenefitAdministration([FromForm] BenefitAdministrationDto benefitAdministration)
        {
            benefitAdministration.EffectiveDate = DateTime.Now;
            var bennefit = new BenefitAdministration
            {
                EffectiveDate = benefitAdministration.EffectiveDate,
                EmployeeID = benefitAdministration.EmployeeID,
                BenefitType = benefitAdministration.BenefitType,
                BenefitAmount = benefitAdministration.BenefitAmount
               
            };
            _dbContext.Add(bennefit);
            await _dbContext.SaveChangesAsync();

            return CreatedAtAction(nameof(CreateBenefitAdministration), new { id = benefitAdministration.EmployeeID }, benefitAdministration);
        }
        [HttpPut("Update-Benefits-By{EmployeeId}")]
        public async Task<IActionResult> UpdateBenefitAdministration(Guid EmployeeId, [FromForm] BenefitAdministrationDto benefitAdministration)
        {
            var benefit = await _dbContext.BenefitAdmin.FindAsync(EmployeeId);
            if (EmployeeId != benefitAdministration.EmployeeID)
            {
                return BadRequest();
            }
            benefit.EmployeeID = benefitAdministration.EmployeeID;
            benefit.BenefitType = benefitAdministration.BenefitType;
            benefit.BenefitAmount = benefitAdministration.BenefitAmount;
            

            _dbContext.Entry(benefitAdministration).State = EntityState.Modified;

            try
            {
                await _dbContext.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!BenefitAdministrationExists(EmployeeId))
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
        private bool BenefitAdministrationExists(Guid EmployeeId)
        {
            return _dbContext.BenefitAdmin.Any(e => e.EmployeeID == EmployeeId);
        }
        // DELETE: api/BenefitAdministration/{id}
        [HttpDelete("Delete-benefit-By/{EmployeeId}")]
        public async Task<IActionResult> DeleteBenefitAdministration(Guid EmployeeId)
        {
            var benefitAdministration = await _dbContext.BenefitAdmin.FindAsync(EmployeeId);
            if (benefitAdministration == null)
            {
                return NotFound();
            }

            _dbContext.BenefitAdmin.Remove(benefitAdministration);
            await _dbContext.SaveChangesAsync();

            return NoContent();
        }
        // GET: api/TaxManagement
        [HttpGet("Get-All-Taxes")]
        public async Task<ActionResult<IEnumerable<TaxManagementDto>>> GetTaxManagements()
        {
            var tax = await _dbContext.Taxes.ToListAsync();
            var tax2 = _mapper.Map<IEnumerable<TaxManagementDto>>(tax);

            return Ok(tax2);
        }
        // GET: api/TaxManagement/{id}
        [HttpGet("Get-Tax-By/{EmployeeId}")]
        public async Task<ActionResult<TaxManagement>> GetTaxManagement(Guid EmployeeId)
        {
            var tax = await _dbContext.Taxes.FirstOrDefaultAsync(o => o.EmployeeId == EmployeeId);

            if (tax == null)
            {
                return NotFound();
            }
            return Ok(tax);
        }
        // POST: api/TaxManagement
        [HttpPost("Add-Staff-Tax")]
        public async Task<ActionResult<TaxManagement>> CreateTaxManagement([FromForm] TaxManagementDto taxManagement)
        {
            
            var tax = new TaxManagement
            {
                EmployeeId = taxManagement.EmployeeId,
                IncomeTax = taxManagement.IncomeTax,
                SocialSecurityTax = taxManagement.SocialSecurityTax,
                MedicareTax = taxManagement.MedicareTax,
                TaxYear     = taxManagement.TaxYear
            };
            _dbContext.Taxes.Add(tax);
            await _dbContext.SaveChangesAsync();

            return CreatedAtAction(nameof(CreateTaxManagement), new { id = taxManagement.EmployeeId }, taxManagement);
        }
        // PUT: api/TaxManagement/{id}
        [HttpPut("Update-Tax-By/{EmployeeId}")]
        public async Task<IActionResult> UpdateTaxManagement(Guid EmployeeId, [FromForm] TaxManagement taxManagement)
        {

            var tax = await _dbContext.Taxes.FindAsync(EmployeeId);
            if (EmployeeId != taxManagement.EmployeeId)
            {
                return BadRequest();
            }
            tax.EmployeeId = taxManagement.EmployeeId;
            tax.IncomeTax = taxManagement.IncomeTax;
            tax.SocialSecurityTax = taxManagement.SocialSecurityTax;
            tax.MedicareTax = taxManagement.MedicareTax;
            tax.TaxYear = taxManagement.TaxYear;
            

            _dbContext.Entry(taxManagement).State = EntityState.Modified;

            try
            {
                await _dbContext.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TaxManagementExists(EmployeeId))
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
        // DELETE: api/TaxManagement/{id}
        [HttpDelete("Delete-Tax-By/{EmployeeId}")]
        public async Task<IActionResult> DeleteTaxManagement(int EmployeeId)
        {
            var taxManagement = await _dbContext.Taxes.FindAsync(EmployeeId);
            if (taxManagement == null)
            {
                return NotFound();
            }

            _dbContext.Taxes.Remove(taxManagement);
            await _dbContext.SaveChangesAsync();

            return NoContent();
        }

        // Helper method to check if a TaxManagement record exists
        private bool TaxManagementExists(Guid EmployeeId)
        {
            return _dbContext.Taxes.Any(e => e.EmployeeId == EmployeeId);
        }


    }

}
