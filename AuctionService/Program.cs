using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using AuctionService.Models;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using MongoDB.Driver;
using AuctionService.Repositories;
using VaultSharp;
using VaultSharp.V1.AuthMethods.Token;
using VaultSharp.V1.AuthMethods;
using VaultSharp.V1.Commons;
using Microsoft.Extensions.Options;
using NLog.Web;
using NLog;
using Microsoft.OpenApi.Models;
using Microsoft.Extensions.DependencyInjection;
using AuctionService.Services;



var logger = NLog.LogManager.Setup().LoadConfigurationFromAppSettings()
.GetCurrentClassLogger();
logger.Debug("init main");

var builder = WebApplication.CreateBuilder(args);

builder.Logging.ClearProviders();
builder.Host.UseNLog();

// BsonSeralizer... fort�ller at hver gang den ser en Guid i alle entiteter skal den serializeres til en string. 
BsonSerializer.RegisterSerializer(new GuidSerializer(BsonType.String));

// Fetch secrets from Vault. Jeg initierer vaultService og bruger metoden derinde GetSecretAsync
var vaultService = new VaultRepository(logger, builder.Configuration);
var mySecret = await vaultService.GetSecretAsync("Secret");
var myIssuer = await vaultService.GetSecretAsync("Issuer");
// logger.Info($"Secret: {mySecret} and Issuer: {myIssuer}");
if (mySecret == null || myIssuer == null )
{
    Console.WriteLine("Failed to retrieve secrets from Vault");
    throw new ApplicationException("Failed to retrieve secrets from Vault");
}
builder.Services
.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters()
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = myIssuer,
        ValidAudience = "http://localhost",
        IssuerSigningKey =
    new SymmetricSecurityKey(Encoding.UTF8.GetBytes(mySecret))
    };
});
// Tilføjer authorization politikker som bliver brugt i controlleren, virker ik
builder.Services.AddAuthorization(options =>
    {
        options.AddPolicy("AdminOnly", policy => policy.RequireRole("Admin"));
        options.AddPolicy("UserOnly", policy => policy.RequireRole("User"));
    });
// Add services to the container.

//tilføjer Repository til services.
builder.Services.AddSingleton<IVaultRepository>(vaultService);

var ConnectionAuctionDB = await vaultService.GetSecretAsync("ConnectionAuctionDB");
builder.Services.Configure<MongoDBSettings>(options =>
{
    options.ConnectionAuctionDB = ConnectionAuctionDB ?? throw new ArgumentNullException("ConnectionAuctionDB environment variable not set");
});
// tilføjer Repository til services
builder.Services.AddSingleton<IAuctionRepository, AuctionRepository>();

// Konfigurer HttpClient for UserService udfra environment variablen UserServiceUrl
var CatalogServiceURL = Environment.GetEnvironmentVariable("ConnectionURI");
if (string.IsNullOrEmpty(CatalogServiceURL))
{
    logger.Error("CatalogServiceURL is missing");
    throw new ApplicationException("CatalogServiceURL is missing");
}
else
{
    logger.Info("CatalogServiceURL is: " + CatalogServiceURL);
}
builder.Services.AddHttpClient<ICatalogRepository, CatalogRepository>(client =>
{
    client.BaseAddress = new Uri(CatalogServiceURL);
});

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();


app.UseSwagger();
app.UseSwaggerUI();

// Start listening for messages from RabbitMQ
var subscriber = new RabbitMQSubscriber("BidToAuc");
await subscriber.StartListening(async (message) =>
{
    // Resolve the auction repository
    var auctionRepository = app.Services.GetRequiredService<IAuctionRepository>();

    // Call the method in the repository
    await auctionRepository.OnBidReceived(message);
});

//app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
