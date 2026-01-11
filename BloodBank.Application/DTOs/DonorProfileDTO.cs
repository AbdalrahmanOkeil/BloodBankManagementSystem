namespace BloodBank.Application.DTOs
{
    public class DonorProfileDTO
    {
        public Guid Id { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string BloodType { get; set; }
        public bool IsEligible { get; set; }
        public DateTime? LastDonationDate { get; set; }
        public string HealthStatus { get; set; } = string.Empty;
        public int TotalDonations = 0;
    }
}
