using Microsoft.EntityFrameworkCore;
using PWN_Backend.Data;
using PWN_Backend.Services;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// 1. 設定 Serilog (從 appsettings.json 讀取配置)
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .CreateLogger();

builder.Host.UseSerilog(); // 取代內建的 Logger

// 2. 註冊資料庫服務 (ApplicationDbContext)
builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    // 從 appsettings.json 讀取 DefaultConnection
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));

    // 將 SQL 語法輸出到 Serilog (進而寫入 .log 檔案)
    options.LogTo(Log.Information, LogLevel.Information)
           .EnableSensitiveDataLogging(); // 開發階段開啟以查看 SQL 參數值
});

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddScoped<MarketDataService>();

//Add AI Service
builder.Services.AddSingleton<AIService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseSerilogRequestLogging(); // 記錄每一次 HTTP 請求的簡要資訊

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

try
{
    Log.Information("Backend Service is starting...");
    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Backend Service starts Failed with [ "+ex.Message+"]");
}
finally
{
    Log.CloseAndFlush();
}
