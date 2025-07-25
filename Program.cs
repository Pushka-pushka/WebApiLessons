using HotelsWebApi.Model;
using HotelsWebApi.Domain;
using HotelsWebApi.Domain.Repositories;
using HotelsWebApi.Auth;
using HotelsWebApi.APIs;




var builder = WebApplication.CreateBuilder(args);

RegisterServices(builder.Services);
var app = builder.Build();
Configure(app);
var apis = app.Services.GetServices<IApi>();
foreach (var api in apis)
{
    if(api is null) throw new InvalidProgramException("Api Not Found");
    api.Register(app);
}


app.Run();



void RegisterServices(IServiceCollection services)
{
    services.AddEndpointsApiExplorer();
    services.AddSwaggerGen();

    services.AddDbContext<HotelDb>(options=>
    {
        options.UseNpgsql(builder.Configuration.GetConnectionString("PostgresConnection"));
    });

    services.AddScoped<IHotelRepository, HotelRepository>();
    services.AddSingleton<ITokenService>(new TokenService());
    services.AddSingleton<IUserRepository>(new UserRepository());
    services.AddAuthorization();
    services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
        .AddJwtBearer(options=>
        {
            options.TokenValidationParameters = new()
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = builder.Configuration["Jwt:Issuer"],
                ValidAudience = builder.Configuration["Jwt:Audience"],
                IssuerSigningKey = new SymmetricSecurityKey(
                    Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
            };
        });
    services.AddTransient<IApi, HotelApi>();
    services.AddTransient<IApi, AuthApi>();

}

void Configure(WebApplication app)
{
    app.UseAuthentication();
    app.UseAuthorization();

    if(app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI();
        using var scope = app.Services.CreateScope();
        var db =  scope.ServiceProvider.GetRequiredService<HotelDb>();
        db.Database.EnsureCreated();
    }
    app.UseHttpsRedirection(); 
}



//  для запросов нужно залогинится - get login?username=name&password=...
// после получить токен
//  пример запроса с токеном - 
// get hotels --header "Authorization: Bearer eyJhbGciOiJodHRwOi8vd3d3LnczLm9yZy8yMDAxLzA0L3htbGRzaWctbW9yZSNobWFjLXNoYTI1NiIsInR5cCI6IkpXVCJ9.eyJodHRwOi8vc2NoZW1hcy54bWxzb2FwLm9yZy93cy8yMDA1LzA1L2lkZW50aXR5L2NsYWltcy9uYW1lIjoiQWxpY2UiLCJodHRwOi8vc2NoZW1hcy54bWxzb2FwLm9yZy93cy8yMDA1LzA1L2lkZW50aXR5L2NsYWltcy9uYW1laWRlbnRpZmllciI6Ijk5YjAwNGJhLTIzNzYtNDk0Yy04MzNjLWY2ZWJhZjM0ZWZiNSIsImV4cCI6MTc1MzM5NjU0OSwiaXNzIjoiUGxhdGludW0iLCJhdWQiOiJQbGF0aW51bSJ9.5kpMnMfhjlvhGElZjAGOB0CY50QSNWAk3bo0hRprYrM"
