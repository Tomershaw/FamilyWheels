namespace NewReservationApi.DTOs;

public class ReservationDTO
{
    public required string Id { get; set; }
    public string Subject { get; set; } = "Not Available";
    public required string DriverId { get; set; } 
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public bool IsAllDay { get; set; }
    public bool IsBlock { get; set; }
    public string CarType { get; set; } = string.Empty;
    public string RecurrenceRule { get; set; } = string.Empty;
}
