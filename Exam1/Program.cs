using Exam1.Entities;
using Exam1.Services;
using Microsoft.EntityFrameworkCore;
using Serilog;

var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;

// Check if Logs folder exist
if (!Directory.Exists("./logs"))
{
    Directory.CreateDirectory("./logs");
}

// Configure Serilog
var today = DateTime.Now.ToString("yyyyMMdd"); // Format tanggal hari ini
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information() // Set log level ke Information
    .WriteTo.File($"./logs/Log-{today}.txt") // Simpan log ke file dengan nama sesuai tanggal hari ini
    .CreateLogger();

builder.Host.UseSerilog();

// Configure sql server
builder.Services.AddEntityFrameworkSqlServer();
builder.Services.AddDbContextPool<Exam1Context>(options =>
{
    var conString = configuration.GetConnectionString("SQLServerDB");
    options.UseSqlServer(conString);
});

// Add services to the container.
builder.Services.AddTransient<TicketService>();
builder.Services.AddTransient<BookTicketService>();
builder.Services.AddTransient<PdfReportService>();

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();

app.MapControllers();

app.Run();
Log.CloseAndFlush();