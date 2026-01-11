namespace BloodBank.Application.DTOs
{
    public class CreateDonorDTO
    {
        public int BloodTypeId { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string HealthStatus { get; set; }
    }
}
