using HRbackend.Models.Payroll;
using HRbackend.Models.SetupModels;

public class SetupModel
{
    public List<DepertmentDto> Departments { get; set; } = new List<DepertmentDto>();

    public List<TaxDto> Taxes { get; set; } = new List<TaxDto>();

    public string DefaultCurrency { get; set; }

    public string WorkweekDays { get; set; }
}

