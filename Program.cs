using HotelsWebApi.Model;
using HotelsWebApi.Domain;
using HotelsWebApi.Domain.Repositories;


var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<HotelDb>(options=>
{
    options.UseNpgsql(builder.Configuration.GetConnectionString("PostgresConnection"));
});

builder.Services.AddScoped<IHotelRepository, HotelRepository>();

var app = builder.Build();

if(app.Environment.IsDevelopment())
{
    using var scope = app.Services.CreateScope();
    var db =  scope.ServiceProvider.GetRequiredService<HotelDb>();
    db.Database.EnsureCreated();
}



var hotels = new List<Hotel>();

app.MapGet("/hotels", async(IHotelRepository repository) => 
    Results.Ok(await repository.GetHotelsAsync()));

app.MapGet("/hotels/{id}", async(int id, IHotelRepository repository)  => 
    await repository.GetHotelAsync(id) is Hotel hotel
    ? Results.Ok(hotel)
    : Results.NotFound());

app.MapPost("/hotels", async([FromBody]Hotel hotel, IHotelRepository repository)=>
    {
       await repository.InsertHotelAsync(hotel);
       await repository.SaveAsync();
        return Results.Created($"/hotels/{hotel.Id}", hotel);
        
    });

app.MapPut("/hotels", async ([FromBody]Hotel hotel, IHotelRepository repository) =>
    {
       await repository.UpdateHotelAsync(hotel);
       await repository.SaveAsync();

        return Results.NoContent();
    });

app.MapDelete("Hotels/{id}", async (int id, IHotelRepository repository) =>
    {
       await repository.DeleteHotelAsync(id);
       await repository.SaveAsync();
        return Results.NoContent();
    

    });

app.UseHttpsRedirection();  





app.Run();


// var hotels = new List<Hotel>();

// app.MapGet("/hotels", () => hotels);
// app.MapGet("/hotels/{id}", (int id) => hotels.FirstOrDefault( h => h.Id == id));
// app.MapPost("/hotels", (Hotel hotel) => hotels.Add(hotel));
// app.MapPut("/hotels", (Hotel hotel) =>{
//     var index = hotels.FindIndex( h => h.Id == hotel.Id);
//     if (index < 0 )
//     {
//         throw new Exception(" Not found");
//     }

//     hotels[index] = hotel;
// });

// app.MapDelete("Hotels/{id}", (int id) =>{
//     var index = hotels.FindIndex(h => h.Id == id);
//       if (index < 0 )
//     {
//         throw new Exception(" Not found");
//     }
//     hotels.RemoveAt(index);

// });

