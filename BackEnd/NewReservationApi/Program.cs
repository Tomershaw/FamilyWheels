using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using NewReservationApi.Database;
using NewReservationApi.DTOs;
using NewReservationApi.Enitities;
using NewReservationApi.Migrations;
using NewReservationApi.Services;
using System.Security.Claims;

//comment

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<ApplicationDbContext>(Options =>
{
    Options.UseSqlServer(builder.Configuration.GetConnectionString("Reservation"));

});
builder.Services.AddCors(options =>
{
    options.AddPolicy("CorsPolicy", builder => builder
        .WithOrigins("http://localhost:5173")
        .AllowAnyMethod()
        .AllowAnyHeader()
        .AllowCredentials());
});

//builder.Services.AddAuthorization();
//builder.Services.AddAuthentication(IdentityConstants.ApplicationScheme)
//    .AddCookie(IdentityConstants.ApplicationScheme)
//    .AddBearerToken(IdentityConstants.BearerScheme);

builder.Services.AddIdentityCore<Driver>()
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddApiEndpoints();

builder.Services.AddAuthentication()
    
   .AddBearerToken(IdentityConstants.BearerScheme);
builder.Services.AddAuthorizationBuilder();

builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.PropertyNamingPolicy = null;
    options.SerializerOptions.Converters.Add(new DateTimeConverter());
});

builder.Services.AddSwaggerGen(opt =>
{
    opt.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Description = "Please enter token",
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        BearerFormat = "JWT",
        Scheme = "bearer"
    });
    opt.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type=ReferenceType.SecurityScheme,
                    Id="Bearer"
                }
            },
            new string[]{}
     }
});
});


var app = builder.Build();

//app.MapPost("/register", async (UserManager<IdentityUser> userManager, RegisterRequest request) =>
//{
//    var user = new IdentityUser { UserName = request.XName, Email = request.Username };
//    var createResult = await userManager.CreateAsync(user, request.Password);

//    if (!createResult.Succeeded)
//    {
//        return Results.BadRequest(new { Errors = createResult.Errors.Select(e => e.Description) });
//    }

//    return Results.Ok("User registration successful");

app.MapGet("/GetFullname", async (ClaimsPrincipal user, UserManager<Driver> userManager) =>
{
    if (user.Identity?.IsAuthenticated != true)
    {
        return Results.Unauthorized();
    }

    var userId = userManager.GetUserId(user);
    var driver = await userManager.FindByIdAsync(userId!);

    if (driver == null)
    {
        return Results.NotFound();
    }

    return Results.Ok(new { Fullname = driver.Fullname });
});

app.MapPost("/AddfullName", async (ClaimsPrincipal user, [FromBody] FullnameDriverDTO FullnameDriverDTO, ApplicationDbContext context, UserManager<Driver> userManager) =>
{
    if (user.Identity?.IsAuthenticated != true)
    {
        return Results.Unauthorized();
    }

    var userId = userManager.GetUserId(user);
    var driver = await userManager.FindByIdAsync(userId!);

    if (driver == null)
    {
        return Results.NotFound();
    }
    driver.PhoneNumber = FullnameDriverDTO.PhoneNumber;
    driver.Fullname = FullnameDriverDTO.Fullname;


    await context.SaveChangesAsync();


    return Results.Ok();
});


app.MapPut("/ChangeReservations" , async (ClaimsPrincipal user, [FromBody] UpdateReservationDTO UpdatereservationDTO, ApplicationDbContext context, UserManager<Driver> userManager) =>
{
    if (user.Identity?.IsAuthenticated != true)
    {
        return Results.Unauthorized();
    }

    var userId = userManager.GetUserId(user);
    var reservation = await context.Reservations
        .FirstOrDefaultAsync(r => r.Id == UpdatereservationDTO.Id);

    if (reservation == null)
    {
        return Results.NotFound("Reservation not found.");
    }

    if (reservation.DriverId != userId)
    {
        return Results.BadRequest("User not found");
    }

    // Replace the existing reservation with the new one
    context.Entry(reservation).CurrentValues.SetValues(UpdatereservationDTO);

    // Save the changes to the database
    await context.SaveChangesAsync();

    return Results.Ok(reservation);
});
//});
app.MapPost("/AddReservations", async (ClaimsPrincipal user, AddResevationDTO addResevationDTO, ApplicationDbContext context, UserManager<Driver> userManager) =>
{
        if (user.Identity?.IsAuthenticated != true)
    {
        return Results.Unauthorized();
    }
    // Get the current user
    var currentUser = await userManager.GetUserAsync(user);
    if (currentUser == null)
    {
        return Results.BadRequest("User not found");
    }
    var car = await context.Cars
                             .Where(x => x.Name == addResevationDTO.CarType && x.FamilyId == currentUser!.FamilyId)
                             .FirstOrDefaultAsync();
    if (car == null)
    {
        return Results.BadRequest("car not found");
    }

    var family = await context.Families
    .Where(x => x.Id == currentUser!.FamilyId)
    .FirstOrDefaultAsync();


    // Create a new reservation
    var newReservation = new Reservation()
    {
        Id = addResevationDTO.Id,
        Car = car,
        StartTime = addResevationDTO.StartTime,
        EndTime = addResevationDTO.EndTime,
        IsAllDay = addResevationDTO.IsAllDay,
        RecurrenceRule = addResevationDTO.RecurrenceRule ?? string.Empty, // Provide default value if null
        Description = addResevationDTO.Description,
        Driver = currentUser,
        Family = family!,
    };
    //var generalDTO = new GeneralDTO();
    //generalDTO.Reservations.Add(reservation);
    context.Reservations.Add(newReservation);
    await context.SaveChangesAsync();

    // Return a success response
    return Results.Ok(new { message = "Reservation added successfully", newReservation.Id });
});

app.MapDelete("/Deletereservation", async (ClaimsPrincipal user, [FromBody] ReservationIdDTO reservationIdDTO, ApplicationDbContext context, UserManager<Driver> userManager) =>
{

    if (user.Identity?.IsAuthenticated != true)
    {
        return Results.Unauthorized();
    }


    var userId = userManager.GetUserId(user);


    var reservation = await context.Reservations
        .FirstOrDefaultAsync(r => r.Id == reservationIdDTO.Id);

    if (reservation == null)
    {
        return Results.NotFound("Reservation not found.");
    }

    // Check if the reservation belongs to the authenticated user
    if (reservation.DriverId != userId)
    {
        return Results.BadRequest("User not found");
    }

    // Delete the reservation
    context.Reservations.Remove(reservation);
    await context.SaveChangesAsync();

    return Results.Ok("Reservation deleted successfully.");
});


app.MapGet("/reservations", async (ClaimsPrincipal user, ApplicationDbContext context, UserManager<Driver> userManager) =>
{


    var currentUser = await userManager.GetUserAsync(user);


    var userFamilyId = currentUser!.FamilyId;

    var reservations = await context.Reservations
        .Include(x => x.Car)
        .Include(x => x.Driver)
        .Where(x => x.FamilyId == userFamilyId)
        .ToListAsync();


    var reservationsDto = reservations.Select(x => new ReservationDTO()
    {
        CarType = x.Car.Name,
        Id = x.Id,
        DriverId = x.DriverId,
        EndTime = x.EndTime,
        IsAllDay = x.IsAllDay,
        IsBlock = x.IsBlock,
        RecurrenceRule = x.RecurrenceRule,
        StartTime = x.StartTime,
        Description = x.Description
    });

    var generalDTO = new GeneralDTO();
    generalDTO.Reservations.AddRange(reservationsDto);

    var userFamilyIdDriver = currentUser!.FamilyId; // Assuming FamilyId is the property of currentUser that contains the FamilyId

    var driversInSameFamily = await context.Drivers
        .Where(d => d.FamilyId == userFamilyIdDriver)
        .ToListAsync();

    var driverDto = driversInSameFamily.Select(x => new DriverDTO()
    {
        Id = x.Id,
        Fullname = x.Fullname,
        DriverName = x.NormalizedUserName,
    });

    generalDTO.Drivers.AddRange(driverDto);

    var cars = await context.Cars
    .ToListAsync();
    var CarDto = cars.Select(x => new CarDTO()
    {
        Id = x.Id,
        CarName = x.Name,
    });
    generalDTO.Cars.AddRange(CarDto);

    generalDTO.CurrentUserId = currentUser.Id;

    return generalDTO;
})

.RequireAuthorization()
.WithName("GetReservations")
.WithOpenApi();


app.UseCors("CorsPolicy");

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.MapIdentityApi<Driver>();//aaaa

app.UseHttpsRedirection();

var summaries = new[]
{
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};


app.Run();

internal record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
public class RegisterRequest
{
    public required string Username { get; set; }
    public required string Password { get; set; }
    public required string XName { get; set; }
}