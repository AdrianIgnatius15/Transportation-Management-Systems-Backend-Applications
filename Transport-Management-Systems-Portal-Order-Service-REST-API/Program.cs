using System.Security.Claims;
using Amazon.S3;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Transport_Management_Systems_Portal_Order_Service_REST_API.Data;
using Transport_Management_Systems_Portal_Order_Service_REST_API.Data.Interfaces;
using Transport_Management_Systems_Portal_Order_Service_REST_API.Data.Repositories;
using Transport_Management_Systems_Portal_Order_Service_REST_API.Middlewares;
using Transport_Management_Systems_Portal_Order_Service_REST_API.Middlewares.Interfaces;
using Transport_Management_Systems_Portal_Order_Service_REST_API.Service.Interface;
using Transport_Management_Systems_Portal_Order_Service_REST_API.Services;
using Transport_Management_Systems_Portal_Order_Service_REST_API.Services.ConfigurationModel;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// builder.Services.AddDbContext<TMSDbContext>(options => options.UseMySql(
//     builder.Configuration.GetConnectionString("TMS-Database"),
//     ServerVersion.AutoDetect(builder.Configuration.GetConnectionString("TMS-Database"))
// ));
builder.Services.AddDbContext<TMSDbContext>(options => options.UseNpgsql(
    builder.Configuration.GetConnectionString("TMS-Database")
));
builder.Services.Configure<DocumentStorageSettings>(builder.Configuration.GetSection("SeaweedFS"));

builder.Services.AddSingleton<IRabbitMQConnection, RabbitMQConnectionMiddleware>();
builder.Services.AddSingleton<IAmazonS3>(_ =>
{
    // var settings = builder.Configuration.GetSection("SeaweedFS").Get<DocumentStorageSettings>()!;
    var config = builder.Configuration.GetSection("SeaweedFS");
    var serviceUrl = config["ServiceURL"];
    var accessKey = config["AccessKey"];
    var secretKey = config["SecretKey"];

    var configuration = new AmazonS3Config
    {
        ServiceURL = serviceUrl,
        ForcePathStyle = true, //Required for SeaweedFS / non-AWS S3
        UseHttp = true,
        SignatureMethod = Amazon.Runtime.SigningAlgorithm.HmacSHA1
    };

    return new AmazonS3Client(accessKey, secretKey, configuration);
});
builder.Services.AddHostedService<BucketInitializer>();
builder.Services.AddTransient<IClaimsTransformation, KeycloakRoleTransformer>();
builder.Services.AddScoped<IOrderRepo, OrderRepo>();
builder.Services.AddScoped<IAddressRepo, AddressRepo>();
builder.Services.AddScoped<IClientRepo, ClientRepo>();
builder.Services.AddScoped<IMQProducer, MqProducer>();
builder.Services.AddScoped<IDocumentStorageService, DocumentStorageService>();

// CORS for Angular front-end (adjust origin as needed)
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.WithOrigins("http://localhost:4200")
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});

// Keycloak / JWT authentication
var keycloakSection = builder.Configuration.GetSection("Keycloak");
var authority = keycloakSection.GetValue<string>("Authority");
var clientId = keycloakSection.GetValue<string>("ClientId");
var requireHttps = keycloakSection.GetValue<bool?>("RequireHttpsMetadata") ?? true;

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.Authority = authority;
        options.RequireHttpsMetadata = requireHttps;
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateAudience = false,
            ValidateIssuer = true,
            RoleClaimType = ClaimTypes.Role
        };
    });

builder.Services.AddAuthorization();

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
    });
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

// app.UseHttpsRedirection();

app.UseCors();

app.UseAuthorization();

app.MapControllers();

app.Run();
