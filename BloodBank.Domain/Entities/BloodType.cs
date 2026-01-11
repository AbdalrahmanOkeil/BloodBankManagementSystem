namespace BloodBank.Domain.Entities
{
    public class BloodType
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public ICollection<Donor> Donors { get; set; } = new List<Donor>();
        public ICollection<Donation> Donations { get; set; } = new List<Donation>();
        public ICollection<BloodRequest> BloodRequests { get; set; } = new List<BloodRequest>();
        public BloodStock Stock { get; set; }
    }
}
