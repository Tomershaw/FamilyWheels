using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using NewReservationApi.Database;
using NewReservationApi.DTOs;
using NewReservationApi.Enitities;
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
//});


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
        Subject = x.Subject
    });

    var generalDTO = new GeneralDTO();
    generalDTO.Reservations.AddRange(reservationsDto);

    var currentUserDriver = await userManager.GetUserAsync(user);
    var userFamilyIdDriver = currentUser!.FamilyId; // Assuming FamilyId is the property of currentUser that contains the FamilyId

    var driversInSameFamily = await context.Drivers
        .Where(d => d.FamilyId == userFamilyIdDriver)
        .ToListAsync();

    var driverDto = driversInSameFamily.Select(x => new DriverDTO()
    {
        Id = x.Id,
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
    return generalDTO;
})

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