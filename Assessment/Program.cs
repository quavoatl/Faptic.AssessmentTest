using Assessment.Business;
using Assessment.Business.Aggregation;
using Assessment.Business.CloseDataIngestion;
using Assessment.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHttpClient();

builder.Services.AddDbContext<AssessmentDbContext>(options =>
    options.UseInMemoryDatabase("AssessmentTest"));

builder.Services.AddScoped<ICloseDataIngestionHandler, BitstampCloseDataIngestionHandler>();
builder.Services.AddScoped<ICloseDataIngestionHandler, BitfinexCloseDataIngestionHandler>();
builder.Services.AddScoped<ICloseDataIngestionHandler, CoinbaseCloseDataIngestionHandler>();

builder.Services.AddScoped<ICloseDataRetrievalService, CloseDataRetrievalService>();
builder.Services.AddScoped<ICloseDataIngestionService, CloseDataIngestionService>();

builder.Services.AddScoped<IAggregationStrategy, AverageAggregationStrategy>();

var app = builder.Build();

using (var serviceScope = app.Services.CreateScope())
{
    var dbContext = serviceScope.ServiceProvider.GetRequiredService<AssessmentDbContext>();
    dbContext.Database.EnsureCreated();
}

app.UseSwagger();
app.UseSwaggerUI();

app.MapControllers();

app.Run();
