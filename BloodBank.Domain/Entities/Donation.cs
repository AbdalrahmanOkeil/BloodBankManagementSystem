using BloodBank.Domain.Enums;

namespace BloodBank.Domain.Entities
{
    public class Donation
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        public Guid DonorId { get; set; }
        public Donor Donor { get; set; }

        public int BloodTypeId { get; set; }
        public BloodType BloodType { get; set; }

        public DateTime DonationDate { get; set; }
        public int UnitsDonated { get; set; } = 1;

        public DonationStatus Status { get; set; } = DonationStatus.Pending;
    }
}
