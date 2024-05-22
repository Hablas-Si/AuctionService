using AuctionService.Repositories;
using MongoDB.Bson.Serialization.Serializers;
using MongoDB.Bson.Serialization;
using MongoDB.Bson;
using AuctionService.Models;
using Microsoft.AspNetCore.HttpsPolicy; // Add this line
using System.Text.Json;
using BiddingService.Repositories;

var builder = WebApplication.CreateBuilder(args);


// BsonSeralizer... fort�ller at hver gang den ser en Guid i alle entiteter skal den serializeres til en string. 
BsonSerializer.RegisterSerializer(new GuidSerializer(BsonType.String));

// OBS: lig dem her op i vault, se opgave
string mySecret = Environment.GetEnvironmentVariable("Secret") ?? "none";
string myIssuer = Environment.GetEnvironmentVariable("Issuer") ?? "none";


builder.Services.Configure<MongoDBSettings>(options =>
{
    options.ConnectionAuction = "mongodb+srv://admin:admin@auctionhouse.dfo2bcd.mongodb.net/" ?? throw new ArgumentNullException("ConnectionAuction environment variable not set");
});
// tilføjer Repository til services
builder.Services.AddSingleton<IAuctionRepository, AuctionRepository>();

// var catalogServiceBaseUrl = Environment.GetEnvironmentVariable("ConnectionURI");
// Konfigurer HttpClient for AuctionHouse
builder.Services.AddHttpClient<ICatalogRepository, CatalogRepository>(client =>
{
    client.BaseAddress = new Uri("http://localhost:5020"); // URL til CatalogService

}).ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler
{
    ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator // Brug kun i udviklingsmiljøer
});

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();


app.UseSwagger();
app.UseSwaggerUI();

//app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
