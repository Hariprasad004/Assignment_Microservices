using Serilog;
using Services.AuthAPI.Models;
using Services.AuthAPI.Service;
using Services.AuthAPI.Service.IService;
using WebUI.Service;
using WebUI.Service.IService;
using WebUI.Utility;

var builder = WebApplication.CreateBuilder(args);

//Adding custom logs
Log.Logger = new LoggerConfiguration().MinimumLevel.Debug().WriteTo.
	File("./Log/LogDetails.txt", rollingInterval: RollingInterval.Day).CreateLogger();

builder.Host.UseSerilog();

// Add services to the container.

builder.Services.Configure<JwtOptions>(builder.Configuration.GetSection("ApiSettings:JwtOptions"));

builder.Services.AddControllers();
builder.Services.AddScoped<IJwtTokenGenerator, JwtTokenGenerator>();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

StaticDetails.DataProcessAPI = builder.Configuration["ServiceUrls:DataProcessAPI"];
builder.Services.AddHttpClient<IBaseService, BaseService>();
builder.Services.AddScoped<IBaseService, BaseService>();
builder.Services.AddSingleton<IPassWordHash, BcryptPasswordHash>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
	app.UseSwagger();
	app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
