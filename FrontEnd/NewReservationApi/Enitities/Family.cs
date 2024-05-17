namespace NewReservationApi.Enitities
{
    public class Family
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string Name { get; set; } = string.Empty;
        public virtual ICollection<Car>? Cars { get; set; }
        public virtual ICollection<Driver>? Drivers { get; set; }
        public virtual ICollection<Reservation>? Reservations { get; set; }

    }
}
