namespace BloodBank.Application.DTOs
{
    public class DonationResponseDTO
    {
        public Guid Id { get; set; }
        public DateTime DonationDate { get; set; }
        public int Units { get; set; }
        public string Status { get; set; }
    }
}
