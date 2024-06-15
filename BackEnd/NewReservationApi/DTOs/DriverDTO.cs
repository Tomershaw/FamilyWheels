using System.Globalization;

namespace NewReservationApi.DTOs
{
    public class DriverDTO
    {
        public string? Id { get; set; }
        public string? DriverName { get; set; }
        public string? Fullname { get; set; }
        public int? GroupId { get; set; } = 1;
        public string? Color { get; set; } = "#bbdc00";

    }
}
