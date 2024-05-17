namespace NewReservationApi.Enitities
{
    public class Car
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string FamilyId { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public required Family Family { get; set; }
        public virtual ICollection<Reservation>? Reservations { get; set; }
    }
}
