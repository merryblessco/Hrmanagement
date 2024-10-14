namespace HRbackend.Models.SetupModels
{
    public class PositionDto
    {
        public Guid Id { get; set; }

        public string Name { get; set; }
        public string? Description { get; set; }
        public Guid? DepartmentId { get; set; }
    }
}
