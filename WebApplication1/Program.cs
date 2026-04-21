using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microondas.Domain.Services;
using Microondas.Domain.Repositories;
using Microondas.Infrastructure.Data;
using Microondas.Infrastructure.Repositories;
using Microondas.WebApplication.Middleware;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

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

builder.Services.AddDbContext<MicroondasDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
    
builder.Services.AddScoped<IAquecimentoRepository, AquecimentoSqlRepository>();
builder.Services.AddScoped<AquecimentoService>(sp =>
{
    var repo = sp.GetRequiredService<IAquecimentoRepository>();
    var programasPreDefinidos = new List<Microondas.Domain.IAquecimento>
    {
        new Microondas.Console.PipocaAquecimento(),
        new Microondas.Console.LeiteAquecimento(),
        new Microondas.Console.CarneAquecimento(),
        new Microondas.Console.FrangoAquecimento(),
        new Microondas.Console.FeijaoAquecimento()
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
