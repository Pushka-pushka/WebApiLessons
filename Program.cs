using HotelsWebApi.Model;
using HotelsWebApi.Domain;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<HotelDb>(options=>
{
    options.UseNpgsql(builder.Configuration.GetConnectionString("PostgresConnection"));
});

var app = builder.Build();

if(app.Environment.IsDevelopment())
{
    using var scope = app.Services.CreateScope();
    var db =  scope.ServiceProvider.GetRequiredService<HotelDb>();
    db.Database.EnsureCreated();
}



var hotels = new List<Hotel>();

app.MapGet("/hotels", async(HotelDb db) => await db.Hotels.ToListAsync());

app.MapGet("/hotels/{id}", async(int id, HotelDb db)  => 
    await db.Hotels.FirstOrDefaultAsync( h => h.Id == id) is Hotel hotel
    ? Results.Ok(hotel)
    : Results.NotFound());

app.MapPost("/hotels", async([FromBody]Hotel hotel, HotelDb db) => 
    {
        db.Hotels.Add(hotel);
        await db.SaveChangesAsync();
        return Results.Created($"/hotels/{hotel.Id}", hotel);
        
    });

app.MapPut("/hotels", async ([FromBody]Hotel hotel, HotelDb db) =>
    {
        var hotelFromDb = await db.Hotels.FindAsync(new object[]{hotel.Id});
        if(hotelFromDb == null) return Results.NotFound();

        hotelFromDb.Name = hotel.Name;
        hotelFromDb.Latitude = hotel.Latitude;
        hotelFromDb.Longitude = hotel.Longitude;
        
        await db.SaveChangesAsync();

        return Results.NoContent();
    });

app.MapDelete("Hotels/{id}", async (int id, HotelDb db) =>
    {
        var hotelFromDb = await db.Hotels.FindAsync(new object [] {id});
        if(hotelFromDb == null) return Results.NotFound();
        db.Hotels.Remove(hotelFromDb);
        await db.SaveChangesAsync();
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

