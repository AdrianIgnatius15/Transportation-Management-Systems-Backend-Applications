using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Transportation_Management_System_Tracking_Service_REST_API.Data;
using Transportation_Management_System_Tracking_Service_REST_API.Data.Interfaces;
using Transportation_Management_System_Tracking_Service_REST_API.Data.Repositories;
using Transportation_Management_System_Tracking_Service_REST_API.Middlewares;
using Transportation_Management_System_Tracking_Service_REST_API.Middlewares.Interfaces;
using Transportation_Management_System_Tracking_Service_REST_API.SignalRControllers;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// --------- Database ------------
builder.Services.AddDbContext<TMSDbContext>(options => 
    options.UseNpgsql(
        builder.Configuration.GetConnectionString("TMS-Database")
    )
);

// --------- Rabbit MQ Messaging Queue Middleware ------------
builder.Services.AddSingleton<IRabbitMQConnection, RabbitMQConnectionMiddleware>();

// --------- Interface contract & actual repository ----------
builder.Services.AddScoped<ITrackingEventRepo, TrackingRepo>();
builder.Services.AddScoped<IMQProducer, MqProducer>();

builder.Services.AddControllers();
builder.Services.AddSignalR();

var keycloakSection = builder.Configuration.GetSection("Keycloak");

builder
    .Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.Authority = keycloakSection.GetValue<string>("Authority");
        options.RequireHttpsMetadata =
            keycloakSection.GetValue<bool?>("RequireHttpsMetadata") ?? true;
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateAudience = false,
            ValidateIssuer = true,
            RoleClaimType = ClaimTypes.Role,
        };

        // SignalR WebSocket handshake can't set Authorization header —
        // the JWT is passed as a query string parameter instead
        options.Events = new JwtBearerEvents
        {
            OnMessageReceived = context =>
            {
                var accessToken = context.Request.Query["access_token"];
                var path = context.HttpContext.Request.Path;

                if (!string.IsNullOrEmpty(accessToken) && path.StartsWithSegments("/hubs/tracking"))
                {
                    context.Token = accessToken;
                }

                return Task.CompletedTask;
            },
        };
    });
builder.Services.AddAuthorization();

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.MapHub<TrackingHubController>("/hubs/tracking");

app.Run();
