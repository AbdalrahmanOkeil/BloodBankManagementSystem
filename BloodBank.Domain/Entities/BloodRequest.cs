using BloodBank.Domain.Enums;

namespace BloodBank.Domain.Entities
{
    public class BloodRequest
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        public Guid HospitalId { get; set; }
        public Hospital Hospital { get; set; }

        public int BloodTypeId { get; set; }
        public BloodType BloodType { get; set; }

        public int UnitsRequested { get; set; }
        public string UrgencyLevel { get; set; }

        public RequestStatus Status { get; set; } = RequestStatus.Pending;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? ApprovedAt { get; set; }
    }
}
