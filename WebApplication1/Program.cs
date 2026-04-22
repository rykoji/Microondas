using Microondas.Domain.Repositories;
using Microondas.Domain.Services;
using Microondas.Infrastructure.Data;
using Microondas.Infrastructure.Repositories;
using Microondas.WebApplication.Middleware;
using Microondas.WebApplication.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Microondas API", Version = "v1" });

    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header usando Bearer. Exemplo: 'Bearer {token}'",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
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
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!))
        };
    });

var encryptedConnectionString = builder.Configuration.GetConnectionString("DefaultConnection")!;
var connectionKey = builder.Configuration.GetConnectionString("EncryptionKey")!;
var connectionString = CryptographyService.Decrypt(encryptedConnectionString, connectionKey);

builder.Services.AddDbContext<MicroondasDbContext>(options =>
    options.UseSqlServer(connectionString));
    
builder.Services.AddScoped<IAquecimentoRepository, AquecimentoSqlRepository>();
builder.Services.AddScoped<AquecimentoService>(sp =>
{
    var repo = sp.GetRequiredService<IAquecimentoRepository>();
    var programasPreDefinidos = new List<Microondas.Domain.IAquecimento>
    {
        new Microondas.Domain.PipocaAquecimento(),
        new Microondas.Domain.LeiteAquecimento(),
        new Microondas.Domain.CarneAquecimento(),
        new Microondas.Domain.FrangoAquecimento(),
        new Microondas.Domain.FeijaoAquecimento()
    };
    return new AquecimentoService(repo, programasPreDefinidos);
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseMiddleware<ExceptionMiddleware>();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
