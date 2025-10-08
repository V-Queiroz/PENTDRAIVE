using Microsoft.EntityFrameworkCore;
using PENTDRIVEApi.Data;
using Pomelo.EntityFrameworkCore.MySql.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

var serverVersion = new MySqlServerVersion(new Version(8, 0, 43));

builder.Services.AddDbContext<AppDbContext>(
    dbContextOptions => dbContextOptions
    .UseMySql(connectionString, serverVersion)
);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();


if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
