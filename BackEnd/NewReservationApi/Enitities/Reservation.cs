using System.Data;

namespace NewReservationApi.Enitities
{
    public class Reservation
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string FamilyId { get; set; } = string.Empty;
        public string DriverId { get; set; } = string.Empty;
        public string CarId { get; set; } = string.Empty;
        public string Description { get; set; } = "Not Available";
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public bool IsAllDay { get; set; }
        public bool IsBlock { get; set; }
        public string RecurrenceRule { get; set; } = string.Empty;
        public required Driver Driver { get; set; }
        public required Family Family { get; set; }
        public required Car Car { get; set; }

    }
}
