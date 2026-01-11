namespace BloodBank.Application.DTOs
{
    public class CreateBloodRequestDTO
    {
        public int BloodTypeId {  get; set; }
        public int Units { get; set; }
        public string UrgencyLevel { get; set; }
    }
}
