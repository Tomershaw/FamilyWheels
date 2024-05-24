namespace NewReservationApi.DTOs
{
    public class AddResevationDTO
    {
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public bool IsAllDay { get; set; }
        public bool IsBlock { get; set; }
        public string CarType { get; set; } = string.Empty;
        public string Description {  get; set; } = string.Empty;
        public string RecurrenceRule { get; set; } = string.Empty;

    }
}
