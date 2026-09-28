using Microsoft.EntityFrameworkCore;
using Svabic.Api.Podaci;

var builder = WebApplication.CreateBuilder(args);

// Servisi
builder.Services.AddDbContext<SvabicDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("Baza")));

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSingleton(TimeProvider.System);

var dozvoljeniOriginiSajta = builder.Configuration
    .GetSection("Cors:DozvoljeniOrigini")
    .Get<string[]>() ?? [];

builder.Services.AddCors(options =>
{
    options.AddPolicy("SajtPolitika", policy =>
    {
        policy.WithOrigins(dozvoljeniOriginiSajta)
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

var app = builder.Build();

// Pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors("SajtPolitika");
app.UseAuthorization();

app.MapControllers();

app.Run();
