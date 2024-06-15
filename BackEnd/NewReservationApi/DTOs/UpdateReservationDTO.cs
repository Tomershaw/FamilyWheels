namespace NewReservationApi.DTOs
{
    public class UpdateReservationDTO
    {
        public required string Id { get; set; }
        public string Description { get; set; } = "Not Available";
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public bool IsAllDay { get; set; }
        public string RecurrenceRule { get; set; } = string.Empty;
        public string CarType { get; set; } = string.Empty;

    }
}
