using Microsoft.EntityFrameworkCore;
using PENTDRIVEApi.Data;
using Pomelo.EntityFrameworkCore.MySql.Infrastructure;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Net;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

var serverVersion = new MySqlServerVersion(new Version(8, 0, 43));

var JwtKey = builder.Configuration["Jwt:Key"]
    ??throw new InvalidOperationException("A configuração 'Jwt:Key' não foi encontrada.");


//CONFIGURAÇÃO DE SERVIÇOS

builder.Services.AddDbContext<AppDbContext>(
    dbContextOptions => dbContextOptions
    .UseMySql(connectionString, serverVersion)
);

//configura a Autenticação JWT
    builder.Services.AddAuthentication(options =>
    {
        // Define o esquema padrão como JWT Bearer
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,

            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            // A chave secreta é lida do appsettings.json e usada para validar a assinatura
            IssuerSigningKey = new SymmetricSecurityKey(
                    Encoding.UTF8.GetBytes(JwtKey.Trim())
            )
        };
    });


builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.ReferenceHandler =
        System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
});
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
        Description = "Insira o token JWT no formato: Bearer {seu token}",
    });

    options.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
    {
        {
            new Microsoft.OpenApi.Models.OpenApiSecurityScheme
            {
                Reference = new Microsoft.OpenApi.Models.OpenApiReference
                {
                    Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string [] {}
        }
    });
});

var app = builder.Build();

//CONFIGURAÇÃO DE MIDDLEWARES

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication(); 
app.UseAuthorization(); 

app.MapControllers();

app.Run();