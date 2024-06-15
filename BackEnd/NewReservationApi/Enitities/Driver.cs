using Microsoft.AspNetCore.Identity;

namespace NewReservationApi.Enitities;

public class Driver : IdentityUser
{
    public string? Fullname { get; set; }    
    public string? FamilyId { get; set; }
    public virtual ICollection<Reservation>? Reservations { get; set; }
    public Family? Family { get; set; }
}
