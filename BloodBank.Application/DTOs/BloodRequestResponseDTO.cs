namespace BloodBank.Application.DTOs
{
    public class BloodRequestResponseDTO
    {
        public Guid Id { get; set; }
        public string BloodType { get; set; }
        public int Units { get; set; }
        public string Status { get; set; }
        public DateTime RequestedAt { get; set; }
    }
}
