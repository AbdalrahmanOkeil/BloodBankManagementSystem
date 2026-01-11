namespace BloodBank.Domain.Entities
{
    public class Hospital
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string UserId { get; set; }

        public string Name { get; set; }
        public string Address { get; set; }

        public ICollection<BloodRequest> BloodRequests { get; set; } = new List<BloodRequest>();
    }
}
