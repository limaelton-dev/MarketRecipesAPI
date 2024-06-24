using MarketRecipesAPI.Data;
using MarketRecipesAPI.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);

// Configuração do JSON Serializer
builder.Services.AddControllers()
    .AddNewtonsoftJson(options =>
    {
        options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;
    });

builder.Services.AddEndpointsApiExplorer();

// Configuração do Swagger
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "MarketRecipesAPI", Version = "v1" });

    // Adicionando configuração de segurança para JWT
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Description = "Please enter token",
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        BearerFormat = "JWT",
        Scheme = "Bearer"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement {
    {
        new OpenApiSecurityScheme {
            Reference = new OpenApiReference {
                Type = ReferenceType.SecurityScheme,
                Id = "Bearer"
            },
            Scheme = "oauth2",
            Name = "Bearer",
            In = ParameterLocation.Header,

        },
        new List<string>()
    }});
});

// Configuração do DbContext
builder.Services.AddDbContext<MarketRecipesContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Adicionando serviços
builder.Services.AddScoped<RecipeService>();
builder.Services.AddScoped<UserService>();

// Configuração da autenticação JWT
builder.Services.AddAuthentication(options =>
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
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidAudience = builder.Configuration["Jwt:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
    };

    options.Events = new JwtBearerEvents
    {
        OnAuthenticationFailed = context =>
        {
            context.Response.StatusCode = 401;
            context.Response.ContentType = "application/json";
            var result = JsonSerializer.Serialize(new
            {
                message = "Token inválido"
            });
            return context.Response.WriteAsync(result);
        },
        OnChallenge = context =>
        {
            context.HandleResponse();
            if (!context.Response.HasStarted)
            {
                context.Response.StatusCode = 401;
                context.Response.ContentType = "application/json";
                var result = JsonSerializer.Serialize(new
                {
                    message = "Token obrigatório para esta rota"
                });
                return context.Response.WriteAsync(result);
            }
            return Task.CompletedTask;
        }
    };
});

var app = builder.Build();

// Configuração do Swagger para o ambiente de desenvolvimento
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.UseMiddleware<JwtMiddleware>();

app.MapControllers();

app.Run();
