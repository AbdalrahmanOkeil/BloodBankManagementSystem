namespace BloodBank.Domain.Entities
{
    public class Donor
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string UserId { get; set; }

        public int BloodTypeId { get; set; }
        public BloodType BloodType { get; set; }

        public DateTime? LastDonationDate { get; set; }
        public int TotalDonations { get; set; }
        public string HealthStatus { get; set; }
        public DateTime DateOfBirth { get; set; }

        public ICollection<Donation> Donations { get; set; } = new List<Donation>();
    }
}
