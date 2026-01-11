using BloodBank.Domain.Enums;

namespace BloodBank.Application.DTOs
{
    public class DonationDTO
    {
        public Guid Id { get; set; }
        public DateTime DonationDate { get; set; }
        public int UnitsDonated { get; set; }
        public DonationStatus Status { get; set; }
        public string BloodType { get; set; }
    }
}
