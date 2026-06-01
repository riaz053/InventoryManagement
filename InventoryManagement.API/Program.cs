using InventoryManagement.API.Data;
using InventoryManagement.API.Infrastructure.Security;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Security.Claims;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

#region SERVICES

// MVC + API Controllers
builder.Services.AddControllersWithViews();

// Session Validator (for single-login system)
builder.Services.AddScoped<SessionValidator>();

// Database
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection")
    )
);

// Swagger
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Enter: Bearer {your token}"
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
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
            new List<string>()
        }
    });
});

#endregion

#region JWT AUTHENTICATION

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
            Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!)
        )
    };

    // 🔥 SESSION VALIDATION (Single device login enforcement)
    options.Events = new JwtBearerEvents
    {
        OnTokenValidated = async context =>
        {
            var validator =
                context.HttpContext.RequestServices.GetRequiredService<SessionValidator>();

            var userId = context.Principal?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var sessionId = context.Principal?.FindFirst("sid")?.Value;

            if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(sessionId))
            {
                context.Fail("Invalid token claims");
                return;
            }

            var isValid = validator.IsValid(userId, sessionId);

            if (!isValid)
            {
                context.Fail("Session expired or logged in from another device");
            }

            // if (!validator.IsValid(userId, sessionId))
            // {
            //     context.Fail("Session invalid");
            // }

            await Task.CompletedTask;
        }
    };
});

builder.Services.AddAuthorization();

#endregion

var app = builder.Build();

#region MIDDLEWARE PIPELINE

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

// 🔥 API ROUTES FIRST
app.MapControllers();

// 🔥 MVC ROUTES SECOND (fallback)
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Auth}/{action=Login}/{id?}"
);

app.Run();

#endregion