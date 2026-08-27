using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using SmartBus.Api.Common;
using SmartBus.Application;
using SmartBus.Application.Common;
using SmartBus.Infrastructure;
using SmartBus.Infrastructure.Persistence;

var builder = WebApplication.CreateBuilder(args);

// ---------- Serviços ----------

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new() { Title = "SmartBus API", Version = "v1" });

    var esquemaJwt = new Microsoft.OpenApi.OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = Microsoft.OpenApi.SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = Microsoft.OpenApi.ParameterLocation.Header,
        Description = "Informe apenas o token JWT (sem o prefixo 'Bearer ')."
    };
    options.AddSecurityDefinition("Bearer", esquemaJwt);
    options.AddSecurityRequirement(documento => new Microsoft.OpenApi.OpenApiSecurityRequirement
    {
        { new Microsoft.OpenApi.OpenApiSecuritySchemeReference("Bearer", documento), new List<string>() }
    });
});

// Camadas Application + Infrastructure registradas via seus próprios
// métodos de extensão — o Program.cs não conhece os detalhes internos.
builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

var jwtOptions = builder.Configuration.GetSection("Jwt").Get<JwtOptions>()
    ?? throw new InvalidOperationException("Seção 'Jwt' não configurada em appsettings.json.");

builder.Services
    .AddAuthentication(options =>
    {
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
            ValidIssuer = jwtOptions.Emissor,
            ValidAudience = jwtOptions.Audiencia,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtOptions.ChaveSecreta))
        };
    });

builder.Services.AddAuthorization();

const string PoliticaCorsAngular = "PoliticaCorsAngular";
builder.Services.AddCors(options =>
{
    options.AddPolicy(PoliticaCorsAngular, policy =>
    {
        policy
            .WithOrigins(
                builder.Configuration.GetSection("Cors:OrigensPermitidas").Get<string[]>()
                ?? new[] { "http://localhost:4200" })
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

var app = builder.Build();

// ---------- Migrations + Seed automáticos em desenvolvimento ----------

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<SmartBusDbContext>();
    DbSeeder.Seed(db);
}

// ---------- Pipeline HTTP ----------

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors(PoliticaCorsAngular);
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();

// Necessário para o WebApplicationFactory<Program> dos testes de integração
// enxergar a classe Program (top-level statements geram uma classe
// implícita internal; este partial a expõe como public).
public partial class Program { }
