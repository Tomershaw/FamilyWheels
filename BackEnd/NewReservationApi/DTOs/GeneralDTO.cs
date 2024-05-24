namespace NewReservationApi.DTOs
{
    public class GeneralDTO
    {
        public string CurrentUserId { get; set; } = string.Empty;
        public List<CarDTO> Cars { get; set; } = new List<CarDTO>();
        public List<DriverDTO> Drivers { get; set; } = new List<DriverDTO>();
        public List<ReservationDTO> Reservations { get; set; } = new List<ReservationDTO>();
    }

}
